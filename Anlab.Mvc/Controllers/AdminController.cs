using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Anlab.Core.Domain;
using AnlabMvc.Data;
using AnlabMvc.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AnlabMvc.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : ApplicationController
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(ApplicationDbContext dbContext, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task<IActionResult> Index()
        {
            var adminUsers = await _userManager.GetUsersInRoleAsync("Admin");
            var adminRole = _roleManager.Roles.Single(a => a.Name == "Admin").Id;
            var userRole = _roleManager.Roles.Single(a => a.Name == "User").Id;

            var users = _dbContext.Users.Include(a => a.Roles).ToList();
            
            var usersRoles = new List<UserRolesModel>();
            foreach (var user in users)
            {
                var ur = new UserRolesModel();
                ur.User = user;                
                if (user.Roles.Any(a => a.RoleId == adminRole))
                {
                    ur.IsAdmin = true;
                }
                else
                {
                    ur.IsAdmin = false;
                }
                if (user.Roles.Any(a => a.RoleId == userRole))
                {
                    ur.IsUser = true;
                }
                else
                {
                    ur.IsUser = false;
                }
                usersRoles.Add(ur);
            }           

            return View(usersRoles);
        }

        public IActionResult ListNonAdminUsers()
        {
            var adminRole = _roleManager.Roles.Single(a => a.Name == "Admin").Id;
            var users = _dbContext.Users.Include(a => a.Roles).Where(w => w.Roles.All(x => x.RoleId != adminRole)).ToList();

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
    }
}