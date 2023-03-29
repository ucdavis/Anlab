using Anlab.Core.Data;
using Anlab.Core.Domain;
using Anlab.Core.Models;
using Anlab.Jobs.MoneyMovement;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Anlab.Core.Extensions;
using Serilog;
using static Anlab.Jobs.MoneyMovement.TransferViewModel;
using Anlab.Core.Models.AggieEnterpriseModels;
using AggieEnterpriseApi.Validation;

namespace Anlab.Core.Services
{
    public interface ISlothService
    {
        Task<SlothResponseModel> MoveMoney(Order order);
        Task ProcessCreditCards();

        Task MoneyHasMoved();
    }

    public class SlothService : ISlothService
    {
        private readonly ApplicationDbContext _dbContext;
        private IAggieEnterpriseService _aggieEnterpriseService;
        private readonly AggieEnterpriseSettings _aeSettings;
        private readonly FinancialSettings _appSettings;

        public SlothService(ApplicationDbContext dbContext, IAggieEnterpriseService aggieEnterpriseService, IOptions<FinancialSettings> appSettings, IOptions<AggieEnterpriseSettings> aeSettings)
        {
            _dbContext = dbContext;
            _aggieEnterpriseService = aggieEnterpriseService;
            _aeSettings = aeSettings.Value;
            _appSettings = appSettings.Value;
        }


        
        //TODO: Add validation?
        public async Task<SlothResponseModel> MoveMoney(Order order)
        {
            if (_appSettings.SlothApiUrl.EndsWith("v1/", StringComparison.OrdinalIgnoreCase) || _appSettings.SlothApiUrl.EndsWith("v2/", StringComparison.OrdinalIgnoreCase))
            {
                Log.Error("Sloth SlothApiUrl should not end with version");
                //Replace the end of the string
                _appSettings.SlothApiUrl = _appSettings.SlothApiUrl.Substring(0, _appSettings.SlothApiUrl.Length - 3);
            }
            var log = Log.ForContext("OrderId", order.Id);
            
            var orderDetails = order.GetOrderDetails();
            var token = _appSettings.SlothApiKey;
            var url = _appSettings.SlothApiUrl;
            if (_aeSettings.UseCoA)
            {
                url = $"{url}v2/"; //Config Change!!!
            }
            else
            {
                url = $"{url}v1/";
            }

            if (string.IsNullOrWhiteSpace(token))
            {
                log.Error("Sloth Token missing");
            }

            var model = new TransactionViewModel();

            if (_aeSettings.UseCoA)
            {
                var creditAccount = _aeSettings.AnlabCoa;
                var detailsChanged = false;
                var savedAccount = orderDetails.Payment.Account;

                if (FinancialChartValidation.GetFinancialChartStringType(orderDetails.Payment.Account) == FinancialChartStringType.Invalid)
                {
                    //Ok, it is an invalid COA format, so 99.99% chance this is still a KFS account. Lets try and convert it:
                    var coa = await _aggieEnterpriseService.ConvertKfsAccount(orderDetails.Payment.Account);
                    if (coa != null)
                    {
                        log.Warning("Payment Account updated to COA, using KFS Convert. Order Id: {id}, KFS Account: {kfs}, COA: {coa}", order.Id, orderDetails.Payment.Account, coa);
                        orderDetails.Payment.AccountName = $"KFS Converted Account: {savedAccount}";
                        orderDetails.Payment.Account = coa; // Assign it here so we can follow through with the validation. Will this get updated in the DB if everything else goes though I think.                        
                        detailsChanged = true;
                    }
                }

                var accountValidation = await _aggieEnterpriseService.IsAccountValid(orderDetails.Payment.Account);
                if (!accountValidation.IsValid)
                {
                    log.Error("Invalid Account: {account}", orderDetails.Payment.Account);
                    return new SlothResponseModel
                    {
                        Success = false,
                        Message = "Invalid Account"
                    };
                }
                if (detailsChanged)
                {
                    order.SaveDetails(orderDetails);
                }

                var debitAccount = orderDetails.Payment.Account;

                model.MerchantTrackingNumber = order.Id.ToString();
                model.MerchantTrackingUrl = $"https://anlab.ucdavis.edu/Reviewer/Details/{order.Id}";
                model.Description = $"{order.RequestNum} - {order.Project}".SpecialTruncation(0,40);
                model.AddMetadata("Project", order.Project);
                model.AddMetadata("RequestNum", order.RequestNum);
                model.AddMetadata("Client Id", order.ClientId);
                if (debitAccount != savedAccount)
                {
                    model.AddMetadata("Converted Account", $"From: {savedAccount} To: {debitAccount}");
                }

                model.Transfers.Add(new TransferViewModel
                {
                    FinancialSegmentString = debitAccount,
                    Amount = orderDetails.GrandTotal,
                    Description = debitAccount != savedAccount ? $"Converted: {savedAccount}".SpecialTruncation(0,40) : $"{order.Project.SpecialTruncation((order.RequestNum.Length + 3), 40)} - {order.RequestNum}",
                    Direction = Directions.Debit,
                });
                model.Transfers.Add(new TransferViewModel
                {
                    FinancialSegmentString = creditAccount,
                    Amount = orderDetails.GrandTotal,
                    Description = $"{order.Project.SpecialTruncation((order.RequestNum.Length + 3), 40)} - {order.RequestNum}",
                    Direction = Directions.Credit,
                });

            }
            else
            {
                var creditAccount = new AccountModel(_appSettings.AnlabAccount);
                var debitAccount = new AccountModel(orderDetails.Payment.Account);


                model.MerchantTrackingNumber = order.Id.ToString();
                model.MerchantTrackingUrl = $"https://anlab.ucdavis.edu/Reviewer/Details/{order.Id}";
                model.Transfers.Add(new TransferViewModel
                {
                    Account = debitAccount.Account.SafeToUpper(),
                    Amount = orderDetails.GrandTotal,
                    Chart = debitAccount.Chart.SafeToUpper(),
                    SubAccount = debitAccount.SubAccount.SafeToUpper(),
                    Description = $"{order.Project.SpecialTruncation((order.RequestNum.Length + 3), 40)} - {order.RequestNum}",
                    Direction = Directions.Debit,
                    ObjectCode = _appSettings.DebitObjectCode
                });
                model.Transfers.Add(new TransferViewModel
                {
                    Account = creditAccount.Account.SafeToUpper(),
                    Amount = orderDetails.GrandTotal,
                    Chart = creditAccount.Chart.SafeToUpper(),
                    SubAccount = creditAccount.SubAccount.SafeToUpper(),
                    Description = $"{order.Project.SpecialTruncation((order.RequestNum.Length + 3), 40)} - {order.RequestNum}",
                    Direction = Directions.Credit,
                    ObjectCode = _appSettings.CreditObjectCode
                });
            }



            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(url);
                client.DefaultRequestHeaders.Add("X-Auth-Token", token);

                log.Information(JsonConvert.SerializeObject(model));

                var response = await client.PostAsync("Transactions", new StringContent(JsonConvert.SerializeObject(model), System.Text.Encoding.UTF8, "application/json"));
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    log.Information($"Sloth Response Not Found for order id {order.Id}");
                }
                if (response.StatusCode == HttpStatusCode.NoContent)
                {
                    log.Information($"Sloth Response No Content for order id {order.Id}");
                }

                if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    log.Error("Sloth Response Bad Request for order {id}", order.Id);
                    var badrequest = await response.Content.ReadAsStringAsync();
                    log.ForContext("data", badrequest, true).Information("Sloth message response");
                    var badRtValue = new SlothResponseModel
                    {
                        Success = false,
                        Message = badrequest
                    };

                    return badRtValue;
                }

                //TODO: Capture errors?

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    log.Information("Sloth Success Response", content);

                    return JsonConvert.DeserializeObject<SlothResponseModel>(content);
                }


                log.Information("Sloth Response didn't have a success code for order {id}", order.Id);
                var badContent = await response.Content.ReadAsStringAsync();
                log.ForContext("data", badContent, true).Information("Sloth message response");
                var rtValue = JsonConvert.DeserializeObject<SlothResponseModel>(badContent);
                rtValue.Success = false;

                return rtValue;
                
            }           
        }

        public async Task MoneyHasMoved()
        {
            var url = _appSettings.SlothApiUrl;
            if (_aeSettings.UseCoA)
            {
                url = $"{url}v2/"; //Config Change!!!
            }
            else
            {
                url = $"{url}v1/";
            }
            
            Log.Information("Beginning UCD money has moved");
            var orders = _dbContext.Orders.Where(a =>
                a.PaymentType == PaymentTypeCodes.UcDavisAccount && a.Paid && a.Status != OrderStatusCodes.Complete).ToList();
            if (orders.Count == 0)
            {
                Log.Information("No UC Davis accounts orders to process");
                return ;
            }
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri($"{url}Transactions/");
                client.DefaultRequestHeaders.Add("X-Auth-Token", _appSettings.SlothApiKey);

                Log.Information($"Processing {orders.Count} orders");
                var updatedCount = 0;
                var roledBackCount = 0;
                foreach (var order in orders)
                {
                    if (!order.SlothTransactionId.HasValue)
                    {
                        Log.Information($"Order {order.Id} missing SlothTransactionId"); //TODO: Log it
                        continue;
                    }
                    var response = await client.GetAsync(order.SlothTransactionId.ToString());
                    if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        Log.Information($"Order {order.Id} NotFound. SlothTransactionId {order.SlothTransactionId.ToString()}"); //TODO: Log it
                        continue;
                    }
                    if (response.StatusCode == HttpStatusCode.NoContent)
                    {
                        Log.Information($"Order {order.Id} NoContent. SlothTransactionId {order.SlothTransactionId.ToString()}"); //TODO: Log it
                        continue;
                    }
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var slothResponse = JsonConvert.DeserializeObject<SlothResponseModel>(content);
                        Log.Information($"Order {order.Id} SlothResponseModel status {slothResponse.Status}. SlothTransactionId {order.SlothTransactionId.ToString()}");
                        if (slothResponse.Status == SlothStatus.Completed)
                        {
                            updatedCount++;
                            order.Status = OrderStatusCodes.Complete;
                            order.History.Add(new History
                            {
                                Action = "Move UCD Money",
                                Status = order.Status,
                                ActorName = "Job",
                                JsonDetails = order.JsonDetails,
                                Notes = "Money Moved",
                            });
                        }
                        if (slothResponse.Status == SlothStatus.Cancelled)
                        {
                            order.Paid = false;
                            order.History.Add(new History
                            {
                                Action = "Move UCD Money",
                                Status = order.Status,
                                ActorName = "Job",
                                JsonDetails = order.JsonDetails,
                                Notes = "Money Movement Cancelled.",
                            });
                            Log.Information($"Order {order.Id} was cancelled. Setting back to unpaid");
                            roledBackCount++;
                            //TODO: Write to the notes field? Trigger off an email?
                        }                        
                    }
                    else
                    {
                        Log.Information($"Order {order.Id} Not Successful. Response code {response.StatusCode}. SlothTransactionId {order.SlothTransactionId.ToString()}"); //TODO: Log it
                    }
                }

                await _dbContext.SaveChangesAsync();
                Log.Information($"Updated {updatedCount} orders. Rolled back {roledBackCount} orders.");
            }
            return;
        }

        public async Task ProcessCreditCards()
        {
            var url = _appSettings.SlothApiUrl;
            if (_aeSettings.UseCoA)
            {
                url = $"{url}v2/"; //Config Change!!!
            }
            else
            {
                url = $"{url}v1/";
            }
            
            Log.Information("Staring Credit Card process");
            var orders = _dbContext.Orders.Include(i => i.ApprovedPayment).Where(a =>
                a.PaymentType == PaymentTypeCodes.CreditCard && a.Paid && a.Status != OrderStatusCodes.Complete && a.ApprovedPayment != null).ToList();
            if (orders.Count == 0)
            {
                Log.Information("No Credit Card orders to process.");
                return;
            }

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri($"{url}Transactions/processor/");
                client.DefaultRequestHeaders.Add("X-Auth-Token", _appSettings.SlothApiKey);

                Log.Information($"Processing Credit Card {orders.Count} orders");
                var updatedCount = 0;
                foreach (var order in orders)
                {
                    var response = await client.GetAsync(order.ApprovedPayment.Transaction_Id);
                    if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        Log.Information($"Credit Card transaction id {order.ApprovedPayment.Transaction_Id} not found for order id {order.Id}");
                        continue;
                    }
                    if (response.StatusCode == HttpStatusCode.NoContent)
                    {
                        Log.Information($"Credit Card transaction id {order.ApprovedPayment.Transaction_Id} no content returned from sloth for order id {order.Id}");
                        continue;
                    }
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var slothResponse = JsonConvert.DeserializeObject<SlothResponseModel>(content);

                        if(slothResponse.Status == SlothStatus.Completed)
                        {
                            updatedCount++;
                            order.KfsTrackingNumber = slothResponse.KfsTrackingNumber;
                            order.SlothTransactionId = slothResponse.Id;
                            order.Status = OrderStatusCodes.Complete;
                            order.History.Add(new History
                            {
                                Action = "Move CC Money",
                                Status = order.Status,
                                ActorName = "Job",
                                JsonDetails = order.JsonDetails,
                            });
                        }
                        else
                        {
                            if(slothResponse.Status == SlothStatus.Rejected)
                            {
                                //This is bad, very very bad.
                                Log.Error($"Credit Card transaction id {order.ApprovedPayment.Transaction_Id} sloth status {slothResponse.Status} was unexpectedly Rejected for order id {order.Id}");
                            }
                            else
                            {
                                Log.Information($"Credit Card transaction id {order.ApprovedPayment.Transaction_Id} sloth status {slothResponse.Status} was not Completed yet for order id {order.Id}");
                            }
                        }

                    }
                    else
                    {
                        Log.Error($"Credit Card transaction id {order.ApprovedPayment.Transaction_Id} sloth response {response.StatusCode} was not success for order id {order.Id}");
                    }
                }

                await _dbContext.SaveChangesAsync();
                Log.Information($"Updated {updatedCount} orders");
            }
            Log.Information("Done Credit Card process");
        }

    }
}
