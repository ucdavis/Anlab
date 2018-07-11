using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anlab.Core.Data;
using Anlab.Core.Domain;
using Anlab.Core.Extensions;
using Anlab.Core.Models;
using Anlab.Jobs.MoneyMovement;
using AnlabMvc.Extensions;
using AnlabMvc.Models.Order;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using AnlabMvc.Helpers;
using Serilog;

namespace AnlabMvc.Services
{
    public interface IOrderService
    {
        void PopulateOrder(OrderSaveModel model, Order orderToUpdate);
        void UpdateAdditionalInfo(Order order);
        Task SendOrderToAnlab(Order order);

        Task<List<TestItemModel>> PopulateTestItemModel(bool showAll = false);

        Task<OverwriteOrderResult> OverwriteOrderFromDb(Order orderToUpdate);

        Task UpdateTestsAndPrices(Order orderToUpdate);

        Task<Order> DuplicateOrder(Order orderToCopy);
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
                orderby i.RequestOrder
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
                    Public = i.Public,
                    AdditionalInfoPrompt = i.AdditionalInfoPrompt,
                    Sop = p.Sop,
                    RequestOrder = i.RequestOrder,
                    LabOrder = i.LabOrder,
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
            var tests = PopulateSelectedTestsItemModel(selectedTestIds, allTests).OrderBy(a => a.LabOrder);

            var calculatedTests = new List<TestDetails>();

            foreach (var test in tests)
            {
                CalculateTest(orderDetails, test, calculatedTests);
            }

            return calculatedTests.ToArray();
        }

        public async Task UpdateTestsAndPrices(Order orderToUpdate)
        {
            var allTests = await PopulateTestItemModel(true);
            orderToUpdate.SaveTestDetails(allTests);

            var tests = CalculateTestDetails(orderToUpdate);

            var orderDetails = orderToUpdate.GetOrderDetails();
            orderDetails.SelectedTests = tests.ToArray();
            orderDetails.Total = orderDetails.SelectedTests.Sum(x => x.Total) + (orderDetails.Payment.IsInternalClient ? orderDetails.InternalProcessingFee : orderDetails.ExternalProcessingFee);

            orderToUpdate.SaveDetails(orderDetails);
        }

        public async Task<Order> DuplicateOrder(Order orderToCopy)
        {
            var order = new Order();
            order.Status = OrderStatusCodes.Created;
            var allTests = await PopulateTestItemModel();
            order.SaveTestDetails(allTests);

            order.Project = orderToCopy.Project;
            order.ClientId = order.ClientId;
            order.ClientName = order.ClientName;
            order.JsonDetails = orderToCopy.JsonDetails;
            var orderDetails = order.GetOrderDetails();
            var tests = CalculateTestDetails(order).ToList();

            var groupTestIds = tests.Where(a => a.Id.StartsWith("G-")).Select(a => a.Id).ToArray();

            if (groupTestIds.Any())
            {
                var testsToRemove = await _labworksService.GetTestsForDiscountedGroups(groupTestIds);
                if (testsToRemove.Any())
                {
                    tests.RemoveAll(a => testsToRemove.Contains(a.Id));
                }
            }

            orderDetails.SelectedTests = tests.ToArray();
            orderDetails.Total = orderDetails.SelectedTests.Sum(x => x.Total) + (orderDetails.Payment.IsInternalClient ? orderDetails.InternalProcessingFee : orderDetails.ExternalProcessingFee);

            order.SaveDetails(orderDetails);

            order.AdditionalEmails = string.Join(";", orderDetails.AdditionalEmails);

            //order.AdditionalEmails = AdditionalEmailsHelper.AddClientInfoEmails(order, orderDetails.ClientInfo); //Maybe need to add person duplicating?

            order.PaymentType = orderToCopy.PaymentType;

            //May not need this because of the CalculateTestDetailsAbove. TODO: Test 1) When test is missing, 2) when test code is different price, 3) when test is not public.
            //foreach (var orderDetailsSelectedTest in orderDetails.SelectedTests)
            //{
            //    if (!tests.Any(a => a.Id == orderDetailsSelectedTest.Id && a.Public))
            //    {
            //        orderDetails.SelectedTests.Remove(orderDetailsSelectedTest);
            //    }
            //}

            return order;
        }

        /// <summary>
        /// Get the test items from Labworks and replace the test items in our order to reflect what was actually done.
        /// </summary>
        /// <param name="orderToUpdate"></param>
        /// <returns></returns>
        public async Task<OverwriteOrderResult> OverwriteOrderFromDb(Order orderToUpdate)
        {
            var rtValue = new OverwriteOrderResult();
            if (string.IsNullOrWhiteSpace(orderToUpdate.RequestNum))
            {
                throw new Exception("RequestNum not populated"); //TODO: Something better
            }

            OrderUpdateFromDbModel orderFromDb = null;
            try
            {
                orderFromDb = await _labworksService.GetRequestDetails(orderToUpdate.RequestNum);
            }
            catch (Exception e)
            {
                rtValue.ErrorMessage = e.Message;
                return rtValue;
            }

            var allTests = orderToUpdate.GetTestDetails();
            var restoreTests = orderToUpdate.GetBackedupTestDetails();
            if (restoreTests.Any())
            {
                foreach (var restoreTest in restoreTests)
                {
                    var test = allTests.FirstOrDefault(a => a.Id == restoreTest.Id);
                    if (test == null)
                    {
                        Log.Information($"Test not found to restore out: {restoreTest}");
                        continue;
                    }

                    test = restoreTest.ShallowCopy();
                }
            }

            var testIds = allTests.Where(a => orderFromDb.TestCodes.Contains(a.Id)).Select(s => s.Id).ToArray();
            var groupTestIds = testIds.Where(a => a.StartsWith("G-")).ToArray();

            if (groupTestIds.Any())
            {
                var savedPrices = new List<TestItemModel>();
                var testsToZeroOut = await _labworksService.GetTestsForDiscountedGroups(groupTestIds);
                foreach (var zeroTest in testsToZeroOut)
                {
                    var test = allTests.FirstOrDefault(a => a.Id == zeroTest);
                    if (test == null)
                    {
                        Log.Information($"Test not found to zero out: {zeroTest}");
                        continue;
                    }
                    savedPrices.Add(test.ShallowCopy());
                    test.ExternalCost = 0;
                    test.InternalCost = 0;
                    test.ExternalSetupCost = 0;
                    test.InternalSetupCost = 0;
                }

                if (savedPrices.Any())
                {
                    rtValue.BackedupTests = savedPrices;
                }
            }

            var tests = PopulateSelectedTestsItemModel(testIds, allTests);

            if (orderFromDb.TestCodes.Count != testIds.Length)
            {
                //Oh No!!! tests were returned that we don't know about
                var foundCodes = allTests.Where(a => testIds.Contains(a.Id)).Select(s => s.Id).Distinct().ToList();
                rtValue.MissingCodes = orderFromDb.TestCodes.Except(foundCodes).ToList();
                
                return rtValue;
            }


            var orderDetails = orderToUpdate.GetOrderDetails();
            orderDetails.Quantity = orderFromDb.Quantity;
            orderDetails.LabworksSampleDisposition = orderFromDb.Disposition; //Don't think this is doing anything

            var calcualtedTests = new List<TestDetails>();
            foreach (var test in tests)
            {
                CalculateTest(orderDetails, test, calcualtedTests);
            }

            rtValue.SelectedTests = calcualtedTests;
            rtValue.ClientId = orderFromDb.ClientId;
            rtValue.Quantity = orderFromDb.Quantity;
            rtValue.RushMultiplier = orderFromDb.RushMultiplier;
            rtValue.LabworksSampleDisposition = orderFromDb.Disposition;

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
            orderToUpdate.ClientId = model.ClientInfo.ClientId;
            orderToUpdate.ClientName = model.ClientInfo.Name;

            orderToUpdate.JsonDetails = JsonConvert.SerializeObject(model);
            var orderDetails = orderToUpdate.GetOrderDetails();
            orderDetails.Payment.Account = orderDetails.Payment.Account.SafeToUpper(); //Make sure any account is all upper case

            var tests = CalculateTestDetails(orderToUpdate);

            orderDetails.SelectedTests = tests.ToArray();
            orderDetails.Total = orderDetails.SelectedTests.Sum(x => x.Total) + (orderDetails.Payment.IsInternalClient ? orderDetails.InternalProcessingFee : orderDetails.ExternalProcessingFee);

            orderToUpdate.SaveDetails(orderDetails);

            orderToUpdate.AdditionalEmails = string.Join(";", orderDetails.AdditionalEmails);

            orderToUpdate.AdditionalEmails = AdditionalEmailsHelper.AddClientInfoEmails(orderToUpdate, orderDetails.ClientInfo);

            if (orderDetails.Payment.IsInternalClient)
            {
                var account = new AccountModel(orderDetails.Payment.Account);
                if (account.Chart == "3" || account.Chart == "L" || account.Chart == "H" || account.Chart == "M") //Removed S as a choice and added H to match _checkUcChart in react code
                {
                    orderToUpdate.PaymentType = PaymentTypeCodes.UcDavisAccount;
                }
                else
                {
                    orderToUpdate.PaymentType = PaymentTypeCodes.UcOtherAccount;
                }
            }
            else
            {
                if (orderDetails.OtherPaymentInfo == null || string.IsNullOrWhiteSpace(orderDetails.OtherPaymentInfo.PaymentType))
                {
                    orderToUpdate.PaymentType = PaymentTypeCodes.CreditCard;
                }
                else
                {
                    orderToUpdate.PaymentType = PaymentTypeCodes.Other;
                }
            }
        }

        public void UpdateAdditionalInfo(Order order)
        {
            var orderDetails = order.GetOrderDetails();

            StringBuilder sbPre = new StringBuilder();
            StringBuilder sb = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(orderDetails.AdditionalInfo))
            {
                sbPre.AppendFormat("{0}{1}", "Client Comments:", Environment.NewLine);
                sbPre.AppendLine(orderDetails.AdditionalInfo);
            }

            if (orderDetails.SampleType == TestCategories.Plant)
            {
                sb.AppendFormat("{0}: {1}{2}", "Plant reporting basis", orderDetails.SampleTypeQuestions.PlantReportingBasis, Environment.NewLine);
            }

            //To do this now, we have to look at the selected tests...
            if (orderDetails.SampleType == TestCategories.Soil)
            {
                sb.AppendFormat("{0}: {1}{2}", "Soil is imported", orderDetails.SelectedTests.Any(a => a.Id == "SP-FOR" || a.Analysis.Equals("Imported Soil", StringComparison.InvariantCultureIgnoreCase)).ToYesNoString(), Environment.NewLine);
            }

            if (orderDetails.SampleType == TestCategories.Water)
            {
                sb.AppendFormat("{0}: {1}{2}", "Water filtered", orderDetails.SampleTypeQuestions.WaterFiltered.ToYesNoString(), Environment.NewLine);
                sb.AppendFormat("{0}: {1} {2}{3}", "Water preservative added", orderDetails.SampleTypeQuestions.WaterPreservativeAdded.ToYesNoString(), orderDetails.SampleTypeQuestions.WaterPreservativeInfo, Environment.NewLine);
                sb.AppendFormat("{0}: {1}{2}", "Water reported in mg/L", orderDetails.SampleTypeQuestions.WaterReportedInMgL.ToYesNoString(), Environment.NewLine);
            }

            if (orderDetails.AdditionalInfoList != null)
            {
                foreach (var item in orderDetails.AdditionalInfoList)
                {
                    if (orderDetails.SelectedTests.Any(a => a.Id == item.Key))
                    {
                        sb.AppendFormat("{0}: {1}{2}", item.Key, item.Value, Environment.NewLine);
                    }
                }
                orderDetails.AdditionalInfoList = new Dictionary<string, string>();
            }

            if (sb.Length > 0)
            {
                sbPre.AppendFormat("{0}{1}", "Automatically Added:", Environment.NewLine);
            }
            orderDetails.AdditionalInfo = sbPre.ToString() + sb.ToString();

            order.SaveDetails(orderDetails);
        }

        public async Task SendOrderToAnlab(Order order)
        {
            await Task.Yield(); //TODO: Remove
            //throw new NotImplementedException();
        }

    }
}

