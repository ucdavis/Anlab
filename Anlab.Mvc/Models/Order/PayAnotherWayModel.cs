using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnlabMvc.Models.Order
{
    public class PayAnotherWayModel
    {
        public OrderReviewModel OrderReviewModel { get; set; }
        public string MailToDetails { get; set; }
    }
}
