using System;
using System.Collections.Generic;
using System.Text;

namespace Anlab.Core.Domain
{
    public class ReviewerOrderView
    {
        public int Id { get; set; }
        public string RequestNum { get; set; }
        public string ClientId { get; set; }
        public string PaymentType { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public string Status { get; set; }
        public bool Paid { get; set; } = false;
        public DateTime? DateFinalized { get; set; }
        public decimal GrandTotal { get; set; }

    }
}
