﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Anlab.Core.Data;
using Anlab.Core.Domain;
using Anlab.Core.Models;
using AnlabMvc.Models.Order;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace AnlabMvc.Services
{
    public interface IOrderService
    {
        void PopulateOrder(OrderSaveModel model, Order orderToUpdate);
        Task SendOrderToAnlab(Order order);

        Task<List<TestItemModel>> PopulateTestItemModel(bool showAll = false);

        Task<OverwriteOrderResult> OverwiteOrderWithTestsCompleted(Order orderToUpdate);
    }

    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILabworksService _labworksService;
        private readonly AppSettings _appSettings;

        public OrderService(ApplicationDbContext context, ILabworksService labworksService, IOptions<AppSettings> appSettings)
        {
            _context = context;
            _labworksService = labworksService;            
            _appSettings = appSettings.Value;
        }

        /// <summary>
        /// A list of all our db test items with the labwork prices
        /// </summary>
        /// <returns></returns>
        public async Task<List<TestItemModel>> PopulateTestItemModel(bool showAll = false)
        {
            var prices = await _labworksService.GetPrices();
            var items = _context.TestItems.AsNoTracking();
            if (!showAll)
            {
                items = items.Where(a => a.Public);
            }

            return GetJoined(prices, items.ToList());
        }

        /// <summary>
        /// A list of test items with the labwork prices for a set of test ids
        /// </summary>
        /// <param name="selectedTestIds"></param>
        /// <returns></returns>
        private IList<TestItemModel> PopulateSelectedTestsItemModel(IEnumerable<string> selectedTestIds, IList<TestItemModel> allTests)
        {
            return allTests.Where(a => selectedTestIds.Contains(a.Id)).ToList();
        }

        private List<TestItemModel> GetJoined(IList<TestItemPrices> prices, List<TestItem> items)
        {
            return (from i in items
                join p in prices on i.Id equals p.Id
                select new TestItemModel
                {
                    Analysis = i.Analysis,
                    Category = i.Category,
                    ExternalCost = Math.Ceiling(p.Cost * _appSettings.NonUcRate),
                    Group = i.Group,
                    Id = i.Id,
                    InternalCost = Math.Ceiling(p.Cost),
                    ExternalSetupCost = Math.Ceiling(p.SetupPrice * _appSettings.NonUcRate),
                    InternalSetupCost = Math.Ceiling(p.SetupPrice),
                    Notes = i.NotesEncoded,
                    Public = i.Public
                }).ToList();
        }

        /// <summary>
        /// For all the tests in the order in our db, update prices, calculate the test items (internal/external, etc.)
        /// </summary>
        /// <param name="orderDetails"></param>
        /// <returns></returns>
        private TestDetails[] CalculateTestDetails(Order order)
        {
            var orderDetails = order.GetOrderDetails();
            var allTests = order.GetTestDetails();
            // TODO: Do we really want to match on ID, or Code, or some combination?
            var selectedTestIds = orderDetails.SelectedTests.Select(t => t.Id);
            var tests = PopulateSelectedTestsItemModel(selectedTestIds, allTests);

            var calcualtedTests = new List<TestDetails>();

            foreach (var test in orderDetails.SelectedTests)
            {
                var dbTest = tests.Single(t => t.Id == test.Id);

                CalculateTest(orderDetails, dbTest, calcualtedTests);
            }

            return calcualtedTests.ToArray();
        }

        /// <summary>
        /// Get the test items from Labworks and replace the test items in our order to reflect what was actually done.
        /// </summary>
        /// <param name="orderToUpdate"></param>
        /// <returns></returns>
        public async Task<OverwriteOrderResult> OverwiteOrderWithTestsCompleted(Order orderToUpdate)
        {
            var rtValue = new OverwriteOrderResult();
            if (string.IsNullOrWhiteSpace(orderToUpdate.RequestNum))
            {
                throw new Exception("RequestNum not populated"); //TODO: Something better
            }
            var testCodes = await _labworksService.GetTestCodesCompletedForOrder(orderToUpdate.RequestNum);

            var allTests = orderToUpdate.GetTestDetails();

            var testIds = _context.TestItems.Where(a => testCodes.Contains(a.Id)).Select(s => s.Id).ToArray(); 
            var tests = PopulateSelectedTestsItemModel(testIds, allTests);

            if (testCodes.Count != testIds.Length)
            {
                //Oh No!!! tests were returned that we don't know about
                var foundCodes = _context.TestItems.Where(a => testIds.Contains(a.Id)).Select(s => s.Id).Distinct().ToList();
                rtValue.MissingCodes = testCodes.Except(foundCodes).ToList();
                
                return rtValue;
            }

            var orderDetails = orderToUpdate.GetOrderDetails();
            var calcualtedTests = new List<TestDetails>();
            foreach (var test in tests)
            {
                CalculateTest(orderDetails, test, calcualtedTests);
            }

            rtValue.SelectedTests = calcualtedTests;

            return rtValue;
        }

        /// <summary>
        /// Calculate the prices for each individual test
        /// </summary>
        /// <param name="orderDetails"></param>
        /// <param name="test"></param>
        /// <param name="calcualtedTests"></param>
        private static void CalculateTest(OrderDetails orderDetails, TestItemModel test, List<TestDetails> calcualtedTests)
        {
            var cost = orderDetails.Payment.IsInternalClient ? test.InternalCost : test.ExternalCost;
            var costAndQuantity = cost * orderDetails.Quantity;

            calcualtedTests.Add(new TestDetails
            {
                Id = test.Id,
                Analysis = test.Analysis,
                SetupCost = orderDetails.Payment.IsInternalClient ? test.InternalSetupCost : test.ExternalSetupCost,
                Cost = cost,
                SubTotal = costAndQuantity,
                Total = costAndQuantity + (orderDetails.Payment.IsInternalClient
                            ? test.InternalSetupCost
                            : test.ExternalSetupCost)
            });
        }

        public void PopulateOrder(OrderSaveModel model, Order orderToUpdate)
        {
            orderToUpdate.Project = model.Project;
            orderToUpdate.ClientId = model.ClientId;

            orderToUpdate.JsonDetails = JsonConvert.SerializeObject(model);
            var orderDetails = orderToUpdate.GetOrderDetails();

            var tests = CalculateTestDetails(orderToUpdate);

            orderDetails.SelectedTests = tests.ToArray();
            orderDetails.Total = orderDetails.SelectedTests.Sum(x => x.Total) + (orderDetails.Payment.ClientType == "uc" ? orderDetails.InternalProcessingFee : orderDetails.ExternalProcessingFee);

            orderToUpdate.SaveDetails(orderDetails);

            orderToUpdate.AdditionalEmails = string.Join(";", orderDetails.AdditionalEmails);
        }

        public async Task SendOrderToAnlab(Order order)
        {
            await Task.Yield(); //TODO: Remove
            //throw new NotImplementedException();
        }

    }
}

