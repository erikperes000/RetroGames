using RetroGames.Model.Model;

namespace RetroGames.Repository.Interfaces;

public interface IPlataformaRepository{
    Task<List<Plataforma>> ListarAsync();

    Task<Plataforma?> BuscarPorIdAsync(uint id);

    Task<bool> ExisteComNomeAsync(string nome, uint? idIgnorar = null);

    Task<bool> PossuiJogosAsync(uint id);

    Task<uint> InserirAsync(Plataforma plataforma);

    Task AlterarAsync(Plataforma plataforma);

    Task ExcluirAsync(uint id);
}