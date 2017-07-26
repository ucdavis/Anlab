using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Anlab.Core.Data;
using Anlab.Core.Models;
using AnlabMvc.Helpers;
using AnlabMvc.Resources;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;


namespace AnlabMvc.Services
{
    public interface ILabworksService
    {
        Task<IList<TestItemPrices>> GetPrices();
        Task<TestItemPrices> GetPrice(string code);

        Task<IList<TestItemPrices>> GetTestsAndPricesDone(string orderRequest);
        Task<IList<string>> Test(string orderRequest);
    }

    public class LabworksService : ILabworksService
    {
        private readonly ApplicationDbContext _context;
        private readonly ConnectionSettings _connectionSettings;

        public LabworksService(ApplicationDbContext context, IOptions<ConnectionSettings> connectionSettings)
        {
            _context = context;
            _connectionSettings = connectionSettings.Value;
        }


        public async Task<IList<TestItemPrices>> GetPrices()
        {
            //TODO: Get the Setup cost if available. Otherwise hard code it
            var codes = _context.TestItems.AsNoTracking().Select(a => a.Code).Distinct().ToArray();
            using (var db = new DbManager(_connectionSettings.AnlabConnection))
            {
                var prices = await db.Connection.QueryAsync<TestItemPrices>(QueryResource.AnlabItemPrices, new {codes});

                return prices as IList<TestItemPrices>;
            }
        }

        public async Task<TestItemPrices> GetPrice(string code)
        {
            var temp = await _context.TestItems.SingleOrDefaultAsync(a => a.Code == code);
            if (temp == null)
            {
                return null;
            }

            using (var db = new DbManager(_connectionSettings.AnlabConnection))
            {
                IEnumerable<TestItemPrices> price =
                    await db.Connection.QueryAsync<TestItemPrices>(QueryResource.AnlabPriceForCode, new {code});

                if (price == null || price.Count() != 1)
                {
                    return null;
                }

                return price.ElementAt(0);
            }
        }

        public async Task<IList<TestItemPrices>> GetTestsAndPricesDone(string orderRequest)
        {
            using (var db = new DbManager(_connectionSettings.AnlabConnection))
            {
                IEnumerable<string> codes = await db.Connection.QueryAsync<string>(QueryResource.AnlabTestsRunForOrder, new {orderRequest});
            }
            return null;
        }

        public async Task<IList<string>> Test(string orderRequest)
        {
            using (var db = new DbManager(_connectionSettings.AnlabConnection))
            {
                IEnumerable<string> codes =
                    await db.Connection.QueryAsync<string>(QueryResource.AnlabTestsRunForOrder, new {orderRequest});
                return codes as IList<string>;
            }

        }
    }

    public class FakeLabworksService : ILabworksService
    {
        private readonly ApplicationDbContext _context;
        private readonly ConnectionSettings _connectionSettings;

        public FakeLabworksService(ApplicationDbContext context, IOptions<ConnectionSettings> connectionSettings)
        {
            _context = context;
            _connectionSettings = connectionSettings.Value;
        }
        public async Task<IList<TestItemPrices>> GetPrices()
        {
            var temp = _context.TestItems.AsNoTracking().ToList();
            var testItems = new List<TestItemPrices>();
            var counter = 1;
            foreach (var testItem in temp.OrderBy(a => a.Id))
            {
                if(testItems.Any(a => a.Code == testItem.Code))
                    continue;
                counter++;
                var tip = new TestItemPrices();
                tip.Code = testItem.Code;
                tip.Cost = counter;
                tip.SetupCost = counter % 2 == 0 ? 25 : 30;
                tip.Multiplier = testItem.Multiplier;
                tip.Name = testItem.Analysis;
                testItems.Add(tip);
            }


            return await Task.FromResult(testItems);

        }

        public async Task<TestItemPrices> GetPrice(string code)
        {

            var temp = await _context.TestItems.SingleOrDefaultAsync(a => a.Code == code);
            if (temp == null)
            {
                return null;
            }
            var tip = new TestItemPrices
            {
                Code = temp.Code,
                Cost = temp.Id + 1,
                SetupCost = (temp.Id + 1)%2 == 0 ? 25 :30,
                Multiplier = temp.Multiplier,
                Name = temp.Analysis
            };

            return tip;
        }

        public Task<IList<TestItemPrices>> GetTestsAndPricesDone(string orderRequest)
        {
            throw new NotImplementedException();
        }

        public Task<IList<string>> Test(string orderRequest)
        {
            throw new NotImplementedException();
        }
    }
}
