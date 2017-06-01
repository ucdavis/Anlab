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


            #region Cal's login
            var calUser = new User
            {
                Email = "cydoval@ucdavis.edu",
                UserName = "cydoval@ucdavis.edu",
                Name = "Calvin Doval"
            };

            var calUserPrincipal = new ClaimsPrincipal();
            userPrincipal.AddIdentity(new ClaimsIdentity(new[] {
                new Claim(ClaimTypes.NameIdentifier, "cydoval"),
                new Claim(ClaimTypes.Name, "Calvin Doval")
            }));
            var calLoginInfo = new ExternalLoginInfo(calUserPrincipal, "CAS", "cydoval", null);

            await _userManager.CreateAsync(calUser);
            await _userManager.AddLoginAsync(calUser, calLoginInfo);
            await _userManager.AddToRoleAsync(calUser, "admin");


            #endregion Cal's login

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
                Analysis = "Aluminum (KCl extraction)",
                Code = "Al (KCL)",
                InternalCost = 18,
                ExternalCost = 27,
                SetupCost = 30,
                FeeSchedule = "16_0711",
                Category = TestCategories.Soil,
                Group = "SF",
                Multiplier = 1
            };

            // create sample tests
            var ammonium = new TestItem
            {
                Analysis = "Ammonium-Nitrogen",
                Code = "NH4-N",
                InternalCost = 11,
                ExternalCost = 17,
                SetupCost = 30,
                FeeSchedule = "16_0711",
                Category = TestCategories.Soil,
                Group = "SF",
                Multiplier = 1
            };

            // create sample tests
            var water1 = new TestItem
            {
                Analysis = "Fake Test1",
                Code = "H2O",
                InternalCost = 8.42m,
                ExternalCost = 12.33m,
                SetupCost = 30,
                FeeSchedule = "16_0711",
                Category = TestCategories.Water,
                Group = "SF",
                Multiplier = 1
            };

            // create sample tests
            var water2 = new TestItem
            {
                Analysis = "Fake Test2",
                Code = "H2O-1",
                InternalCost = 9.22m,
                ExternalCost = 32.13m,
                SetupCost = 30,
                FeeSchedule = "16_0711",
                Category = TestCategories.Water,
                Group = "SF",
                Multiplier = 1
            };

            // create sample tests
            var plant1 = new TestItem
            {
                Analysis = "Fake Test3",
                Code = "Fake",
                InternalCost = 1.22m,
                ExternalCost = 2.13m,
                SetupCost = 30,
                FeeSchedule = "16_0711",
                Category = TestCategories.Plant,
                Group = "SF",
                Multiplier = 1
            };

            for (int i = 0; i < 30; i++)
            {
                var plantx = new TestItem
                {
                    Analysis = string.Format("Fake Plant{0}", i),
                    Code = string.Format("Fake{0}", i),
                    InternalCost = 1.22m,
                    ExternalCost = 2.13m,
                    SetupCost = 30,
                    FeeSchedule = "16_0711",
                    Category = TestCategories.Plant,
                    Group = i < 10 ? "SF" : "SS",
                    Multiplier = 1
                };
                _context.Add(plantx);
            }

            for (int i = 0; i < 5; i++)
            {
                var plantx = new TestItem
                {
                    Analysis = string.Format("Fake Other{0}", i),
                    Code = string.Format("Fake{0}", i),
                    InternalCost = 5.22m,
                    ExternalCost = 7.13m,
                    SetupCost = 30,
                    FeeSchedule = "16_0711",
                    Category = TestCategories.Other,
                    Group = i < 10 ? "SF" : "SS",
                    Multiplier = 1
                };
                _context.Add(plantx);
            }

            _context.Add(aluminum);
            _context.Add(ammonium);
            _context.Add(water1);
            _context.Add(water2);
            _context.Add(plant1);

            await _context.SaveChangesAsync();


            // Seed with orders here, and maybe create users to test with
        }

    }

}
