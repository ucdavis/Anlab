using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AnlabMvc.Models.Reviewer
{
    public class ReviewerTotalModel
    {
        [Display(Name = "Finalized Start")]
        public DateTime? fStart { get; set; }

        [Display(Name = "Finalized End")]
        public DateTime? fEnd { get; set; }

        [Display(Name = "Created Start")]
        public DateTime? cStart { get; set; }

        [Display(Name = "Created End")]
        public DateTime? cEnd { get; set; }

        public IList<Anlab.Core.Domain.Order> Orders { get; set; }
    }
}
