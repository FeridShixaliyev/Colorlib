using Colorlib.DAL;
using Colorlib.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Colorlib.Extentions;
using Microsoft.AspNetCore.Authorization;

namespace Colorlib.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Authorize(Roles = "Admin,Moderator")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _sql;
        private readonly IWebHostEnvironment _env;

        public ProductController(AppDbContext sql,IWebHostEnvironment env)
        {
            _sql = sql;
            _env = env;
        }
        public async Task<IActionResult> Index()
        {
            List<Product> products = await _sql.Products.ToListAsync();
            return View(products);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            if (!ModelState.IsValid) return View();
            if (product == null) return NotFound();
            if (product.ImageFile != null)
            {
                if (!product.ImageFile.IsImage())
                {
                    ModelState.AddModelError("ImageFile","Sekilin formati duzgun deyil!");
                    return View();
                }
                if (!product.ImageFile.IsSizeOk(5))
                {
                    ModelState.AddModelError("ImageFile", "Sekil 5 mb-dan boyuk ola bilmaz!");
                    return View();
                }
                product.Image = product.ImageFile.SaveImage(_env.WebRootPath,"assets/image");
            }
            else
            {
                ModelState.AddModelError("","Sekil yuklenmeyib");
                return View();
            }
            await _sql.Products.AddAsync(product);
            await _sql.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            Product product = await _sql.Products.FindAsync(id);
            if (product == null) return NotFound();
            return View(product);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id,Product product)
        {
            if (!ModelState.IsValid) return View();
            Product existProduct = await _sql.Products.FirstOrDefaultAsync(p=>p.Id==id);
            if (existProduct == null) return NotFound();
            existProduct.Name = product.Name;
            existProduct.Text = product.Text;
            existProduct.Price = product.Price;
            if (product.ImageFile != null)
            {
                if (!product.ImageFile.IsImage())
                {
                    ModelState.AddModelError("ImageFile", "Sekilin formati duzgun deyil!");
                    return View();
                }
                if (!product.ImageFile.IsSizeOk(5))
                {
                    ModelState.AddModelError("ImageFile", "Sekil 5 mb-dan boyuk ola bilmaz!");
                    return View();
                }
                Helpers.Helper.DeleteImg(_env.WebRootPath,"assets/image",existProduct.Image);
                existProduct.Image = product.ImageFile.SaveImage(_env.WebRootPath,"assets/image");
            }
            else
            {
                ModelState.AddModelError("","Sekil yukleyin");
            }
            await _sql.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Delete(int? id)
        {
            Product product = await _sql.Products.FindAsync(id);
            if (product == null) return NotFound();
            Helpers.Helper.DeleteImg(_env.WebRootPath,"assets/image",product.Image);
             _sql.Products.Remove(product);
            await _sql.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
