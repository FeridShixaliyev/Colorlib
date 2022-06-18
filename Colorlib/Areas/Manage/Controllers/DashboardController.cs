using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Colorlib.Areas.Manage.Controllers
{
    public class DashboardController : Controller
    {
        [Area("Manage")]
        [Authorize(Roles ="Admin,Moderator")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
