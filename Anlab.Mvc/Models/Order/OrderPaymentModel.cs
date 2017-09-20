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
        
        [RegularExpression(@"(\w)-(\w{7})\/?(\w{5})?", ErrorMessage = "The account must be in the format X-XXXXXXX or X-XXXXXXX/XXXXX")]
        public string OverrideAccount { get; set; }
    }
}
