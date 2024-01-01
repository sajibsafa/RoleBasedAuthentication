using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace TestLogin.ViewModel;

public class AppUserVM: IdentityUser
{
    public string Name { get; set; }
    public string UserName { get; set; }
    public string Address { get; set; }
    public string Email { get; set; }

    public string? Password { get; set; }
    [Compare("Password", ErrorMessage = "Password dot not match.")]
    [Required]
    public string? ConfirmPassword { get; set; }

}
