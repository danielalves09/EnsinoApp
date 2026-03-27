using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;

namespace EnsinoApp.Services.Email;

public class SmtpEmailService : IEmailService
{
    private readonly SmtpSettings _settings;
    private readonly ILogger<SmtpEmailService> _logger;

    public SmtpEmailService(IOptions<SmtpSettings> settings, ILogger<SmtpEmailService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task EnviarAsync(string destinatario, string assunto, string corpoHtml)
    {
        await EnviarParaVariosAsync(new[] { destinatario }, assunto, corpoHtml);
    }

    public async Task EnviarParaVariosAsync(IEnumerable<string> destinatarios, string assunto, string corpoHtml)
    {
        try
        {
            using var client = new SmtpClient(_settings.Host, _settings.Port)
            {
                Credentials = new NetworkCredential(_settings.Usuario, _settings.Senha),
                EnableSsl = _settings.UsarSsl
            };

            using var mensagem = new MailMessage
            {
                From = new MailAddress(_settings.Remetente, _settings.NomeRemetente),
                Subject = assunto,
                Body = corpoHtml,
                IsBodyHtml = true
            };

            foreach (var dest in destinatarios)
            {
                if (!string.IsNullOrWhiteSpace(dest))
                    mensagem.To.Add(dest);
            }

            if (mensagem.To.Count == 0) return;

            await client.SendMailAsync(mensagem);

            _logger.LogInformation(
                "Email enviado | Assunto: {Assunto} | Destinatários: {Total}",
                assunto, mensagem.To.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Erro ao enviar email | Assunto: {Assunto}", assunto);
            // Não relança — o background service não deve parar por falha de email
        }
    }
}

/// <summary>Configurações SMTP lidas do appsettings.json.</summary>
public class SmtpSettings
{
    public string Host { get; set; } = null!;
    public int Port { get; set; } = 587;
    public string Usuario { get; set; } = null!;
    public string Senha { get; set; } = null!;
    public bool UsarSsl { get; set; } = true;
    public string Remetente { get; set; } = null!;
    public string NomeRemetente { get; set; } = "EnsinoApp – CCVideira";
}