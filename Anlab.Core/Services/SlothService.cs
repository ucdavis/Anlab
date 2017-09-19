﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Anlab.Core.Data;
using Anlab.Core.Domain;
using Anlab.Core.Models;
using Anlab.Jobs.MoneyMovement;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Anlab.Core.Services
{
    public interface ISlothService
    {
        Task<SlothResponseModel> MoveMoney(Order order);
        Task<bool> ProcessCreditCards(IConfigurationRoot config);

        //TODO: Move the CreditCard one here.
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

            var objectCode = _appSettings.ObjectCode;

            var model = new TransactionViewModel();
            model.MerchantTrackingNumber = order.Id.ToString();
            model.ProcessorTrackingNumber = order.Id.ToString(); //TODO: Remove once optional
            model.Transfers.Add(new TransferViewModel { Account = debitAccount.Account , Amount = orderDetails.GrandTotal, Chart = debitAccount.Chart, SubAccount = debitAccount.SubAccount, Description = $"{order.Project} - {order.RequestNum}", Direction = "Debit", ObjectCode = objectCode });
            model.Transfers.Add(new TransferViewModel { Account = creditAccount.Account, Amount = orderDetails.GrandTotal, Chart = creditAccount.Chart, SubAccount = creditAccount.SubAccount, Description = $"{order.Project} - {order.RequestNum}", Direction = "Credit", ObjectCode = objectCode });
            //TODO: Is the Object code on the Credit line too?

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

            return null;
        }

        public async Task<bool> ProcessCreditCards(IConfigurationRoot config) //Have to pass here, can't get DI working for the job
        {
            var token = config.GetSection("Financial:SlothApiKey").Value;
            var url = config.GetSection("Financial:SlothApiUrl").Value;
            var orders = _dbContext.Orders.Include(i => i.ApprovedPayment).Where(a =>
                a.ApprovedPayment != null && a.Paid && a.Status != OrderStatusCodes.Complete).ToList();
            if (orders.Count == 0)
            {
                return false;
            }

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri($"{url}Transactions/processor/");
                client.DefaultRequestHeaders.Add("X-Auth-Token", token);

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
