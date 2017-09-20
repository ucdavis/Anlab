using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AnlabMvc.Models.Order
{
    public class OrderPaymentModel
    {
        public OrderPaymentModel()
        {
            OrderReviewModel = new OrderReviewModel();
        }
        public OrderReviewModel OrderReviewModel { get; set; }
        
        [RegularExpression(@"([A-Z0-9])-([A-Z0-9]{7})\/?([A-Z0-9]{5})?")]
        public string OverrideAccount { get; set; }
    }
}
