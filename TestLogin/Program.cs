using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TestLogin.DbCon;
using TestLogin.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicatiionDbContext>(x => x.UseSqlServer(builder.Configuration.GetConnectionString("Conn")));
builder.Services.AddIdentity<AppUser, IdentityRole>(
        option =>
        {
            option.Password.RequiredUniqueChars = 0;
            option.Password.RequireUppercase = false;
            option.Password.RequireLowercase = false;
            option.Password.RequiredLength = 8;
            option.Password.RequireNonAlphanumeric = false;

        }

        ).AddEntityFrameworkStores<ApplicatiionDbContext>()
    .AddDefaultTokenProviders();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsProduction())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
