using EnsinoApp.Services.Casal;
using EnsinoApp.ViewModels.Casal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnsinoApp.Controllers;

[Authorize(Roles = "Admin,Pastor,Coordenador")]
public class CasalController : Controller
{
    private readonly ICasalService _casalService;

    public CasalController(ICasalService casalService)
    {
        _casalService = casalService;
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