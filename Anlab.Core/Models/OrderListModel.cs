using System;

namespace Anlab.Core.Models
{
    public class OrderListModel
    {
        public int Id { get; set; }
        public string RequestNum { get; set; }
        public string Project { get; set; }
        public string Status { get; set; }
        public bool Paid { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public Guid ShareIdentifier { get; set; }
    }
}
