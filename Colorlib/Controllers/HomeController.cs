using Colorlib.DAL;
using Colorlib.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Colorlib.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _sql;

        public HomeController(AppDbContext sql)
        {
            _sql = sql;
        }
        public async Task<IActionResult> Index()
        {
            List<Product> products =await _sql.Products.ToListAsync();
            return View(products);
        }
    }
}
