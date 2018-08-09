using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;

namespace Anlab.Core.Models
{
    public class OtherPaymentInfo
    {
        [Display(Name = "Payment Type")]
        public string PaymentType { get; set; }
        
        [Required]
        [Display(Name = "Company/Campus Name")]
        public string CompanyName { get; set; }
        
        [Required]
        [Display(Name = "Billing Contact Name")]
        public string AcName { get; set; }
        
        [Required]
        [Display(Name = "Billing Contact Address")]
        public string AcAddr { get; set; }
        
        [Required]
        [EmailAddress]
        [Display(Name = "Billing Contact Email")]
        public string AcEmail { get; set; }
        
        [Required]
        [Display(Name = "Billing Contact Phone Number")]
        public string AcPhone { get; set; }
        
        //[Required] 
        [Display(Name = "PO #")]
        public string PoNum { get; set; }
    }
}
