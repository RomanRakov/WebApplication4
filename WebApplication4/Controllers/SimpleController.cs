using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication4.Controllers
{
    public class SimpleController : Controller
    {
        [AllowAnonymous]
        public IActionResult Public()
        {
            var isAuth = User.Identity?.IsAuthenticated ?? false;
            var name = User.Identity?.Name ?? "нет";
            return Content($"Public page. Auth: {isAuth}, Name: {name}");
        }

        [Authorize]
        public IActionResult Private()
        {
            return Content($"Private page. You are: {User.Identity?.Name}");
        }
    }
}