using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Anlab.Core.Domain
{
    // Add profile data for application users by adding properties to the User class
    public class User : IdentityUser
    {
        [Required]
        [StringLength(50)]
        [Display(Name = "First Name")]        
        public string FirstName { get; set; }

        [Required]
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
    }
}
