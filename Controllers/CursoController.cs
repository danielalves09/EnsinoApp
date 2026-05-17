// ── Arquivo: Controllers/CursoController.cs ──────────────────────────────────
// Alterações em relação ao original:
//   1. Injetar ILayoutCertificadoService
//   2. Preencher LayoutsDisponiveis no Adicionar e Editar
//   3. Mapear IdLayoutCertificado no Create/Update
//   4. Incluir LayoutCertificado no FindByIdDashboard

using EnsinoApp.Models.Entities;
using EnsinoApp.Services.Campus;
using EnsinoApp.Services.Cursos;
using EnsinoApp.Services.LayoutCertificado;
using EnsinoApp.Services.Util;
using EnsinoApp.ViewModels.Cursos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using EnsinoApp.Models.Enums;

namespace EnsinoApp.Controllers;

[Authorize(Roles = "Admin,Pastor,Coordenador")]
public class CursoController : Controller
{
    private readonly ICursoService _cursoService;
    private readonly ICampusService _campusService;
    private readonly IUtilService _utilService;
    private readonly ILayoutCertificadoService _layoutService;


    private const int TAMANHO_PAGINA = 10;

    public CursoController(
        ICursoService cursoService,
        ICampusService campusService,
        IUtilService utilService,
        ILayoutCertificadoService layoutService)
    {
        _cursoService = cursoService;
        _campusService = campusService;
        _utilService = utilService;
        _layoutService = layoutService;
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
            NomeCampus = c.Campus?.Nome ?? string.Empty,
            IdLayoutCertificado = c.IdLayoutCertificado,
            NomeLayoutCertificado = c.LayoutCertificado?.Nome
        });

        ViewBag.Filtro = filtro;
        ViewBag.NumeroPagina = pagina;
        ViewBag.TotalPaginas = Math.Ceiling((decimal)viewModel.Count() / TAMANHO_PAGINA);

        return View(viewModel.Skip((pagina - 1) * TAMANHO_PAGINA).Take(TAMANHO_PAGINA));
    }


    public async Task<IActionResult> Adicionar()
    {
        var vm = new CursoViewModel();
        await PreencherSelectLists(vm);
        return View(vm);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Adicionar(CursoViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await PreencherSelectLists(model);
            return View(model);
        }

        var curso = new Curso
        {
            Nome = model.Nome,
            Descricao = model.Descricao,
            Ativo = model.Ativo,
            IdCampus = model.IdCampus,
            IdLayoutCertificado = model.IdLayoutCertificado
        };

        _cursoService.Create(curso);
        return RedirectToAction(nameof(Index));
    }


    public async Task<IActionResult> Editar(int id)
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
            NomeCampus = curso.Campus?.Nome ?? string.Empty,
            IdLayoutCertificado = curso.IdLayoutCertificado,
            NomeLayoutCertificado = curso.LayoutCertificado?.Nome
        };

        await PreencherSelectLists(model);
        return View(model);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(CursoViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await PreencherSelectLists(model);
            return View(model);
        }

        var curso = _cursoService.FindById(model.Id);
        if (curso == null) return NotFound();

        curso.Nome = model.Nome;
        curso.Descricao = model.Descricao;
        curso.Ativo = model.Ativo;
        curso.IdCampus = model.IdCampus;
        curso.IdLayoutCertificado = model.IdLayoutCertificado;

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
        var curso = _cursoService.FindByIdDashboard(id);
        if (curso == null) return NotFound();

        var turmas = curso.Turmas.Select(t => new CursoDashboardViewModel.TurmaInfo
        {
            Id = t.Id,
            NomeLider = _utilService.GetNomeSobrenome(t.Lider.NomeMarido, t.Lider.NomeEsposa),
            imgLider = t.Lider.FotoPerfil,
            DataInicio = t.DataInicio,
            DataFim = t.DataFim,
            Status = t.Status,
            Casais = t.Matriculas.Select(m => new CursoDashboardViewModel.CasalInfo
            {
                Id = m.Casal.Id,
                NomeMarido = m.Casal.NomeConjuge1,
                NomeEsposa = m.Casal.NomeConjuge2,
                StatusCasal = (StatusCasal)m.Casal.Status,
                StatusPresenca = (StatusPresenca)m.Casal.Status
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
        var resultado = _cursoService.FindAll()
            .Where(c => c.Nome.ToLower().Contains(filtro.ToLower())
                     && c.IdCampus == int.Parse(campusId))
            .Select(c => new { Id = c.Id, Nome = c.Nome, NomeCampus = c.Campus.Nome });

        return Json(resultado);
    }

    private async Task PreencherSelectLists(CursoViewModel vm)
    {
        ViewBag.Campuses = new SelectList(
            _campusService.FindAll(), "Id", "Nome", vm.IdCampus);

        var layouts = await _layoutService.FindAllAtivosAsync();
        vm.LayoutsDisponiveis = new SelectList(
            layouts, "Id", "Nome", vm.IdLayoutCertificado);
    }
}