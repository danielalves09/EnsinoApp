using System.Diagnostics;
using EnsinoApp.Models;
using EnsinoApp.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EnsinoApp.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<Usuario> _userManager;

        public HomeController(ILogger<HomeController> logger, UserManager<Usuario> userManager)
        {
            _logger = logger;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            string nomeUsuario = "Visitante";

            if (User.Identity!.IsAuthenticated)
            {
                // Busca o usuário logado pelo Id
                var user = await _userManager.GetUserAsync(User);

                if (user != null)
                {
                    //Opção para exibir o nome do casal futuramente
                    //nomeUsuario = user.NomeMarido + (string.IsNullOrEmpty(user.NomeEsposa) ? "" : " & " + user.NomeEsposa);
                    nomeUsuario = user.NomeMarido;
                }
            }

            ViewBag.NomeUsuario = nomeUsuario;

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
