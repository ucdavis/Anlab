using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Anlab.Core.Domain;
using AnlabMvc.Data;
using AnlabMvc.Models.Order;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace AnlabMvc.Services
{
    public interface IOrderService
    {
        Task PopulateOrder(OrderSaveModel model, Order orderToUpdate, bool fromAdmin = false);
        Task SendOrderToAnlab(Order order);
    }

    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;

        public OrderService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task PopulateOrder(OrderSaveModel model, Order orderToUpdate, bool fromAdmin = false)
        {
            orderToUpdate.Project = model.Project;
            orderToUpdate.JsonDetails = JsonConvert.SerializeObject(model);
            var orderDetails = orderToUpdate.GetOrderDetails();
            var testItemIds = orderDetails.SelectedTests.Select(a => a.Id).ToArray();
            var selectedTests = await _context.TestItems.Where(a => testItemIds.Contains(a.Id)).ToListAsync();
            var isUcClient = string.Equals(orderDetails.Payment.ClientType, "uc", StringComparison.OrdinalIgnoreCase);
            if (isUcClient)
            {
                orderDetails.Total = selectedTests.Sum(a => a.InternalCost);
            }
            else
            {
                if (string.Equals(orderDetails.Payment.ClientType, "other", StringComparison.OrdinalIgnoreCase))
                {
                    orderDetails.Total = selectedTests.Sum(a => a.ExternalCost);
                }
                else
                {
                    throw new Exception("What! unknown payment!!!");
                }
            }
            orderDetails.Total = orderDetails.Total * orderDetails.Quantity;
            AddAdditionalFees(orderDetails, isUcClient);
            orderDetails.Total += selectedTests.Sum(a => a.SetupCost);
            if (fromAdmin)
            {
                orderDetails.Total += orderDetails.AdjustmentAmount;
            }
            else
            {
                orderDetails.AdjustmentAmount = 0;
                orderDetails.LabComments = String.Empty;
            }

            orderToUpdate.SaveDetails(orderDetails);
            orderToUpdate.AdditionalEmails = string.Join(";", orderDetails.AdditionalEmails);
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
