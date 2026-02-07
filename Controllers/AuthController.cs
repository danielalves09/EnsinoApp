using Microsoft.AspNetCore.Mvc;

namespace EnsinoApp.Controllers;

public class AuthController : Controller
{

    public IActionResult Login()
    {

        return View();
    }
}