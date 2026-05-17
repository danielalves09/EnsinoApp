using EnsinoApp.Models.Entities;
using EnsinoApp.Services.LayoutCertificado;
using EnsinoApp.Services.Notifications;
using EnsinoApp.ViewModels.LayoutCertificado;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnsinoApp.Controllers;

[Authorize(Roles = "Admin,Pastor,Coordenador")]
public class LayoutCertificadoController : Controller
{
    private readonly ILayoutCertificadoService _service;
    private readonly INotificationService _notification;
    private readonly IWebHostEnvironment _env;

    public LayoutCertificadoController(
        ILayoutCertificadoService service,
        INotificationService notification,
        IWebHostEnvironment env)
    {
        _service = service;
        _notification = notification;
        _env = env;
    }

    // GET /LayoutCertificado
    public async Task<IActionResult> Index()
    {
        var layouts = await _service.FindAllAsync();

        var vm = layouts.Select(l => new ListarLayoutCertificadoViewModel
        {
            Id = l.Id,
            Nome = l.Nome,
            Descricao = l.Descricao,
            Orientacao = l.Orientacao,
            Ativo = l.Ativo,
            DataCriacao = l.DataCriacao,
            QtdCursosVinculados = l.Cursos.Count
        }).ToList();

        return View(vm);
    }

    // GET /LayoutCertificado/Adicionar
    public IActionResult Adicionar()
    {
        var vm = new LayoutCertificadoFormViewModel
        {
            TemplateHtml = LayoutCertificadoFormViewModel.TemplateDefault,
            VariaveisDisponiveis = _service.GetVariaveisDisponiveis().ToList()
        };
        return View(vm);
    }

    // POST /LayoutCertificado/Adicionar
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Adicionar(LayoutCertificadoFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.VariaveisDisponiveis = _service.GetVariaveisDisponiveis().ToList();
            return View(model);
        }

        var imagemFundoUrl = await SalvarImagemFundo(model.NovaImagemFundo);

        var layout = new LayoutCertificado
        {
            Nome = model.Nome,
            Descricao = model.Descricao,
            TemplateHtml = model.TemplateHtml,
            ImagemFundoUrl = imagemFundoUrl,
            Orientacao = model.Orientacao,
            Ativo = model.Ativo,
            DataCriacao = DateTime.Now
        };

        await _service.CreateAsync(layout);
        _notification.Success("Layout de certificado criado com sucesso!");
        return RedirectToAction(nameof(Index));
    }

    // GET /LayoutCertificado/Editar/{id}
    public async Task<IActionResult> Editar(int id)
    {
        var layout = await _service.FindByIdAsync(id);
        if (layout is null) return NotFound();

        var vm = new LayoutCertificadoFormViewModel
        {
            Id = layout.Id,
            Nome = layout.Nome,
            Descricao = layout.Descricao,
            TemplateHtml = layout.TemplateHtml,
            ImagemFundoUrl = layout.ImagemFundoUrl,
            Orientacao = layout.Orientacao,
            Ativo = layout.Ativo,
            VariaveisDisponiveis = _service.GetVariaveisDisponiveis().ToList()
        };

        return View(vm);
    }

    // POST /LayoutCertificado/Editar
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(LayoutCertificadoFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.VariaveisDisponiveis = _service.GetVariaveisDisponiveis().ToList();
            return View(model);
        }

        var layout = await _service.FindByIdAsync(model.Id);
        if (layout is null) return NotFound();

        var imagemFundoUrl = model.NovaImagemFundo is not null
            ? await SalvarImagemFundo(model.NovaImagemFundo)
            : model.ImagemFundoUrl;

        layout.Nome = model.Nome;
        layout.Descricao = model.Descricao;
        layout.TemplateHtml = model.TemplateHtml;
        layout.ImagemFundoUrl = imagemFundoUrl;
        layout.Orientacao = model.Orientacao;
        layout.Ativo = model.Ativo;

        await _service.UpdateAsync(layout);
        _notification.Success("Layout atualizado com sucesso!");
        return RedirectToAction(nameof(Index));
    }

    // GET /LayoutCertificado/Preview/{id}
    public async Task<IActionResult> Preview(int id)
    {
        var layout = await _service.FindByIdAsync(id);
        if (layout is null) return NotFound();

        var variaveis = DadosExemplo(layout);
        var htmlProcessado = _service.ProcessarTemplate(layout.TemplateHtml, variaveis);

        var vm = new PreviewLayoutViewModel
        {
            Id = layout.Id,
            Nome = layout.Nome,
            Descricao = layout.Descricao,
            Orientacao = layout.Orientacao,
            HtmlProcessado = htmlProcessado,
            VariaveisDisponiveis = _service.GetVariaveisDisponiveis().ToList()
        };

        return View(vm);
    }

    // GET /LayoutCertificado/PreviewHtml/{id}  — HTML puro para iframe
    [AllowAnonymous]
    public async Task<IActionResult> PreviewHtml(int id)
    {
        var layout = await _service.FindByIdAsync(id);
        if (layout is null) return NotFound();

        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        var variaveis = new Dictionary<string, string>
        {
            ["{{NomeCasal}}"] = "João Silva e Maria Silva",
            ["{{NomeCurso}}"] = "Casados Para Sempre",
            ["{{DataConclusao}}"] = DateTime.Today.ToString("dd/MM/yyyy"),
            ["{{NomeLider}}"] = "Pedro e Ana",
            ["{{NomeCampus}}"] = "Campus Central",
            ["{{LogoUrl}}"] = $"{baseUrl}/images/logovideira3.png",
            ["{{FundoUrl}}"] = layout.ImagemFundoUrl is not null
                                        ? $"{baseUrl}{layout.ImagemFundoUrl}"
                                        : $"{baseUrl}/images/bordaCertificado4.png",
            ["{{CodigoValidacao}}"] = "CERT-PREVIEW-2024",
            ["{{QRCodeBase64}}"] = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNk+M9QDwADhgGAWjR9awAAAABJRU5ErkJggg==",
            ["{{FontWoff2Url}}"] = Path.Combine(_env.WebRootPath, "fonts", "greatvibes-regular.woff2").Replace("\\", "/"),
            ["{{FontWoffUrl}}"] = Path.Combine(_env.WebRootPath, "fonts", "greatvibes-regular.woff").Replace("\\", "/"),
            ["{{FontTtfUrl}}"] = Path.Combine(_env.WebRootPath, "fonts", "greatvibes-regular.ttf").Replace("\\", "/"),
        };

        var html = _service.ProcessarTemplate(layout.TemplateHtml, variaveis);
        return Content(html, "text/html");
    }

    // POST /LayoutCertificado/Excluir/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Excluir(int id)
    {
        try
        {
            await _service.DeleteAsync(id);
            _notification.Success("Layout excluído com sucesso!");
        }
        catch (InvalidOperationException ex)
        {
            _notification.Error(ex.Message);
        }

        return RedirectToAction(nameof(Index));
    }

    // POST /LayoutCertificado/ToggleAtivo/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleAtivo(int id)
    {
        var layout = await _service.FindByIdAsync(id);
        if (layout is null) return NotFound();

        layout.Ativo = !layout.Ativo;
        await _service.UpdateAsync(layout);
        _notification.Success(layout.Ativo ? "Layout ativado!" : "Layout desativado!");
        return RedirectToAction(nameof(Index));
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private Dictionary<string, string> DadosExemplo(LayoutCertificado layout)
    {
        return new Dictionary<string, string>
        {
            ["{{NomeCasal}}"] = "João Silva e Maria Silva",
            ["{{NomeCurso}}"] = "Casados Para Sempre",
            ["{{DataConclusao}}"] = DateTime.Today.ToString("dd/MM/yyyy"),
            ["{{NomeLider}}"] = "Pedro e Ana",
            ["{{NomeCampus}}"] = "Campus Central",
            ["{{LogoUrl}}"] = "/images/logovideira3.png",
            ["{{FundoUrl}}"] = layout.ImagemFundoUrl ?? "/images/bordaCertificado4.png",
            ["{{CodigoValidacao}}"] = "CERT-PREVIEW-2024",
            ["{{QRCodeBase64}}"] = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNk+M9QDwADhgGAWjR9awAAAABJRU5ErkJggg==",
            ["{{FontWoff2Url}}"] = "/fonts/greatvibes-regular.woff2",
            ["{{FontWoffUrl}}"] = "/fonts/greatvibes-regular.woff",
            ["{{FontTtfUrl}}"] = "/fonts/greatvibes-regular.ttf",
        };
    }

    private async Task<string?> SalvarImagemFundo(IFormFile? arquivo)
    {
        if (arquivo is null || arquivo.Length == 0) return null;

        var pasta = Path.Combine(_env.WebRootPath, "uploads", "certificados");
        Directory.CreateDirectory(pasta);

        var extensao = Path.GetExtension(arquivo.FileName);
        var nomeArquivo = $"fundo_{Guid.NewGuid():N}{extensao}";
        var caminho = Path.Combine(pasta, nomeArquivo);

        await using var stream = new FileStream(caminho, FileMode.Create);
        await arquivo.CopyToAsync(stream);

        return $"/uploads/certificados/{nomeArquivo}";
    }
}