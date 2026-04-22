using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApplication4.Data;
using WebApplication4.Models;

namespace WebApplication4.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> ProcessLead(string name, string phone)
        {
            // Проверка регулярным выражением (на стороне сервера для надежности)
            var phoneRegex = new System.Text.RegularExpressions.Regex(@"^(\+7|8|7)[\s\-]?\(?[489][0-9]{2}\)?[\s\-]?[0-9]{3}[\s\-]?[0-9]{2}[\s\-]?[0-9]{2}$");

            if (string.IsNullOrEmpty(name) || !phoneRegex.IsMatch(phone ?? ""))
            {
                return Json(new { success = false, message = "Проверьте правильность введенных данных" });
            }

            var lead = new Lead { Name = name, Phone = phone };
            _context.Leads.Add(lead);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Спасибо! Данные сохранены. Мы скоро свяжемся с вами." });
        }
        public IActionResult Index()
        {
            HttpContext.Session.SetString("UserName", "Roman");

            return View();
        }

        public IActionResult SetTheme()
        {
            Response.Cookies.Append("theme", "dark");

            return RedirectToAction("Index");
        }

        public IActionResult GetTheme()
        {
            var theme = Request.Cookies["theme"];

            ViewBag.Theme = theme;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
