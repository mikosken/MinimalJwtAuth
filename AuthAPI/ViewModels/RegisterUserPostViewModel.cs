using System.ComponentModel.DataAnnotations;

namespace AuthAPI.ViewModels
{
    public class RegisterUserPostViewModel
    {
        // ViewModel for POST action RegisterUser().
        // If we wish to collect more required information on registration we
        // could do it here.
        [Required]
        [EmailAddress()]
        public string? Email { get; set; }
        [Required]
        public string? Password { get; set; }
    }
}