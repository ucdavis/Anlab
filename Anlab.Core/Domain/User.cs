using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

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
        public string ClientId { get; set; }

        [Phone]
        public string Phone { get; set; }

        [StringLength(16)]
        public string Account { get; set; }

        public ICollection<MailMessage> MailMessages { get; set; }

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
