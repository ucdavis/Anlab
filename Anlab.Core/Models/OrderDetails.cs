using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Anlab.Core.Domain;

namespace Anlab.Core.Models
{
    public class OrderDetails
    {
        public OrderDetails()
        {

        }

        public int Quantity { get; set; }
        public string SampleType { get; set; }
        [DataType(DataType.MultilineText)]
        public string AdditionalInfo { get; set; }
        public IList<TestItem> SelectedTests { get; set; }

        public Decimal Total { get; set; }

        public Payment Payment { get; set; }

        public IList<string> AdditionalEmails { get; set; }

        public string Project { get; set; }

        public bool Grind { get; set; } //Soil and Plant

        public bool ForeignSoil { get; set; } //Soil

        public bool FilterWater { get; set; } //Water
    }

}
