using Microsoft.AspNetCore.Mvc;

namespace EnsinoApp.Controllers;

public class LandingController : Controller
{
    private readonly ILogger<LandingController> _logger;

    public LandingController(ILogger<LandingController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Index()
    {
        _logger.LogInformation("Acesso à página de aterrissagem");
        return View();
    }
}