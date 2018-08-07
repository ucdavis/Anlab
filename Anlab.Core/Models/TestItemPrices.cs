using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Anlab.Core.Domain;
using Anlab.Core.Extensions;

namespace Anlab.Core.Models
{

    public class TestItemPrices
    {
        public string Id { get; set; }
        /// <summary>
        /// This is the test price per sample, Except when PROC which is the processing fee per order
        /// </summary>
        public decimal Cost
        {
            get
            {
                if (string.Equals(Id, "PROC", StringComparison.OrdinalIgnoreCase))
                {
                    return InternalCost;
                }
                if (Nonrep && Noninv)
                {
                    return 0;
                }
                return InternalCost;
            }
        }
        /// <summary>
        /// This is the setup fee per test
        /// </summary>
        public decimal SetupPrice
        {
            get
            {
                if (Nonrep && !Id.IsGroupTest())
                {
                    return 0;
                }

                if (Id.ClearOutSetupPrice())
                {
                    return 0;
                }
                return Math.Ceiling(SetupCost * Multiplier);
            }
        }
        public decimal InternalCost { get; set; }
        public decimal SetupCost { get; set; } 
        public string Name { get; set; }
        public int Multiplier { get; set; }

        public string Sop { get; set; }
        
        /// <summary>
        /// No Cost when this is true AND Noninv is true (Unless id is PROC)
        /// </summary>
        public bool Nonrep { get; set; }
        
        /// <summary>
        /// No setup cost when true
        /// </summary>
        public bool Noninv { get; set; }
    }

}
