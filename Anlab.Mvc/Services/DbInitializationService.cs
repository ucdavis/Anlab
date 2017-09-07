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
using System.Text.Encodings.Web;
using AnlabMvc.Models.Order;

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
            await _roleManager.CreateAsync(new IdentityRole(RoleCodes.Admin));
            await _roleManager.CreateAsync(new IdentityRole(RoleCodes.User));
            await _roleManager.CreateAsync(new IdentityRole(RoleCodes.Accounts));

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

            LoadTestItems();



            await _context.SaveChangesAsync();


            // Seed with orders here, and maybe create users to test with
        }

        private void CreateOrders(User user)
        {
            var xxx = @"{""Quantity"":2,""SampleType"":""Soil"",""AdditionalInfo"":""Sample"",""SelectedTests"":[{""Id"":""PH-S"",""Analysis"":""pH"",""Cost"":20.0,""SetupCost"":45.0,""SubTotal"":40.0,""Total"":85.0},{""Id"":""EC-S"",""Analysis"":""EC"",""Cost"":20.0,""SetupCost"":45.0,""SubTotal"":40.0,""Total"":85.0},{""Id"":""ESP-S"",""Analysis"":""ESP"",""Cost"":0.0,""SetupCost"":0.0,""SubTotal"":0.0,""Total"":0.0},{""Id"":""HCO3-S"",""Analysis"":""HCO3"",""Cost"":20.0,""SetupCost"":45.0,""SubTotal"":40.0,""Total"":85.0},{""Id"":""GRIND"",""Analysis"":""Grind"",""Cost"":9.0,""SetupCost"":45.0,""SubTotal"":18.0,""Total"":63.0},{""Id"":""SP-FOR"",""Analysis"":""Imported Soil"",""Cost"":14.0,""SetupCost"":0.0,""SubTotal"":28.0,""Total"":28.0}],""Total"":346.0,""Payment"":{""ClientType"":""other"",""Account"":null,""IsInternalClient"":false},""AdditionalEmails"":[],""Project"":""2"",""LabComments"":null,""AdjustmentAmount"":0.0,""GrandTotal"":346.0, ""ClientId"":""XYZ"", ""InternalProcessingFee"":30,""ExternalProcessingFee"":45}";

            var order = new Order
            {
                ClientId = "XYZ",
                CreatorId = user.Id,
                Project = "Test Project",
                Status = OrderStatusCodes.Created,
                RequestNum = "17P138",
                JsonDetails = xxx,
                ShareIdentifier = Guid.NewGuid()
            };
            _context.Add(order);

            order = new Order
            {
                ClientId = "XYZ",
                CreatorId = user.Id,
                Project = "Test Project",
                Status = OrderStatusCodes.Confirmed,
                RequestNum = "17P138",
                JsonDetails = xxx,
                ShareIdentifier = Guid.NewGuid()
            };
            _context.Add(order);

            order = new Order
            {
                ClientId = "XYZ",
                CreatorId = user.Id,
                Project = "Test Project",
                Status = OrderStatusCodes.Received,
                RequestNum = "17P138",
                JsonDetails = xxx,
                ShareIdentifier = Guid.NewGuid()
            };
            _context.Add(order);

            order = new Order
            {
                ClientId = "XYZ",
                CreatorId = user.Id,
                Project = "Test Project",
                Status = OrderStatusCodes.Accepted,
                RequestNum = "17P138",
                JsonDetails = xxx,
                ShareIdentifier = Guid.NewGuid()
            };
            _context.Add(order);

            order = new Order
            {
                ClientId = "XYZ",
                CreatorId = user.Id,
                Project = "Test Project",
                Status = OrderStatusCodes.Accepted,
                Paid = true,
                RequestNum = "17P138",
                JsonDetails = xxx,
                ShareIdentifier = Guid.NewGuid()
            };
            _context.Add(order);

            order = new Order
            {
                ClientId = "XYZ",
                CreatorId = user.Id,
                Project = "Test Project",
                Status = OrderStatusCodes.Complete,
                RequestNum = "17P138",
                JsonDetails = xxx,
                ShareIdentifier = Guid.NewGuid()
            };
            _context.Add(order);
        }

        private void LoadTestItems()
        {
            CreateTestItem("PROC", "Processing Fee", TestCategories.Other, "Special", null, false);

            //Meh
            //CreateTestItem("()", "Just to get it to pass", TestCategories.Soil, "Special", null, false); //We now filter this out in the query
            CreateTestItem("-BCL-P-IC", "Just to get it to pass", TestCategories.Soil, "Special", null, false);
            CreateTestItem("-BNA-PMF", "Just to get it to pass", TestCategories.Soil, "Special", null, false);
            CreateTestItem("-DCL-P-IC", "Just to get it to pass", TestCategories.Soil, "Special", null, false);
            CreateTestItem("-DNA-PMF", "Just to get it to pass", TestCategories.Soil, "Special", null, false);
            CreateTestItem("-LCL-P-IC", "Just to get it to pass", TestCategories.Soil, "Special", null, false);
            CreateTestItem("-LNA-PMF", "Just to get it to pass", TestCategories.Soil, "Special", null, false);
            CreateTestItem("-PCL-P-IC", "Just to get it to pass", TestCategories.Soil, "Special", null, false);
            CreateTestItem("-PNA-PMF", "Just to get it to pass", TestCategories.Soil, "Special", null, false);
            CreateTestItem("-SCL-P-IC", "Just to get it to pass", TestCategories.Soil, "Special", null, false);
            CreateTestItem("-SNA-PMF", "Just to get it to pass", TestCategories.Soil, "Special", null, false);
            CreateTestItem("D", "Just to get it to pass", TestCategories.Soil, "Special", null, false);
            CreateTestItem("M", "Just to get it to pass", TestCategories.Soil, "Special", null, false);

            //Soil
            CreateTestItem("GRIND", "Grind", string.Format("{0}|{1}|{2}", TestCategories.Soil, TestCategories.Plant,TestCategories.Other ), "Special");
            CreateTestItem("SP-FOR", "Imported Soil", TestCategories.Soil, "Special");

            CreateTestItem("#SALIN", "Soil Salinity Group 1 [SP, pH, EC, Ca, Mg, Na, Cl, B, HCO3, CO3]", TestCategories.Soil, "DISCOUNTED GROUPS:");
            CreateTestItem("#SALIN-2", "Soil Salinity Group 2 [SP, pH, EC, Ca, Mg, Na, Cl, B]", TestCategories.Soil, "DISCOUNTED GROUPS:");
            CreateTestItem("#FERT", "Soil Fertility Group 1 [NO3-N, Olsen-P, X-K]", TestCategories.Soil, "DISCOUNTED GROUPS:");
            CreateTestItem("#FERT2", "Soil Fertility Group 2 [NO3-N, Olsen-P, X-K, X-Na, X-Ca, X-Mg, OM (LOI), pH, CEC (Estimated)]", TestCategories.Soil, "DISCOUNTED GROUPS:");
            CreateTestItem("#NC-S", "Total Nitrogen & Carbon [N, C]", TestCategories.Soil, "DISCOUNTED GROUPS:");
            CreateTestItem("#NAF-S", "Nitrate & Ammonium [NO3-N, NH4-N]", TestCategories.Soil, "DISCOUNTED GROUPS:");
            CreateTestItem("#XCAT", "Exchangeable Cations [X-K, X-Na, X-Ca, X-Mg]", TestCategories.Soil, "DISCOUNTED GROUPS:");
            CreateTestItem("#MICRE", "Extractable Micronutrients [DTPA: Zn, Mn, Fe, Cu]", TestCategories.Soil, "DISCOUNTED GROUPS:");
            CreateTestItem("#AD-MICR", "Acid Digestible Micronutrients [Zn, Mn, Fe, Cu]", string.Format("{0}|{1}", TestCategories.Soil, TestCategories.Water), "DISCOUNTED GROUPS:");

            CreateTestItem("TOC-S", "TOC", TestCategories.Soil, "FERTILITY:");
            CreateTestItem("C-S", "C", TestCategories.Soil, "FERTILITY:");
            CreateTestItem("N-CE-S", "N", TestCategories.Soil, "FERTILITY:");
            CreateTestItem("TKN-S", "TKN", TestCategories.Soil, "FERTILITY:");
            CreateTestItem("NO3-S", "NO3-N", TestCategories.Soil, "FERTILITY:");
            CreateTestItem("NH4F-S", "NH4-N", TestCategories.Soil, "FERTILITY:");
            CreateTestItem("BRAY-P", "Bray-P", TestCategories.Soil, "FERTILITY:");
            CreateTestItem("P-OS", "Olsen-P", TestCategories.Soil, "FERTILITY:");
            CreateTestItem("SO4-S", "SO4-S", TestCategories.Soil, "FERTILITY:");
            CreateTestItem("#XK-X", "X-K", TestCategories.Soil, "FERTILITY:");
            CreateTestItem("X-NA", "X-Na", TestCategories.Soil, "FERTILITY:");
            CreateTestItem("X-CA", "X-Ca", TestCategories.Soil, "FERTILITY:");
            CreateTestItem("X-MG", "X-Mg", TestCategories.Soil, "FERTILITY:");
            CreateTestItem("ZN-S", "Zn (DTPA)", TestCategories.Soil, "FERTILITY:");
            CreateTestItem("MN-S", "Mn (DTPA)", TestCategories.Soil, "FERTILITY:");
            CreateTestItem("FE-S", "Fe (DTPA)", TestCategories.Soil, "FERTILITY:");
            CreateTestItem("CU-S", "Cu (DTPA)", TestCategories.Soil, "FERTILITY:");
            CreateTestItem("AL-KCL", "Al (KCl Extraction)", TestCategories.Soil, "FERTILITY:");

            CreateTestItem("SP", "SP", TestCategories.Soil, "SAT PASTE EXT:");
            CreateTestItem("PH-S", "pH", TestCategories.Soil, "SAT PASTE EXT:");
            CreateTestItem("EC-S", "EC", TestCategories.Soil, "SAT PASTE EXT:");
            CreateTestItem("SAR-S", "SAR", TestCategories.Soil, "SAT PASTE EXT:");
            CreateTestItem("ESP-S", "ESP", TestCategories.Soil, "SAT PASTE EXT:", "We use our special powers to figure this out.");
            CreateTestItem("CA-S", "Ca", TestCategories.Soil, "SAT PASTE EXT:");
            CreateTestItem("MG-S", "Mg", TestCategories.Soil, "SAT PASTE EXT:");
            CreateTestItem("NA-S", "Na", TestCategories.Soil, "SAT PASTE EXT:");
            CreateTestItem("CL-S", "Cl", TestCategories.Soil, "SAT PASTE EXT:");
            CreateTestItem("B-S", "B", TestCategories.Soil, "SAT PASTE EXT:");
            CreateTestItem("K-SOLS", "K", TestCategories.Soil, "SAT PASTE EXT:");
            CreateTestItem("NO3-SP", "NO3-N", TestCategories.Soil, "SAT PASTE EXT:");
            CreateTestItem("HCO3-S", "HCO3", TestCategories.Soil, "SAT PASTE EXT:");
            CreateTestItem("CO3-S", "CO3", TestCategories.Soil, "SAT PASTE EXT:");
            CreateTestItem("SO4-SP", "SO4-S", TestCategories.Soil, "SAT PASTE EXT:");


            CreateTestItem("CEC"      , "CEC", TestCategories.Soil, "PHYSIO CHEM:");
            CreateTestItem("#CEC-EST" , "CEC (Estimated)", TestCategories.Soil, "PHYSIO CHEM:");
            CreateTestItem("OM-LOI"   , "OM (LOI)", TestCategories.Soil, "PHYSIO CHEM:");
            CreateTestItem("#CORG-LOI", "Org.C (LOI)", TestCategories.Soil, "PHYSIO CHEM:");
            CreateTestItem("CACO3"    , "CaCO3", TestCategories.Soil, "PHYSIO CHEM:");
            CreateTestItem("ATMP3"    , "Moisture Retention: 0.33 atm", TestCategories.Soil, "PHYSIO CHEM:");
            CreateTestItem("ATM1"     , "Moisture Retention: 1 atm", TestCategories.Soil, "PHYSIO CHEM:");
            CreateTestItem("ATM5"     , "Moisture Retention: 5 atm", TestCategories.Soil, "PHYSIO CHEM:");
            CreateTestItem("ATM10"    , "Moisture Retention: 10 atm", TestCategories.Soil, "PHYSIO CHEM:");
            CreateTestItem("ATM15"    , "Moisture Retention: 15 atm	", TestCategories.Soil, "PHYSIO CHEM:");
            CreateTestItem("#PSA"     , "Particle Size [Sand/Silt/Clay]", TestCategories.Soil, "PHYSIO CHEM:");
            CreateTestItem("SANDVF"   , "Very Fine Sand", TestCategories.Soil, "PHYSIO CHEM:");

            CreateTestItem("P-ST", "P ", TestCategories.Soil, "ACID DIGESTIBLES:");
            CreateTestItem("ZN_TOT", "Zn", TestCategories.Soil, "ACID DIGESTIBLES:");
            CreateTestItem("MN_TOT", "Mn", TestCategories.Soil, "ACID DIGESTIBLES:");
            CreateTestItem("FE_TOT", "Fe", TestCategories.Soil, "ACID DIGESTIBLES:");
            CreateTestItem("CU_TOT", "Cu", TestCategories.Soil, "ACID DIGESTIBLES:");
            CreateTestItem("CD_TOT", "Cd", TestCategories.Soil, "ACID DIGESTIBLES:");
            CreateTestItem("SE-ST", "Se", TestCategories.Soil, "ACID DIGESTIBLES:");
            CreateTestItem("AS-ST", "As", TestCategories.Soil, "ACID DIGESTIBLES:");

            CreateTestItem("OM", "OM (Walkley-Black)", TestCategories.Soil, "ALTERNATE METHODS:");
            CreateTestItem("#CORG", "Org.C (W-B)", TestCategories.Soil, "ALTERNATE METHODS:");
            CreateTestItem("AL-OX", "Al", TestCategories.Soil, "ALTERNATE METHODS:");
            CreateTestItem("FE-OX", "Fe", TestCategories.Soil, "ALTERNATE METHODS:");
            CreateTestItem("SI-OX", "Si", TestCategories.Soil, "ALTERNATE METHODS:");
            CreateTestItem("AL-PY", "Al", TestCategories.Soil, "ALTERNATE METHODS:");
            CreateTestItem("FE-PY", "Fe ", TestCategories.Soil, "ALTERNATE METHODS:");
            CreateTestItem("SI-PY", "Si", TestCategories.Soil, "ALTERNATE METHODS:");
            CreateTestItem("NO3S-WET", "NO3-N (undried soil)", TestCategories.Soil, "ALTERNATE METHODS:");
            CreateTestItem("NH4S-WET", "NH4-N (undried soil) ", TestCategories.Soil, "ALTERNATE METHODS:");
            CreateTestItem("#NN-WET	", "Nitrate & Ammonium (undried soil) [NO3-N, NH4-N]", TestCategories.Soil, "ALTERNATE METHODS:");

            CreateTestItem("NO3-W", "NO3-N", string.Format("{0}|{1}", TestCategories.Soil, TestCategories.Water), "TESTS ON CLIENT-PROVIDED EXTRACTS:");
            CreateTestItem("NH4-W", "NH4-N", string.Format("{0}|{1}", TestCategories.Soil, TestCategories.Water), "TESTS ON CLIENT-PROVIDED EXTRACTS:");
            CreateTestItem("#NA-E", "Nitrate & Ammonium [NO3-N, NH4-N]", string.Format("{0}|{1}", TestCategories.Soil, TestCategories.Water), "TESTS ON CLIENT-PROVIDED EXTRACTS:");

            CreateTestItem("WRAP", "Sample Encapsulation (for N &/or C isotope testing)", string.Format("{0}|{1}", TestCategories.Soil, TestCategories.Plant), "OTHER SERVICES REQUESTED:");

            //Plant
            
            CreateTestItem("#NUTRA2", "Nutrient Panel A [N, P, K]", TestCategories.Plant, "DISCOUNTED GROUPS: ");
            CreateTestItem("#NUTRB", "Nutrient Panel B [S, B, Ca, Mg]", TestCategories.Plant, "DISCOUNTED GROUPS: ");
            CreateTestItem("#NUTRC", "Nutrient Panel C [Zn, Mn, Fe, Cu] ", TestCategories.Plant, "DISCOUNTED GROUPS: ");
            CreateTestItem("#PLANT-D3", "Nutrient Panel D [Panels A, B & C tests", TestCategories.Plant, "DISCOUNTED GROUPS: ");
            CreateTestItem("#PLANT-E", "Nutrient Panel E [NO3-N, PO4-P, K, Panels B & C tests] ", TestCategories.Plant, "DISCOUNTED GROUPS: ");
            CreateTestItem("#NA-PMF", "Add Na to a Nutrient Panel", TestCategories.Plant, "DISCOUNTED GROUPS: ");
            CreateTestItem("#NC-P", "Total Nitrogen & Carbon [N, C]", TestCategories.Plant, "DISCOUNTED GROUPS: ");
            CreateTestItem("#NA-P", "Nitrate & Ammonium [NO3-N, NH4-N]", TestCategories.Plant, "DISCOUNTED GROUPS: ");
            CreateTestItem("#XNPK", "Extractable N-P-K Group 1 [NO3-N, NH4-N, PO4-P, K]", TestCategories.Plant, "DISCOUNTED GROUPS: ");
            CreateTestItem("#XNPKF-2", "Extractable N-P-K Group 2 [NO3-N, PO4-P, K]", TestCategories.Plant, "DISCOUNTED GROUPS: ");
            CreateTestItem("#CARBS", "Soluble Carbohydrates Group 1 [Fructose, Glucose, Sucrose]", TestCategories.Plant, "DISCOUNTED GROUPS: ");
            CreateTestItem("#CARBS2", "Soluble Carbohydrates Group 2 [Fructose, Glucose, Sucrose, Sorbitol]", TestCategories.Plant, "DISCOUNTED GROUPS: ");
            CreateTestItem("#TCARB", "Carbohydrate Panel [TNC, Starch, Fructose, Glucose, Sucrose, Total Glucose]", TestCategories.Plant, "DISCOUNTED GROUPS: ");
            CreateTestItem("#FEED1", "Feed Group 1 [DM, Protein, ADF, TDN]", TestCategories.Plant, "DISCOUNTED GROUPS: ");
            CreateTestItem("#FEED2", "Feed Group 2 [DM, Protein, ADF, TDN, Ash, Fat]", TestCategories.Plant, "DISCOUNTED GROUPS: ");

            CreateTestItem("C-P", "C", TestCategories.Plant, "TOTALS:");
            CreateTestItem("N-P", "N", TestCategories.Plant, "TOTALS:");
            CreateTestItem("TKN-P", "TKN", TestCategories.Plant, "TOTALS:");

            CreateTestItem("P-TOT", "P", TestCategories.Plant, "ACID DIGESTIBLES:");
            CreateTestItem("K-TMW", "K", TestCategories.Plant, "ACID DIGESTIBLES:");
            CreateTestItem("S-TOT", "S", TestCategories.Plant, "ACID DIGESTIBLES:");
            CreateTestItem("B-PMF", "B", TestCategories.Plant, "ACID DIGESTIBLES:");
            CreateTestItem("CA-PMF", "Ca", TestCategories.Plant, "ACID DIGESTIBLES:");
            CreateTestItem("MG-PMF", "Mg", TestCategories.Plant, "ACID DIGESTIBLES:");
            CreateTestItem("NA-PMF", "Na", TestCategories.Plant, "ACID DIGESTIBLES:");
            CreateTestItem("ZN-PM", "Zn", TestCategories.Plant, "ACID DIGESTIBLES:");
            CreateTestItem("MN-PM", "Mn", TestCategories.Plant, "ACID DIGESTIBLES:");
            CreateTestItem("FE-PM", "Fe", TestCategories.Plant, "ACID DIGESTIBLES:");
            CreateTestItem("CU-PM", "Cu", TestCategories.Plant, "ACID DIGESTIBLES:");
            CreateTestItem("BA-PMF", "Ba", TestCategories.Plant, "ACID DIGESTIBLES:");
            CreateTestItem("CD", "Cd", TestCategories.Plant, "ACID DIGESTIBLES:");
            CreateTestItem("CR", "Cr", TestCategories.Plant, "ACID DIGESTIBLES:");
            CreateTestItem("CO", "Co", TestCategories.Plant, "ACID DIGESTIBLES:");
            CreateTestItem("PB", "Pb", TestCategories.Plant, "ACID DIGESTIBLES:");
            CreateTestItem("MO", "Mo", TestCategories.Plant, "ACID DIGESTIBLES:");
            CreateTestItem("NI", "Ni", TestCategories.Plant, "ACID DIGESTIBLES:");
            CreateTestItem("V", "V", TestCategories.Plant, "ACID DIGESTIBLES:");
            CreateTestItem("SE", "Se", TestCategories.Plant, "ACID DIGESTIBLES:");
            CreateTestItem("AS-PT", "As", TestCategories.Plant, "ACID DIGESTIBLES:");
            CreateTestItem("CR-OXIDE", "Cr (oxide)", TestCategories.Plant, "ACID DIGESTIBLES:");
            CreateTestItem("SI-%", "Si", TestCategories.Plant, "ACID DIGESTIBLES:");
            CreateTestItem("AL", "Al", TestCategories.Plant, "ACID DIGESTIBLES:");

            CreateTestItem("NO3-P", "NO3-N", TestCategories.Plant, "EXTRACTABLES:");
            CreateTestItem("NH4-P", "NH4-N", TestCategories.Plant, "EXTRACTABLES:");
            CreateTestItem("K-TOT", "K", TestCategories.Plant, "EXTRACTABLES:");
            CreateTestItem("CL-P-IC", "Cl", TestCategories.Plant, "EXTRACTABLES:");
            CreateTestItem("PO4-P", "PO4-P", TestCategories.Plant, "EXTRACTABLES:");
            CreateTestItem("SO4-PM", "SO4-S", TestCategories.Plant, "EXTRACTABLES:");

            CreateTestItem("DM", "DM", TestCategories.Plant, "FEED TESTS:");
            CreateTestItem("DM55", "Partial DM (dried at 55OC)", string.Format("{0}|{1}",TestCategories.Plant, TestCategories.Other), "FEED TESTS:");
            CreateTestItem("#PROT", "Protein", TestCategories.Plant, "FEED TESTS:");
            CreateTestItem("ADFRF", "ADF", TestCategories.Plant, "FEED TESTS:");
            CreateTestItem("#ADFAF", "ADF (ash free)", TestCategories.Plant, "FEED TESTS:");
            CreateTestItem("#TDNRF", "TDN", TestCategories.Plant, "FEED TESTS:");
            CreateTestItem("#LIG-AF", "Lignin", TestCategories.Plant, "FEED TESTS:");
            CreateTestItem("#ADIN", "ADIN", TestCategories.Plant, "FEED TESTS:");
            CreateTestItem("NDFRF", "NDF", TestCategories.Plant, "FEED TESTS:");
            CreateTestItem("#NDFAF", "NDF (ash free)", TestCategories.Plant, "FEED TESTS:");
            CreateTestItem("#CELLULOS", "Cellulose", TestCategories.Plant, "FEED TESTS:");
            CreateTestItem("HEMICELL", "Hemicellulose", TestCategories.Plant, "FEED TESTS:");
            CreateTestItem("ASH", "Ash", TestCategories.Plant, "FEED TESTS:");
            CreateTestItem("FAT", "Fat", TestCategories.Plant, "FEED TESTS:");
            CreateTestItem("FAT-R", "Fat (Rinse)", TestCategories.Plant, "FEED TESTS:");
            CreateTestItem("PHENOLS", "Total Phenols *", TestCategories.Plant, "FEED TESTS:");
            CreateTestItem("ASCORBIC", "Ascorbic Acid *", TestCategories.Plant, "FEED TESTS:");
            CreateTestItem("CAROTENE", "β-Carotene *", TestCategories.Plant, "FEED TESTS:");
            CreateTestItem("TOCOPH", "α-Tocopherol *", TestCategories.Plant, "FEED TESTS:");
            CreateTestItem("#STARCH", "Starch", TestCategories.Plant, "FEED TESTS:");
            CreateTestItem("#TNC", "TNC", TestCategories.Plant, "FEED TESTS:");
            CreateTestItem("FRUC", "Fructose", TestCategories.Plant, "FEED TESTS:");
            CreateTestItem("GLUC", "Glucose", TestCategories.Plant, "FEED TESTS:");
            CreateTestItem("SUCR", "Sucrose", TestCategories.Plant, "FEED TESTS:");
            CreateTestItem("SORB", "Sorbitol", TestCategories.Plant, "FEED TESTS:");
            CreateTestItem("TGLUC", "Total Glucose", TestCategories.Plant, "FEED TESTS:");

            CreateTestItem("PH-M", "pH (water 1:5)", TestCategories.Plant, "MANURE and COMPOST TESTS:");
            CreateTestItem("EC-M", "EC (water 1:5)", TestCategories.Plant, "MANURE and COMPOST TESTS:");
            
            
            CreateTestItem("MICRO", "Acid Digestion (for analysis by ICP-MS)", TestCategories.Plant, "OTHER SERVICES REQUESTED:");


            //Water
            CreateTestItem("#WSUIT", "Water Suitability Group 1 [pH, EC, SAR, Ca, Mg, Na, Cl, B, HCO3, CO3]", TestCategories.Water, "DISCOUNTED GROUPS:");
            CreateTestItem("#WSUIT-2", "Water Suitability Group 2 [pH, EC, SAR, Ca, Mg, Na, Cl, B]", TestCategories.Water, "DISCOUNTED GROUPS:");
            CreateTestItem("#AD-SALTS", "Acid Digestible Salts [K, Ca, Mg, Na]", TestCategories.Water, "DISCOUNTED GROUPS:");            
            CreateTestItem("#AD-HM", "Acid Digestible Heavy Metals [Cd, Cr, Pb, Ni]", TestCategories.Water, "DISCOUNTED GROUPS:");            
            CreateTestItem("#CRBBI", "Bicarbonate & Carbonate [HCO3, CO3]", TestCategories.Water, "DISCOUNTED GROUPS:");
            CreateTestItem("#IC-PANEL", "Ion Chromatography Panel [Cl, SO4]", TestCategories.Water, "DISCOUNTED GROUPS:");
            CreateTestItem("#ANIONS", "Anion Panel [Cl, SO4-S (soluble S), NO3-N, HCO3]", TestCategories.Water, "DISCOUNTED GROUPS:");
            CreateTestItem("#SLSLT", "Soluble Salts [K, Ca, Mg, Na]", TestCategories.Water, "DISCOUNTED GROUPS:");
            CreateTestItem("#MICRS", "Soluble Micronutrients [Zn, Mn, Fe, Cu]", TestCategories.Water, "DISCOUNTED GROUPS:");
            CreateTestItem("#SOL-HM", "Soluble Heavy Metals [Cd, Cr, Pb, Ni]", TestCategories.Water, "DISCOUNTED GROUPS:");

            CreateTestItem("PH-W", "pH", TestCategories.Water, "???");
            CreateTestItem("EC-W", "EC", TestCategories.Water, "???");
            CreateTestItem("TC-W", "Total C", TestCategories.Water, "???");
            CreateTestItem("TOC-W", "TOC", TestCategories.Water, "???");
            CreateTestItem("DOC-W", "DOC", TestCategories.Water, "???", "DOC-WF for unfiltered samples");
            CreateTestItem("TN-W", "Total N", TestCategories.Water, "???");
            CreateTestItem("TKN-W", "TKN", TestCategories.Water, "???");
            CreateTestItem("#SAR-W", "SAR", TestCategories.Water, "???");
            CreateTestItem("#ESP-W", "ESP", TestCategories.Water, "???");
            CreateTestItem("#HARD", "Hardness", TestCategories.Water, "???");
            CreateTestItem("CL-W", "Cl", TestCategories.Water, "???");
            CreateTestItem("HCO3-W", "HCO3", TestCategories.Water, "???");
            CreateTestItem("CO3-W", "CO3", TestCategories.Water, "???");
            CreateTestItem("P-W", "PO4-P (soluble P)", TestCategories.Water, "???");
            CreateTestItem("TDS", "TDS", TestCategories.Water, "???");
            CreateTestItem("TS", "TS", TestCategories.Water, "???");
            CreateTestItem("TSS", "TSS", TestCategories.Water, "???");
            CreateTestItem("VSS", "VSS", TestCategories.Water, "???", "requires TSS");
            CreateTestItem("ALK", "Alkalinity", TestCategories.Water, "???");
            CreateTestItem("TURBID", "Turbidity", TestCategories.Water, "???");

            CreateTestItem("K-SOLW", "K", TestCategories.Water, "SOLUBLE MINERALS:");
            CreateTestItem("SO4-W", "SO4-S (soluble S)", TestCategories.Water, "SOLUBLE MINERALS:");
            CreateTestItem("CA-W", "Ca", TestCategories.Water, "SOLUBLE MINERALS:");
            CreateTestItem("MG-W", "Mg", TestCategories.Water, "SOLUBLE MINERALS:");
            CreateTestItem("NA-W", "Na", TestCategories.Water, "SOLUBLE MINERALS:");
            CreateTestItem("B-W	 ", "B", TestCategories.Water, "SOLUBLE MINERALS:");
            CreateTestItem("ZN-W", "Zn", TestCategories.Water, "SOLUBLE MINERALS:");
            CreateTestItem("MN-W", "Mn", TestCategories.Water, "SOLUBLE MINERALS:");
            CreateTestItem("FE-W", "Fe", TestCategories.Water, "SOLUBLE MINERALS:");
            CreateTestItem("CU-W", "Cu", TestCategories.Water, "SOLUBLE MINERALS:");
            CreateTestItem("AL-W", "Al", TestCategories.Water, "SOLUBLE MINERALS:");
            CreateTestItem("SI-W", "Si", TestCategories.Water, "SOLUBLE MINERALS:");
            CreateTestItem("V-W	 ", "V", TestCategories.Water, "SOLUBLE MINERALS:");
            CreateTestItem("CD-W", "Cd", TestCategories.Water, "SOLUBLE MINERALS:");
            CreateTestItem("CR-W", "Cr", TestCategories.Water, "SOLUBLE MINERALS:");
            CreateTestItem("PB-W", "Pb", TestCategories.Water, "SOLUBLE MINERALS:");
            CreateTestItem("NI-W", "Ni", TestCategories.Water, "SOLUBLE MINERALS:");
            CreateTestItem("CL-WX", "Cl in mg/L", TestCategories.Water, "SOLUBLE MINERALS:");
            CreateTestItem("CA-WX", "Ca in mg/L", TestCategories.Water, "SOLUBLE MINERALS:");
            CreateTestItem("MG-WX", "Mg in mg/L", TestCategories.Water, "SOLUBLE MINERALS:");
            CreateTestItem("NA-WX", "Na in mg/L", TestCategories.Water, "SOLUBLE MINERALS:");

            CreateTestItem("P-WT", "P", TestCategories.Water, "ACID DIGESTIBLE MINERALS:");
            CreateTestItem("K-WT", "K", TestCategories.Water, "ACID DIGESTIBLE MINERALS:");
            CreateTestItem("S-WT", "S", TestCategories.Water, "ACID DIGESTIBLE MINERALS:");
            CreateTestItem("CA-WT", "Ca", TestCategories.Water, "ACID DIGESTIBLE MINERALS:");
            CreateTestItem("MG-WT", "Mg", TestCategories.Water, "ACID DIGESTIBLE MINERALS:");
            CreateTestItem("NA-WT", "Na", TestCategories.Water, "ACID DIGESTIBLE MINERALS:");
            CreateTestItem("B-WT", "B", TestCategories.Water, "ACID DIGESTIBLE MINERALS:");
            CreateTestItem("ZN-WT", "Zn", TestCategories.Water, "ACID DIGESTIBLE MINERALS:");
            CreateTestItem("MN-WT", "Mn", TestCategories.Water, "ACID DIGESTIBLE MINERALS:");
            CreateTestItem("FE-WT", "Fe", TestCategories.Water, "ACID DIGESTIBLE MINERALS:");
            CreateTestItem("CU-WT", "Cu", TestCategories.Water, "ACID DIGESTIBLE MINERALS:");
            CreateTestItem("CD-WT", "Cd", TestCategories.Water, "ACID DIGESTIBLE MINERALS:");
            CreateTestItem("CR-WT", "Cr", TestCategories.Water, "ACID DIGESTIBLE MINERALS:");
            CreateTestItem("PB-WT", "Pb", TestCategories.Water, "ACID DIGESTIBLE MINERALS:");
            CreateTestItem("MO-WT", "Mo", TestCategories.Water, "ACID DIGESTIBLE MINERALS:");
            CreateTestItem("NI-WT", "Ni", TestCategories.Water, "ACID DIGESTIBLE MINERALS:");
            CreateTestItem("HG-WT", "Hg", TestCategories.Water, "ACID DIGESTIBLE MINERALS:", "DO NOT USE");
            CreateTestItem("AL-WT", "Al", TestCategories.Water, "ACID DIGESTIBLE MINERALS:");
            CreateTestItem("SE-W", "Se", TestCategories.Water, "ACID DIGESTIBLE MINERALS:");
            CreateTestItem("AS-WT", "As", TestCategories.Water, "ACID DIGESTIBLE MINERALS:");

            CreateTestItem("SO4-W-IC", "SO4 (Ion Chromatography)", TestCategories.Water, "ALTERNATE METHODS:");
            CreateTestItem("CL-W-IC", "Cl (Ion Chromatography)", TestCategories.Water, "ALTERNATE METHODS:");
            CreateTestItem("P-W-ICP", "Cl (Ion Chromatography)", TestCategories.Water, "ALTERNATE METHODS:");
        }

        private void CreateTestItem(string code, string analysis, string category, string group, string notes = null, bool pub = true)
        {
            var testItem = new TestItem
            {
                Id = code,
                Analysis = analysis,
                Category = category,
                Group = group,
                Notes = notes,
                Public = pub
            };
            _context.Add(testItem);
        }
    }

}
