using System;
using System.Collections.Generic;
using Anlab.Core.Data;
using Anlab.Core.Models;
using AnlabMvc.Models.Order;
using AnlabMvc.Models.Roles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Anlab.Core.Extensions;
using Anlab.Core.Domain;
using AnlabMvc.Models.Reviewer;
using Newtonsoft.Json;
using AnlabMvc.Services;
using System.Linq.Expressions;

namespace AnlabMvc.Controllers
{
    [Authorize(Roles = RoleCodes.Admin + "," + RoleCodes.Reports)]
    public class ReviewerController : ApplicationController
    {
        private readonly ApplicationDbContext _context;
        private readonly ILabworksService _labworksService;

        public ReviewerController(ApplicationDbContext context, ILabworksService labworksService)
        {
            _context = context;
            _labworksService = labworksService;
        }

        public async Task<IActionResult> Index()
        {
            var lastActions = DateTime.UtcNow.AddDays(-60);

            var reviewerOrder = await _context.ReviewerOrderViews.Where(a =>
                a.Status == OrderStatusCodes.Finalized ||
                (a.Status == OrderStatusCodes.Complete &&  a.Updated >= lastActions)).AsNoTracking().ToListAsync();
            
            return View(reviewerOrder);
        }

        public async Task<IActionResult> Totals(ReviewerTotalModel model)
        {
            if (model.fStart == null && model.fEnd == null && model.cStart == null && model.cEnd == null)
            {
                Message = "Please select a filter.";
                model.Orders = new List<ReviewerOrderView>();
                return View(model);
            }

            var orders = _context.ReviewerOrderViews.Where(a => a.DateFinalized != null).AsQueryable();
            if (model.fStart != null)
            {
                orders = orders.Where(a => a.DateFinalized >= model.fStart.Value.Date.FromPacificTime());
            }

            if (model.fEnd != null)
            {
                orders = orders.Where(a => a.DateFinalized <= model.fEnd.Value.Date.AddDays(1).FromPacificTime());
            }

            if (model.cStart != null)
            {
                orders = orders.Where(a => a.DateFinalized >= model.cStart.Value.Date.FromPacificTime());
            }

            if (model.cEnd != null)
            {
                orders = orders.Where(a => a.DateFinalized <= model.cEnd.Value.Date.AddDays(1).FromPacificTime());
            }

            model.Orders = await orders.ToListAsync();

            return View(model);
        }

        public async Task<IActionResult> Adjustments(AdjustmentsModel model)
        {
            if (model.fStart == null && model.fEnd == null && model.cStart == null && model.cEnd == null)
            {
                Message = "Please select a filter.";
                model.Orders = new List<AdjustmentsView>();
                return View(model);
            }

            var orders = _context.AdjustmentsView.Where(a => a.DateFinalized != null).AsQueryable();
            if (model.fStart != null)
            {
                orders = orders.Where(a => a.DateFinalized >= model.fStart.Value.Date.FromPacificTime());
            }

            if (model.fEnd != null)
            {
                orders = orders.Where(a => a.DateFinalized <= model.fEnd.Value.Date.AddDays(1).FromPacificTime());
            }

            if (model.cStart != null)
            {
                orders = orders.Where(a => a.DateFinalized >= model.cStart.Value.Date.FromPacificTime());
            }

            if (model.cEnd != null)
            {
                orders = orders.Where(a => a.DateFinalized <= model.cEnd.Value.Date.AddDays(1).FromPacificTime());
            }

            model.Orders = await orders.ToListAsync();

            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            var order = await _context.Orders.IgnoreQueryFilters().Include(i => i.Creator).SingleOrDefaultAsync(o => o.Id == id && o.Status != OrderStatusCodes.Created);

            if (order == null)
            {
                return NotFound();
            }

            var model = new OrderReviewModel();
            model.Order = order;
            model.OrderDetails = order.GetOrderDetails();
            model.HideLabDetails = false;

            await GetHistories(id, model);

            //Will be null if not finished
            model.LabworksFinished = await _labworksService.IsFinishedInLabworks(order.RequestNum);
            await CheckFinalEmailSent(id, model);

            return View(model);
        }

        public async Task<IActionResult> EmailList(DateTime? start, DateTime? end, string emailType, string orderAction)
        {
            var model = new EmailListModel();
            if (start == null && end == null)
            {
                model.Start = DateTime.UtcNow.ToPacificTime().Date.AddDays(-30);
                model.End = DateTime.UtcNow.ToPacificTime().Date;
            }
            else
            {
                model.Start = start?.Date;
                model.End = end?.Date;
            }
            model.EmailType = emailType ?? "PI";
            model.OrderAction = orderAction ?? "Finalized";

            //Could potentially use the history table, but them it needs a foreign key to the order table
            var query = _context.MailMessages.Include(a => a.Order).ThenInclude(a => a.Creator).Include(a => a.Order).AsQueryable();
            if (model.Start != null)
            {
                query = query.Where(a => a.CreatedAt >= model.Start.Value.Date.FromPacificTime());
            }
            if (model.End != null)
            {
                query = query.Where(a => a.CreatedAt <= model.End.Value.Date.AddDays(1).FromPacificTime());
            }
            if(model.OrderAction == "Confirmed")
            {
                query = query.Where(a => a.Subject == "Work Order Confirmation" || a.Subject.StartsWith("Work Request Confirmation"));
            }
            else
            {
                query = query.Where(a => a.Subject.StartsWith("Work Request Finalized"));
            }
            model.EmailAddresses = await query.Select(EmailListModel.Projection(model.EmailType == "PI")).Distinct().Where(a => a != null && a != string.Empty).ToListAsync();
          
            return View(model);
        }


        public async Task<IActionResult> HistoricalSales(DateTime? start, DateTime? end)
        {
            var model = new HistoricalSalesModel();
            if (start == null && end == null)
            {
                model.Start = new DateTime(DateTime.UtcNow.Year, 1, 1).Date;
                model.End = DateTime.UtcNow.ToPacificTime().Date;
            }
            else
            {
                model.Start = start?.Date;
                model.End = end?.Date;
            }

            var query = _context.HistoricalSalesViews.AsQueryable();
            if (model.Start != null)
            {
                query = query.Where(a => a.DateFinalized >= model.Start.Value.Date.FromPacificTime());
            }
            if (model.End != null)
            {
                query = query.Where(a => a.DateFinalized <= model.End.Value.Date.AddDays(1).FromPacificTime());
            }

            var results = await query.AsNoTracking().ToListAsync(); //Grab all the results for the date range for processing

            foreach (var item in results)
            {
                if (item.IsInternal)
                {
                    if (item.InternalProcessingFee > 0)
                    {
                        AddOrCreateTest(model.Rows, "ProcessingFee", "*** Processing Fee ***", item.IsInternal, 1, item.InternalProcessingFee);
                    }
                }
                else
                {
                    if (item.ExternalProcessingFee > 0)
                    {
                        AddOrCreateTest(model.Rows, "ProcessingFee", "*** Processing Fee ***", item.IsInternal, 1, item.ExternalProcessingFee);
                    }
                }
                //serialize item.SelectedTests into a list of TestDetails
                var tests = JsonConvert.DeserializeObject<List<TestDetails>>(item.SelectedTests);
                foreach (var test in tests)
                {
                    if (test.SetupCost > 0)
                    {
                        AddOrCreateTest(model.Rows, "SetupCost", "*** Setup Cost ***", item.IsInternal, 1, test.SetupCost);
                    }
                    AddOrCreateTest(model.Rows, test.Id, test.Analysis, item.IsInternal, item.Quantity, test.SubTotal);
                }

            }

            return View(model);
        }

        public async Task<IActionResult> OrdersWithSkippedFinalEmail()
        {
            var model = await _context.Orders.Where(a => a.SkippedFinalEmail).OrderBy(a => a.DateFinalized).Select(OrderProjection()).ToListAsync();
            return View(model);
        }

        //create a projection from orders to ReviewerOrderView
        private  Expression<Func<Order, ReviewerOrderView>> OrderProjection()
        {
            return o => new ReviewerOrderView
            {
                Id = o.Id,
                RequestNum = o.RequestNum,
                ClientId = o.ClientId,
                PaymentType = o.PaymentType,
                Created = o.Created,
                Updated = o.Updated,
                Status = o.Status,
                Paid = o.Paid,
                DateFinalized = o.DateFinalized,
                GrandTotal = 0.0m //Ignored
            };
        }


        private void AddOrCreateTest(List<HistoricalSalesRowModel> rows, string testCode, string analysis, bool isInternal, int quantity, decimal total)
        {
            if (!rows.Any(a => a.TestCode == testCode))
            {
                rows.Add(new HistoricalSalesRowModel
                {
                    TestCode = testCode,
                    Analysis = analysis,
                    InternalQuantity = 0,
                    ExternalQuantity = 0,
                    InternalTotal = 0.00m,
                    ExternalTotal = 0.00m
                });
            }
            var row = rows.Single(a => a.TestCode == testCode);
            if (isInternal)
            {
                row.InternalQuantity += quantity;
                row.InternalTotal += total;
            }
            else
            {
                row.ExternalQuantity += quantity;
                row.ExternalTotal += total;
            }
        }


        private async Task GetHistories(int id, OrderReviewModel model)
        {
            model.History = await _context.History.Where(a => a.OrderId == id).Select(s =>
                new History
                {
                    Action = s.Action,
                    ActionDateTime = s.ActionDateTime,
                    Id = s.Id,
                    Status = s.Status,
                    ActorId = s.ActorId,
                    ActorName = s.ActorName,
                    Notes = s.Notes
                }).OrderBy(o => o.ActionDateTime).ToListAsync(); //Basically filtering out jsonDetails
        }

        private async Task CheckFinalEmailSent(int id, OrderReviewModel model)
        {
            //Get subjects of emails send for this order
            var emails = await _context.MailMessages.Where(a => a.Order.Id == id).Select(s => s.Subject).ToListAsync();

            model.WasFinalEmailSent = emails.Any(a => a.StartsWith("Work Request Finalized"));
            model.WasFinalEmailSkipped = emails.Any(a => a.StartsWith("Work Request Finalized") && a.EndsWith("Bypass Client"));
        }
    }
}
