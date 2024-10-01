using Microsoft.AspNetCore.Mvc;

namespace MyShop.Web.Areas.Admin.Controllers;

public class UsersController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
