using RetroGames.Model.Model;

namespace RetroGames.Repository.Interfaces;

public interface IJogoRepository{
    Task<(List<Jogo> Jogos, int Total)> ListarAsync(
        int pagina,
        int tamanhoPagina,
        string? nome = null,
        uint? idPlataforma = null,
        List<uint>? ids = null,
        bool incluirInativos = false);

    Task<Jogo?> BuscarPorIdAsync(uint id);
    Task<uint> InserirAsync(Jogo jogo);
    Task AlterarAsync(Jogo jogo);
    Task DesativarAsync(uint id);
}