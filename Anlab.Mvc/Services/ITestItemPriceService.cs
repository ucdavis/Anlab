using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Anlab.Core.Models;

namespace AnlabMvc.Services
{
    public interface ITestItemPriceService
    {
        IList<TestItemPrices> GetPrices();
        TestItemPrices GetPrice(string code);
    }

    public class FakeTestItemPriceService : ITestItemPriceService
    {
        public IList<TestItemPrices> GetPrices()
        {
            var testItems = new List<TestItemPrices>();
            var item = new TestItemPrices();
            item.Code = "Al (KCL)";
            item.Cost = 5.00m;
            item.SetupCost = 30m;
            testItems.Add(item);

            item = new TestItemPrices();
            item.Code = "NH4-N";
            item.Cost = 4.00m;
            item.SetupCost = 25m;
            testItems.Add(item);

            item = new TestItemPrices();
            item.Code = "H2O";
            item.Cost = 3.00m;
            item.SetupCost = 20m;
            testItems.Add(item);

            item = new TestItemPrices();
            item.Code = "Fake2";
            item.Cost = 3.00m;
            item.SetupCost = 20m;
            testItems.Add(item);

            return testItems;

        }

        public TestItemPrices GetPrice(string code)
        {
            throw new NotImplementedException();
        }
    }
}
