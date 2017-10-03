using System;
using System.Collections.Generic;
using System.Text;

namespace Anlab.Core.Models
{
    public class LabReceiveModel
    {
        public bool Confirm { get; set; }
        public string LabComments { get; set; }
        public decimal AdjustmentAmount { get; set; }
    }
}
