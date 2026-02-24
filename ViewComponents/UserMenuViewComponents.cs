using EnsinoApp.Models.Entities;
using EnsinoApp.Services.Usuarios;
using EnsinoApp.ViewModels.Menu;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EnsinoApp.ViewComponents
{
    public class UserMenuViewComponent : ViewComponent
    {
        private readonly IUsuariosService _usuariosService;
        private readonly UserManager<Usuario> _userManager;

        public UserMenuViewComponent(IUsuariosService usuariosService, UserManager<Usuario> userManager)
        {
            _usuariosService = usuariosService;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (!User.Identity.IsAuthenticated)
                return Content(""); // se não estiver logado, retorna vazio

            var user = await _userManager.GetUserAsync((System.Security.Claims.ClaimsPrincipal)User);

            var model = new UserMenuViewModel
            {
                Nome = _usuariosService.GetNomeReduzido(user!.NomeMarido, user.NomeEsposa),
                FotoUrl = string.IsNullOrEmpty(user.FotoPerfil) ? "/images/avatar-default.jpg" : user.FotoPerfil
            };

            return View(model);
        }
    }

}