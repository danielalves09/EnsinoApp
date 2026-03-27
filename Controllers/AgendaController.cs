using EnsinoApp.Services.Agenda;
using EnsinoApp.Services.Turmas;
using EnsinoApp.ViewModels.Agenda;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnsinoApp.Controllers;

[Authorize(Roles = "Admin,Pastor,Coordenador,Lider")]
public class AgendaController : Controller
{
    private readonly IAgendaService _agendaService;
    private readonly ITurmaService _turmaService;

    public AgendaController(IAgendaService agendaService, ITurmaService turmaService)
    {
        _agendaService = agendaService;
        _turmaService = turmaService;
    }

    // GET /agenda/turma/{idTurma}
    public async Task<IActionResult> Index(int idTurma)
    {
        var turma = _turmaService.FindById(idTurma);
        if (turma is null) return NotFound();

        var agenda = await _agendaService.GetAgendaTurmaAsync(idTurma);

        ViewBag.IdTurma = idTurma;
        ViewBag.NomeCurso = turma.Curso.Nome;
        ViewBag.NomeCampus = turma.Campus.Nome;
        ViewBag.DiaSemana = DiaSemanaLabel(turma.DiaSemana);

        return View(agenda);
    }

    // GET /agenda/editar/{id}
    public async Task<IActionResult> Editar(int id)
    {
        var item = await _agendaService.FindByIdAsync(id);
        if (item is null) return NotFound();

        var vm = new EditarAgendaViewModel
        {
            Id = item.Id,
            IdTurma = item.IdTurma,
            NumeroLicao = item.Licao.NumeroSemana,
            TituloLicao = item.Licao.Titulo,
            DataAula = item.DataAula,
            Local = item.Local,
            Observacoes = item.Observacoes
        };

        return View(vm);
    }

    // POST /agenda/editar/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(EditarAgendaViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        await _agendaService.AtualizarLocalAsync(model.Id, model.Local, model.Observacoes);

        TempData["ToastrSuccess"] = "Local da aula atualizado com sucesso!";
        return RedirectToAction(nameof(Index), new { idTurma = model.IdTurma });
    }

    // GET /agenda/eventos/{idTurma}  ← chamado pelo FullCalendar via fetch
    [HttpGet]
    public async Task<IActionResult> Eventos(int idTurma)
    {
        var agenda = await _agendaService.GetAgendaTurmaAsync(idTurma);

        var eventos = agenda.Select(a => new CalendarioEventoDto
        {
            Title = $"{a.NumeroLicao}. {a.TituloLicao}",
            Start = a.DataAula.ToString("yyyy-MM-dd"),
            Url = Url.Action("Editar", "Agenda", new { id = a.Id }),
            Color = a.StatusAula switch
            {
                StatusAula.Realizada => "#16a34a",
                StatusAula.Hoje => "#7c3aed",
                StatusAula.Futura => "#2563eb",
                _ => "#2563eb"
            }
        });

        return Json(eventos);
    }

    // ─── Helper ───────────────────────────────────────────────────────────────

    private static string DiaSemanaLabel(DayOfWeek d) => d switch
    {
        DayOfWeek.Sunday => "Domingo",
        DayOfWeek.Monday => "Segunda-feira",
        DayOfWeek.Tuesday => "Terça-feira",
        DayOfWeek.Wednesday => "Quarta-feira",
        DayOfWeek.Thursday => "Quinta-feira",
        DayOfWeek.Friday => "Sexta-feira",
        DayOfWeek.Saturday => "Sábado",
        _ => "-"
    };
}