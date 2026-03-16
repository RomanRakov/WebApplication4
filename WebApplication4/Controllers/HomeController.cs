using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApplication4.Models;
using Microsoft.AspNetCore.Http;

namespace WebApplication4.Controllers
{
    public class HomeController : Controller
    {
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
