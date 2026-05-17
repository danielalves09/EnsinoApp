using System.ComponentModel.DataAnnotations;

namespace EnsinoApp.ViewModels.LayoutCertificado;

public class LayoutCertificadoFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "O nome do layout é obrigatório")]
    [MaxLength(200, ErrorMessage = "Máximo 200 caracteres")]
    [Display(Name = "Nome do Layout")]
    public string Nome { get; set; } = null!;

    [MaxLength(500, ErrorMessage = "Máximo 500 caracteres")]
    [Display(Name = "Descrição")]
    public string? Descricao { get; set; }

    [Required(ErrorMessage = "O template HTML é obrigatório")]
    [Display(Name = "Template HTML")]
    public string TemplateHtml { get; set; } = TemplateDefault;

    [Display(Name = "Imagem de Fundo Atual")]
    public string? ImagemFundoUrl { get; set; }

    [Required(ErrorMessage = "A orientação é obrigatória")]
    [Display(Name = "Orientação")]
    public string Orientacao { get; set; } = "Landscape";

    [Display(Name = "Layout Ativo")]
    public bool Ativo { get; set; } = true;

    // Upload de nova imagem de fundo (opcional)
    public IFormFile? NovaImagemFundo { get; set; }

    // Variáveis disponíveis para exibir no editor
    public List<(string Variavel, string Descricao)> VariaveisDisponiveis { get; set; } = new();

    // ── Template padrão ao criar novo layout ────────────────────────────────
    public const string TemplateDefault = @"<!DOCTYPE html>
<html lang=""pt-br"">
<head>
  <meta charset=""UTF-8"">
  <style>
    html, body { width:100%; height:100%; margin:0; padding:0; font-family:Arial,sans-serif; }
    body {
      background-image: url(""{{FundoUrl}}"");
      background-size: 100% 100%;
      background-repeat: no-repeat;
    }
    .certificado {
      width:100%; height:100%;
      padding:50px 60px; box-sizing:border-box;
      display:flex; flex-direction:column;
      justify-content:flex-start; align-items:center;
      text-align:center;
    }
    .logo        { width:150px; margin:20px auto; }
    .subtitulo   { font-size:18px; margin-top:70px; }
    .nome-casal  { font-size:45px; color:#000; margin:10px 0; font-style:italic; }
    .curso       { font-size:24px; color:#27AE60; font-weight:bold; margin-top:20px 0 10px; }
    .data        { font-size:16px; margin-bottom:40px; }
    .assinaturas { width:100%; display:table; margin-top:50px; }
    .assinatura  { display:table-cell; text-align:center; vertical-align:top; }
    .linha       { border-top:1px solid #000; width:200px; margin:0 auto 5px; }
    .footer      { width:100%; text-align:center; font-size:14px; margin-top:40px; }
  </style>
</head>
<body>
  <div class=""certificado"">
    <img src=""{{LogoUrl}}"" class=""logo"" />
    <div class=""subtitulo"">Certificamos que o casal</div>
    <div class=""nome-casal"">{{NomeCasal}}</div>
    <div style=""font-size:18px;margin-top:40px"">concluiu com êxito o curso</div>
    <div class=""curso"">{{NomeCurso}}</div>
    <div class=""data"">Concluído em: {{DataConclusao}}</div>
    <div class=""assinaturas"">
      <div class=""assinatura"">
        <div class=""linha""></div>
        <div><b>{{NomeLider}}</b></div>
        <div>Líderes</div>
      </div>
      <div style=""display:table-cell;text-align:center;vertical-align:top"">
        <img src=""{{QRCodeBase64}}"" alt=""QR Code"" style=""width:120px;height:120px;"" />
        <div style=""font-size:12px"">Escaneie para validar</div>
      </div>
      <div class=""assinatura"">
        <div class=""linha""></div>
        <div><b>{{NomeCampus}}</b></div>
        <div>Campus</div>
      </div>
    </div>
    <div class=""footer"">
      EnsinoApp • Certificado Oficial • Código: <b>{{CodigoValidacao}}</b>
    </div>
  </div>
</body>
</html>";
}