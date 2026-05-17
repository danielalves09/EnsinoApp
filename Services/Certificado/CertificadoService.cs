// ── Arquivo: Services/Certificado/CertificadoService.cs ──────────────────────
// Alterações em relação ao original:
//   1. Injetar ILayoutCertificadoService
//   2. Novo método GerarHtmlComLayoutAsync que usa o layout do curso
//      ou cai no template padrão (CertificadoTemplate.cshtml) quando nulo
//   3. GerarCertificadoPdfAsync passa a aceitar opcionalmente o layout

using DinkToPdf;
using DinkToPdf.Contracts;
using EnsinoApp.Data.Configurations;
using EnsinoApp.Repositories.Matricula;
using EnsinoApp.Services.LayoutCertificado;
using EnsinoApp.ViewModels.Certificado;
using Microsoft.Extensions.Options;
using QRCoder;

namespace EnsinoApp.Services.Certificado;

public class CertificadoService : ICertificadoService
{
    private readonly IMatriculaRepository _matriculaRepository;
    private readonly IPdfService _pdfService;
    private readonly IWebHostEnvironment _env;
    private readonly IRazorViewToStringRenderer _razorRenderer;
    private readonly IConverter _converter;
    private readonly AppSettings _appSettings;

    // ── NOVO ──────────────────────────────────────────────────────────────────
    private readonly ILayoutCertificadoService _layoutService;
    // ─────────────────────────────────────────────────────────────────────────

    public CertificadoService(
        IMatriculaRepository matriculaRepository,
        IPdfService pdfService,
        IWebHostEnvironment env,
        IRazorViewToStringRenderer razorRenderer,
        IConverter converter,
        IOptions<AppSettings> options,
        ILayoutCertificadoService layoutService)   // ← NOVO parâmetro
    {
        _matriculaRepository = matriculaRepository;
        _pdfService = pdfService;
        _env = env;
        _razorRenderer = razorRenderer;
        _converter = converter;
        _appSettings = options.Value;
        _layoutService = layoutService;
    }

    // ── GerarCertificadosAsync (stub mantido) ─────────────────────────────────
    public async Task<List<string>> GerarCertificadosAsync()
    {
        throw new NotImplementedException();
    }

    // ── GerarCertificadoPdfAsync — usa layout do curso quando disponível ──────
    public async Task<byte[]> GerarCertificadoPdfAsync(CertificadoViewModel model)
    {
        string html;

        if (model.IdLayoutCertificado.HasValue)
        {
            // Usa o template HTML dinâmico cadastrado no módulo de layouts
            html = await GerarHtmlComLayoutAsync(model);
        }
        else
        {
            // Fallback: template Razor padrão (CertificadoTemplate.cshtml)
            html = await _razorRenderer.RenderViewToStringAsync(
                "/Views/Lider/CertificadoTemplate.cshtml", model);
        }

        var orientacao = model.Orientacao == "Portrait"
            ? Orientation.Portrait
            : Orientation.Landscape;

        var doc = new HtmlToPdfDocument
        {
            GlobalSettings =
            {
                PaperSize   = PaperKind.A4,
                Orientation = orientacao,
            },
            Objects =
            {
                new ObjectSettings
                {
                    HtmlContent = html,
                    WebSettings =
                    {
                        LoadImages              = true,
                        EnableJavascript        = true,
                        EnableIntelligentShrinking = true,
                        DefaultEncoding         = "utf-8"
                    }
                }
            }
        };

        return _converter.Convert(doc);
    }

    // ── Gera o HTML a partir do template dinâmico do banco ────────────────────
    private async Task<string> GerarHtmlComLayoutAsync(CertificadoViewModel model)
    {
        if (!model.IdLayoutCertificado.HasValue)
            throw new InvalidOperationException("IdLayoutCertificado não informado.");

        var layout = await _layoutService.FindByIdAsync(model.IdLayoutCertificado.Value)
            ?? throw new KeyNotFoundException(
                $"Layout de certificado #{model.IdLayoutCertificado} não encontrado.");

        var variaveis = new Dictionary<string, string>
        {
            ["{{NomeCasal}}"] = model.NomeCasal,
            ["{{NomeCurso}}"] = model.NomeCurso,
            ["{{DataConclusao}}"] = model.DataConclusao.ToString("dd/MM/yyyy"),
            ["{{NomeLider}}"] = model.NomeLider,
            ["{{NomeCampus}}"] = model.NomeCampus,
            ["{{LogoUrl}}"] = $"file:///{model.LogoUrl.Replace("\\", "/")}",
            ["{{FundoUrl}}"] = layout.ImagemFundoUrl is not null
                                        ? $"file:///{Path.Combine(_env.WebRootPath, layout.ImagemFundoUrl.TrimStart('/')).Replace("\\", "/")}"
                                        : $"file:///{model.FundoUrl.Replace("\\", "/")}",
            ["{{CodigoValidacao}}"] = model.CodigoValidacao,
            ["{{QRCodeBase64}}"] = model.QRCodeBase64,
            ["{{FontWoff2Url}}"] = Path.Combine(_env.WebRootPath, "fonts", "greatvibes-regular.woff2").Replace("\\", "/"),
            ["{{FontWoffUrl}}"] = Path.Combine(_env.WebRootPath, "fonts", "greatvibes-regular.woff").Replace("\\", "/"),
            ["{{FontTtfUrl}}"] = Path.Combine(_env.WebRootPath, "fonts", "greatvibes-regular.ttf").Replace("\\", "/"),
        };

        return _layoutService.ProcessarTemplate(layout.TemplateHtml, variaveis);
    }

    // ── Helpers ───────────────────────────────────────────────────────────

    public string GerarNomeCasal(string nome1, string nome2)
    {
        nome1 = nome1?.Trim() ?? "";
        nome2 = nome2?.Trim() ?? "";
        if (string.IsNullOrEmpty(nome1)) return nome2;
        if (string.IsNullOrEmpty(nome2)) return nome1;
        if (nome2.Contains(nome1)) return nome2;
        return $"{nome1} e {nome2}";
    }

    public string GerarNomeLideres(string nome1, string nome2)
        => $"{GetPrimeiroNome(nome1)} e {GetPrimeiroNome(nome2)}";

    private static string GetPrimeiroNome(string nomeCompleto)
    {
        if (string.IsNullOrEmpty(nomeCompleto)) return string.Empty;
        var partes = nomeCompleto.Split(' ');
        return partes[0];
    }

    public string GerarCodigoValidacao()
        => Guid.NewGuid().ToString().Replace("-", "")[..10].ToUpper();

    public string GerarQRCode(string codigoValidacao)
    {
        var url = $"{_appSettings.CertificadoBaseUrl}{codigoValidacao}";
        using var generator = new QRCodeGenerator();
        using var data = generator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
        var qrCode = new PngByteQRCode(data);
        var bytes = qrCode.GetGraphic(10);
        return $"data:image/png;base64,{Convert.ToBase64String(bytes)}";
    }
}
