using EnsinoApp.Models.Entities;
using EnsinoApp.Services.Turmas;
using EnsinoApp.ViewModels.Turmas;
using Microsoft.AspNetCore.Mvc;

namespace EnsinoApp.Controllers;

public class TurmaController : Controller
{
    private readonly ITurmaService _turmaService;

    public TurmaController(ITurmaService turmaService)
    {
        _turmaService = turmaService;
    }

    public IActionResult Index()
    {
        var turmas = _turmaService.FindAll();
        return View(turmas);
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