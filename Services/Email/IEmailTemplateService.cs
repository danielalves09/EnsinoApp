namespace EnsinoApp.Services.Email;

public interface IEmailTemplateService
{
    string NovoUsuario(string nomeMarido, string nomeEsposa, string email, string senha);
    string InscricaoConfirmada(string nomeMarido, string nomeEsposa,
        string nomeCurso, string nomeCampus,
        bool participaGC, string? nomeGC, DateTime dataInscricao);

    string ResetPasswordEmail(string nome, string codigo);
}