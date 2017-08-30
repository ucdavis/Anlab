using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Anlab.Core.Domain;

namespace Anlab.Core.Models
{
    public class OrderDetails
    {
        public int Quantity { get; set; }
        public string SampleType { get; set; }
        [DataType(DataType.MultilineText)]
        public string AdditionalInfo { get; set; }
        public IList<TestDetails> SelectedTests { get; set; }

        public Decimal Total { get; set; }

        public Payment Payment { get; set; }

        public string[] AdditionalEmails { get; set; }

        public string Project { get; set; }
        [DataType(DataType.MultilineText)]
        public string LabComments { get; set; }
        public decimal AdjustmentAmount { get; set; }
        public decimal GrandTotal { get
            {
                return AdjustmentAmount + Total;
            }
        }
        public string ClientId { get; set; }
        public decimal ProcessingFee { get; set; } = 50; //TODO get from db
    }

}
