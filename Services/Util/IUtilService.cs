namespace EnsinoApp.Services.Util;


public interface IUtilService
{
    string GetNomeReduzido(string nome1, string nome2);
    string GetNomeSobrenome(string nome1, string nome2);
}