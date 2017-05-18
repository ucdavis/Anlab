using Anlab.Core.Domain;
using Anlab.Core.Models;
using System.Collections.Generic;

namespace AnlabMvc.Models.Order
{
    public class OrderSaveModel
    {
        public int? OrderId { get; set; }

        public int Quantity { get; set; }
        public string SampleType { get; set; }

        public string AdditionalInfo { get; set; }

        public TestItem[] SelectedTests { get; set; }
        public decimal Total { get; set; }

        public string Project { get; set; }

        public Payment Payment { get; set; }

        public IList<string> AdditionalEmails { get; set; }
    }
}
