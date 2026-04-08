using EnsinoApp.Models.Entities;
using EnsinoApp.Services.Campus;
using EnsinoApp.Services.Notifications;
using EnsinoApp.Services.PeriodoInscricao;
using EnsinoApp.ViewModels.PeriodoInscricao;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnsinoApp.Controllers;

[Authorize(Roles = "Admin,Pastor,Coordenador")]
public class PeriodoInscricaoController : Controller
{
    private readonly IPeriodoInscricaoService _service;
    private readonly ICampusService _campusService;
    private readonly INotificationService _notification;

    public PeriodoInscricaoController(
        IPeriodoInscricaoService service,
        ICampusService campusService,
        INotificationService notification)
    {
        _service = service;
        _campusService = campusService;
        _notification = notification;
    }

    // GET: /PeriodoInscricao/Gerenciar?idCurso=1
    public async Task<IActionResult> Gerenciar(int idCurso, string nomeCurso)
    {
        var periodos = await _service.FindByCursoAsync(idCurso);

        var vm = periodos.Select(p => new PeriodoInscricaoViewModel
        {
            Id = p.Id,
            IdCurso = p.IdCurso,
            IdCampus = p.IdCampus,
            NomeCampus = p.Campus.Nome,
            NomeCurso = p.Curso.Nome,
            DataAbertura = p.DataAbertura,
            DataEncerramento = p.DataEncerramento,
            VagasTotal = p.VagasTotal,
            VagasOcupadas = p.VagasOcupadas,
            Ativo = p.Ativo
        }).ToList();

        ViewBag.IdCurso = idCurso;
        ViewBag.NomeCurso = nomeCurso;

        return View(vm);
    }

    // GET: /PeriodoInscricao/Adicionar?idCurso=1
    public IActionResult Adicionar(int idCurso, string nomeCurso)
    {
        var campuses = _campusService.FindAll();
        ViewBag.Campuses = campuses;
        ViewBag.NomeCurso = nomeCurso;

        return View(new PeriodoInscricaoFormViewModel
        {
            IdCurso = idCurso,
            NomeCurso = nomeCurso,
            DataAbertura = DateTime.Today,
            DataEncerramento = DateTime.Today.AddDays(30)
        });
    }

    // POST: /PeriodoInscricao/Adicionar
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Adicionar(PeriodoInscricaoFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Campuses = _campusService.FindAll();
            ViewBag.NomeCurso = model.NomeCurso;
            return View(model);
        }

        if (model.DataEncerramento <= model.DataAbertura)
        {
            ModelState.AddModelError(nameof(model.DataEncerramento),
                "A data de encerramento deve ser posterior à abertura.");
            ViewBag.Campuses = _campusService.FindAll();
            ViewBag.NomeCurso = model.NomeCurso;
            return View(model);
        }

        var periodo = new PeriodoInscricao
        {
            IdCurso = model.IdCurso,
            IdCampus = model.IdCampus,
            DataAbertura = model.DataAbertura,
            DataEncerramento = model.DataEncerramento,
            VagasTotal = model.VagasTotal,
            VagasOcupadas = 0,
            Ativo = model.Ativo
        };

        await _service.CreateAsync(periodo);
        _notification.Success("Período de inscrição criado com sucesso!");

        return RedirectToAction(nameof(Gerenciar),
            new { idCurso = model.IdCurso, nomeCurso = model.NomeCurso });
    }

    // GET: /PeriodoInscricao/Editar/5
    public async Task<IActionResult> Editar(int id)
    {
        var periodo = await _service.FindByIdAsync(id);
        if (periodo is null) return NotFound();

        ViewBag.Campuses = _campusService.FindAll();
        ViewBag.NomeCurso = periodo.Curso.Nome;

        return View(new PeriodoInscricaoFormViewModel
        {
            Id = periodo.Id,
            IdCurso = periodo.IdCurso,
            NomeCurso = periodo.Curso.Nome,
            IdCampus = periodo.IdCampus,
            NomeCampus = periodo.Campus.Nome,
            DataAbertura = periodo.DataAbertura,
            DataEncerramento = periodo.DataEncerramento,
            VagasTotal = periodo.VagasTotal,
            Ativo = periodo.Ativo
        });
    }

    // POST: /PeriodoInscricao/Editar
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(PeriodoInscricaoFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Campuses = _campusService.FindAll();
            ViewBag.NomeCurso = model.NomeCurso;
            return View(model);
        }

        if (model.DataEncerramento <= model.DataAbertura)
        {
            ModelState.AddModelError(nameof(model.DataEncerramento),
                "A data de encerramento deve ser posterior à abertura.");
            ViewBag.Campuses = _campusService.FindAll();
            ViewBag.NomeCurso = model.NomeCurso;
            return View(model);
        }

        var periodo = await _service.FindByIdAsync(model.Id);
        if (periodo is null) return NotFound();

        periodo.IdCampus = model.IdCampus;
        periodo.DataAbertura = model.DataAbertura;
        periodo.DataEncerramento = model.DataEncerramento;
        periodo.VagasTotal = model.VagasTotal;
        periodo.Ativo = model.Ativo;

        await _service.UpdateAsync(periodo);
        _notification.Success("Período atualizado com sucesso!");

        return RedirectToAction(nameof(Gerenciar),
            new { idCurso = model.IdCurso, nomeCurso = model.NomeCurso });
    }

    // POST: /PeriodoInscricao/ToggleAtivo/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleAtivo(int id, int idCurso, string nomeCurso)
    {
        try
        {
            await _service.ToggleAtivoAsync(id);
            _notification.Success("Status do período atualizado!");
        }
        catch (Exception ex)
        {
            _notification.Error(ex.Message);
        }

        return RedirectToAction(nameof(Gerenciar), new { idCurso, nomeCurso });
    }

    // POST: /PeriodoInscricao/Excluir/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Excluir(int id, int idCurso, string nomeCurso)
    {
        await _service.DeleteAsync(id);
        _notification.Info("Período removido.");
        return RedirectToAction(nameof(Gerenciar), new { idCurso, nomeCurso });
    }
}