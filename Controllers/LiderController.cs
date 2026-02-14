using System.Security.Claims;
using EnsinoApp.Models.Entities;
using EnsinoApp.Models.Enums;
using EnsinoApp.Services.Licao;
using EnsinoApp.Services.Lider;
using EnsinoApp.Services.Matricula;
using EnsinoApp.Services.Turmas;
using EnsinoApp.ViewModels.Lider;
using EnsinoApp.ViewModels.Matricula;
using EnsinoApp.ViewModels.Relatorios;
using EnsinoApp.ViewModels.Turmas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EnsinoApp.Controllers;


[Authorize(Roles = "Lider")]
//[Authorize]
public class LiderController : Controller
{
    private readonly ILiderService _service;
    private readonly ILicaoService _licaoService;
    private readonly ITurmaService _turmaService;
    private readonly IMatriculaService _matriculaService;
    private readonly UserManager<Usuario> _userManager;


    public LiderController(ILiderService service, ILicaoService licaoService, ITurmaService turmaService, IMatriculaService matriculaService, UserManager<Usuario> userManager)
    {
        _service = service;
        _licaoService = licaoService;
        _turmaService = turmaService;
        _matriculaService = matriculaService;
        _userManager = userManager;
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



}
