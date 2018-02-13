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
        public SampleTypeQuestions SampleTypeQuestions { get; set; }
        [DataType(DataType.MultilineText)]
        public string AdditionalInfo { get; set; }
        public Dictionary<string,string> AdditionalInfoList { get; set; } = new Dictionary<string, string>();
        public IList<TestDetails> SelectedTests { get; set; }

        public Decimal Total { get; set; }

        public Payment Payment { get; set; }

        public string[] AdditionalEmails { get; set; }

        public string Project { get; set; }
        public string Commodity { get; set; }
        public DateTime DateSampled { get; set; }

        [DataType(DataType.MultilineText)]
        public string LabComments { get; set; }
        public decimal AdjustmentAmount { get; set; }
        public decimal GrandTotal { get
            {
                return AdjustmentAmount + Total;
            }
        }
        public ClientInfo ClientInfo { get; set; }
        public OtherPaymentInfo OtherPaymentInfo { get; set; }
        public decimal InternalProcessingFee { get; set; }
        public decimal ExternalProcessingFee { get; set; }

        public decimal? RushMultiplier { get; set; }
    }

}
