using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Anlab.Core.Data;
using Anlab.Core.Domain;
using Anlab.Core.Models;
using AnlabMvc.Models.Order;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace AnlabMvc.Services
{
    public interface IOrderService
    {
        Task PopulateOrder(OrderSaveModel model, Order orderToUpdate);
        void PopulateOrderWithLabDetails(OrderSaveModel model, Order orderToUpdate);
        Task SendOrderToAnlab(Order order);
    }

    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;

        public OrderService(ApplicationDbContext context)
        {
            _context = context;
        }

        private IEnumerable<TestDetails> CalculateTestDetails(OrderDetails orderDetails)
        {
            var isUcClient = string.Equals(orderDetails.Payment.ClientType, "uc", StringComparison.OrdinalIgnoreCase);

            foreach (var test in orderDetails.SelectedTests)
            {
                var cost = isUcClient ? test.InternalCost : test.ExternalCost;
                var costAndQuantity = cost * orderDetails.Quantity;

                yield return new TestDetails
                {
                    Analysis = test.Analysis,
                    Code = test.Code,
                    SetupCost = test.SetupCost,
                    InternalCost = test.InternalCost,
                    ExternalCost = test.ExternalCost,
                    Cost = cost,
                    SubTotal = costAndQuantity,
                    Total = costAndQuantity + test.SetupCost
                };
            }
        }
        public async Task PopulateOrder(OrderSaveModel model, Order orderToUpdate)
        {
            // TODO: get test items from DB to make sure we are using proper $$
            orderToUpdate.Project = model.Project;
            orderToUpdate.JsonDetails = JsonConvert.SerializeObject(model);
            var orderDetails = orderToUpdate.GetOrderDetails();

            var isUcClient = string.Equals(orderDetails.Payment.ClientType, "uc", StringComparison.OrdinalIgnoreCase);

            var tests = CalculateTestDetails(orderDetails).ToArray();

            orderDetails.SelectedTests = tests;
            orderDetails.Total = tests.Sum(x=>x.Total);

            AddAdditionalFees(orderDetails, isUcClient);

            orderToUpdate.SaveDetails(orderDetails);
            orderToUpdate.AdditionalEmails = string.Join(";", orderDetails.AdditionalEmails);
        }

        public void PopulateOrderWithLabDetails(OrderSaveModel model, Order orderToUpdate)
        {
            var orderDetails = orderToUpdate.GetOrderDetails();
            orderDetails.Total += orderDetails.AdjustmentAmount;
            orderToUpdate.SaveDetails(orderDetails);
        }

        public async Task SendOrderToAnlab(Order order)
        {
            //TODO: Implement this.
            return;
        }

        private static void AddAdditionalFees(OrderDetails orderDetails, bool isUcClient)
        {
            if (string.Equals(orderDetails.SampleType, "Water", StringComparison.OrdinalIgnoreCase))
            {
                if (orderDetails.FilterWater)
                {
                    orderDetails.Total += orderDetails.Quantity * (isUcClient ? 11 : 17);
                }
            }
            if (string.Equals(orderDetails.SampleType, "Soil", StringComparison.OrdinalIgnoreCase))
            {
                if (orderDetails.Grind)
                {
                    orderDetails.Total += orderDetails.Quantity * (isUcClient ? 6 : 9);
                }
                if (orderDetails.ForeignSoil)
                {
                    orderDetails.Total += orderDetails.Quantity * (isUcClient ? 9 : 14);
                }
            }
            if (string.Equals(orderDetails.SampleType, "Plant", StringComparison.OrdinalIgnoreCase))
            {
                if (orderDetails.Grind)
                {
                    orderDetails.Total += orderDetails.Quantity * (isUcClient ? 6 : 9);
                }
            }
        }
    }
}
