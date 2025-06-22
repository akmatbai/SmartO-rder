using Microsoft.AspNetCore.Mvc;

namespace SmartO_rder.Controllers
{
    [Route("auth-PK")]
    public class AuthRedirectController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return Redirect("/Identity/Account/Login");
        }
    }
}
