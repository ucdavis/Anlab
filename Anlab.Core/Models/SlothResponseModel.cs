
using System;

namespace Anlab.Jobs.MoneyMovement
{
    public class SlothResponseModel
    {
        public Guid Id { get; set; }
        public string KfsTrackingNumber { get; set; }
        public string Status { get; set; }

        public bool Success { get; set; } = true;
        public string Message { get; set; } //Error Message
    }
    public class SlothStatus
    {
        public const string PendingApproval = "PendingApproval";
        public const string Scheduled = "Scheduled";
        public const string Processing = "Processing";
        public const string Completed = "Completed";
        public const string Rejected = "Rejected";
        public const string Cancelled = "Cancelled";
    }
}
