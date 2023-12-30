using Microsoft.AspNetCore.Identity;

namespace TestLogin.ViewModel;

public class AppUserVM: IdentityUser
{
    public string Name { get; set; }
    public string UserName { get; set; }
    public string Address { get; set; }
    public string Email { get; set; }

}
