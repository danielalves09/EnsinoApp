using EnsinoApp.Models.Entities;
using EnsinoApp.Services.Matricula;
using EnsinoApp.Services.Turmas;
using EnsinoApp.ViewModels.Matricula;
using Microsoft.AspNetCore.Mvc;

namespace EnsinoApp.Controllers;

public class MatriculaController : Controller
{
    private readonly IMatriculaService _matriculaService;
    //private readonly ICasalService _casalService;
    private readonly ITurmaService _turmaService;

    public MatriculaController(IMatriculaService service)
    {
        _matriculaService = service;
    }

    public async Task<IActionResult> Index()
    {
        var matriculas = await _matriculaService.FindAllAsync();
        return View(matriculas);
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