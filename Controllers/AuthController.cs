using EnsinoApp.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EnsinoApp.Controllers;

public class AuthController : Controller
{
    private readonly SignInManager<Usuario> _signInManager;
    private readonly UserManager<Usuario> _userManager;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        SignInManager<Usuario> signInManager,
        UserManager<Usuario> userManager,
        ILogger<AuthController> logger)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        _logger.LogInformation("Acesso à tela de login | ReturnUrl: {ReturnUrl}", returnUrl);
        ViewData["ReturnUrl"] = returnUrl;
        return View(new LoginViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (!ModelState.IsValid)
        {
            _logger.LogWarning(
                "Tentativa de login com ModelState inválido | Email: {Email}",
                model.Email);
            return View(model);
        }

        try
        {
            _logger.LogInformation(
                "Tentativa de login | Email: {Email}",
                model.Email);

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                _logger.LogWarning(
                    "Login inválido - usuário não encontrado | Email: {Email}",
                    model.Email);

                ModelState.AddModelError(string.Empty, "Email ou senha inválidos");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(
                user,
                model.Password,
                model.RememberMe,
                lockoutOnFailure: false);

            if (result.Succeeded)
            {
                _logger.LogInformation(
                    "Login realizado com sucesso | UserId: {UserId} | Email: {Email}",
                    user.Id,
                    user.Email);

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    _logger.LogInformation(
                        "Redirecionando usuário para ReturnUrl | UserId: {UserId} | Url: {ReturnUrl}",
                        user.Id,
                        returnUrl);

                    return Redirect(returnUrl);
                }

                if (await _userManager.IsInRoleAsync(user, "Lider"))
                {
                    _logger.LogInformation(
                        "Redirecionando usuário para área de Líder | UserId: {UserId}",
                        user.Id);

                    return RedirectToAction("Index", "Lider");
                }

                if (
                    await _userManager.IsInRoleAsync(user, "Admin") ||
                    await _userManager.IsInRoleAsync(user, "Pastor") ||
                    await _userManager.IsInRoleAsync(user, "Coordenador")
                )
                {
                    _logger.LogInformation(
                        "Redirecionando usuário para Home | UserId: {UserId}",
                        user.Id);

                    return RedirectToAction("Index", "Home");
                }
            }

            _logger.LogWarning(
                "Login inválido - senha incorreta | UserId: {UserId} | Email: {Email}",
                user.Id,
                user.Email);

            ModelState.AddModelError(string.Empty, "Email ou senha inválidos");
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Erro inesperado durante login | Email: {Email}",
                model.Email);

            throw; // deixa o middleware global tratar
        }
    }

    public async Task<IActionResult> Logout()
    {
        var userName = User?.Identity?.Name ?? "Anônimo";

        _logger.LogInformation(
            "Logout realizado | User: {User}",
            userName);

        await _signInManager.SignOutAsync();
        return RedirectToAction(nameof(Login));
    }
}
