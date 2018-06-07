using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Anlab.Core.Domain
{
    // Add profile data for application users by adding properties to the User class
    public class User : IdentityUser
    {
        [StringLength(50)]
        [Display(Name = "First Name")]        
        public string FirstName { get; set; }

        [StringLength(50)]
        [Display(Name = "Last Name")]        
        public string LastName { get; set; }

        [Required]
        [StringLength(256)]
        [Display(Name = "Name")]        
        public string Name { get; set; }

        [StringLength(16)]
        [Display(Name = "Client ID")]
        public string ClientId { get; set; }

        [StringLength(256)]
        public string Phone { get; set; } //Note, there is also a "PhoneNumber" field in the DB...

        [StringLength(50)]       
        public string Account { get; set; }

        [StringLength(1000)]
        [Display(Name = "Company Name")]
        public string CompanyName { get; set; }
        [StringLength(250)]
        [Display(Name = "Billing Contact Name")]
        public string BillingContactName { get; set; }
        [StringLength(2000)]
        [Display(Name = "Billing Contact Address")]
        public string BillingContactAddress { get; set; }
        [Display(Name = "Billing Contact Phone")]
        public string BillingContactPhone { get; set; }
        [EmailAddress]
        [Display(Name = "Billing Contact Email")]
        public string BillingContactEmail { get; set; }

        [Required]
        [EmailAddress]
        public override string Email { get; set; }

        public ICollection<MailMessage> MailMessages { get; set; }

        ///// <summary>
        ///// Navigation property for the roles this user belongs to.
        ///// </summary>
        //public virtual ICollection<IdentityUserRole<int>> Roles { get; } = new List<IdentityUserRole<int>>();

        public string GetFirstName()
        {
            if (!string.IsNullOrWhiteSpace($"{FirstName}{LastName}"))
            {
                return FirstName;
            }
            if (Name.Split(' ').Length == 2)
            {
                return Name.Split(' ')[0];
            }
            return String.Empty;
        }
        public string GetLastName()
        {
            if (!string.IsNullOrWhiteSpace($"{FirstName}{LastName}"))
            {
                return LastName;
            }
            if (Name.Split(' ').Length == 2)
            {
                return Name.Split(' ')[1];
            }
            return String.Empty;
        }
    }
}
