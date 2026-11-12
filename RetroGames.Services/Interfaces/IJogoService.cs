using RetroGames.Model.Model;

namespace RetroGames.Services.Interfaces;

public interface IJogoService{
    Task<(List<JogoDetalheServiceModel> Jogos, int Total)> ListarAsync(
        int pagina,
        int tamanhoPagina,
        string? nome = null,
        uint? idPlataforma = null,
        uint? idGenero = null,
        bool incluirInativos = false);

    Task<JogoDetalheServiceModel> BuscarPorIdAsync(uint id);

    Task<JogoDetalheServiceModel> InserirAsync(
        Jogo jogo,
        List<uint> idsGeneros);

    Task<JogoDetalheServiceModel> AlterarAsync(
        Jogo jogo,
        List<uint> idsGeneros);

    Task DesativarAsync(uint id);
}