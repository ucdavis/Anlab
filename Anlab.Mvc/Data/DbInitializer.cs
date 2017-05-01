using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using Anlab.Core.Domain;
using AnlabMvc.Models;
using Microsoft.AspNetCore.Identity;

namespace AnlabMvc.Data
{
    // Run on app startup for now to seed DB with initial values
    public static class DbInitializer
    {
        public static async Task Initialize(ApplicationDbContext context, UserManager<User> userManager)
        {
            context.Database.EnsureCreated();

            if (context.Users.Any()) return; // Do nothing if there is already user data in the system

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

            await userManager.CreateAsync(scottUser);
            await userManager.AddLoginAsync(scottUser, loginInfo);

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

            await userManager.CreateAsync(jasonUser);
            await userManager.AddLoginAsync(jasonUser, jasonLoginInfo);

            // create a new sample order

            var order = new Order
            {
                ClientId = "XYZ",
                CreatorId = scottUser.Id,
                Project = "Test Project",
            };

            context.Add(order);

            await context.SaveChangesAsync();


            // Seed with orders here, and maybe create users to test with
        }
    }
}
