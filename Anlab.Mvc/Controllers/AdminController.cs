using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Anlab.Core.Data;
using Anlab.Core.Domain;
using AnlabMvc.Models.Order;
using AnlabMvc.Models.Roles;
using AnlabMvc.Models.User;
using AnlabMvc.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AnlabMvc.Controllers
{
    [Authorize(Roles = RoleCodes.Admin + "," + RoleCodes.User)]
    public class AdminController : ApplicationController
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IOrderService _orderService;

        public AdminController(ApplicationDbContext dbContext, UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IOrderService orderService)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _roleManager = roleManager;
            _orderService = orderService;
        }

        [Authorize(Roles = RoleCodes.Admin)]
        public async Task<IActionResult> Index()
        {
            // TODO: find better way than super select
            var usersInRoles = _dbContext.Users.Select(u => new UserRolesModel { User = u }).ToList();

            foreach (var userRole in usersInRoles)
            {

                userRole.IsAdmin = await _userManager.IsInRoleAsync(userRole.User, RoleCodes.Admin);
                userRole.IsUser = await _userManager.IsInRoleAsync(userRole.User, RoleCodes.User);

            }

            return View(usersInRoles);
        }

        public async Task<IActionResult> ListNonAdminUsers()
        {
            // TODO: handle if there are other non-admin roles
            var users = await _userManager.GetUsersInRoleAsync(RoleCodes.User);

            return View(users);
        }

        public IActionResult EditUser(string id)
        {
            var user = _dbContext.Users.SingleOrDefault(a => a.Id == id);
            if (user == null)
            {
                ErrorMessage = "User Not Found";
                return RedirectToAction("ListNonAdminUsers");
            }

            return View(user);
        }

        [HttpPost]
        public IActionResult EditUser(string id, [Bind("FirstName,LastName,Name,Phone,Account,ClientId")]User user)
        {
            var userToUpdate = _dbContext.Users.SingleOrDefault(a => a.Id == id);
            if (userToUpdate == null)
            {
                ErrorMessage = "User Not Found";
                return RedirectToAction("ListNonAdminUsers");
            }
            if (ModelState.IsValid)
            {
                userToUpdate.FirstName = user.FirstName;
                userToUpdate.LastName = user.LastName;
                userToUpdate.Name = user.Name;
                userToUpdate.Phone = user.Phone;
                userToUpdate.Account = user.Account?.ToUpper();
                userToUpdate.ClientId = user.ClientId;

                _dbContext.Update(userToUpdate);
                _dbContext.SaveChanges();

                return RedirectToAction("ListNonAdminUsers");
            }

            return View(user);
        }

        [Authorize(Roles = RoleCodes.Admin)]
        [HttpPost]
        public async Task<IActionResult> AddUserToRole(string userId, string role, bool add)
        {
            var user = _dbContext.Users.Single(a => a.Id == userId);
            if (add)
            {
                await _userManager.AddToRoleAsync(user, role);
            }
            else
            {
                if (user.Id == CurrentUserId)
                {
                    ErrorMessage = "Can't remove your own permissions.";
                    return RedirectToAction("Index");
                }
                await _userManager.RemoveFromRoleAsync(user, role);
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> MailQueue()
        {
            // Right now, show unsent pending emails, failures, and successfully sent within 30 days.
            // TODO: Review filter

            var messages = await _dbContext.MailMessages.Where(x => x.Sent == null || !x.Sent.Value || x.Sent.Value && x.SentAt != null && x.SentAt.Value >= DateTime.UtcNow.AddDays(-30)).AsNoTracking().ToListAsync();
            return View(messages);
        }

        public IActionResult ViewMessage(int id)
        {

            var message = _dbContext.MailMessages.AsNoTracking().SingleOrDefault(x => x.Id == id);
            return View(message);
        }
    }
}