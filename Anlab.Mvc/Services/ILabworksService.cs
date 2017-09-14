﻿using System;
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
        Task<IList<string>> GetTestCodesCompletedForOrder(string RequestNum);
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

        /// <summary>
        /// Get the Labworks prices for all test items in our db
        /// </summary>
        /// <returns></returns>
        public async Task<IList<TestItemPrices>> GetPrices()
        {
            var codes = _context.TestItems.AsNoTracking().Select(a => a.Id).Distinct().ToArray();
            using (var db = new DbManager(_connectionSettings.AnlabConnection))
            {
                var prices = await db.Connection.QueryAsync<TestItemPrices>(QueryResource.AnlabItemPrices, new {codes});

                return prices as IList<TestItemPrices>;
            }
        }

        /// <summary>
        /// Get the Labwork price for a single test item code.
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task<TestItemPrices> GetPrice(string code)
        {
            var temp = await _context.TestItems.SingleOrDefaultAsync(a => a.Id == code);
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

        /// <summary>
        /// Get the test codes for an order that exist in labworks so we can update our order details with what was actually done.
        /// </summary>
        /// <param name="RequestNum"></param>
        /// <returns></returns>
        public async Task<IList<string>> GetTestCodesCompletedForOrder(string RequestNum)
        {
            using (var db = new DbManager(_connectionSettings.AnlabConnection))
            {
                //TODO: maybe we should only return tests with a $ amount
                IEnumerable<string> codes =
                    await db.Connection.QueryAsync<string>(QueryResource.AnlabTestsRunForOrder, new {RequestNum});
                if (codes.Count() <= 0)
                {
                    throw new Exception("No codes found");
                }
                return codes as IList<string>;
            }

        }
    }

    /// <summary>
    /// Only uncomment if you can't access the Labworks db...
    /// </summary>
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
                if (testItems.Any(a => a.Id == testItem.Id))
                    continue;
                counter++;
                var tip = new TestItemPrices();
                tip.Id = testItem.Id;
                if (tip.Id == "PROC")
                    tip.Cost = 30;
                else
                    tip.Cost = counter;
                tip.SetupCost = 30;
                tip.Multiplier = 1;
                tip.Name = testItem.Analysis;
                testItems.Add(tip);
            }


            return await Task.FromResult(testItems);

        }

        public async Task<TestItemPrices> GetPrice(string code)
        {

            var temp = await _context.TestItems.SingleOrDefaultAsync(a => a.Id == code);
            if (temp == null)
            {
                return null;
            }
            if (code == "PROC" || code == "SETUP")
            {
                var tip1 = new TestItemPrices
                {
                    Id = temp.Id,
                    Cost = 30,
                    SetupCost = 0,
                    Multiplier = 1,
                    Name = temp.Analysis
                };

                return tip1;
            }

            var tip = new TestItemPrices
            {
                Id = temp.Id,
                Cost = 25,
                SetupCost = 30,
                Multiplier = 1,
                Name = temp.Analysis
            };

            return tip;
        }

        public async Task<IList<string>> GetTestCodesCompletedForOrder(string RequestNum)
        {
            IList<string> codes = new List<string>(5);
            codes.Add("-SNA-PMF");
            codes.Add("-PCL-P-IC");
            codes.Add("X-NA");
            codes.Add("X-MG");
            codes.Add("SP-FOR");
            return await Task.FromResult(codes);
        }
    }
}
