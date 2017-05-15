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
    }

}
