using EnsinoApp.Config;
using EnsinoApp.Data.Configurations;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace EnsinoApp.Services.Email;

public class EmailService : IEmailService
{
    private readonly EmailSettings _settings;
    private readonly IEmailTemplateService _templateService;
    private readonly ILogger<EmailService> _logger;

    public EmailService(
        IOptions<EmailSettings> settings,
        IEmailTemplateService templateService,
        ILogger<EmailService> logger)
    {
        _settings = settings.Value;
        _templateService = templateService;
        _logger = logger;
    }

    public async Task SendAsync(string toEmail, string toName, string subject, string htmlBody)
    {
        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_settings.FromName, _settings.FromEmail));
            message.To.Add(new MailboxAddress(toName, toEmail));
            message.Subject = subject;

            message.Body = new TextPart("html") { Text = htmlBody };

            using var client = new SmtpClient();
            await client.ConnectAsync(_settings.Host, _settings.Port,
                _settings.UseSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None);
            await client.AuthenticateAsync(_settings.Username, _settings.Password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            _logger.LogInformation("Email enviado para {Email} | Assunto: {Subject}", toEmail, subject);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao enviar email para {Email}", toEmail);
            // Não propaga a exceção — email falhar não deve derrubar o fluxo principal
        }
    }

    public async Task SendNovoUsuarioAsync(
        string toEmail, string nomeMarido, string nomeEsposa, string senha)
    {
        var html = _templateService.NovoUsuario(nomeMarido, nomeEsposa, toEmail, senha);
        await SendAsync(toEmail, $"{nomeMarido} e {nomeEsposa}",
            "🎉 Bem-vindo ao EnsinoApp · CCVideira — seus dados de acesso", html);
    }

    public async Task SendInscricaoConfirmadaAsync(
        string toEmailMarido, string toEmailEsposa,
        string nomeMarido, string nomeEsposa,
        string nomeCurso, string nomeCampus,
        bool participaGC, string? nomeGC,
        DateTime dataInscricao)
    {
        var html = _templateService.InscricaoConfirmada(
            nomeMarido, nomeEsposa, nomeCurso, nomeCampus,
            participaGC, nomeGC, dataInscricao);

        var subject = $"✅ Inscrição recebida — {nomeCurso} · CCVideira";

        // Envia para os dois cônjuges em paralelo
        await Task.WhenAll(
            SendAsync(toEmailMarido, nomeMarido, subject, html),
            SendAsync(toEmailEsposa, nomeEsposa, subject, html)
        );
    }

    public async Task SendResetPasswordAsync(string destinatario, string assunto, string corpo)
    {
        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_settings.FromName, _settings.FromEmail));
            message.To.Add(new MailboxAddress("", destinatario));
            message.Subject = assunto;

            message.Body = new TextPart("html") { Text = corpo };

            using var client = new SmtpClient();
            await client.ConnectAsync(_settings.Host, _settings.Port,
                _settings.UseSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None);
            await client.AuthenticateAsync(_settings.Username, _settings.Password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            _logger.LogInformation("Email enviado para {Email} | Assunto: {Subject}", destinatario, assunto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao enviar email para {Email}", destinatario);

        }
    }
}