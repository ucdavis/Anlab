using System;
namespace Anlab.Core.Domain
{
    public class AdjustmentsView
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

        public decimal AdjustmentAmount { get; set; }
        public string Reason { get; set; }

    }
}
