using EnsinoApp.Models.Entities;
using EnsinoApp.Services.Campus;
using EnsinoApp.Services.Usuarios;
using EnsinoApp.ViewModels.Usuario;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EnsinoApp.Controllers;


[Authorize]
public class UsuariosController : Controller
{

    private readonly IUsuariosService _usuarioService;

    private readonly UserManager<Usuario> _userManager;
    private readonly RoleManager<IdentityRole<int>> _roleManager;
    private const int TAMANHO_PAGINA = 10;

    public UsuariosController(UserManager<Usuario> userManager, RoleManager<IdentityRole<int>> roleManager, IUsuariosService usuarioService)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _usuarioService = usuarioService;
    }

    public IActionResult Index(string filtro)
    {
        var usuarios = _usuarioService.FindAll();

        if (!string.IsNullOrWhiteSpace(filtro))
        {
            filtro = filtro.ToLower();

            usuarios = usuarios.Where(u =>
                u.Email!.ToLower().Contains(filtro) ||
                u.NomeMarido.ToLower().Contains(filtro) ||
                u.NomeEsposa.ToLower().Contains(filtro)
            );
        }

        var model = usuarios.Select(u => new ListarUsuarioViewModel
        {
            Id = u.Id,
            Email = u.Email!,
            NomeMarido = u.NomeMarido,
            NomeEsposa = u.NomeEsposa,
            Campus = u.Campus.Nome,
            Supervisao = u.Supervisao?.Nome ?? "-",
            Ativo = u.Ativo
        });

        return View(model);
    }

    public IActionResult Adicionar()
    {
        return View(new AdicionarUsuarioViewModel());

    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Adicionar(AdicionarUsuarioViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var usuario = new Usuario
        {
            UserName = model.Email,
            Email = model.Email,
            IdCampus = model.IdCampus,
            NomeMarido = model.NomeMarido ?? string.Empty,
            NomeEsposa = model.NomeEsposa ?? string.Empty,
            IdSupervisao = model.IdSupervisao,
            Ativo = model.Ativo,
            DataCriacao = DateTime.Now
        };

        var resultado = await _userManager.CreateAsync(usuario, model.Senha);

        if (!resultado.Succeeded)
        {
            foreach (var erro in resultado.Errors)
                ModelState.AddModelError("", erro.Description);

            return View(model);
        }

        // Criar role se não existir
        if (!await _roleManager.RoleExistsAsync(model.Role))
        {
            await _roleManager.CreateAsync(new IdentityRole<int>(model.Role));
        }

        await _userManager.AddToRoleAsync(usuario, model.Role);

        return RedirectToAction(nameof(Index));
    }

}