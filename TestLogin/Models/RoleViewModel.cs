using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace TestLogin.Models;

public class RoleViewModel
{

    [Display(Name = "Role Name")]
    public string RoleName { get; set; }
}
