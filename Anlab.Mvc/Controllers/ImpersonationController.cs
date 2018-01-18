using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Anlab.Core.Data;
using Anlab.Core.Domain;
using AnlabMvc.Extensions;
using AnlabMvc.Models.Roles;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace AnlabMvc.Controllers
{
    [Authorize]
    public class ImpersonationController : ApplicationController
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public ImpersonationController(ApplicationDbContext context,
            UserManager<User> userManager,
            SignInManager<User> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        [Authorize(Roles = RoleCodes.Admin)]
        public async Task<IActionResult> ImpersonateUser(String email)
        {

            var currentUserId = User.GetUserId();
            Log.Information($"Impersonation Begins: {currentUserId} . Impersonating {email}");

            var impersonatedUser = await _userManager.FindByEmailAsync(email);

            var userPrincipal = await _signInManager.CreateUserPrincipalAsync(impersonatedUser);

            userPrincipal.Identities.First().AddClaim(new Claim("OriginalUserId", currentUserId));
            userPrincipal.Identities.First().AddClaim(new Claim("IsImpersonating", "true"));

            // sign out the current user
            await _signInManager.SignOutAsync();

            await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, userPrincipal);

            return RedirectToAction("Index", "Home");
        }
        [Authorize]
        public async Task<IActionResult> StopImpersonation()
        {
            if (!User.IsImpersonating())
            {
                throw new Exception("You are not impersonating now. Can't stop impersonation");
            }

            var originalUserId = User.FindFirst("OriginalUserId").Value;

            var originalUser = await _userManager.FindByIdAsync(originalUserId);

            await _signInManager.SignOutAsync();

            await _signInManager.SignInAsync(originalUser, isPersistent: true);

            Log.Information($"Impersonation Ends: {originalUserId}");

            return RedirectToAction("Index", "Home");
        }
    }
}
