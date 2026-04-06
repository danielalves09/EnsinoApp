using EnsinoApp.Models.Entities;
using EnsinoApp.Services.Email;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EnsinoApp.Controllers;

public class AuthController : Controller
{
    private readonly SignInManager<Usuario> _signInManager;
    private readonly UserManager<Usuario> _userManager;
    private readonly ILogger<AuthController> _logger;

    private readonly IEmailService _emailService;

    private readonly IEmailTemplateService _emailTemplateService;

    // Código fica em cache apenas em memória de processo.
    // Para produção com múltiplas instâncias use IDistributedCache ou tabela no banco.
    private static readonly Dictionary<string, (string Codigo, DateTime Expira)> _codigos = new();

    public AuthController(
        SignInManager<Usuario> signInManager,
        UserManager<Usuario> userManager,
        ILogger<AuthController> logger,
        IEmailService emailService,
        IEmailTemplateService emailTemplateService)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _logger = logger;
        _emailService = emailService;
        _emailTemplateService = emailTemplateService;
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
                var roles = await _userManager.GetRolesAsync(user);

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

                if (roles.Contains("Lider"))
                {
                    _logger.LogInformation(
                        "Redirecionando usuário para área de Líder | UserId: {UserId}",
                        user.Id);

                    return RedirectToAction("Index", "Lider");
                }

                if (
                    roles.Any(r => r is "Admin" or "Pastor" or "Coordenador")
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

    // =========== ESQUECI MINHA SENHA =============

    [HttpGet]
    public IActionResult ForgotPassword() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            ModelState.AddModelError("email", "Informe um e-mail válido.");
            return View();
        }

        var user = await _userManager.FindByEmailAsync(email);

        // Não revela se o e-mail existe ou não (segurança)
        if (user != null)
        {
            var codigo = GerarCodigo();
            _codigos[email.ToLower()] = (codigo, DateTime.UtcNow.AddMinutes(15));

            var corpo = _emailTemplateService.ResetPasswordEmail(user.NomeMarido, codigo);

            try
            {
                await _emailService.SendResetPasswordAsync(email, "Redefinição de senha — EnsinoApp", corpo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Falha ao enviar código de recuperação para {Email}", email);
                ModelState.AddModelError(string.Empty, "Não foi possível enviar o e-mail. Tente novamente.");
                return View();
            }
        }

        // Sempre redireciona, mesmo se o e-mail não existir
        TempData["RecoveryEmail"] = email;
        return RedirectToAction(nameof(VerifyCode));
    }

    // =========== VERIFICAR CÓDIGO =============

    [HttpGet]
    public IActionResult VerifyCode()
    {
        ViewBag.Email = TempData.Peek("RecoveryEmail")?.ToString();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult VerifyCode(string email, string codigo)
    {
        ViewBag.Email = email;

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(codigo))
        {
            ModelState.AddModelError(string.Empty, "Informe o e-mail e o código.");
            return View();
        }

        var key = email.ToLower();

        if (!_codigos.TryGetValue(key, out var entrada))
        {
            ModelState.AddModelError(string.Empty, "Código inválido ou expirado. Solicite um novo.");
            return View();
        }

        if (entrada.Expira < DateTime.UtcNow)
        {
            _codigos.Remove(key);
            ModelState.AddModelError(string.Empty, "Código expirado. Solicite um novo.");
            return View();
        }

        if (!string.Equals(entrada.Codigo, codigo.Trim(), StringComparison.OrdinalIgnoreCase))
        {
            ModelState.AddModelError(string.Empty, "Código incorreto. Verifique e tente novamente.");
            return View();
        }

        // Código válido — gera token real do Identity para troca de senha
        TempData["RecoveryEmail"] = email;
        TempData["CodigoValido"] = true;
        return RedirectToAction(nameof(ResetPassword), new { email });
    }

    // =========== NOVA SENHA =============

    [HttpGet]
    public IActionResult ResetPassword(string email)
    {
        if (TempData["CodigoValido"] is not true)
            return RedirectToAction(nameof(ForgotPassword));

        TempData.Keep("CodigoValido");
        ViewBag.Email = email;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(string email, string novaSenha, string confirmarSenha)
    {
        ViewBag.Email = email;

        if (string.IsNullOrWhiteSpace(novaSenha) || novaSenha != confirmarSenha)
        {
            ModelState.AddModelError(string.Empty, novaSenha != confirmarSenha
                ? "As senhas não coincidem."
                : "Informe a nova senha.");
            return View();
        }

        if (novaSenha.Length < 6)
        {
            ModelState.AddModelError(string.Empty, "A senha deve ter pelo menos 6 caracteres.");
            return View();
        }

        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
            return RedirectToAction(nameof(Login));

        // Remove o código usado
        _codigos.Remove(email.ToLower());

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, token, novaSenha);

        if (!result.Succeeded)
        {
            foreach (var erro in result.Errors)
                ModelState.AddModelError(string.Empty, erro.Description);
            return View();
        }

        _logger.LogInformation("Senha redefinida com sucesso para {Email}", email);
        TempData["ToastrSuccess"] = "Senha redefinida com sucesso!";
        return RedirectToAction(nameof(Login));
    }

    // HELPER
    private static string GerarCodigo()
    {
        var rng = new Random();
        return rng.Next(100_000, 999_999).ToString();
    }


}
