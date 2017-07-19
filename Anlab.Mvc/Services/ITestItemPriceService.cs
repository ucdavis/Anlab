using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Anlab.Core.Data;
using Anlab.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace AnlabMvc.Services
{
    public interface ITestItemPriceService
    {
        Task<IList<TestItemPrices>> GetPrices();
        Task<TestItemPrices> GetPrice(string code);
    }

    public class FakeTestItemPriceService : ITestItemPriceService
    {
        private readonly ApplicationDbContext _context;

        public FakeTestItemPriceService(ApplicationDbContext context)
        {
            _context = context;
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
                tip.Multiplier = 1;
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
                Multiplier = 1,
                Name = temp.Analysis
            };

            return tip;
        }
    }
}
