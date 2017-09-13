using System;
using System.Collections.Generic;
using System.Text;

namespace Anlab.Core.Models
{
    public class TestItemModel
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public decimal InternalCost { get; set; }
        public decimal ExternalCost { get; set; }
        public decimal InternalSetupCost { get; set; }
        public decimal ExternalSetupCost { get; set; }
        public string Category { get; set; }
        public string[] Categories => Category.Split('|');
        public string Group { get; set; }
        public string Analysis { get; set; }
        public string Description { get; set; }
        public string Notes { get; set; }
        public bool Public { get; set; }
    }
}
