using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnlabMvc.Models.Reviewer
{
    public class ReviewerTotalModel
    {
        public DateTime? fStart { get; set; }
        public DateTime? fEnd { get; set; }
        public DateTime? cStart { get; set; }
        public DateTime? cEnd { get; set; }

        public IList<Anlab.Core.Domain.Order> Orders { get; set; }
    }
}
