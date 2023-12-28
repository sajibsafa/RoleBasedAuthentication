using AspNetCore.Reporting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Net.Mime;
using System.Text;
using TestLogin.DbCon;
using TestLogin.Models;
using TestLogin.service;
using TestLogin.ViewModel;

namespace TestLogin.Controllers;
[Authorize]
public class AccountController(RoleManager<IdentityRole> roleManager, ApplicatiionDbContext applicatiionDbContext, SignInManager<AppUser> SignInManager, UserManager<AppUser> userManager, IWebHostEnvironment webHostEnvironment) : Controller
{



    //private readonly IWebHostEnvironment webHostEnvironment;

    //public AccountController(IWebHostEnvironment webHostEnvironment)
    //{
    //    this.webHostEnvironment = webHostEnvironment;
    //}
    [AllowAnonymous]
    public IActionResult Login()
    {
        return View();
    }
    [AllowAnonymous]
    public IActionResult Reg()
    {
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reg(RegistrarVm registrar)
    {
        if (ModelState.IsValid)
        {
            AppUser appUser = new()
            {
                UserName = registrar.Email,
                Name = registrar.Name,
                Email = registrar.Email,
                Address = registrar.Address,
            };
            var result = await userManager.CreateAsync(appUser, registrar.Password);
            if (result.Succeeded)
            {
                await SignInManager.SignInAsync(appUser, false);
                return RedirectToAction("Login", "Account");
            }
        }
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginVm loginVm)
    {
        if (ModelState.IsValid)
        {
            var result = await SignInManager.PasswordSignInAsync(loginVm.UserName!, loginVm.Password!, loginVm.RememberMe, false);
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            ModelState.AddModelError("", "Invalid login attempt.");
            return View(loginVm);

        }
        return View(loginVm);
    }


    public async Task<IActionResult> LogOut()
    {
        await SignInManager.SignOutAsync();
        return RedirectToAction("Login", "Account");
    }

   

    [Authorize] // Restrict access to authenticated users
    public async Task<IActionResult> Print()
    {
        // Check if the user is authenticated
        if (User.Identity.IsAuthenticated)
        {
            // Authorized user - proceed with printing logic
            var data = await userManager.Users.ToListAsync();
            string reportName = "TestReport.pdf";
            string reportPath = Path.Combine(webHostEnvironment.ContentRootPath, "Report", "Registration.rdlc");
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Encoding.GetEncoding("utf-8");
            LocalReport report = new LocalReport(reportPath);
            report.AddDataSource("RegistrationDataSet", data.ToList());

            Dictionary<string, string> parameters = new Dictionary<string, string>();

            var result = report.Execute(RenderType.Pdf, 1, parameters);
            var content = result.MainStream.ToArray();
            var contentDisposition = new ContentDisposition
            {
                FileName = reportName,
                Inline = true,
            };
            Response.Headers.Add("Content-Disposition", contentDisposition.ToString());
            return File(content, MediaTypeNames.Application.Pdf);
        }
        // If not authenticated, redirect to login or handle unauthorized access
        return RedirectToAction("Login", "Account");
      }

    [AllowAnonymous]
    public IActionResult CreateRole()
    {
        return View();
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> CreateRole(RoleViewModel roleName)
    {
        {
            //if (ModelState.IsValid)
            //{

                string roleNamevaklue = "rajdip";
                var role = new IdentityRole(roleNamevaklue);
                var result = await roleManager.CreateAsync(role);

                if (result.Succeeded)
                {

                    return RedirectToAction("Index", "Home");
                }


                ModelState.AddModelError("", "Role creation failed.");
            //}


            return View(roleName);
        }

    }



    [AllowAnonymous]
    public IActionResult CreateUserRole()
    {
        ViewData["UserList"] = new SelectList(userManager.Users.Select(x => new { Id = x.Id, Name = x.UserName }).ToList(), "Id", "Name");
        ViewData["RoleList"] = new SelectList(roleManager.Roles.Select(x => new { Id = x.Id, Name = x.Name }).ToList(), "Id", "Name");
        

        return View();
    }


    [HttpPost]
    public async Task<IActionResult> CreateUserRole(RoleUserVm model)
    {
        if (ModelState.IsValid)
        {
            
            var role = await roleManager.FindByIdAsync(model.RoleId);

           
            var user = await userManager.FindByIdAsync(model.UserId);

       
            if (role != null && user != null)
            {
             
                if (!await userManager.IsInRoleAsync(user, role.Name))
                {
                  
                    var result = await userManager.AddToRoleAsync(user, role.Name);

                    if (result.Succeeded)
                    {
                     
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        
                        ModelState.AddModelError("", "Failed to assign role to user.");
                    }
                }
                else
                {
                  
                    ModelState.AddModelError("", "User is already assigned to the selected role.");
                }
            }
            else
            {
           
                ModelState.AddModelError("", "Role or user not found.");
            }
        }

        return View("CreateUserRole", model);
    }





}
