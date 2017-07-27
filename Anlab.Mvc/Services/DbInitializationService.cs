using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Anlab.Core.Data;
using Anlab.Core.Domain;
using AnlabMvc.Models.Roles;
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
            string[] codesList = new[] {"ADFAF",
                "ADFRF",
                "ADIN",
                "AG-W",
                "AG-WT",
                "AL",
                "AL-CIT",
                "AL-KCL",
                "AL-OX",
                "AL-PY",
                "-BCL-P-IC",
                "-BNA-PMF",
                "-DCL-P-IC",
                "-DNA-PMF",
                "-LCL-P-IC",
                "-LNA-PMF",
                "-PCL-P-IC",
                "-PNA-PMF",
                "-SCL-P-IC",
                "-SNA-PMF",
                "CL-P-IC",
                "D",
                "M",
                "NA-PMF",
                "-BNH4-W",
                "-BNO3-W",
                "-DNH4-W",
                "-DNO3-W",
                "-LNH4-W",
                "-LNO3-W",
                "-PNH4-W",
                "-PNO3-W",
                "-SNH4-W",
                "-SNO3-W",
                "G-NA-E",
                "NH4-W",
                "NO3-W",
                "AL-W",
                "AL-WT",
                "ALK-WX",
                "AS-PT",
                "AS-ST",
                "AS-W",
                "AS-WT",
                "ASH",
                "ATM1",
                "ATM10",
                "ATM15",
                "ATM1P5",
                "ATM2",
                "ATM5",
                "ATMP1",
                "ATMP3",
                "ATMP5",
                "ATMP7",
                "AU",
                "B-PMF",
                "B-S",
                "B-SX",
                "B-W",
                "B-WT",
                "BA",
                "BD",
                "BRAY-P",
                "C-P",
                "C-P-CW",
                "C-S",
                "C-S-CW",
                "CA-PMF",
                "CA-S",
                "CA-TOX",
                "CA-W",
                "CA-WT",
                "CA-WX",
                "CACO3",
                "CAFFEINE",
                "CARBAMAZ",
                "CD",
                "CD-S",
                "CD-W",
                "CD-WT",
                "CD_TOT",
                "CEC",
                "CELLULOS",
                "CL-P-IC",
                "CL-S",
                "CL-W",
                "CL-W-IC",
                "CL-WX",
                "CL21-S",
                "CLAY",
                "CO",
                "CO-S",
                "CO_TOT",
                "CO3-S",
                "CO3-W",
                "CR",
                "CR-OXIDE"};

            var codesList2 = codesList.Distinct().ToArray();


            await _context.Database.EnsureCreatedAsync();

            if (_context.Users.Any()) return; // Do nothing if there is already user data in the system

            // create roles
            await _roleManager.CreateAsync(new IdentityRole(RoleCodes.Admin));
            await _roleManager.CreateAsync(new IdentityRole(RoleCodes.User));

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
            await _userManager.AddToRoleAsync(scottUser, RoleCodes.Admin);

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
            await _userManager.AddToRoleAsync(jasonUser, RoleCodes.Admin);
            await _userManager.AddToRoleAsync(jasonUser, RoleCodes.User);

            var lauraUser = new User
            {
                Email = "laholstege@ucdavis.edu",
                UserName = "laholstege@ucdavis.edu",
                Name = "Laura Holstege"
            };

            var lauraUserPrincipal = new ClaimsPrincipal();
            userPrincipal.AddIdentity(new ClaimsIdentity(new[] {
                new Claim(ClaimTypes.NameIdentifier, "holstege"),
                new Claim(ClaimTypes.Name, "Laura Holstege")
            }));
            var lauraLoginInfo = new ExternalLoginInfo(lauraUserPrincipal, "CAS", "holstege", null);

            await _userManager.CreateAsync(lauraUser);
            await _userManager.AddLoginAsync(lauraUser, lauraLoginInfo);
            await _userManager.AddToRoleAsync(lauraUser, RoleCodes.Admin);
            await _userManager.AddToRoleAsync(lauraUser, RoleCodes.User);

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
            await _userManager.AddToRoleAsync(calUser, RoleCodes.Admin);


            #endregion Cal's login

            // create a new sample order

            CreateOrders(jasonUser);
            CreateOrders(scottUser);
            CreateOrders(calUser);
            CreateOrders(lauraUser);

            // create sample tests

            var count = codesList2.Length;

            for (int i = 0; i < count; i++)
            {
                var plantx = new TestItem
                {
                    Analysis = string.Format("Fake Plant{0}", i),
                    Code = codesList[i],
                    InternalCost = 1.22m,
                    ExternalCost = 2.13m,
                    SetupCost = 30,
                    FeeSchedule = "16_0711",
                    Category = TestCategories.Plant,
                    Group = i < 10 ? "SF" : "SS",
                    Multiplier = 1,
                    Notes = i % 3 == 0 ? string.Format("Test Tooltip {0}", i) : string.Empty,
                };
                _context.Add(plantx);
            }


            await _context.SaveChangesAsync();


            // Seed with orders here, and maybe create users to test with
        }

        private void CreateOrders(User user)
        {
            var xxx = @"{""Quantity"":2,""SampleType"":""Plant"",""AdditionalInfo"":"""",""SelectedTests"":[{""Id"":1,""Analysis"":""Fake Plant0"",""Code"":""ADFAF"",""Cost"":42.0,""SetupCost"":45.0,""SubTotal"":84.0,""Total"":129.0},{""Id"":62,""Analysis"":""Fake Plant9"",""Code"":""AL-PY"",""Cost"":33.0,""SetupCost"":45.0,""SubTotal"":66.0,""Total"":111.0},{""Id"":63,""Analysis"":""Fake Plant8"",""Code"":""AL-OX"",""Cost"":33.0,""SetupCost"":45.0,""SubTotal"":66.0,""Total"":111.0}],""Total"":351.0,""Payment"":{""ClientType"":""other"",""Account"":"""",""IsInternalClient"":false},""AdditionalEmails"":[],""Project"":""TestProj"",""LabComments"":"""",""AdjustmentAmount"":0.0}";

            var order = new Order
            {
                ClientId = "XYZ",
                CreatorId = user.Id,
                Project = "Test Project",
                Status = "Created",
                RequestNum = "17P138",
                JsonDetails = xxx
            };


            _context.Add(order);

            order = new Order
            {
                ClientId = "XYZ",
                CreatorId = user.Id,
                Project = "Test Project",
                Status = "Received",
                RequestNum = "17P138",
                JsonDetails = xxx
            };


            _context.Add(order);
        }
    }

}
