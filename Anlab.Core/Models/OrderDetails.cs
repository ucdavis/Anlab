using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Anlab.Core.Domain;
using Anlab.Core.Models;


namespace AnlabMvc.Models.Order
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

        public string LabComments { get; set; }
        public decimal AdjustmentAmount { get; set; }
    }

}
