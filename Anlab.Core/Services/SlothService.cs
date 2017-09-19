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

namespace Anlab.Core.Services
{
    public interface ISlothService
    {
        Task<SlothResponseModel> MoveMoney(Order order);
        Task<bool> ProcessCreditCards(FinancialSettings financialSettings);
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

            var model = new TransactionViewModel();
            model.MerchantTrackingNumber = order.Id.ToString();
            model.ProcessorTrackingNumber = order.Id.ToString(); //TODO: Remove once optional
            model.Transfers.Add(new TransferViewModel { Account = debitAccount.Account , Amount = orderDetails.GrandTotal, Chart = debitAccount.Chart, SubAccount = debitAccount.SubAccount, Description = $"{order.Project} - {order.RequestNum}", Direction = "Debit", ObjectCode = _appSettings.DebitObjectCode });
            model.Transfers.Add(new TransferViewModel { Account = creditAccount.Account, Amount = orderDetails.GrandTotal, Chart = creditAccount.Chart, SubAccount = creditAccount.SubAccount, Description = $"{order.Project} - {order.RequestNum}", Direction = "Credit", ObjectCode = _appSettings.CreditObjectCode });

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(url);
                client.DefaultRequestHeaders.Add("X-Auth-Token", token);

                var response = await client.PostAsync("Transactions", new StringContent(JsonConvert.SerializeObject(model), System.Text.Encoding.UTF8, "application/json"));
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    //Console.WriteLine("No tFound");
                }
                if (response.StatusCode == HttpStatusCode.NoContent)
                {
                    //Console.WriteLine("No Content");
                }

                //TODO: Capture errors?

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<SlothResponseModel>(content);

                }

                //var content2 = await response.Content.ReadAsStringAsync();
                //var xxx = JsonConvert.DeserializeObject(content2);

            }

            return new SlothResponseModel { Success = false };
        }

        public async Task<bool> ProcessCreditCards(FinancialSettings financialSettings) //Have to pass here, can't get DI working for the job
        {
            var orders = _dbContext.Orders.Include(i => i.ApprovedPayment).Where(a =>
                !a.IsInternalClient && a.Paid && a.Status != OrderStatusCodes.Complete).ToList();
            if (orders.Count == 0)
            {
                return false;
            }

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri($"{financialSettings.SlothApiUrl}Transactions/processor/");
                client.DefaultRequestHeaders.Add("X-Auth-Token", financialSettings.SlothApiKey);

                Console.WriteLine($"Processing {orders.Count} orders");
                var updatedCount = 0;
                foreach (var order in orders)
                {
                    var response = await client.GetAsync(order.ApprovedPayment.Transaction_Id);
                    if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        continue;
                    }
                    if (response.StatusCode == HttpStatusCode.NoContent)
                    {
                        continue;
                    }
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var slothResponse = JsonConvert.DeserializeObject<SlothResponseModel>(content);

                        updatedCount++;
                        order.KfsTrackingNumber = slothResponse.KfsTrackingNumber;
                        order.SlothTransactionId = slothResponse.Id.ToString();
                        order.Status = OrderStatusCodes.Complete;
                    }
                }

                await _dbContext.SaveChangesAsync();
                Console.WriteLine($"Updated {updatedCount} orders");
            }

            return true;
        }
    }
}
