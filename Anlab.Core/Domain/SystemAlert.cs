using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anlab.Core.Domain
{
    public class SystemAlert
    {
        public int Id { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public bool IsActive { get; set; } = false;
        public string Content { get; set; } //Required in the DB, but gets set by controller from Markdown
        [Required]
        public string Markdown { get; set; }
        public bool Danger { get; set; } = false;
    }
}
