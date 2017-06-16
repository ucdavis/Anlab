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

        public IActionResult OpenOrders()
        {
            //TODO: update this when we know status. Add filter?
            var orders = _dbContext.Orders.Where(a => a.Status != OrderStatusCodes.Created && a.Status != OrderStatusCodes.Complete)
                .Include(i => i.Creator).ToList();

            return View(orders);
        }

        public async Task<IActionResult> Details(int id)
        {
            var order = await _dbContext.Orders.Include(i => i.Creator).SingleOrDefaultAsync(o => o.Id == id && o.Status != OrderStatusCodes.Created);

            if (order == null)
            {
                return NotFound(id);
            }

            var model = new OrderReviewModel();
            model.Order = order;
            model.OrderDetails = order.GetOrderDetails();

            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var order = await _dbContext.Orders.SingleOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound(id);
            }


            var model = new OrderEditModel
            {
                TestItems = _dbContext.TestItems.AsNoTracking().ToArray(),
                Order = order
            };

            return View(model);
        }

        public IActionResult ListUsersOrders(string id)
        {
            var orders = _dbContext.Orders.Where(a => a.CreatorId == id && a.Status != OrderStatusCodes.Created).ToArray();
            return View(orders);
        }

        [HttpPost]
        public async Task<IActionResult> Save([FromBody]OrderSaveModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = new List<string>();
                foreach (var result in ModelState.Values)
                {
                    foreach (var errs in result.Errors)
                    {
                        errors.Add(errs.ErrorMessage);
                    }
                }

                //TODO: A better way to return errors. Or maybe not, they shouldn't really ever happen.
                return Json(new { success = false, message = "There were problems with your order. Unable to save. Errors: " + string.Join("--", errors) });
            }

            var idForRedirection = 0;

            if (model.OrderId.HasValue)
            {
                var orderToUpdate = await _dbContext.Orders.SingleAsync(a => a.Id == model.OrderId.Value);

                await _orderService.PopulateOrder(model, orderToUpdate);


                idForRedirection = model.OrderId.Value;
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                return Json(new { success = false, message = "Order Id not found." });
            }


            return Json(new { success = true, id = idForRedirection });
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
    }
}