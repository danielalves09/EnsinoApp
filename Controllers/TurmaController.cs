using EnsinoApp.Models.Entities;
using EnsinoApp.Models.Enums;
using EnsinoApp.Services.Campus;
using EnsinoApp.Services.Cursos;
using EnsinoApp.Services.Turmas;
using EnsinoApp.ViewModels.Turmas;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EnsinoApp.Controllers;

public class TurmaController : Controller
{
    private readonly ITurmaService _turmaService;
    private readonly ICursoService _cursoService;
    private readonly ICampusService _campusService;

    public TurmaController(ITurmaService turmaService, ICursoService cursoService, ICampusService campusService)
    {
        _turmaService = turmaService;
        _cursoService = cursoService;
        _campusService = campusService;
    }

    /*public IActionResult Index()
    {
        var turmas = _turmaService.FindAll();
        return View(turmas);
    }*/

    public IActionResult Index(int? idCurso, int? idCampus, StatusTurma? status, int pagina = 1)
    {
        var turmas = _turmaService.FindAll();

        if (idCurso.HasValue)
            turmas = turmas.Where(t => t.IdCurso == idCurso.Value).ToList();
        if (idCampus.HasValue)
            turmas = turmas.Where(t => t.IdCampus == idCampus.Value).ToList();
        if (status.HasValue)
            turmas = turmas.Where(t => t.Status == status.Value).ToList();

        var viewModel = turmas.Select(t => new TurmaViewModel
        {
            Id = t.Id,
            NomeCurso = t.Curso.Nome,
            NomeCampus = t.Campus.Nome,
            NomeLider = $"{t.Lider.NomeMarido} / {t.Lider.NomeEsposa}",
            DataInicio = t.DataInicio,
            DataFim = t.DataFim,
            Status = t.Status
        }).ToList();

        ViewBag.Cursos = new SelectList(_cursoService.FindAll(), "Id", "Nome", idCurso);
        ViewBag.Campuses = new SelectList(_campusService.FindAll(), "Id", "Nome", idCampus);
        ViewBag.Status = new SelectList(Enum.GetValues(typeof(StatusTurma)), status);

        const int TAMANHO_PAGINA = 10;
        ViewBag.NumeroPagina = pagina;
        ViewBag.TotalPaginas = Math.Ceiling((decimal)viewModel.Count() / TAMANHO_PAGINA);

        return View(viewModel.Skip((pagina - 1) * TAMANHO_PAGINA).Take(TAMANHO_PAGINA));
    }

    public IActionResult Dashboard(int id)
    {
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
            Status = turma.Status,
            CasaisMatriculados = turma.Matriculas.Select(m => new CasalMatriculadoViewModel
            {
                Nome = $"{m.Casal.NomeConjuge1} / {m.Casal.NomeConjuge2}",
                Presenca = m.Relatorios.OrderByDescending(r => r.DataRegistro).FirstOrDefault()?.Presenca ?? StatusPresenca.Ausente,
                UltimaLicao = m.Relatorios.OrderByDescending(r => r.DataRegistro).FirstOrDefault()?.DataLicao
            }).ToList()
        };

        return View(viewModel);
    }



    public IActionResult Adicionar()
    {
        return View(new TurmaViewModel
        {
            DataInicio = DateTime.Today,
            DataFim = DateTime.Today.AddMonths(3)
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Adicionar(TurmaViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var turma = new Turma
        {
            IdCurso = model.IdCurso,
            IdCampus = model.IdCampus,
            IdLider = model.IdLider,
            DataInicio = model.DataInicio,
            DataFim = model.DataFim,
            Status = model.Status
        };

        _turmaService.Create(turma);
        return RedirectToAction(nameof(Index));
    }
}