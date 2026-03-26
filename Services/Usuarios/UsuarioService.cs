using EnsinoApp.Models.Entities;
using EnsinoApp.Repositories.Usuarios;

namespace EnsinoApp.Services.Usuarios;

public class UsuariosService : IUsuariosService
{
    private readonly IUsuariosRepository _usuarioRepository;

    public UsuariosService(IUsuariosRepository usuarioRepository)
    {
        _usuarioRepository = usuarioRepository;
    }

    public IEnumerable<Usuario> FindAll()
    {
        return _usuarioRepository.FindAll();
    }

    public IEnumerable<Usuario> FindByCampus(int idCampus)
    {
        throw new NotImplementedException();
    }

    public Usuario? FindById(int id)
    {
        var usuarioFind = _usuarioRepository.FindById(id);

        return usuarioFind;
    }

    public IEnumerable<Usuario> findBySupervisao(int idSupervisao)
    {
        throw new NotImplementedException();
    }

    public string GetNomeReduzido(string nome1, string nome2)
    {

        return $"{GetPrimeiroNome(nome1)} e {GetPrimeiroNome(nome2)}";
    }

    private string GetPrimeiroNome(string nomeCompleto)
    {
        if (string.IsNullOrEmpty(nomeCompleto)) return string.Empty;
        var partes = nomeCompleto.Split(' ');
        if (partes.Length >= 2)
            return $"{partes[0]}"; // Primeiro nome
        return partes[0];
    }

    public Task<int> ContarLideresAsync() => _usuarioRepository.ContarLideresAsync();
}