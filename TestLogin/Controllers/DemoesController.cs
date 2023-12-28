using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TestLogin.Controllers
{
    public class DemoesController : Controller
    {

        [Authorize(Roles = "dipon")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
