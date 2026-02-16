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

    [HttpGet]
    public IActionResult Validar() => View();

    [HttpPost]
    public async Task<IActionResult> Validar(string codigo)
    {
        if (string.IsNullOrWhiteSpace(codigo))
        {
            ViewBag.Mensagem = "Informe um código válido.";
            return View();
        }

        var matricula = await _matriculaService.GetByCodigoValidacaoAsync(codigo);
        if (matricula == null)
        {
            ViewBag.Mensagem = "Certificado não encontrado ou inválido.";
            return View();
        }

        return View("ResultadoValidacao", matricula);
    }


}