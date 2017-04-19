using System.ComponentModel.DataAnnotations;

namespace AnlabMvc.Models.AccountViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
