namespace EnsinoApp.Services.Email;

public class EmailTemplateService : IEmailTemplateService
{

  private static string dominio = "http://189.124.212.152";
  // ── Layout base compartilhado ──────────────────────────────────────
  private static string Wrap(string title, string bodyContent) => $@"
<!DOCTYPE html>
<html lang=""pt-BR"">
<head>
  <meta charset=""utf-8""/>
  <meta name=""viewport"" content=""width=device-width,initial-scale=1""/>
  <title>{title}</title>
</head>
<body style=""margin:0;padding:0;background:#f1f5f9;font-family:'Segoe UI',Arial,sans-serif"">
  <table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background:#f1f5f9;padding:32px 16px"">
    <tr><td align=""center"">

      <!-- Card -->
      <table width=""600"" cellpadding=""0"" cellspacing=""0""
             style=""background:#ffffff;border-radius:16px;overflow:hidden;
                    box-shadow:0 4px 24px rgba(0,0,0,.08);max-width:100%"">

        <!-- Header -->
        <tr>
          <td style=""background:linear-gradient(135deg,#1e40af 0%,#2563eb 100%);
                      padding:32px 40px;text-align:center"">
            <p style=""margin:0 0 8px;font-size:13px;color:rgba(255,255,255,.7);
                       letter-spacing:1px;text-transform:uppercase;font-weight:600"">
              CCVideira · Ensino
            </p>
            <h1 style=""margin:0;font-size:22px;font-weight:800;color:#ffffff;
                        letter-spacing:-.3px"">
              {title}
            </h1>
          </td>
        </tr>

        <!-- Body -->
        <tr><td style=""padding:36px 40px"">{bodyContent}</td></tr>

        <!-- Footer -->
        <tr>
          <td style=""background:#f8fafc;border-top:1px solid #e2e8f0;
                      padding:20px 40px;text-align:center"">
            <p style=""margin:0;font-size:12px;color:#94a3b8"">
              © {DateTime.Now.Year} EnsinoApp · CCVideira — Todos os direitos reservados
            </p>
            <p style=""margin:4px 0 0;font-size:11px;color:#cbd5e1"">
              Este é um e-mail automático, por favor não responda.
            </p>
          </td>
        </tr>

      </table>
    </td></tr>
  </table>
</body>
</html>";

  // ── Linha de info reutilizável ─────────────────────────────────────
  private static string InfoRow(string icon, string label, string value) => $@"
      <tr>
        <td style=""padding:10px 0;border-bottom:1px solid #f1f5f9;vertical-align:top"">
          <span style=""display:inline-block;width:28px;height:28px;border-radius:8px;
                        background:#eff6ff;color:#2563eb;text-align:center;
                        line-height:28px;font-size:13px;margin-right:10px;
                        vertical-align:middle"">{icon}</span>
          <span style=""font-size:12px;color:#94a3b8;font-weight:600;
                        text-transform:uppercase;letter-spacing:.3px"">{label}</span>
          <br/>
          <span style=""font-size:15px;font-weight:700;color:#1e293b;
                        margin-left:38px"">{value}</span>
        </td>
      </tr>";

  // ── Template 1: Novo Usuário ───────────────────────────────────────
  public string NovoUsuario(
      string nomeMarido, string nomeEsposa, string email, string senha)
  {
    var body = $@"
      <p style=""margin:0 0 6px;font-size:15px;color:#475569"">Olá,</p>
      <h2 style=""margin:0 0 20px;font-size:20px;font-weight:800;color:#1e293b"">
        {nomeMarido} e {nomeEsposa}! 👋
      </h2>
      <p style=""margin:0 0 24px;font-size:14px;color:#64748b;line-height:1.7"">
        Seu acesso ao <strong>EnsinoApp</strong> foi criado com sucesso.
        Use as credenciais abaixo para entrar na plataforma e acompanhar
        suas turmas e relatórios.
      </p>

      <!-- Credenciais -->
      <table width=""100%"" cellpadding=""0"" cellspacing=""0""
             style=""background:#f8fafc;border:1px solid #e2e8f0;
                    border-radius:12px;margin-bottom:28px"">
        <tr>
          <td style=""padding:20px 24px"">
            <p style=""margin:0 0 12px;font-size:11px;font-weight:700;
                       color:#94a3b8;text-transform:uppercase;letter-spacing:.5px"">
              Dados de acesso
            </p>
            <table width=""100%"" cellpadding=""0"" cellspacing=""0"">
              {InfoRow("✉", "E-mail", email)}
              {InfoRow("🔑", "Senha", senha)}
            </table>
          </td>
        </tr>
      </table>

      <p style=""margin:0 0 24px;font-size:13px;color:#94a3b8;
                 background:#fffbeb;border:1px solid #fde68a;border-radius:8px;
                 padding:12px 16px"">
        ⚠️ Por segurança, recomendamos alterar sua senha após o primeiro acesso
        em <strong>Meu Perfil → Alterar Senha</strong>.
      </p>

      <div style=""text-align:center;margin-bottom:8px"">
        <a href=""{dominio}"" style=""display:inline-block;background:#2563eb;color:#ffffff;
                              font-weight:700;font-size:14px;padding:14px 32px;
                              border-radius:10px;text-decoration:none"">
          Acessar o sistema
        </a>
      </div>";

    return Wrap("Bem-vindo ao EnsinoApp", body);
  }

  // ── Template 2: Inscrição Confirmada ──────────────────────────────
  public string InscricaoConfirmada(
      string nomeMarido, string nomeEsposa,
      string nomeCurso, string nomeCampus,
      bool participaGC, string? nomeGC,
      DateTime dataInscricao)
  {
    var gcRow = participaGC && !string.IsNullOrWhiteSpace(nomeGC)
        ? InfoRow("🏠", "Grupo de Crescimento (GC)", nomeGC!)
        : "";

    var body = $@"
      <!-- Ícone de confirmação -->
      <div style=""text-align:center;margin-bottom:28px"">
        <div style=""display:inline-block;width:64px;height:64px;border-radius:50%;
                     background:#f0fdf4;line-height:64px;font-size:30px"">✅</div>
      </div>

      <h2 style=""margin:0 0 8px;font-size:20px;font-weight:800;
                  color:#1e293b;text-align:center"">
        Inscrição recebida!
      </h2>
      <p style=""margin:0 0 28px;font-size:14px;color:#64748b;
                 line-height:1.7;text-align:center"">
        Olá, <strong>{nomeMarido}</strong> e <strong>{nomeEsposa}</strong>!
        Recebemos sua inscrição. Nossa equipe entrará em contato em breve
        para confirmar a matrícula e informar a turma.
      </p>

      <!-- Resumo da inscrição -->
      <table width=""100%"" cellpadding=""0"" cellspacing=""0""
             style=""background:#f8fafc;border:1px solid #e2e8f0;
                    border-radius:12px;margin-bottom:28px"">
        <tr>
          <td style=""padding:20px 24px"">
            <p style=""margin:0 0 12px;font-size:11px;font-weight:700;
                       color:#94a3b8;text-transform:uppercase;letter-spacing:.5px"">
              Resumo da inscrição
            </p>
            <table width=""100%"" cellpadding=""0"" cellspacing=""0"">
              {InfoRow("🎓", "Curso", nomeCurso)}
              {InfoRow("📍", "Campus", nomeCampus)}
              {InfoRow("👥", "Participa de GC", participaGC ? "Sim" : "Não")}
              {gcRow}
              {InfoRow("📅", "Data da inscrição", dataInscricao.ToString("dd/MM/yyyy 'às' HH:mm"))}
            </table>
          </td>
        </tr>
      </table>

      <!-- Próximos passos -->
      <table width=""100%"" cellpadding=""0"" cellspacing=""0""
             style=""background:#eff6ff;border:1px solid #bfdbfe;
                    border-radius:12px;margin-bottom:8px"">
        <tr>
          <td style=""padding:20px 24px"">
            <p style=""margin:0 0 10px;font-size:13px;font-weight:700;
                       color:#1e40af"">📋 Próximos passos</p>
            <ol style=""margin:0;padding-left:20px;font-size:13px;
                        color:#475569;line-height:2"">
              <li>Nossa equipe analisará sua inscrição</li>
              <li>Entraremos em contato para confirmar a turma</li>
              <li>Você receberá os detalhes de data e horário das aulas</li>
            </ol>
          </td>
        </tr>
      </table>";

    return Wrap("Inscrição Recebida — " + nomeCurso, body);
  }

  private static string MontarEmailCodigo(string nome, string codigo) => $"""
        <!DOCTYPE html>
        <html lang="pt-br">
        <head><meta charset="utf-8"/></head>
        <body style="font-family:'Nunito','Segoe UI',sans-serif;background:#f8fafc;margin:0;padding:32px 16px">
          <div style="max-width:480px;margin:0 auto;background:#fff;border-radius:14px;overflow:hidden;box-shadow:0 4px 20px rgba(0,0,0,.08)">

            <div style="background:linear-gradient(135deg,#1e40af,#2563eb);padding:28px 32px;text-align:center">
              <h1 style="color:#fff;font-size:1.4rem;font-weight:800;margin:0">🔐 Redefinição de Senha</h1>
              <p style="color:rgba(255,255,255,.75);font-size:.85rem;margin:6px 0 0">EnsinoApp · CCVideira</p>
            </div>

            <div style="padding:32px">
              <p style="color:#334155;font-size:.95rem;margin:0 0 16px">Olá, <strong>{nome}</strong>!</p>
              <p style="color:#64748b;font-size:.88rem;line-height:1.6;margin:0 0 24px">
                Recebemos uma solicitação para redefinir a senha da sua conta. Use o código abaixo. Ele é válido por <strong>15 minutos</strong>.
              </p>

              <div style="background:#eff6ff;border:2px dashed #93c5fd;border-radius:12px;padding:20px;text-align:center;margin-bottom:24px">
                <p style="font-size:.72rem;font-weight:700;color:#3b82f6;text-transform:uppercase;letter-spacing:.5px;margin:0 0 8px">Seu código</p>
                <p style="font-size:2.2rem;font-weight:900;color:#1e40af;letter-spacing:8px;margin:0;font-family:monospace">{codigo}</p>
              </div>

              <p style="color:#94a3b8;font-size:.78rem;line-height:1.6;margin:0">
                Se você não solicitou esta redefinição, ignore este e-mail. Sua senha permanece inalterada.
              </p>
            </div>

            <div style="background:#f8fafc;padding:16px 32px;text-align:center;border-top:1px solid #e2e8f0">
              <p style="color:#cbd5e1;font-size:.72rem;margin:0">© {DateTime.Now.Year} EnsinoApp · CCVideira</p>
            </div>
          </div>
        </body>
        </html>
        """;

  public string ResetPasswordEmail(string nome, string codigo)
  {
    return MontarEmailCodigo(nome, codigo);
  }
}