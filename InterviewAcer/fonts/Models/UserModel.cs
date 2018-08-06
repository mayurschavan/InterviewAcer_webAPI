using System.ComponentModel.DataAnnotations;

namespace InterviewAcer.Models
{
    public class UserModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }


        [Required]
        [MaxLength(30)]
        public string Name { get; set; }

        [Required]
        [StringLength(100, ErrorMessage =
        "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        public string LicenseKey { get; set; }
    }
}