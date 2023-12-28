using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TestLogin.Controllers
{
    public class DemosController : Controller
    {
       

        public IActionResult Index()
        {
            return View();
        }
    }
}
