using System.ComponentModel.DataAnnotations;

namespace Poced.Web.Models
{
    public class RegisterModel
    {
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [Compare("Password", ErrorMessage = "Password confirmation not the same as the password")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        [Required]
        public string Email { get; set; }
    }

    public class RegisterExternalModel
    {
        [Required]
        public string Email { get; set; }
    }
}