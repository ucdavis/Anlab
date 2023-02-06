using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Anlab.Core.Data;
using Anlab.Core.Domain;
using Anlab.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AnlabMvc.Controllers
{
    [Authorize]
    public class ProfileController : ApplicationController
    {
        private readonly ApplicationDbContext _context;
        private readonly AggieEnterpriseSettings _AeSettings;

        public ProfileController(ApplicationDbContext context, IOptions<AggieEnterpriseSettings> aeSettings)
        {
            _context = context;
            _AeSettings = aeSettings.Value;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Users.SingleOrDefaultAsync(x=>x.Id == CurrentUserId));
        }

        public async Task<IActionResult> Edit()
        {
            ViewBag.UseCoA = _AeSettings.UseCoA;
            return View(await _context.Users.SingleOrDefaultAsync(x => x.Id == CurrentUserId));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(User user)
        {
            ViewBag.UseCoA = _AeSettings.UseCoA;
            var userToUpdate = await _context.Users.SingleOrDefaultAsync(x => x.Id == CurrentUserId);

            if (ModelState.IsValid)
            {
                userToUpdate.Email = user.Email.ToLower();
                userToUpdate.NormalizedEmail = user.Email.ToUpper();
                userToUpdate.FirstName = user.FirstName;
                userToUpdate.LastName = user.LastName;
                userToUpdate.Name = user.Name;
                userToUpdate.Phone = user.Phone;
                userToUpdate.Account = user.Account?.ToUpper();
                userToUpdate.ClientId = user.ClientId?.ToUpper();
                userToUpdate.CompanyName = user.CompanyName;
                userToUpdate.BillingContactName = user.BillingContactName;
                userToUpdate.BillingContactAddress = user.BillingContactAddress;
                userToUpdate.BillingContactEmail = user.BillingContactEmail;
                userToUpdate.BillingContactPhone = user.BillingContactPhone;

                _context.Update(userToUpdate);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            return View(user);
        }
    }
}
