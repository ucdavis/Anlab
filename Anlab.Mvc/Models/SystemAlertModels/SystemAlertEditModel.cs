using System.ComponentModel.DataAnnotations;

namespace AnlabMvc.Models.SystemAlertModels
{
    public class SystemAlertEditModel
    {
        public int Id { get; set; }

        [Required]
        public string Markdown { get; set; }
        public bool Danger { get; set; } = false;

        [Required]
        [MaxLength(50)]
        public string Description { get; set; }
    }
}
