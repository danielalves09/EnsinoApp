using EnsinoApp.Models.Entities;
using EnsinoApp.Models.Enums;
using EnsinoApp.Services.Campus;
using EnsinoApp.Services.Casal;
using EnsinoApp.Services.Notifications;
using EnsinoApp.ViewModels.Casal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EnsinoApp.Controllers;

[Authorize(Roles = "Admin,Pastor,Coordenador")]
public class CasalController : Controller
{
    private readonly ICasalService _casalService;
    private readonly ICampusService _campusService;
    private readonly INotificationService _notification;
    private const int TAMANHO_PAGINA = 15;

    public CasalController(ICasalService casalService, ICampusService campusService, INotificationService notification)
    {
        _casalService = casalService;
        _campusService = campusService;
        _notification = notification;
    }

    public async Task<IActionResult> Index(int? idCampus, StatusCasal? status, string? filtro, int pagina = 1)
    {
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

        ViewBag.Campuses = new SelectList(_campusService.FindAll(), "Id", "Nome", idCampus);
        ViewBag.Status = new SelectList(Enum.GetValues(typeof(StatusCasal)), status);
        ViewBag.Filtro = filtro;
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

        await _casalService.DeleteAsync(id);
        _notification.Success("Casal excluído com sucesso!");
        return RedirectToAction(nameof(Index));
    }


    /// <summary>
    /// Retorna os dados completos de um casal em JSON para o modal da tela de matrículas.
    /// GET /Casal/Detalhes/{id}
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Detalhes(int id)
    {
        var casal = await _casalService.FindByIdComMatriculasAsync(id);

        if (casal is null)
            return NotFound(new { mensagem = "Casal não encontrado." });

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
