using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnlabMvc.Models.Order
{
    public class OrderResultsModel
    {
        public OrderReviewModel OrderReviewModel { get; set; } = new OrderReviewModel();

        public Dictionary<string, string> PaymentDictionary { get; set; }

        public bool ShowCreditCardPayment { get; set; } = false;
        
        public string CyberSourceUrl { get; set; }
     }
}
