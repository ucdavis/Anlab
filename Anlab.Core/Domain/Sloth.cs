using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Anlab.Core.Domain
{
    public class Sloth
    {
        [Key]
        public string Id { get; set; }
        
        public string KfsTrackingNumber { get; set; }
        
        public string JsonDetails { get; set; }
    }
}
