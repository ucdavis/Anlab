using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Anlab.Core.Domain;

namespace AnlabMvc.Models.Order
{
    public class OrderEditModel
    {
        public TestItem[] TestItems { get; set; }
        public Anlab.Core.Domain.Order Order { get; set; }

        public string DefaultAccount { get; set; }
    }
}
