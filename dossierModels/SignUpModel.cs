using System;
using System.ComponentModel.DataAnnotations;
namespace DashboardData.Models
{
    public class SignUpModel
    {
        [Required(ErrorMessage = "L'email est requis.")]
        public String email { get; set; } 
        [Required(ErrorMessage = "le mot de passe est requis.")]
        [MinLength(6)]

        public String password { get; set; }
        [Required(ErrorMessage = "Confirmation du mot de passe est requis.")]
        [Compare("Password", ErrorMessage ="the password are not the same")]

        public String confirmPassword { get; set; }

    }
}
