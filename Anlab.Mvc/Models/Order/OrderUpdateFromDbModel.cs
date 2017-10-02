using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnlabMvc.Models.Order
{
    public class OrderUpdateFromDbModel
    {
        public string ClientId { get; set; }
        public int Quantity { get; set; }
        public IList<string> TestCodes { get; set; }

        public decimal RushMultiplier { get; set; } = 1;
    }
}
