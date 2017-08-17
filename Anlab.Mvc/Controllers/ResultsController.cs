using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Anlab.Core.Data;
using Microsoft.EntityFrameworkCore;
using AnlabMvc.Models.Order;
using AnlabMvc.Services;

namespace AnlabMvc.Controllers
{
    public class ResultsController : ApplicationController
    {
        private readonly ApplicationDbContext _context;
        private readonly IFileStorageService _fileStorageService;

        public ResultsController(ApplicationDbContext context, IFileStorageService fileStorageService)
        {
            _context = context;
            _fileStorageService = fileStorageService;
        }

        public async Task<IActionResult> Index()
        {
            var model = await _context.Orders.Where(a => a.CreatorId == CurrentUserId && a.Status == OrderStatusCodes.Complete)
                .ToArrayAsync();

            return View(model);
        }

        public async Task<IActionResult> Link(Guid id)
        {
            var order = await _context.Orders.Include(i => i.Creator).SingleOrDefaultAsync(o => o.ShareIdentifier == id);

            if (order == null)
            {
                ErrorMessage = "id: " + id;
                return RedirectToAction("Index");
            }

            if (order.Status != OrderStatusCodes.Complete)
            {
                ErrorMessage = "You can only view results of complete orders.";
                return RedirectToAction("Index");
            }

            var model = new OrderReviewModel();
            model.Order = order;
            model.OrderDetails = order.GetOrderDetails();
            model.TestItems = _context.TestItems
                        .Where(a => model.OrderDetails.SelectedTests.Select(s => s.Id).Contains(a.Id)).ToList();

            return View(model);
        }

        public async Task<IActionResult> Download(Guid id)
        {
            var order = await _context.Orders.SingleOrDefaultAsync(o => o.ShareIdentifier == id);

            var result = await _fileStorageService.GetSharedAccessSignature(order.ResultsFileIdentifier);
            return Redirect(result.UploadUrl);
        }
    }
}