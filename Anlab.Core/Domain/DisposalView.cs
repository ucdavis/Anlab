using System;
using System.Collections.Generic;
using System.Text;

namespace Anlab.Core.Domain
{
    public class DisposalView
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string AdditionalEmails { get; set; }
        public string ClientId { get; set; }
        public string Status { get; set; }
        public DateTime? DateFinalized { get; set; }
        public string RequestNum { get; set; }
        public string LabworksSampleDisposition { get; set; }
        public string SampleDisposition { get; set; }

    }
}
