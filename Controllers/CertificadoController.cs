using EnsinoApp.Services.Matricula;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnsinoApp.Controllers;

[AllowAnonymous]
public class CertificadoController : Controller
{
    private readonly IMatriculaService _matriculaService;

    public CertificadoController(IMatriculaService matriculaService)
    {
        _matriculaService = matriculaService;
    }

    [HttpGet("Certificado/Validar/{codigo?}")]
    public async Task<IActionResult> Validar(string? codigo)
    {

        if (string.IsNullOrWhiteSpace(codigo))
            return View();

        var matricula = await _matriculaService.GetByCodigoValidacaoAsync(codigo);

        if (matricula == null)
        {
            ViewBag.Mensagem = "Certificado não encontrado ou inválido.";
            return View();
        }

        return View("ResultadoValidacao", matricula);


    }

    [HttpPost]
    public IActionResult Verificar(string codigo)
    {
        return RedirectToAction(nameof(Validar), new { codigo });
    }


}