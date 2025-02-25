using Anlab.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anlab.Core.Models
{
    public class LabOrderListModel
    {
        public IList<Order> Orders { get; set; }
        public IList<LabworksFinishedModel> LabworksFinished { get; set; }
    }
}
