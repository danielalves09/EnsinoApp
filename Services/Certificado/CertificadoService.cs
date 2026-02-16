using EnsinoApp.Repositories.Matricula;
using EnsinoApp.ViewModels.Certificado;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Mvc.Razor;
using System.IO.Compression;
using QRCoder;
using EnsinoApp.Data.Configurations;
using Microsoft.Extensions.Options;

namespace EnsinoApp.Services.Certificado;

public class CertificadoService : ICertificadoService
{
    private readonly IMatriculaRepository _matriculaRepository;
    private readonly IPdfService _pdfService;
    private readonly IWebHostEnvironment _env;
    private readonly IRazorViewToStringRenderer _razorRenderer;
    private readonly IConverter _converter;
    private readonly AppSettings _appSettings;

    public CertificadoService(
        IMatriculaRepository matriculaRepository,
        IPdfService pdfService,
        IWebHostEnvironment env,
        IRazorViewToStringRenderer razorRenderer,
        IConverter converter,
        IOptions<AppSettings> options)
    {
        _matriculaRepository = matriculaRepository;
        _pdfService = pdfService;
        _env = env;
        _razorRenderer = razorRenderer;
        _converter = converter;
        _appSettings = options.Value;
    }


    public async Task<List<string>> GerarCertificadosAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<byte[]> GerarCertificadoPdfAsync(CertificadoViewModel model)
    {
        // Renderiza o HTML da view para string
        string html = await _razorRenderer.RenderViewToStringAsync(
            "/Views/Lider/CertificadoTemplate.cshtml", model);

        // Define as opções de PDF
        var doc = new HtmlToPdfDocument()
        {
            GlobalSettings = {
                PaperSize = PaperKind.A4,
                Orientation = Orientation.Landscape,
                //Margins = new MarginSettings { Top = 20, Bottom = 20 },
                //DPI = 300
            },
            Objects = {
                new ObjectSettings() {
                    HtmlContent = html,
                    WebSettings = { DefaultEncoding = "utf-8" }
                }
            }
        };

        return _converter.Convert(doc);
    }

    public string GerarNomeCasal(string nome1, string nome2)
    {
        nome1 = nome1?.Trim() ?? "";
        nome2 = nome2?.Trim() ?? "";

        if (string.IsNullOrEmpty(nome1)) return nome2;
        if (string.IsNullOrEmpty(nome2)) return nome1;

        // Evita duplicação, caso nome2 contenha o nome1
        if (nome2.Contains(nome1)) return nome2;

        return $"{nome1} e {nome2}";
    }

    public string GerarNomeLideres(string nome1, string nome2)
    {

        return $"{GetPrimeiroNome(nome1)} e {GetPrimeiroNome(nome2)}";
    }

    private string GetPrimeiroNome(string nomeCompleto)
    {
        if (string.IsNullOrEmpty(nomeCompleto)) return string.Empty;
        var partes = nomeCompleto.Split(' ');
        if (partes.Length >= 2)
            return $"{partes[0]}"; // Primeiro nome
        return partes[0];
    }

    public Task<byte[]> GerarPdfCertificadoAsync(Models.Entities.Matricula matricula)
    {
        throw new NotImplementedException();
    }

    public string GerarCodigoValidacao()
    {
        var codigoValidacao = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 10).ToUpper();
        return codigoValidacao;

    }

    public string GerarQRCode(string codigoValidacao)
    {
        var urlValidacao = $"{_appSettings.CertificadoBaseUrl}{codigoValidacao}";

        using var qrGenerator = new QRCodeGenerator();
        using var qrCodeData = qrGenerator.CreateQrCode(urlValidacao, QRCodeGenerator.ECCLevel.Q);
        using var qrCode = new QRCode(qrCodeData);
        using var bitmap = qrCode.GetGraphic(20);

        using var ms = new MemoryStream();
        bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
        var base64 = Convert.ToBase64String(ms.ToArray());
        return $"data:image/png;base64,{base64}";
    }
}
