using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Anlab.Core.Domain;
using Anlab.Core.Models;

namespace AnlabMvc.Models.Order
{
    public class OrderEditModel
    {
        public TestItemModel[] TestItems { get; set; }
        public Anlab.Core.Domain.Order Order { get; set; }

        public string DefaultAccount { get; set; }

        public decimal InternalProcessingFee { get; set; }
        public decimal ExternalProcessingFee { get; set; }
    }
}
