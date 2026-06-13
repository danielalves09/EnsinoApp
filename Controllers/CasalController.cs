using EnsinoApp.Models.Entities;
using EnsinoApp.Models.Enums;
using EnsinoApp.Services.Campus;
using EnsinoApp.Services.Casal;
using EnsinoApp.Services.Notifications;
using EnsinoApp.ViewModels.Casal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EnsinoApp.Controllers;

[Authorize(Roles = "Admin,Pastor,Coordenador")]
public class CasalController : Controller
{
    private readonly ICasalService _casalService;
    private readonly ICampusService _campusService;
    private readonly INotificationService _notification;
    private readonly UserManager<Usuario> _userManager;
    private const int TAMANHO_PAGINA = 15;

    public CasalController(
        ICasalService casalService,
        ICampusService campusService,
        INotificationService notification,
        UserManager<Usuario> userManager)
    {
        _casalService = casalService;
        _campusService = campusService;
        _notification = notification;
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

    public async Task<IActionResult> Index(int? idCampus, StatusCasal? status, string? filtro, int pagina = 1)
    {
        var campusFiltro = await GetCampusFiltroAsync();

        // Se não é Admin, força o campus do usuário e ignora filtro da URL
        if (campusFiltro.HasValue)
            idCampus = campusFiltro.Value;

        var casais = await _casalService.FindAllAsync();

        // Filtros
        if (idCampus.HasValue)
            casais = casais.Where(c => c.IdCampus == idCampus.Value).ToList();

        if (status.HasValue)
            casais = casais.Where(c => c.Status == status.Value).ToList();

        if (!string.IsNullOrWhiteSpace(filtro))
        {
            filtro = filtro.Trim().ToLower();
            casais = casais.Where(c =>
                c.NomeConjuge1.ToLower().Contains(filtro) ||
                c.NomeConjuge2.ToLower().Contains(filtro) ||
                c.EmailConjuge1.ToLower().Contains(filtro) ||
                c.EmailConjuge2.ToLower().Contains(filtro)
            ).ToList();
        }

        // Campus no dropdown filtrado pelo campus do usuário quando não for Admin
        var campusList = campusFiltro.HasValue
            ? _campusService.FindAll().Where(c => c.Id == campusFiltro.Value)
            : _campusService.FindAll();

        ViewBag.Campuses = new SelectList(campusList, "Id", "Nome", idCampus);
        ViewBag.Status = new SelectList(Enum.GetValues(typeof(StatusCasal)), status);
        ViewBag.Filtro = filtro;
        ViewBag.EhAdmin = !campusFiltro.HasValue;
        ViewBag.NumeroPagina = pagina;
        ViewBag.TotalPaginas = (int)Math.Ceiling((decimal)casais.Count / TAMANHO_PAGINA);

        return View(casais.Skip((pagina - 1) * TAMANHO_PAGINA).Take(TAMANHO_PAGINA));
    }

    public IActionResult Adicionar()
    {
        return View(new AdicionarCasalViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Adicionar(AdicionarCasalViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        // Verifica acesso ao campus informado no formulário
        var campusFiltro = await GetCampusFiltroAsync();
        if (campusFiltro.HasValue && model.IdCampus != campusFiltro.Value)
            return Forbid();

        var casal = new Casal
        {
            NomeConjuge1 = model.NomeConjuge1,
            NomeConjuge2 = model.NomeConjuge2,
            TelefoneConjuge1 = model.TelefoneConjuge1,
            TelefoneConjuge2 = model.TelefoneConjuge2,
            EmailConjuge1 = model.EmailConjuge1,
            EmailConjuge2 = model.EmailConjuge2,
            IdCampus = model.IdCampus,
            Status = model.Status,
            Cep = model.Cep ?? string.Empty,
            Rua = model.Rua ?? string.Empty,
            Numero = model.Numero ?? string.Empty,
            Complemento = model.Complemento,
            Bairro = model.Bairro ?? string.Empty,
            Cidade = model.Cidade ?? string.Empty,
            Estado = model.Estado ?? string.Empty,
        };

        await _casalService.CreateAsync(casal);
        _notification.Success("Casal cadastrado com sucesso!");
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Editar(int id)
    {
        var casal = await _casalService.FindByIdAsync(id);
        if (casal == null) return NotFound();

        // Verifica acesso
        var campusFiltro = await GetCampusFiltroAsync();
        if (campusFiltro.HasValue && casal.IdCampus != campusFiltro.Value)
            return Forbid();

        var campus = _campusService.FindById(casal.IdCampus);

        var model = new AdicionarCasalViewModel
        {
            Id = casal.Id,
            NomeConjuge1 = casal.NomeConjuge1,
            NomeConjuge2 = casal.NomeConjuge2,
            TelefoneConjuge1 = casal.TelefoneConjuge1,
            TelefoneConjuge2 = casal.TelefoneConjuge2,
            EmailConjuge1 = casal.EmailConjuge1,
            EmailConjuge2 = casal.EmailConjuge2,
            IdCampus = casal.IdCampus,
            NomeCampus = campus?.Nome ?? string.Empty,
            Status = casal.Status,
            Cep = casal.Cep,
            Rua = casal.Rua,
            Numero = casal.Numero,
            Complemento = casal.Complemento,
            Bairro = casal.Bairro,
            Cidade = casal.Cidade,
            Estado = casal.Estado,
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(AdicionarCasalViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var casal = await _casalService.FindByIdAsync(model.Id);
        if (casal == null) return NotFound();

        // Verifica acesso ao casal original
        var campusFiltro = await GetCampusFiltroAsync();
        if (campusFiltro.HasValue && casal.IdCampus != campusFiltro.Value)
            return Forbid();

        casal.NomeConjuge1 = model.NomeConjuge1;
        casal.NomeConjuge2 = model.NomeConjuge2;
        casal.TelefoneConjuge1 = model.TelefoneConjuge1;
        casal.TelefoneConjuge2 = model.TelefoneConjuge2;
        casal.EmailConjuge1 = model.EmailConjuge1;
        casal.EmailConjuge2 = model.EmailConjuge2;
        casal.IdCampus = model.IdCampus;
        casal.Status = model.Status;
        casal.Cep = model.Cep ?? string.Empty;
        casal.Rua = model.Rua ?? string.Empty;
        casal.Numero = model.Numero ?? string.Empty;
        casal.Complemento = model.Complemento;
        casal.Bairro = model.Bairro ?? string.Empty;
        casal.Cidade = model.Cidade ?? string.Empty;
        casal.Estado = model.Estado ?? string.Empty;

        await _casalService.UpdateAsync(casal);
        _notification.Success("Casal atualizado com sucesso!");
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Excluir(int id)
    {
        var casal = await _casalService.FindByIdAsync(id);
        if (casal == null) return NotFound();

        // Verifica acesso
        var campusFiltro = await GetCampusFiltroAsync();
        if (campusFiltro.HasValue && casal.IdCampus != campusFiltro.Value)
            return Forbid();

        await _casalService.DeleteAsync(id);
        _notification.Success("Casal excluído com sucesso!");
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Detalhes(int id)
    {
        var casal = await _casalService.FindByIdComMatriculasAsync(id);

        if (casal is null)
            return NotFound(new { mensagem = "Casal não encontrado." });

        // Verifica acesso
        var campusFiltro = await GetCampusFiltroAsync();
        if (campusFiltro.HasValue && casal.IdCampus != campusFiltro.Value)
            return Forbid();

        var vm = new CasalDetalheViewModel
        {
            Id = casal.Id,
            NomeConjuge1 = casal.NomeConjuge1,
            NomeConjuge2 = casal.NomeConjuge2,
            EmailConjuge1 = casal.EmailConjuge1,
            EmailConjuge2 = casal.EmailConjuge2,
            TelefoneConjuge1 = casal.TelefoneConjuge1,
            TelefoneConjuge2 = casal.TelefoneConjuge2,
            Rua = casal.Rua,
            Numero = casal.Numero,
            Complemento = casal.Complemento,
            Bairro = casal.Bairro,
            Cidade = casal.Cidade,
            Estado = casal.Estado,
            Cep = casal.Cep,
            NomeCampus = casal.Campus?.Nome ?? string.Empty,
            Status = casal.Status.ToString(),

            Matriculas = casal.Matriculas.Select(m => new CasalDetalheViewModel.MatriculaResumoViewModel
            {
                NomeCurso = m.Turma?.Curso?.Nome ?? string.Empty,
                NomeTurma = $"Turma #{m.IdTurma}",
                Status = m.Status.ToString(),
                DataMatricula = m.DataMatricula,
                DataConclusao = m.DataConclusao,
                CertificadoEmitido = m.CertificadoEmitido
            })
            .OrderByDescending(m => m.DataMatricula)
            .ToList()
        };

        return Json(vm);
    }
}
