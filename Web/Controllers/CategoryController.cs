using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class CategoryController : ControllerBase
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
