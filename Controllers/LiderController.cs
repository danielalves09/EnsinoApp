using System.IO.Compression;
using System.Security.Claims;
using EnsinoApp.Data.Configurations;
using EnsinoApp.Models.Entities;
using EnsinoApp.Models.Enums;
using EnsinoApp.Services.Certificado;
using EnsinoApp.Services.Licao;
using EnsinoApp.Services.Lider;
using EnsinoApp.Services.Matricula;
using EnsinoApp.Services.Turmas;
using EnsinoApp.ViewModels.Certificado;
using EnsinoApp.ViewModels.Lider;
using EnsinoApp.ViewModels.Matricula;
using EnsinoApp.ViewModels.Relatorios;
using EnsinoApp.ViewModels.Turmas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;

namespace EnsinoApp.Controllers;


[Authorize(Roles = "Lider")]
//[Authorize]
public class LiderController : Controller
{
    private readonly ILiderService _service;
    private readonly ILicaoService _licaoService;
    private readonly ITurmaService _turmaService;
    private readonly IMatriculaService _matriculaService;
    private readonly ICertificadoService _certificadoService;
    private readonly UserManager<Usuario> _userManager;
    private readonly IWebHostEnvironment _env;
    private readonly AppSettings _appSettings;


    public LiderController(ILiderService service, ILicaoService licaoService, ITurmaService turmaService, IMatriculaService matriculaService, UserManager<Usuario> userManager, ICertificadoService certificadoService, IWebHostEnvironment env, IOptions<AppSettings> options)
    {
        _service = service;
        _licaoService = licaoService;
        _turmaService = turmaService;
        _matriculaService = matriculaService;
        _userManager = userManager;
        _certificadoService = certificadoService;
        _env = env;
        _appSettings = options.Value;
    }

    public async Task<IActionResult> Index()
    {
        var idUsuarioClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(idUsuarioClaim))
            return Unauthorized();

        int idUsuario = int.Parse(idUsuarioClaim);

        var turmas = await _service.ObterTurmasAsync(idUsuario);

        var user = await _userManager.GetUserAsync(User);

        var vm = new LiderDashboardViewModel
        {
            NomeLider = user.NomeMarido + " e " + user.NomeEsposa,
            TotalTurmasAtivas = turmas.Count(t => t.Status == Models.Enums.StatusTurma.Acomecar),
            TotalTurmasConcluidas = turmas.Count(t => t.Status == Models.Enums.StatusTurma.Concluida),
            TotalCasaisAtivos = turmas.Sum(t => t.Matriculas.Count(m => m.Status == Models.Enums.StatusMatricula.Ativa)),
            TotalRelatoriosLancados = turmas.Sum(t => t.Matriculas.Sum(m => m.Relatorios.Count)),
            TotalRelatoriosPendentes = turmas.Sum(t => t.Matriculas.Count() * t.Curso.Licoes.Count()) - turmas.Sum(t => t.Matriculas.Sum(m => m.Relatorios.Count)),
            Turmas = turmas.Select(t => new ViewModels.Lider.TurmaResumoViewModel
            {
                Id = t.Id,
                NomeCurso = t.Curso.Nome,
                NomeCampus = t.Campus.Nome,
                TotalCasais = t.Matriculas.Count,
                TotalRelatoriosLancados = t.Matriculas.Sum(m => m.Relatorios.Count),
                TotalRelatoriosPendentes = t.Matriculas.Count * t.Curso.Licoes.Count() - t.Matriculas.Sum(m => m.Relatorios.Count),
                TotalLicoes = t.Curso.Licoes.Count(),
                LicoesConcluidas = t.Matriculas.Sum(m => m.Relatorios.Count),
                StatusTurma = t.Status,
                DataInicio = t.DataInicio,
                DataFim = t.DataFim
            }).OrderByDescending(t => t.DataInicio).ToList()
        };

        return View(vm);
    }



    public async Task<IActionResult> Turma(int id)
    {
        //var matriculas = await _service.ObterMatriculasAsync(id);
        //return View(matriculas);

        var turma = _turmaService.FindById(id);
        if (turma == null) return NotFound();

        var viewModel = new TurmaDashboardViewModel
        {
            Id = turma.Id,
            NomeCurso = turma.Curso.Nome,
            NomeCampus = turma.Campus.Nome,
            NomeLider = $"{turma.Lider.NomeMarido} / {turma.Lider.NomeEsposa}",
            DataInicio = turma.DataInicio,
            DataFim = turma.DataFim,
            TotalLicoes = turma.Curso.Licoes.Count(),
            LicoesConcluidas = turma.Matriculas.Sum(m => m.Relatorios.Count),
            Status = turma.Status,
            CasaisMatriculados = turma.Matriculas.Select(m => new ViewModels.Turmas.CasalMatriculadoViewModel
            {
                Nome = $"{m.Casal.NomeConjuge1} / {m.Casal.NomeConjuge2}",
                Presenca = m.Relatorios.OrderByDescending(r => r.DataRegistro).FirstOrDefault()?.Presenca ?? StatusPresenca.Ausente,
                UltimaLicao = m.Relatorios.OrderByDescending(r => r.DataRegistro).FirstOrDefault()?.DataLicao,
                Matricula = m
            }).ToList()
        };
        var totalCertificados = await _matriculaService.CountMatriculasConcluidasSemCertificadoAsync();
        viewModel.TotalCertificadosPendentes = totalCertificados;

        return View(viewModel);
    }

    public async Task<IActionResult> Relatorios(int idTurma)
    {
        var relatorios = await _service.ObterRelatoriosAsync(idTurma);
        return View(relatorios);
    }

    public async Task<IActionResult> CriarRelatorio(int idTurma)
    {
        var turma = _turmaService.FindById(idTurma);

        if (turma == null)
            return NotFound();

        var licoes = await _licaoService.FindByCursoAsync(turma.IdCurso);
        var matriculas = await _matriculaService.FindByTurmaAsync(idTurma);

        var vm = new RelatorioCreateViewModel
        {
            IdTurma = idTurma,
            IdCurso = turma.IdCurso,
            DataLicao = DateTime.Today,

            Licoes = licoes.Select(l => new SelectListItem
            {
                Value = l.Id.ToString(),
                Text = $"{l.NumeroSemana}. {l.Titulo}"
            }).ToList(),

            Casais = matriculas.Select(m => new RelatorioCasalItemViewModel
            {
                IdMatricula = m.Id,
                NomeCasal = $"{m.Casal.NomeConjuge1} e {m.Casal.NomeConjuge2}",
                Presenca = StatusPresenca.Presente
            }).ToList()
        };

        return View(vm);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CriarRelatorio(RelatorioCreateViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var idUsuarioClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(idUsuarioClaim))
            return Unauthorized();

        int idUsuario = int.Parse(idUsuarioClaim);

        foreach (var item in model.Casais)
        {
            var relatorio = new RelatorioSemanal
            {
                IdMatricula = item.IdMatricula,
                IdLicao = model.IdLicao,
                Presenca = item.Presenca,
                Observacoes = item.Observacoes ?? string.Empty,
                IdUsuario = idUsuario,
                DataRegistro = DateTime.Now,
                DataLicao = model.DataLicao
            };

            await _service.CriarRelatorioAsync(relatorio);
        }

        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> ConcluirCurso(int idMatricula)
    {
        var podeConcluir = await _matriculaService.PodeConcluirCursoAsync(idMatricula);

        if (!podeConcluir)
        {
            TempData["Erro"] = "Esta matrícula ainda não concluiu todas as lições.";
            return RedirectToAction(nameof(Index));
        }

        var viewModel = new ConcluirCursoViewModel
        {
            IdMatricula = idMatricula
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConcluirCursoConfirmado(int idMatricula)
    {
        try
        {
            await _matriculaService.ConcluirCursoAsync(idMatricula);
            TempData["Sucesso"] = "Curso concluído com sucesso!";
        }
        catch (Exception ex)
        {
            TempData["Erro"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> GerarCertificados()
    {
        var matriculas = await _matriculaService.GetConcluidasSemCertificadoAsync();

        if (!matriculas.Any())
        {
            TempData["ToastrInfo"] = "Não há certificados pendentes para gerar.";
            return RedirectToAction("Index");
        }


        var certificadosFolder = Path.Combine(_env.WebRootPath, _appSettings.CertificadosFolder);
        if (!Directory.Exists(certificadosFolder))
            Directory.CreateDirectory(certificadosFolder);

        var memoryStream = new MemoryStream();

        using (var zip = new System.IO.Compression.ZipArchive(memoryStream, ZipArchiveMode.Create, true))
        {
            foreach (var mat in matriculas)
            {
                string codValidacao = _certificadoService.GerarCodigoValidacao();
                var model = new CertificadoViewModel
                {
                    NomeCasal = _certificadoService.GerarNomeCasal(mat.Casal.NomeConjuge1, mat.Casal.NomeConjuge2),
                    NomeCurso = mat.Turma.Curso.Nome,
                    DataConclusao = mat.DataConclusao ?? DateTime.Now,
                    NomeLider = _certificadoService.GerarNomeLideres(mat.Turma.Lider.NomeMarido, mat.Turma.Lider.NomeEsposa),
                    NomeCampus = mat.Turma.Campus.Nome,
                    LogoUrl = Path.Combine(_env.WebRootPath, "images", "logovideira3.png"),
                    FundoUrl = Path.Combine(_env.WebRootPath, "images", "bordaCertificado4.png").Replace("\\", "/"),
                    CodigoValidacao = codValidacao,
                    QRCodeBase64 = _certificadoService.GerarQRCode(codValidacao)
                };

                // Gera o PDF
                var pdfBytes = await _certificadoService.GerarCertificadoPdfAsync(model);
                if (pdfBytes == null || pdfBytes.Length == 0)
                    continue;

                //Salva o arquivo no servidor
                var arquivoCaminho = Path.Combine(certificadosFolder, $"Certificado_{mat.IdCasal}.pdf");
                await System.IO.File.WriteAllBytesAsync(arquivoCaminho, pdfBytes);

                //Adiciona ao ZIP para download
                var entry = zip.CreateEntry($"Certificado_{mat.IdCasal}.pdf");
                using var entryStream = entry.Open();
                await entryStream.WriteAsync(pdfBytes, 0, pdfBytes.Length);

                mat.CertificadoEmitido = true;
                mat.CodigoValidacao = codValidacao;
                mat.CaminhoCertificado = $"/{_appSettings.CertificadosFolder}/Certificado_{mat.IdCasal}.pdf";
                await _matriculaService.UpdateAsync(mat);
            }
        }

        memoryStream.Position = 0;

        TempData["ToastrSuccess"] = $"Certificados gerados: {matriculas.Count}";
        return File(memoryStream, "application/zip", "Certificados.zip");

    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GerarCertificadosConfirmado()
    {
        await _certificadoService.GerarCertificadosAsync();

        TempData["Sucesso"] = "Certificados gerados com sucesso!";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> VerCertificado(int idMatricula)
    {
        var matricula = await _matriculaService.FindByIdAsync(idMatricula);

        if (matricula == null || !matricula.CertificadoEmitido)
            return NotFound("Certificado não encontrado.");

        if (string.IsNullOrEmpty(matricula.CaminhoCertificado))
            return BadRequest("Certificado ainda não foi gerado.");

        return Redirect(matricula.CaminhoCertificado);
    }



}
