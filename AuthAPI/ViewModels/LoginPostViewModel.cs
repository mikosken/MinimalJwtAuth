using System.ComponentModel.DataAnnotations;

namespace AuthAPI.ViewModels
{
    public class LoginPostViewModel
    {
        // ViewModel for POST action Login().
        // All information needed to verify and login user goes here.
        [Required]
        public string? UserName { get; set; }
        [Required]
        public string? Password { get; set; }
    }
}