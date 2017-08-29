using System;
using Anlab.Core.Domain;
using Anlab.Core.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AnlabMvc.Models.Order
{
    public class OrderSaveModel
    {

        public int? OrderId { get; set; }
        [Range(1, 100)]
        public int Quantity { get; set; }

        [Required]
        public string SampleType { get; set; }

        public string AdditionalInfo { get; set; }

        [MinLength(1, ErrorMessage = "You must select at least 1 test.")]
        public TestDetails[] SelectedTests { get; set; }

        public decimal Total { get; set; }

        [Required]
        [StringLength(256)]
        public string Project { get; set; }

        public Payment Payment { get; set; }

        public string[] AdditionalEmails { get; set; } = new string[0];

        public string LabComments { get; set; }
        public decimal AdjustmentAmount { get; set; }
        public string ClientId { get; set; }
    }
}
