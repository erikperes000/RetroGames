using RetroGames.Model.Model;

namespace RetroGames.Services.Interfaces;

public interface IPlataformaService{
    Task<List<Plataforma>> ListarAsync();

    Task<Plataforma> BuscarPorIdAsync(uint id);

    Task<Plataforma> InserirAsync(Plataforma plataforma);

    Task AlterarAsync(Plataforma plataforma);

    Task ExcluirAsync(uint id);
}
