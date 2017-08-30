using System.ComponentModel.DataAnnotations;

namespace AnlabMvc.Models.AccountViewModels
{
    public class ExternalLoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
