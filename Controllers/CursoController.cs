using EnsinoApp.Models.Entities;
using EnsinoApp.Services.Cursos;
using EnsinoApp.Services.Campus;
using EnsinoApp.ViewModels.Cursos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using EnsinoApp.Models.Enums;

namespace EnsinoApp.Controllers;

public class CursoController : Controller
{
    private readonly ICursoService _cursoService;
    private readonly ICampusService _campusService;

    private const int TAMANHO_PAGINA = 10;
    public CursoController(ICursoService cursoService, ICampusService campusService)
    {
        _cursoService = cursoService;
        _campusService = campusService;
    }

    public IActionResult Index(string filtro, int pagina = 1)
    {
        var cursos = _cursoService.FindAll();
        var viewModel = cursos.Select(c => new CursoViewModel
        {
            Id = c.Id,
            Nome = c.Nome,
            Descricao = c.Descricao,
            Ativo = c.Ativo,
            IdCampus = c.IdCampus,
            NomeCampus = c.Campus?.Nome ?? string.Empty
        });

        ViewBag.Filtro = filtro;
        ViewBag.NumeroPagina = pagina;
        ViewBag.TotalPaginas = Math.Ceiling((decimal)viewModel.Count() / TAMANHO_PAGINA);

        return View(viewModel.Skip((pagina - 1) * TAMANHO_PAGINA).Take(TAMANHO_PAGINA));

    }

    public IActionResult Adicionar()
    {
        return View(new CursoViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Adicionar(CursoViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var curso = new Curso
        {
            Nome = model.Nome,
            Descricao = model.Descricao,
            Ativo = model.Ativo,
            IdCampus = model.IdCampus
        };

        _cursoService.Create(curso);
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Editar(int id)
    {
        var curso = _cursoService.FindById(id);
        if (curso == null) return NotFound();

        var model = new CursoViewModel
        {
            Id = curso.Id,
            Nome = curso.Nome,
            Descricao = curso.Descricao,
            Ativo = curso.Ativo,
            IdCampus = curso.IdCampus,
            NomeCampus = curso.Campus?.Nome ?? string.Empty
        };

        ViewBag.Campuses = new SelectList(_campusService.FindAll(), "Id", "Nome", model.IdCampus);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Editar(CursoViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Campuses = new SelectList(_campusService.FindAll(), "Id", "Nome", model.IdCampus);
            return View(model);
        }

        var curso = _cursoService.FindById(model.Id);
        if (curso == null) return NotFound();

        curso.Nome = model.Nome;
        curso.Descricao = model.Descricao;
        curso.Ativo = model.Ativo;
        curso.IdCampus = model.IdCampus;

        _cursoService.Update(curso);
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Deletar(int id)
    {
        _cursoService.Delete(id);
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Dashboard(int id)
    {
        var curso = _cursoService.FindByIdDashboard(id); // Certifique-se que inclui Turmas -> Matriculas -> Casal

        if (curso == null) return NotFound();

        var turmas = curso.Turmas.Select(t => new CursoDashboardViewModel.TurmaInfo
        {
            Id = t.Id,
            NomeLider = t.Lider.NomeMarido + " e " + t.Lider.NomeEsposa,
            DataInicio = t.DataInicio,
            DataFim = t.DataFim,
            Status = t.Status,
            Casais = t.Matriculas.Select(m => new CursoDashboardViewModel.CasalInfo
            {
                Id = m.Casal.Id,
                NomeMarido = m.Casal.NomeConjuge1,
                NomeEsposa = m.Casal.NomeConjuge2,
                StatusCasal = (Models.Enums.StatusCasal)m.Casal.Status,
                StatusPresenca = (Models.Enums.StatusPresenca)m.Casal.Status
            }).ToList()
        }).ToList();

        var model = new CursoDashboardViewModel
        {
            Id = curso.Id,
            Nome = curso.Nome,
            Descricao = curso.Descricao,
            Ativo = curso.Ativo,
            NomeCampus = curso.Campus.Nome,
            TotalTurmas = curso.Turmas.Count,
            TotalCasais = curso.Turmas.Sum(t => t.Matriculas.Count),
            CasaisAtivos = curso.Turmas.Sum(t => t.Matriculas.Count(m => m.Casal.Status == StatusCasal.Ativo)),
            Turmas = turmas
        };

        return View(model);
    }

    public IActionResult Buscar(string filtro, string campusId)
    {
        var curso = _cursoService.FindAll().Where(c => c.Nome.ToLower().Contains(filtro.ToLower()) && c.IdCampus == int.Parse(campusId)).Select(c => new
        {
            Id = c.Id,
            Nome = c.Nome,
            NomeCampus = c.Campus.Nome,
        });

        Console.WriteLine(curso);

        return Json(curso);


    }



}
