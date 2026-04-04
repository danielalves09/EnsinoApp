using EnsinoApp.Models.Entities;
using EnsinoApp.Models.Enums;
using EnsinoApp.Services.Agenda;
using EnsinoApp.Services.Campus;
using EnsinoApp.Services.Cursos;
using EnsinoApp.Services.Licao;
using EnsinoApp.Services.Lider;
using EnsinoApp.Services.Notifications;
using EnsinoApp.Services.Turmas;
using EnsinoApp.Services.Util;
using EnsinoApp.ViewModels.Turmas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EnsinoApp.Controllers;

[Authorize(Roles = "Admin,Pastor,Coordenador")]
public class TurmaController : Controller
{
    private readonly ITurmaService _turmaService;
    private readonly ICursoService _cursoService;
    private readonly ICampusService _campusService;

    private readonly ILiderService _liderService;

    private readonly IAgendaService _agendaService;
    private readonly ILicaoService _licaoService;

    private readonly IUtilService _utilService;

    private readonly INotificationService _notification;

    public TurmaController(ITurmaService turmaService, ICursoService cursoService, ICampusService campusService, ILiderService liderService, INotificationService notification, IUtilService utilService, IAgendaService agendaService, ILicaoService licaoService)
    {
        _turmaService = turmaService;
        _cursoService = cursoService;
        _campusService = campusService;
        _liderService = liderService;
        _notification = notification;
        _utilService = utilService;
        _agendaService = agendaService;
        _licaoService = licaoService;
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
            NomeLider = _utilService.GetNomeSobrenome(t.Lider.NomeMarido, t.Lider.NomeEsposa),
            imgLider = t.Lider.FotoPerfil,
            DataInicio = t.DataInicio,
            DataFim = t.DataFim,
            DiaSemanaLabel = _utilService.DiaSemanaLabel(t.DiaSemana.ToString()),
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
            imgLider = turma.Lider.FotoPerfil,
            DataInicio = turma.DataInicio,
            DataFim = turma.DataFim,
            DiaSemanaLabel = _utilService.DiaSemanaLabel(turma.DiaSemana.ToString()),
            TotalLicoes = turma.Curso.Licoes.Count(),
            LicoesConcluidas = turma.Matriculas.Sum(m => m.Relatorios.Count) / (turma.Matriculas.Count == 0 ? 1 : turma.Matriculas.Count),
            Status = turma.Status,
            CasaisMatriculados = turma.Matriculas.Select(m => new CasalMatriculadoViewModel
            {
                Nome = $"{m.Casal.NomeConjuge1} / {m.Casal.NomeConjuge2}",
                PrimeiroNome = $"{m.Casal.NomeConjuge1.Split(' ')[0]} e {m.Casal.NomeConjuge2.Split(' ')[0]}",
                Presenca = m.Relatorios.OrderByDescending(r => r.DataRegistro).FirstOrDefault()?.Presenca ?? StatusPresenca.Ausente,
                QtdPresencas = m.Relatorios.Count(r => r.Presenca == StatusPresenca.Presente),
                QtdFaltas = m.Relatorios.Count(r => r.Presenca == StatusPresenca.Ausente),
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
    public async Task<IActionResult> Adicionar(TurmaViewModel model)
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
            Status = model.Status,
            DiaSemana = model.DiaSemana
        };

        _turmaService.Create(turma);

        // ── Geração automática da agenda ─────────────────────────────────────────
        // Busca as lições do curso para gerar as datas
        var licoes = await _licaoService.FindByCursoAsync(turma.IdCurso);

        if (licoes.Any())
        {
            await _agendaService.GerarAgendaAsync(turma, licoes);
            _notification.Success("Turma criada e agenda gerada automaticamente!");
        }
        else
        {
            _notification.Info("Turma criada. Adicione lições ao curso para gerar a agenda.");
        }
        // ─────────────────────────────────────────────────────────────────────────

        return RedirectToAction(nameof(Index));
    }

    /* [HttpPost]
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
    } */

    public async Task<IActionResult> Relatorios(int idTurma)
    {
        var relatorios = await _liderService.ObterRelatoriosAsync(idTurma);
        if (relatorios.Count > 0)
        {
            return View(relatorios);
        }
        else
        {
            _notification.Info("Nenhum relatório encontrado para esta turma.");
            return RedirectToAction(nameof(Turma), new { id = idTurma });
        }
    }
}