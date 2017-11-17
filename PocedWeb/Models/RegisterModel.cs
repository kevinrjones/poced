using System.ComponentModel.DataAnnotations;

namespace PocedWeb.Models
{
    public class RegisterModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        [Compare("Password", ErrorMessage = "Password confirmation not the same as the password")]
        public string ConfirmPassword { get; set; }
    }

    public class RegisterExternalModel
    {
        [Required]
        public string Username { get; set; }
    }
}