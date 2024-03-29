using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;

namespace AnlabMvc.Models.Reviewer
{
    public class HistoricalSalesModel
    {
        //Filters
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? Start { get; set; }
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? End { get; set; }

        public List<HistoricalSalesRowModel> Rows { get; set; } = new List<HistoricalSalesRowModel>();
    }

    public class HistoricalSalesRowModel
    {
        public string TestCode { get; set; }
        public string Analysis { get; set; }
        public int InternalQuantity { get; set; }
        public int ExternalQuantity { get; set; }
        public decimal InternalTotal { get; set; }
        public decimal ExternalTotal { get; set; }
    }
}
