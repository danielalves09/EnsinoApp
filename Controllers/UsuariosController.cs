using EnsinoApp.Models.Entities;
using EnsinoApp.Services.Campus;
using EnsinoApp.Services.Email;
using EnsinoApp.Services.Notifications;
using EnsinoApp.Services.Supervisao;
using EnsinoApp.Services.Usuarios;
using EnsinoApp.Services.Util;
using EnsinoApp.ViewModels.Usuario;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EnsinoApp.Controllers;

public class UsuariosController : Controller
{

    private readonly IUsuariosService _usuarioService;
    private readonly UserManager<Usuario> _userManager;
    private readonly SignInManager<Usuario> _signInManager;
    private readonly RoleManager<IdentityRole<int>> _roleManager;
    private readonly IWebHostEnvironment _env;
    private readonly INotificationService _notification;
    private readonly IEmailService _emailService;

    private readonly ICampusService _campusService;
    private readonly ISupervisaoService _supervisaoService;

    private readonly IUtilService _utilService;
    private const int TAMANHO_PAGINA = 10;

    public UsuariosController(UserManager<Usuario> userManager, RoleManager<IdentityRole<int>> roleManager, IUsuariosService usuarioService, IWebHostEnvironment env, INotificationService notification, SignInManager<Usuario> signInManager, IUtilService utilService, IEmailService emailService, ICampusService campusService, ISupervisaoService supervisaoService)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _usuarioService = usuarioService;
        _env = env;
        _notification = notification;
        _signInManager = signInManager;
        _utilService = utilService;
        _emailService = emailService;
        _campusService = campusService;
        _supervisaoService = supervisaoService;
    }

    [Authorize(Roles = "Admin,Pastor,Coordenador,Supervisor")]
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
            TipoUsuario = _userManager.GetRolesAsync(u).Result.FirstOrDefault() ?? "Não definido",
            Ativo = u.Ativo
        });

        return View(model);
    }

    [Authorize(Roles = "Admin,Pastor,Coordenador,Supervisor")]
    public IActionResult Adicionar()
    {
        return View(new AdicionarUsuarioViewModel());

    }

    [Authorize(Roles = "Admin,Pastor,Coordenador,Supervisor")]
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



        // Enviar Confirmação por Email
        _ = _emailService.SendNovoUsuarioAsync(
            model.Email,
            _utilService.GetPrimeiroNome(model.NomeMarido) ?? string.Empty,
            _utilService.GetPrimeiroNome(model.NomeEsposa) ?? string.Empty,
            model.Senha);

        _notification.Success("Usuário criado com sucesso.");

        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin,Pastor,Coordenador,Supervisor")]
    public async Task<IActionResult> Editar(int id)
    {
        var usuario = _usuarioService.FindById(id);
        if (usuario == null) return NotFound();

        var campus = _campusService.FindById(usuario.IdCampus);
        var supervisao = usuario.IdSupervisao.HasValue
            ? _supervisaoService.FindById(usuario.IdSupervisao.Value)
            : null;

        var roles = await _userManager.GetRolesAsync(usuario);

        var model = new EditarUsuarioViewModel
        {
            Id = id,
            Email = usuario.Email ?? string.Empty,
            NomeMarido = usuario.NomeMarido,
            NomeEsposa = usuario.NomeEsposa,
            IdCampus = usuario.IdCampus,
            NomeCampus = campus?.Nome ?? string.Empty,
            IdSupervisao = usuario.IdSupervisao,
            NomeSupervisao = supervisao?.Nome ?? string.Empty,
            Role = roles.FirstOrDefault() ?? string.Empty,
            Ativo = usuario.Ativo,
            // Senha deixada em branco intencionalmente — campo opcional na edição
        };

        return View(model);
    }


    [Authorize(Roles = "Admin,Pastor,Coordenador,Supervisor")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(EditarUsuarioViewModel model)
    {
        // Na edição a senha é opcional — remove os erros de validação de senha se vazio
        if (string.IsNullOrWhiteSpace(model.Senha))
        {
            ModelState.Remove(nameof(model.Senha));
            ModelState.Remove(nameof(model.ConfirmarSenha));
        }

        if (!ModelState.IsValid)
            return View(model);

        var usuario = await _userManager.FindByIdAsync(model.Id.ToString());
        if (usuario == null) return NotFound();

        // Atualiza os dados básicos
        usuario.NomeMarido = model.NomeMarido ?? string.Empty;
        usuario.NomeEsposa = model.NomeEsposa ?? string.Empty;
        usuario.Email = model.Email;
        usuario.UserName = model.Email;
        usuario.IdCampus = model.IdCampus;
        usuario.IdSupervisao = model.IdSupervisao;
        usuario.Ativo = model.Ativo;

        var updateResult = await _userManager.UpdateAsync(usuario);
        if (!updateResult.Succeeded)
        {
            foreach (var erro in updateResult.Errors)
                ModelState.AddModelError("", erro.Description);
            return View(model);
        }

        // Atualiza role se informada
        if (!string.IsNullOrWhiteSpace(model.Role))
        {
            var rolesAtuais = await _userManager.GetRolesAsync(usuario);
            if (rolesAtuais.Any())
                await _userManager.RemoveFromRolesAsync(usuario, rolesAtuais);

            if (!await _roleManager.RoleExistsAsync(model.Role))
                await _roleManager.CreateAsync(new IdentityRole<int>(model.Role));

            await _userManager.AddToRoleAsync(usuario, model.Role);
        }

        // Atualiza senha somente se preenchida
        if (!string.IsNullOrWhiteSpace(model.Senha))
        {
            if (model.Senha != model.ConfirmarSenha)
            {
                ModelState.AddModelError(nameof(model.ConfirmarSenha), "As senhas não conferem.");
                return View(model);
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(usuario);
            var senhaResult = await _userManager.ResetPasswordAsync(usuario, token, model.Senha);

            if (!senhaResult.Succeeded)
            {
                foreach (var erro in senhaResult.Errors)
                    ModelState.AddModelError("", erro.Description);
                return View(model);
            }
        }

        // Se o usuário editado é o próprio logado, atualiza o cookie
        var usuarioLogado = await _userManager.GetUserAsync(User);
        if (usuarioLogado?.Id == usuario.Id)
            await _signInManager.RefreshSignInAsync(usuario);

        _notification.Success("Usuário atualizado com sucesso.");
        return RedirectToAction(nameof(Index));
    }


    [Authorize(Roles = "Admin,Pastor,Coordenador,Supervisor")]
    public async Task<IActionResult> Excluir(int id)
    {
        var usuarioLogado = await _userManager.GetUserAsync(User);
        if (usuarioLogado?.Id == id)
        {
            _notification.Error("Você não pode excluir sua própria conta.");
            return RedirectToAction(nameof(Index));
        }

        var usuario = await _userManager.FindByIdAsync(id.ToString());
        if (usuario == null) return NotFound();

        var resultado = await _userManager.DeleteAsync(usuario);
        if (!resultado.Succeeded)
        {
            _notification.Error("Não foi possível excluir o usuário.");
            return RedirectToAction(nameof(Index));
        }

        _notification.Success("Usuário excluído com sucesso.");
        return RedirectToAction(nameof(Index));
    }




    [HttpGet]
    public IActionResult Buscar(string filtro)
    {
        var usuarios = _usuarioService.FindAll()
            .Where(u => u.NomeMarido.ToLower().Contains(filtro.ToLower()) || u.NomeEsposa.ToLower().Contains(filtro.ToLower()))
            .Select(u => new
            {
                u.Id,
                Nome = $"{u.NomeMarido} / {u.NomeEsposa}",
                u.Email,
                NomeCampus = u.Campus.Nome
            })
            .ToList();

        return Json(usuarios);
    }

    [Authorize(Roles = "Admin,Pastor,Coordenador,Supervisor, Lider")]
    public async Task<IActionResult> Perfil()
    {
        var user = await _userManager.GetUserAsync(User);
        var usuario = user != null ? _usuarioService.FindById(user.Id) : null;
        var model = new UsuarioPerfilViewModel
        {
            Nome = _utilService.GetNomeSobrenome(user!.NomeMarido, user.NomeEsposa),
            NomeMarido = user.NomeMarido,
            NomeEsposa = user.NomeEsposa,
            Email = user.Email,
            FotoPerfilUrl = user.FotoPerfil,
            Role = (await _userManager.GetRolesAsync(user)).FirstOrDefault() ?? "Não definida",
            Campus = usuario!.Campus.Nome,
            Supervisao = usuario.Supervisao?.Nome
        };

        return View(model);
    }

    [Authorize(Roles = "Admin,Pastor,Coordenador,Supervisor, Lider")]
    [HttpPost]
    public async Task<IActionResult> AlterarFoto(UsuarioPerfilViewModel model)
    {
        if (model.Foto == null || model.Foto.Length == 0)
        {
            _notification.Warning("Selecione uma imagem válida.");
            return RedirectToAction(nameof(Perfil));
        }

        var user = await _userManager.GetUserAsync(User);

        var pasta = Path.Combine(_env.WebRootPath, "uploads", "perfis");
        if (!Directory.Exists(pasta))
            Directory.CreateDirectory(pasta);

        var extensao = Path.GetExtension(model.Foto.FileName);

        // fallback de segurança
        if (string.IsNullOrEmpty(extensao))
            extensao = ".png";

        var nomeArquivo = $"usuario_{user.Id}{extensao}";
        var caminhoFisico = Path.Combine(pasta, nomeArquivo);

        using (var stream = new FileStream(caminhoFisico, FileMode.Create))
        {
            await model.Foto.CopyToAsync(stream);
        }

        user.FotoPerfil = $"/uploads/perfis/{nomeArquivo}";
        await _userManager.UpdateAsync(user);

        _notification.Success("Foto atualizada com sucesso.");
        return RedirectToAction(nameof(Perfil));
    }

    [Authorize(Roles = "Admin,Pastor,Coordenador,Supervisor, Lider")]
    [HttpPost]
    public async Task<IActionResult> AlterarSenha(UsuarioPerfilViewModel model)
    {

        var user = await _userManager.GetUserAsync(User);

        var result = await _userManager.ChangePasswordAsync(
            user,
            model.SenhaAtual,
            model.NovaSenha
        );

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(model);
        }

        await _signInManager.RefreshSignInAsync(user);
        _notification.Success("Senha alterada com sucesso.");

        return RedirectToAction(nameof(Perfil));
    }

    [Authorize(Roles = "Admin,Pastor,Coordenador,Supervisor, Lider")]
    [HttpPost]
    public async Task<IActionResult> AtualizarDados(UsuarioPerfilViewModel model)
    {

        var user = await _userManager.GetUserAsync(User);

        user.NomeMarido = model.NomeMarido ?? string.Empty;
        user.NomeEsposa = model.NomeEsposa ?? string.Empty;
        user.Email = model.Email;
        user.UserName = model.Email;

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            _notification.Error("Erro ao atualizar dados.");
            return View(model);
        }

        await _signInManager.RefreshSignInAsync(user);
        _notification.Success("Dados atualizados com sucesso.");

        return RedirectToAction(nameof(Perfil));
    }

}