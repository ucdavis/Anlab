using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Anlab.Core.Models;

namespace AnlabMvc.Models.Order
{
    public class OrderReceiptModel
    {
        public Anlab.Core.Domain.Order Order { get; set; }
        public OrderDetails OrderDetails { get; set; }

        public string MaskedCreditCard { get; set; }
        public string ApprovedAmount { get; set; }
    }
}
