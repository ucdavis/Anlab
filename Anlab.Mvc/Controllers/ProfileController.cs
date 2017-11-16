using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Anlab.Core.Data;
using Anlab.Core.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AnlabMvc.Controllers
{
    [Authorize]
    public class ProfileController : ApplicationController
    {
        private readonly ApplicationDbContext _context;

        public ProfileController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Users.SingleOrDefaultAsync(x=>x.Id == CurrentUserId));
        }

        public async Task<IActionResult> Edit()
        {
            return View(await _context.Users.SingleOrDefaultAsync(x => x.Id == CurrentUserId));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("FirstName,LastName,Name,Phone,Account,ClientId")]User user)
        {
            var userToUpdate = await _context.Users.SingleOrDefaultAsync(x => x.Id == CurrentUserId);

            if (ModelState.IsValid)
            {
                userToUpdate.FirstName = user.FirstName;
                userToUpdate.LastName = user.LastName;
                userToUpdate.Name = user.Name;
                userToUpdate.Phone = user.Phone;
                userToUpdate.Account = user.Account?.ToUpper();
                userToUpdate.ClientId = user.ClientId;

                _context.Update(userToUpdate);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            return View(user);
        }
    }
}
