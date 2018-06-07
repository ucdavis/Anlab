using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace AnlabMvc.Models.Order
{
    public class OverrideOrderModel
    {
        public OrderReviewModel OrderReviewModel { get; set; } = new OrderReviewModel();
        public bool IsDeleted { get; set; }
        public bool Paid { get; set; }
        public string Status { get; set; }
        public string Emails { get; set; }

        public IFormFile UploadFile { get; set; }
    }
}
