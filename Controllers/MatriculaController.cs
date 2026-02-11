using EnsinoApp.Models.Entities;
using EnsinoApp.Services.Casal;
using EnsinoApp.Services.Inscricao;
using EnsinoApp.Services.Matricula;
using EnsinoApp.Services.Turmas;
using EnsinoApp.ViewModels.Matricula;
using Microsoft.AspNetCore.Mvc;

namespace EnsinoApp.Controllers;

public class MatriculaController : Controller
{
    private readonly ICasalService _casalService;
    private readonly IInscricaoOnlineService _inscricaoService;
    private readonly IMatriculaService _matriculaService;
    private readonly ITurmaService _turmaService;

    public MatriculaController(
            ICasalService casalService,
            IInscricaoOnlineService inscricaoService,
            IMatriculaService matriculaService,
            ITurmaService turmaService)
    {
        _casalService = casalService;
        _inscricaoService = inscricaoService;
        _matriculaService = matriculaService;
        _turmaService = turmaService;
    }

    public IActionResult Index()
    {
        var dashboard = new MatriculaDashboardViewModel
        {
            TotalCasais = _casalService.ContarTotal(),
            TotalInscricoes = _inscricaoService.ContarTotal(),
            InscricoesPendentes = _inscricaoService.ContarPendentes(),
            MatriculasAtivas = _matriculaService.ContarAtivas(),
            TurmasAtivas = _turmaService.ContarAtivas(),

            InscricoesPendentesLista = _inscricaoService.ObterPendentesResumo(),
            Casais = _casalService.ObterResumoCasais(),
            Turmas = _turmaService.ObterResumoTurmasAtivas()
        };

        return View(dashboard);
    }

    public async Task<IActionResult> Details(int id)
    {
        var matricula = await _matriculaService.FindByIdAsync(id);
        if (matricula == null) return NotFound();
        return View(matricula);
    }

    public IActionResult Cadastrar()
    {
        //ViewBag.Casais = _casalService.FindAll();
        ViewBag.Turmas = _turmaService.FindAll();
        return View(new MatriculaViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cadastrar(MatriculaViewModel model)
    {
        if (!ModelState.IsValid)
        {
            //ViewBag.Casais = _casalService.FindAll();
            ViewBag.Turmas = _turmaService.FindAll();
            return View(model);
        }

        var matricula = new Matricula
        {
            IdCasal = model.IdCasal,
            IdTurma = model.IdTurma,
            DataMatricula = DateTime.Now,
            NomeGC = model.NomeGC,
            Status = Models.Enums.StatusMatricula.Ativa
        };

        await _matriculaService.CreateAsync(matricula);
        return RedirectToAction(nameof(Index));
    }


}