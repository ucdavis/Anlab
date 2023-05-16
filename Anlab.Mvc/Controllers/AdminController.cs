using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Anlab.Core.Data;
using Anlab.Core.Domain;
using AnlabMvc.Models.MailMessageModels;
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
    [Authorize(Roles = RoleCodes.Admin + "," + RoleCodes.LabUser)]
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

        [Authorize(Roles = RoleCodes.Admin)]
        public async Task<IActionResult> Index()
        {
            var adminUsers = await _userManager.GetUsersInRoleAsync(RoleCodes.Admin);
            var labUsers = await _userManager.GetUsersInRoleAsync(RoleCodes.LabUser);
            var reportUsers = await _userManager.GetUsersInRoleAsync(RoleCodes.Reports);

            var users = adminUsers.Union(labUsers).Union(reportUsers);

            var usersInRoles = users.Select(u => new UserRolesModel
            {
                User = u,
                IsAdmin = adminUsers.Contains(u),
                IsReports = reportUsers.Contains(u),
                IsLabUser = labUsers.Contains(u)
            }).ToList();

            return View(usersInRoles);
        }

        [Authorize(Roles = RoleCodes.Admin)]
        [HttpGet]
        public async Task<IActionResult> SearchAdminUser(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                ErrorMessage = "Nothing entered to search.";
                return RedirectToAction("Index");
            }
            var user = await _dbContext.Users.SingleOrDefaultAsync(a => a.NormalizedUserName == id.ToUpper().Trim());
            if (user == null)
            {
                ErrorMessage = $"Email {id} not found.";
                return RedirectToAction("Index");
            }

            return RedirectToAction("EditAdmin", new {id = user.Id});
        }

        [Authorize(Roles = RoleCodes.Admin)]
        [HttpGet]
        public async Task<IActionResult> EditAdmin(string id)
        {
            var model = new UserRolesModel();
            model.User = _dbContext.Users.SingleOrDefault(a => a.Id == id);
            if (model.User == null)
            {
                return NotFound();
            }
            model.IsAdmin = await _userManager.IsInRoleAsync(model.User, RoleCodes.Admin);
            model.IsLabUser = await _userManager.IsInRoleAsync(model.User, RoleCodes.LabUser);
            model.IsReports = await _userManager.IsInRoleAsync(model.User, RoleCodes.Reports);

            return View(model);
        }

        //Admin and Lab User Access
        public async Task<IActionResult> ListClients()
        {
            // TODO: filter out admin and lab users
            var users = await _dbContext.Users.AsNoTracking().ToListAsync();

            return View(users);
        }

        //Admin and Lab User Access
        public async Task<IActionResult> EditUser(string id)
        {
            var user = await _dbContext.Users.SingleOrDefaultAsync(a => a.Id == id);
            if (user == null)
            {
                ErrorMessage = "User Not Found.";
                return RedirectToAction("ListClients");
            }

            return View(user);
        }

        //Admin and Lab User Access
        [HttpPost]
        public async Task<IActionResult> EditUser(string id, User user)
        {
            var userToUpdate = await _dbContext.Users.SingleOrDefaultAsync(a => a.Id == id);
            if (userToUpdate == null)
            {
                ErrorMessage = "User Not Found.";
                return RedirectToAction("ListClients");
            }

            if (id != user.Id)
            {
                throw new Exception("User id did not match passed value.");
            }
            if (ModelState.IsValid)
            {
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

                _dbContext.Update(userToUpdate);
                await _dbContext.SaveChangesAsync();

                Message = "User Updated.";

                return RedirectToAction("ListClients");
            }

            ErrorMessage = "The user had invalid data.";

            return View(user);
        }

        [Authorize(Roles = RoleCodes.Admin)]
        [HttpPost]
        public async Task<IActionResult> AddUserToRole(string userId, string role, bool add)
        {
            var user = await _dbContext.Users.SingleAsync(a => a.Id == userId);
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

            return RedirectToAction("EditAdmin", new {id=user.Id});
        }

        //[Authorize(Roles =  RoleCodes.Admin)] //Enable if we need to add roles
        //public async Task<IActionResult> CreateRole(string role) 
        //{
        //    await _roleManager.CreateAsync(new IdentityRole(role));
        //    return Content($"Added {role} Role");
        //}

        public async Task<IActionResult> MailQueue(int? id = null, bool allFailed = false)
        {
            // Right now, show unsent pending emails, failures, and successfully sent within 30 days.
            // TODO: Review filter

            ViewBag.allFailed = allFailed;

            List<MailMessage> messages = null;
            if (allFailed)
            {
                messages = await _dbContext.MailMessages.Include(i => i.Order).Where(x => x.Sent == false).AsNoTracking().ToListAsync();
                return View(messages);
            }
            if (id.HasValue)
            {
                messages = await _dbContext.MailMessages.IgnoreQueryFilters().Include(i => i.Order).Where(x => x.Order.Id == id).AsNoTracking().ToListAsync();
            }
            else
            {
                messages = await _dbContext.MailMessages.Include(i => i.Order).Where(x =>
                    x.Sent == null || !x.Sent.Value || x.Sent.Value && x.SentAt != null &&
                    x.SentAt.Value >= DateTime.UtcNow.AddDays(-30)).AsNoTracking().ToListAsync();
            }
            
            return View(messages);
        }

        public async Task<IActionResult> ViewMessage(int id)
        {
            var message = await _dbContext.MailMessages.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id);
            if (message == null)
            {
                return NotFound();
            }
            return View(message);
        }

        [Authorize(Roles = RoleCodes.Admin)]
        [HttpGet]
        public async Task<IActionResult> FixEmail(int id)
        {
            var mm = await _dbContext.MailMessages.Include(i=> i.Order).AsNoTracking().SingleOrDefaultAsync(x => x.Id == id);
            if (mm == null)
            {
                return NotFound();
            }
            var editMailMessageModel = new EditMailMessageModel(mm);

            return View(editMailMessageModel);
        }

        [Authorize(Roles = RoleCodes.Admin)]
        [HttpPost]
        public async Task<IActionResult> FixEmail(int id, EditMailMessageModel model)
        {
            var mm = await _dbContext.MailMessages.Include(i=> i.Order).SingleOrDefaultAsync(x => x.Id == id);
            if (mm == null || model.Id != id || model.OrderId != mm.Order.Id)
            {
                return NotFound();
            }

            var saveSendTo = mm.SendTo;            

            if (ModelState.IsValid)
            {
                mm.SendTo = model.SendTo;
                mm.Subject = model.Subject;
                if (model.Resend)
                {
                    mm.Sent = null;
                    mm.FailureCount = 0;
                }

                var extraMessage = model.Resend
                    ? "The mail message will attempt to send again."
                    : "You did not choose to try and re-send";

                if (model.Unsend)
                {
                    mm.Sent = true;
                    extraMessage = "Message marked as sent and will not be resent";
                }
                var user = _dbContext.Users.Single(a => a.Id == CurrentUserId);
                var order = mm.Order;

                var historyNote = extraMessage;
                if (saveSendTo != mm.SendTo)
                {
                    historyNote = $"Email List Changed. Original: {saveSendTo} New: {mm.SendTo} . {extraMessage}";
                }

                order.History.Add(new History
                {
                    Action = "Fix Email",
                    Status = order.Status,
                    ActorId = user.NormalizedUserName,
                    ActorName = user.Name,
                    JsonDetails = order.JsonDetails,
                    Notes = historyNote
                });



                await _dbContext.SaveChangesAsync();

                Message = $"Mail Message updated. {extraMessage}";

                return RedirectToAction("MailQueue", "Admin", new{id=model.OrderId});
            }

            ErrorMessage = "Unable to save";

            return View(model);
        }
    }
}
