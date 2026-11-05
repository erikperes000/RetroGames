using RetroGames.Model.Model;

namespace RetroGames.Repository.Interfaces;

public interface IJogoRepository{
    List<Jogo> Listar(int pagina, int tamanhoPagina, out int total, string? nome = null, uint? idPlataforma = null, List<uint>? ids = null, bool incluirInativos = false);
    Jogo? BuscarPorId(uint id);
    uint Inserir(Jogo jogo);
    void Alterar(Jogo jogo);
    void Desativar(uint id);
}
