using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Anlab.Core.Data;
using Anlab.Core.Models;
using AnlabMvc.Helpers;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;


namespace AnlabMvc.Services
{
    public interface ITestItemPriceService
    {
        Task<IList<TestItemPrices>> GetPrices();
        Task<TestItemPrices> GetPrice(string code);
    }

    public class TestItemPriceService : ITestItemPriceService
    {
        private readonly ApplicationDbContext _context;
        private readonly ConnectionSettings _connectionSettings;

        public TestItemPriceService(ApplicationDbContext context, IOptions<ConnectionSettings> connectionSettings)
        {
            _context = context;
            _connectionSettings = connectionSettings.Value;
        }


        public async Task<IList<TestItemPrices>> GetPrices()
        {
            //TODO: Async
            //TODO: Get the Setup cost if available. Otherwise hard code it
            var codes = _context.TestItems.AsNoTracking().Select(a => a.Code).Distinct().ToArray();
            using (var db = new DbManager(_connectionSettings.AnlabConnection))
            {
                List<TestItemPrices> prices = db.Connection.Query<TestItemPrices>("SELECT [ACODE] as Code,[APRICE] as Cost,[ANAME] as 'Name',[WORKUNIT] as Multiplier FROM [ANL_LIST] where ACODE in @codes", new { codes }).ToList();

                return await Task.FromResult(prices);
            }
        }

        public Task<TestItemPrices> GetPrice(string code)
        {
            throw new NotImplementedException();
        }
    }

    public class FakeTestItemPriceService : ITestItemPriceService
    {
        private readonly ApplicationDbContext _context;
        private readonly ConnectionSettings _connectionSettings;

        public FakeTestItemPriceService(ApplicationDbContext context, IOptions<ConnectionSettings> connectionSettings)
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
    }
}
