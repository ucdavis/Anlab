using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anlab.Core.Domain
{
    public class HistoricalSalesView
    {
        public int Id { get; set; }
        public DateTime DateFinalized { get; set; } //We filter out any nulls here in the view
        public string SelectedTests { get; set; } //Json of model TestDetails
        public bool IsInternal { get; set; }
        public decimal Rush { get; set; }
        public decimal InternalProcessingFee { get; set; }
        public decimal ExternalProcessingFee { get; set; }
        public int Quantity { get; set; }
    }
}
