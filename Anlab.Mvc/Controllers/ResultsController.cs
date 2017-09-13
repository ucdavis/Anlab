using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Anlab.Core.Data;
using Microsoft.EntityFrameworkCore;
using AnlabMvc.Models.Order;
using AnlabMvc.Services;
using Microsoft.AspNetCore.Authorization;

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

        public async Task<IActionResult> Link(Guid id)
        {
            var order = await _context.Orders.Include(i => i.Creator).SingleOrDefaultAsync(o => o.ShareIdentifier == id);

            if (order == null)
            {
                return NotFound();
            }

            if (order.Status != OrderStatusCodes.Complete)
            {
                return NotFound();
            }

            var model = new OrderReviewModel();
            model.Order = order;
            model.OrderDetails = order.GetOrderDetails();

            return View(model);
        }

        public async Task<IActionResult> Download(Guid id)
        {
            var order = await _context.Orders.SingleOrDefaultAsync(o => o.ShareIdentifier == id);

            var result = await _fileStorageService.GetSharedAccessSignature(order.ResultsFileIdentifier);
            return Redirect(result.AccessUrl);
        }
    }
}