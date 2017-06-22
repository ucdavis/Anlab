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
            var adminRole = await _roleManager.FindByNameAsync(RoleCodes.Admin);
            var userRole = await _roleManager.FindByNameAsync(RoleCodes.User);

            var usersRoles = _dbContext.Users.Select(u => new UserRolesModel
            {
                User = u,
                IsAdmin = u.Roles.Any(a => a.RoleId == adminRole.Id),
                IsUser = u.Roles.Any(a => a.RoleId == userRole.Id)
            }).ToList();

            return View(usersRoles);
        }

        public async Task<IActionResult> ListNonAdminUsers()
        {
            var adminRole = await _roleManager.FindByNameAsync(RoleCodes.Admin);
            var users = _dbContext.Users.Where(w => w.Roles.All(x => x.RoleId != adminRole.Id)).ToList();

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
        [ValidateAntiForgeryToken]
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
                userToUpdate.Account = user.Account;
                userToUpdate.ClientId = user.ClientId;

                _dbContext.Update(userToUpdate);
                _dbContext.SaveChanges();

                return RedirectToAction("ListNonAdminUsers");
            }

            return View(user);
        }

        [Authorize(Roles = RoleCodes.Admin)]
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
            // Right now, show unsent pending emails.
            // TODO: show recently sent

            var messages = await _dbContext.MailMessages.Where(x => x.Sent == null).AsNoTracking().ToListAsync();

            return View(messages);
        }

        public async Task<IActionResult> ViewMessage(int id)
        {

            var messages = await _dbContext.MailMessages.Where(x => x.Id == id).AsNoTracking().ToListAsync();



            return View(messages[0]);
        }
    }
}