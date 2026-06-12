using EnsinoApp.Models.Entities;
using EnsinoApp.Models.Enums;
using EnsinoApp.Services.Campus;
using EnsinoApp.Services.Casal;
using EnsinoApp.Services.Cursos;
using EnsinoApp.Services.Inscricao;
using EnsinoApp.Services.Matricula;
using EnsinoApp.Services.Notifications;
using EnsinoApp.Services.Turmas;
using EnsinoApp.ViewModels.Inscricao;
using EnsinoApp.ViewModels.Matricula;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EnsinoApp.Controllers;

[Authorize(Roles = "Admin,Pastor,Coordenador")]
public class MatriculaController : Controller
{
    private readonly ICasalService _casalService;
    private readonly IInscricaoOnlineService _inscricaoService;
    private readonly IMatriculaService _matriculaService;
    private readonly ITurmaService _turmaService;
    private readonly ICursoService _cursoService;
    private readonly ICampusService _campusService;
    private readonly INotificationService _notificationService;
    private readonly UserManager<Usuario> _userManager;

    private const int TAMANHO_PAGINA = 10;

    public MatriculaController(
        ICasalService casalService,
        IInscricaoOnlineService inscricaoService,
        IMatriculaService matriculaService,
        ITurmaService turmaService,
        ICursoService cursoService,
        ICampusService campusService,
        INotificationService notificationService,
        UserManager<Usuario> userManager)
    {
        _casalService = casalService;
        _inscricaoService = inscricaoService;
        _matriculaService = matriculaService;
        _turmaService = turmaService;
        _cursoService = cursoService;
        _campusService = campusService;
        _notificationService = notificationService;
        _userManager = userManager;
    }

    // Retorna o IdCampus do usuário se não for Admin, null se for Admin ──
    private async Task<int?> GetCampusFiltroAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return null;

        var roles = await _userManager.GetRolesAsync(user);
        if (roles.Contains("Admin"))
            return null; // Admin vê tudo

        return user.IdCampus; // Pastor/Coordenador/outro tipo filtram pelo próprio campus
    }

    public async Task<IActionResult> Index(int? idCurso, int? idCampus, bool? apenasConvidados, int pagina = 1)
    {
        var campusFiltro = await GetCampusFiltroAsync();

        // Se não é Admin, força o campus do usuário e ignora filtro da URL
        if (campusFiltro.HasValue)
            idCampus = campusFiltro.Value;

        var listaInscricoesPendentes = _inscricaoService.ObterPendentesResumo();

        if (idCurso.HasValue)
            listaInscricoesPendentes = listaInscricoesPendentes.Where(i => i.IdCurso == idCurso.Value).ToList();

        if (idCampus.HasValue)
            listaInscricoesPendentes = listaInscricoesPendentes.Where(i => i.IdCampus == idCampus.Value).ToList();

        if (apenasConvidados == true)
            listaInscricoesPendentes = listaInscricoesPendentes.Where(i => i.Convidado).ToList();

        // Convidados sempre primeiro
        listaInscricoesPendentes = listaInscricoesPendentes
            .OrderByDescending(i => i.Convidado)
            .ThenBy(i => i.DataInscricao)
            .ToList();

        // Calcula totais filtrados por campus quando não for Admin
        int totalCasais;
        int totalInscricoes;
        int inscricoesPendentes;
        int matriculasAtivas;
        int turmasAtivas;

        if (campusFiltro.HasValue)
        {
            var casaisDoCampus = await _casalService.FindAllAsync();
            totalCasais = casaisDoCampus.Count(c => c.IdCampus == campusFiltro.Value);

            var todasInscricoes = _inscricaoService.ObterPendentesResumo();
            totalInscricoes = todasInscricoes.Count(i => i.IdCampus == campusFiltro.Value);
            inscricoesPendentes = listaInscricoesPendentes.Count;

            var todasMatriculas = await _matriculaService.FindAllAsync();
            matriculasAtivas = todasMatriculas.Count(m =>
                m.Turma?.Campus?.Id == campusFiltro.Value &&
                m.Status == StatusMatricula.Ativa);

            turmasAtivas = _turmaService.FindAll().Count(t =>
                t.IdCampus == campusFiltro.Value &&
                t.Status == StatusTurma.EmAndamento);
        }
        else
        {
            totalCasais = _casalService.ContarTotal();
            totalInscricoes = _inscricaoService.ContarTotal();
            inscricoesPendentes = _inscricaoService.ContarPendentes();
            matriculasAtivas = await _matriculaService.ContarAtivas();
            turmasAtivas = _turmaService.ContarAtivas();
        }

        var dashboard = new MatriculaDashboardViewModel
        {
            TotalCasais = totalCasais,
            TotalInscricoes = totalInscricoes,
            InscricoesPendentes = inscricoesPendentes,
            MatriculasAtivas = matriculasAtivas,
            TurmasAtivas = turmasAtivas,
            InscricoesPendentesLista = listaInscricoesPendentes,
            Turmas = _turmaService.ObterResumoTurmasAtivas()
        };

        ViewBag.IdCursoSelecionado = idCurso;
        ViewBag.IdCampusSelecionado = idCampus;
        ViewBag.ApenasConvidados = apenasConvidados;
        ViewBag.EhAdmin = !campusFiltro.HasValue;

        // Cursos e campus filtrados pelo campus do usuário quando não for Admin
        var cursos = campusFiltro.HasValue
            ? _cursoService.FindAll().Where(c => c.IdCampus == campusFiltro.Value)
            : _cursoService.FindAll();

        var campusList = campusFiltro.HasValue
            ? _campusService.FindAll().Where(c => c.Id == campusFiltro.Value)
            : _campusService.FindAll();

        ViewBag.Cursos = new SelectList(cursos, "Id", "Nome", idCurso);
        ViewBag.Campuses = new SelectList(campusList, "Id", "Nome", idCampus);
        ViewBag.NumeroPagina = pagina;
        ViewBag.TotalPaginas = (int)Math.Ceiling((decimal)listaInscricoesPendentes.Count / TAMANHO_PAGINA);

        dashboard.InscricoesPendentesLista = listaInscricoesPendentes
            .Skip((pagina - 1) * TAMANHO_PAGINA)
            .Take(TAMANHO_PAGINA)
            .ToList();

        return View(dashboard);
    }

    public async Task<IActionResult> Cadastrar(int id)
    {
        var inscricao = await _inscricaoService.FindByIdAsync(id);
        if (inscricao == null)
            return NotFound();

        // Verifica se o usuário tem acesso a esta inscrição
        var campusFiltro = await GetCampusFiltroAsync();
        if (campusFiltro.HasValue && inscricao.IdCampus != campusFiltro.Value)
            return Forbid();

        var model = new MatriculaFormViewModel
        {
            IdInscricao = inscricao.Id,
            NomeCasal = $"{inscricao.NomeMarido} e {inscricao.NomeEsposa}",
            NomeGC = inscricao.NomeGC,
            NomeCampus = inscricao.Campus.Nome,
            SelectTurmas = new SelectList(await _turmaService.FindAllAtivasAsync(inscricao.IdCurso), "Id", "Descricao")
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cadastrar(MatriculaFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.SelectTurmas = new SelectList(await _turmaService.FindAllAtivasAsync(model.IdCurso), "Id", "Descricao");
            return View(model);
        }

        var inscricao = await _inscricaoService.FindByIdAsync(model.IdInscricao);
        if (inscricao == null)
            return NotFound();

        // Verifica acesso
        var campusFiltro = await GetCampusFiltroAsync();
        if (campusFiltro.HasValue && inscricao.IdCampus != campusFiltro.Value)
            return Forbid();

        // ── Verifica se o casal já existe pelo e-mail de qualquer cônjuge ────
        var casal = await _casalService.FindByEmailAsync(inscricao.EmailMarido)
                 ?? await _casalService.FindByEmailAsync(inscricao.EmailEsposa);

        if (casal != null)
        {
            // Casal já existe — usa o registro existente e notifica o usuário
            _notificationService.Success(
                $"Matricula realizada com sucesso! " +
                $"Casal já cadastrado no sistema. Reutilizando o registro de " +
                $"{casal.NomeConjuge1} e {casal.NomeConjuge2}.");
        }
        else
        {
            // Casal não existe — cria um novo registro
            casal = new Casal
            {
                NomeConjuge1 = inscricao.NomeMarido,
                NomeConjuge2 = inscricao.NomeEsposa,
                TelefoneConjuge1 = inscricao.TelefoneMarido,
                TelefoneConjuge2 = inscricao.TelefoneEsposa,
                EmailConjuge1 = inscricao.EmailMarido,
                EmailConjuge2 = inscricao.EmailEsposa,
                DataNascimentoConjuge1 = inscricao.DataNascimentoMarido,
                DataNascimentoConjuge2 = inscricao.DataNascimentoEsposa,
                Rua = inscricao.Rua,
                Numero = inscricao.Numero,
                Complemento = inscricao.Complemento,
                Bairro = inscricao.Bairro,
                Cidade = inscricao.Cidade,
                Estado = inscricao.Estado,
                Cep = inscricao.Cep,
                Status = StatusCasal.Ativo,
                IdCampus = inscricao.IdCampus
            };

            casal = await _casalService.CreateAsync(casal);
        }
        // ─────────────────────────────────────────────────────────────────────

        var matricula = new Matricula
        {
            IdCasal = casal.Id,
            IdTurma = model.IdTurma,
            NomeGC = inscricao.NomeGC,
            Status = StatusMatricula.Ativa,
            DataMatricula = DateTime.Now
        };

        await _matriculaService.CreateAsync(matricula);

        inscricao.Processada = true;
        await _inscricaoService.UpdateAsync(inscricao);

        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> Detalhes(int id)
    {
        var inscricao = await _inscricaoService.FindByIdAsync(id);

        if (inscricao is null)
            return NotFound(new { mensagem = "Inscrição não encontrada." });

        // Verifica acesso
        var campusFiltro = await GetCampusFiltroAsync();
        if (campusFiltro.HasValue && inscricao.IdCampus != campusFiltro.Value)
            return Forbid();

        var vm = new InscricaoDetalheViewModel
        {
            Id = inscricao.Id,
            NomeConjuge1 = inscricao.NomeMarido,
            NomeConjuge2 = inscricao.NomeEsposa,
            EmailConjuge1 = inscricao.EmailMarido,
            EmailConjuge2 = inscricao.EmailEsposa,
            TelefoneConjuge1 = inscricao.TelefoneMarido,
            TelefoneConjuge2 = inscricao.TelefoneEsposa,
            DataNascimentoMarido = inscricao.DataNascimentoMarido,
            DataNascimentoEsposa = inscricao.DataNascimentoEsposa,
            Rua = inscricao.Rua ?? string.Empty,
            Numero = inscricao.Numero ?? string.Empty,
            Complemento = inscricao.Complemento,
            Bairro = inscricao.Bairro ?? string.Empty,
            Cidade = inscricao.Cidade ?? string.Empty,
            Estado = inscricao.Estado ?? string.Empty,
            Cep = inscricao.Cep ?? string.Empty,
            NomeCampus = inscricao.Campus?.Nome ?? string.Empty,
            GC = inscricao.NomeGC ?? string.Empty,
            Convidado = inscricao.Convidado,
            NomeCasalConvidador = inscricao.NomeCasalConvidador
        };

        return Json(vm);
    }

    [HttpPost]
    public async Task<IActionResult> DeletarInscricao(int idInscricao)
    {
        var inscricao = await _inscricaoService.FindByIdAsync(idInscricao);
        if (inscricao == null)
            return NotFound();

        // Verifica acesso
        var campusFiltro = await GetCampusFiltroAsync();
        if (campusFiltro.HasValue && inscricao.IdCampus != campusFiltro.Value)
            return Forbid();

        await _inscricaoService.DeletarAsync(idInscricao);
        _notificationService.Success("Inscrição deletada com sucesso.");
        return RedirectToAction("Index");
    }
}
