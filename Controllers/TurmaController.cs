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
using Microsoft.AspNetCore.Identity;
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
    private readonly UserManager<Usuario> _userManager;

    public TurmaController(
        ITurmaService turmaService,
        ICursoService cursoService,
        ICampusService campusService,
        ILiderService liderService,
        INotificationService notification,
        IUtilService utilService,
        IAgendaService agendaService,
        ILicaoService licaoService,
        UserManager<Usuario> userManager)
    {
        _turmaService = turmaService;
        _cursoService = cursoService;
        _campusService = campusService;
        _liderService = liderService;
        _notification = notification;
        _utilService = utilService;
        _agendaService = agendaService;
        _licaoService = licaoService;
        _userManager = userManager;
    }

    // ── Retorna IdCampus do usuário se não for Admin, null se for Admin ──
    private async Task<int?> GetCampusFiltroAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return null;

        var roles = await _userManager.GetRolesAsync(user);
        if (roles.Contains("Admin"))
            return null;

        return user.IdCampus;
    }

    public async Task<IActionResult> Index(int? idCurso, int? idCampus, StatusTurma? status, int pagina = 1)
    {
        var campusFiltro = await GetCampusFiltroAsync();

        // Se não é Admin, força o campus do usuário e ignora filtro da URL
        if (campusFiltro.HasValue)
            idCampus = campusFiltro.Value;

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

        // Cursos e campus filtrados pelo campus do usuário quando não for Admin
        var cursos = campusFiltro.HasValue
            ? _cursoService.FindAll().Where(c => c.IdCampus == campusFiltro.Value)
            : _cursoService.FindAll();

        var campusList = campusFiltro.HasValue
            ? _campusService.FindAll().Where(c => c.Id == campusFiltro.Value)
            : _campusService.FindAll();

        ViewBag.Cursos = new SelectList(cursos, "Id", "Nome", idCurso);
        ViewBag.Campuses = new SelectList(campusList, "Id", "Nome", idCampus);
        ViewBag.Status = new SelectList(Enum.GetValues(typeof(StatusTurma)), status);
        ViewBag.EhAdmin = !campusFiltro.HasValue;

        const int TAMANHO_PAGINA = 20;
        ViewBag.NumeroPagina = pagina;
        ViewBag.TotalPaginas = Math.Ceiling((decimal)viewModel.Count() / TAMANHO_PAGINA);

        return View(viewModel.Skip((pagina - 1) * TAMANHO_PAGINA).Take(TAMANHO_PAGINA));
    }

    public async Task<IActionResult> Dashboard(int id)
    {
        var turma = _turmaService.FindById(id);
        if (turma == null) return NotFound();

        // Verifica acesso
        var campusFiltro = await GetCampusFiltroAsync();
        if (campusFiltro.HasValue && turma.IdCampus != campusFiltro.Value)
            return Forbid();

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

        // Verifica acesso ao campus informado no formulário
        var campusFiltro = await GetCampusFiltroAsync();
        if (campusFiltro.HasValue && model.IdCampus != campusFiltro.Value)
            return Forbid();

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

    public async Task<IActionResult> Editar(int id)
    {
        var turma = _turmaService.FindById(id);
        if (turma == null) return NotFound();

        // Verifica acesso
        var campusFiltro = await GetCampusFiltroAsync();
        if (campusFiltro.HasValue && turma.IdCampus != campusFiltro.Value)
            return Forbid();

        var model = new TurmaViewModel
        {
            Id = turma.Id,
            IdCurso = turma.IdCurso,
            NomeCurso = turma.Curso.Nome,
            IdCampus = turma.IdCampus,
            NomeCampus = turma.Campus.Nome,
            IdLider = turma.IdLider,
            NomeLider = _utilService.GetNomeSobrenome(turma.Lider.NomeMarido, turma.Lider.NomeEsposa),
            imgLider = turma.Lider.FotoPerfil,
            DataInicio = turma.DataInicio,
            DataFim = turma.DataFim,
            Status = turma.Status
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(TurmaViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var turma = _turmaService.FindById(model.Id);
        if (turma == null) return NotFound();

        // Verifica acesso à turma original
        var campusFiltro = await GetCampusFiltroAsync();
        if (campusFiltro.HasValue && turma.IdCampus != campusFiltro.Value)
            return Forbid();

        turma.IdCurso = model.IdCurso;
        turma.IdCampus = model.IdCampus;
        turma.IdLider = model.IdLider;
        turma.DataInicio = model.DataInicio;
        turma.DataFim = model.DataFim;
        turma.Status = model.Status;

        _turmaService.Update(turma);
        _notification.Success("Turma atualizada com sucesso!");

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Relatorios(int idTurma)
    {
        // Verifica acesso à turma
        var turma = _turmaService.FindById(idTurma);
        if (turma != null)
        {
            var campusFiltro = await GetCampusFiltroAsync();
            if (campusFiltro.HasValue && turma.IdCampus != campusFiltro.Value)
                return Forbid();
        }

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
