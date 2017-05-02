using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Anlab.Core.Domain;
using AnlabMvc.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace AnlabMvc.Services
{
    public interface IDbInitializationService
    {
        Task Initialize();
        Task RecreateAndInitialize();
    }

    public class DbInitializationService : IDbInitializationService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DbInitializationService(ApplicationDbContext context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task RecreateAndInitialize()
        {
            await _context.Database.EnsureDeletedAsync();

            await Initialize();
        }

        public async Task Initialize()
        {
            await _context.Database.EnsureCreatedAsync();

            if (_context.Users.Any()) return; // Do nothing if there is already user data in the system

            // create roles
            await _roleManager.CreateAsync(new IdentityRole("admin"));
            await _roleManager.CreateAsync(new IdentityRole("user"));

            var scottUser = new User
            {
                Email = "srkirkland@ucdavis.edu",
                UserName = "srkirkland@ucdavis.edu",
                Name = "Scott Kirkland"
            };

            var userPrincipal = new ClaimsPrincipal();
            userPrincipal.AddIdentity(new ClaimsIdentity(new[] {
                new Claim(ClaimTypes.NameIdentifier, "postit"),
                new Claim(ClaimTypes.Name, "Scott Kirkland")
            }));
            var loginInfo = new ExternalLoginInfo(userPrincipal, "CAS", "postit", null);

            await _userManager.CreateAsync(scottUser);
            await _userManager.AddLoginAsync(scottUser, loginInfo);
            await _userManager.AddToRoleAsync(scottUser, "admin");

            var jasonUser = new User
            {
                Email = "jsylvestre@ucdavis.edu",
                UserName = "jsylvestre@ucdavis.edu",
                Name = "Jason Sylvestre"
            };

            var jasonUserPrincipal = new ClaimsPrincipal();
            userPrincipal.AddIdentity(new ClaimsIdentity(new[] {
                new Claim(ClaimTypes.NameIdentifier, "jsylvest"),
                new Claim(ClaimTypes.Name, "Jason Sylvestre")
            }));
            var jasonLoginInfo = new ExternalLoginInfo(jasonUserPrincipal, "CAS", "jsylvest", null);

            await _userManager.CreateAsync(jasonUser);
            await _userManager.AddLoginAsync(jasonUser, jasonLoginInfo);
            await _userManager.AddToRoleAsync(jasonUser, "admin");

            // create a new sample order

            var order = new Order
            {
                ClientId = "XYZ",
                CreatorId = scottUser.Id,
                Project = "Test Project",
            };

            _context.Add(order);

            // create sample tests
            var aluminum = new TestItem
            {
                Id = 3030,
                Analysis = "Aluminum (KCl extraction)",
                Code = "Al (KCL)",
                InternalCost = 18,
                ExternalCost = 27,
                FeeSchedule = "16_0711",
                Multiplier = 1
            };

            _context.Add(aluminum);

            await _context.SaveChangesAsync();


            // Seed with orders here, and maybe create users to test with
        }

    }

}
