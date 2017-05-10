using System;
using System.Collections;
using System.Collections.Generic;
using Anlab.Core.Domain;

namespace AnlabMvc.Models.Order
{
    public class OrderReviewModel
    {
        public Anlab.Core.Domain.Order Order { get; set; }
        public OrderDetails OrderDetails { get; set; }
    }

    public class OrderDetails
    {
        public OrderDetails()
        {
  
        }

        public int Quantity { get; set; }
        public string SampleType { get; set; }
        public string AdditionalInfo { get; set; }
        public IList<TestItem> SelectedTests { get; set; }

        public Decimal Total { get; set; }
    }
}
