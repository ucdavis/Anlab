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
        private readonly FinancialSettings _appSettings;

        public SlothService(ApplicationDbContext dbContext, IOptions<FinancialSettings> appSettings)
        {
            _dbContext = dbContext;
            _appSettings = appSettings.Value;
        }



        //TODO: Add validation?
        public async Task<SlothResponseModel> MoveMoney(Order order)
        {
            var orderDetails = order.GetOrderDetails();
            var token = _appSettings.SlothApiKey;
            var url = _appSettings.SlothApiUrl;
            var creditAccount = new AccountModel(_appSettings.AnlabAccount);
            var debitAccount = new AccountModel(orderDetails.Payment.Account);

            if (string.IsNullOrWhiteSpace(token))
            {
                Log.Error("Sloth Token missing");
            }


            var model = new TransactionViewModel();
            model.MerchantTrackingNumber = order.Id.ToString();
            model.MerchantTrackingUrl = $"https://anlab.ucdavis.edu/Reviewer/Details/{order.Id}";
            model.Transfers.Add(new TransferViewModel { Account = debitAccount.Account.SafeToUpper() , Amount = orderDetails.GrandTotal, Chart = debitAccount.Chart.SafeToUpper(), SubAccount = debitAccount.SubAccount.SafeToUpper(), Description = $"{order.Project} - {order.RequestNum}", Direction = "Debit", ObjectCode = _appSettings.DebitObjectCode });
            model.Transfers.Add(new TransferViewModel { Account = creditAccount.Account.SafeToUpper(), Amount = orderDetails.GrandTotal, Chart = creditAccount.Chart.SafeToUpper(), SubAccount = creditAccount.SubAccount.SafeToUpper(), Description = $"{order.Project} - {order.RequestNum}", Direction = "Credit", ObjectCode = _appSettings.CreditObjectCode });

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(url);
                client.DefaultRequestHeaders.Add("X-Auth-Token", token);

                Log.Information(JsonConvert.SerializeObject(model));

                var response = await client.PostAsync("Transactions", new StringContent(JsonConvert.SerializeObject(model), System.Text.Encoding.UTF8, "application/json"));
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    Log.Information($"Sloth Response Not Found for order id {order.Id}");
                }
                if (response.StatusCode == HttpStatusCode.NoContent)
                {
                    Log.Information($"Sloth Response No Content for order id {order.Id}");
                }

                //TODO: Capture errors?

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<SlothResponseModel>(content);

                }


                Log.Information("Sloth Response didn't have a success code for order {id}", order.Id);
                var badContent = await response.Content.ReadAsStringAsync();                    
                Log.ForContext("data", badContent, true).Information("Sloth message response");
                var rtValue = JsonConvert.DeserializeObject<SlothResponseModel>(badContent);
                rtValue.Success = false;

                return rtValue;
                
            }           
        }

        public async Task MoneyHasMoved()
        {
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
                client.BaseAddress = new Uri($"{_appSettings.SlothApiUrl}Transactions/");
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
                        if (slothResponse.Status == "Completed")
                        {
                            updatedCount++;
                            order.Status = OrderStatusCodes.Complete;
                        }
                        if (slothResponse.Status == "Cancelled")
                        {
                            order.Paid = false;
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
            Log.Information("Staring Credit Card process");
            var orders = _dbContext.Orders.Include(i => i.ApprovedPayment).Where(a =>
                a.PaymentType == PaymentTypeCodes.CreditCard && a.Paid && a.Status != OrderStatusCodes.Complete).ToList();
            if (orders.Count == 0)
            {
                Log.Information("No Credit Card orders to process.");
                return;
            }

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri($"{_appSettings.SlothApiUrl}Transactions/processor/");
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

                        updatedCount++;
                        order.KfsTrackingNumber = slothResponse.KfsTrackingNumber;
                        order.SlothTransactionId = slothResponse.Id;
                        order.Status = OrderStatusCodes.Complete;
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
