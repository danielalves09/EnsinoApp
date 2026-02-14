using EnsinoApp.Repositories.Matricula;
using EnsinoApp.ViewModels.Certificado;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace EnsinoApp.Services.Certificado;

public class CertificadoService : ICertificadoService
{
    private readonly IMatriculaRepository _matriculaRepository;
    private readonly IPdfService _pdfService;
    private readonly IWebHostEnvironment _env;

    public CertificadoService(
        IMatriculaRepository matriculaRepository,
        IPdfService pdfService,
        IWebHostEnvironment env)
    {
        _matriculaRepository = matriculaRepository;
        _pdfService = pdfService;
        _env = env;
    }

    /*  public async Task GerarCertificadosAsync()
     {
         var matriculas = await _matriculaRepository
             .GetConcluidasSemCertificadoAsync();

         using var memoryStream = new MemoryStream();

         foreach (var matricula in matriculas)
         {
             var pdfBytes = await GerarPdfCertificadoAsync(matricula);
             memoryStream.Write(pdfBytes, 0, pdfBytes.Length);

             matricula.CertificadoEmitido = true;
             //matricula.CaminhoCertificado = caminho;

             await _matriculaRepository.UpdateAsync(matricula);
         }
     } */

    public async Task<List<string>> GerarCertificadosAsync()
    {
        var matriculas = await _matriculaRepository.GetConcluidasSemCertificadoAsync();

        var certificadosGerados = new List<string>();

        // pasta onde os certificados serão salvos
        var pastaCertificados = Path.Combine(_env.WebRootPath, "certificados");
        if (!Directory.Exists(pastaCertificados))
            Directory.CreateDirectory(pastaCertificados);

        foreach (var matricula in matriculas)
        {
            var pdfBytes = await GerarPdfCertificadoAsync(matricula);

            // nome do arquivo: Certificado_NomeCasal.pdf
            var nomeArquivo = $"Certificado_{matricula.Casal.NomeConjuge1}_{matricula.Casal.NomeConjuge2}.pdf";
            var caminhoArquivo = Path.Combine(pastaCertificados, nomeArquivo);

            await File.WriteAllBytesAsync(caminhoArquivo, pdfBytes);

            // marca a matrícula como certificado emitido
            matricula.CertificadoEmitido = true;
            matricula.CaminhoCertificado = $"/certificados/{nomeArquivo}";

            await _matriculaRepository.UpdateAsync(matricula);

            certificadosGerados.Add(caminhoArquivo);
        }

        return certificadosGerados;
    }

    public async Task<byte[]> GerarPdfCertificadoAsync(Models.Entities.Matricula matricula)
    {
        var model = new CertificadoViewModel
        {

            NomeCasal = string.Concat(matricula.Casal.NomeConjuge1 + " e ", matricula.Casal.NomeConjuge2),
            NomeCurso = matricula.Turma.Curso.Nome,
            DataConclusao = matricula.DataConclusao ?? DateTime.Now,
            NomeLider = matricula.Turma.Lider.NomeMarido,
            NomeCampus = matricula.Turma.Campus.Nome,
            LogoUrl = Path.Combine(_env.WebRootPath, "images", "logovideira.png"),
            FundoUrl = Path.Combine(_env.WebRootPath, "images", "logovideira2.png")
        };

        var pdfBytes = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(30);

                // fundo decorativo
                page.Background()
                    .Image(model.FundoUrl, ImageScaling.FitArea);

                //.Image(model.FundoUrl, ImageScaling.FitArea, opacity: 0.1f);
                page.Content()
                    .Column(col =>
                    {
                        // borda decorativa
                        col.Item().Padding(15).Border(1).BorderColor(Colors.Grey.Lighten1)
                            .Column(inner =>
                            {
                                // Logo
                                //inner.Item().AlignCenter()
                                //.Image(model.LogoUrl, ImageScaling.FitHeight);

                                inner.Item().PaddingVertical(20)
                                    .Text("CERTIFICADO DE CONCLUSÃO")
                                    .FontSize(32)
                                    .Bold()
                                    .FontColor(Colors.Blue.Medium)
                                    .AlignCenter();

                                inner.Item().PaddingVertical(10)
                                    .Text("Certificamos que")
                                    .FontSize(18)
                                    .AlignCenter();

                                // Nome do casal em fonte cursiva
                                inner.Item().PaddingVertical(10)
                                    .Text(model.NomeCasal)
                                    .FontSize(28)
                                    .FontFamily("Great Vibes") // Fonte cursiva elegante
                                    .Bold()
                                    .FontColor(Colors.Black)
                                    .AlignCenter();

                                inner.Item().PaddingVertical(10)
                                    .Text($"concluiu com êxito o curso de")
                                    .FontSize(18)
                                    .AlignCenter();

                                inner.Item().PaddingVertical(5)
                                    .Text(model.NomeCurso)
                                    .FontSize(24)
                                    .Bold()
                                    .FontColor(Colors.Green.Darken1)
                                    .AlignCenter();

                                inner.Item().PaddingVertical(10)
                                    .Text($"Concluído em: {model.DataConclusao:dd/MM/yyyy}")
                                    .FontSize(16)
                                    .AlignCenter();

                                // Assinaturas
                                inner.Item().PaddingTop(40)
                                    .Row(row =>
                                    {
                                        row.RelativeItem()
                                            .Column(col2 =>
                                            {
                                                col2.Item().Text("_________________________").AlignCenter();
                                                col2.Item().Text("Assinatura do Líder").FontSize(14).AlignCenter();
                                                col2.Item().Text(model.NomeLider).FontSize(16).Bold().AlignCenter();
                                            });

                                        row.RelativeItem()
                                            .Column(col2 =>
                                            {
                                                col2.Item().Text("_________________________").AlignCenter();
                                                col2.Item().Text("Campus").FontSize(14).AlignCenter();
                                                col2.Item().Text(model.NomeCampus).FontSize(16).Bold().AlignCenter();
                                            });
                                    });
                            });
                    });
            });
        }).GeneratePdf();

        return await Task.FromResult(pdfBytes);
    }

}
