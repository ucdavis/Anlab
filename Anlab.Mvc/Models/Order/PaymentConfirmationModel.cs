using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Anlab.Core.Models;

namespace AnlabMvc.Models.Order
{
    public class PaymentConfirmationModel
    {
        public Anlab.Core.Domain.Order Order { get; set; }
        public OtherPaymentInfo OtherPaymentInfo { get; set; }
    }
}
