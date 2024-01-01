using AspNetCore.Reporting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System.Net.Mime;
using System.Net.NetworkInformation;
using System.Text;
using TestLogin.DbCon;
using TestLogin.Models;
using TestLogin.service;
using TestLogin.ViewModel;

using System.Linq;
using AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel;

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

    //[AllowAnonymous]
    //public IActionResult Edit()
    //{
    //    return View();
    //}

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
            //if (result.Succeeded)
            //{
                await SignInManager.SignInAsync(appUser, false);
                return RedirectToAction("Login", "Account");
           // }
        }
        return View();
    }

    //[HttpPost]
    //[AllowAnonymous]
    //[ValidateAntiForgeryToken]
    

    //todo this function 
    //public async Task<IActionResult> Edit(RegistrarVm registrar)
    //{
    //    if (ModelState.IsValid)
    //    {
    //        AppUser appUser = new()
    //        {
    //            UserName = registrar.Email,
    //            Name = registrar.Name,
    //            Email = registrar.Email,
    //            Address = registrar.Address,
    //        };
    //        var result = await userManager.CreateAsync(appUser, registrar.Password);
    //        if (result.Succeeded)
    //        {
    //            await SignInManager.SignInAsync(appUser, false);
    //            return RedirectToAction("Login", "Account");
    //        }
    //    }
    //    return View();
    //}



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


    [HttpGet]
    public async Task<ActionResult> Edit(string username)
    {
        var result = await userManager.FindByNameAsync(username);

        AppUserVM appUserVm = new()
        {

            Name = result.Name,
            UserName = result.UserName,
            Address = result.Address,
            Email = result.Email,
            Id= result.Id,

            
        };

        return View(appUserVm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Edit(AppUserVM registrar)
    {
        if (ModelState.IsValid)
        {
            var userToUpdate =  userManager.Users.FirstOrDefault(u => u.Id == registrar.Id);


            if (userToUpdate != null)
            {
                userToUpdate.UserName = registrar.UserName;
                userToUpdate.Address = registrar.Address;
                userToUpdate.Email = registrar.Email;
                userToUpdate.Name = registrar.Name;
              
              
              
                var result = await userManager.UpdateAsync(userToUpdate);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "User not found.");
            }
        }

        return View();
    }


    [AllowAnonymous]
    public IActionResult ForgotPassword()
    {
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordVm model)
    {
        if (ModelState.IsValid)
        {
            var user =  userManager.Users.FirstOrDefault(u => u.Email== model.Email);

            //userManager.FindByEmailAsync(model.Email);
            if (user == null || !(await userManager.IsEmailConfirmedAsync(user)))
            {

                TempData["UserId"] = user?.Id;

                return RedirectToAction("ForgotPasswordConfirmation");

            }

          
           

            
        }

        return View(model);
    }

    [AllowAnonymous]
    public IActionResult ForgotPasswordConfirmation()
    {

        var id = TempData["UserId"] as string; // Assuming UserId is of type string

        var viewModel = new RegistrarVm
        {
            Id = id // Assigning the retrieved id to the Id property of the RegistrarVm
        };

        return View(viewModel);
    }





}
