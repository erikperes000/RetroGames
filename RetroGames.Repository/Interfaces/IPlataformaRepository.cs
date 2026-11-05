using RetroGames.Model.Model;

namespace RetroGames.Repository.Interfaces;

public interface IPlataformaRepository{
    List<Plataforma> Listar();
    Plataforma? BuscarPorId(uint id);

    bool ExisteComNome(string nome, uint? idIgnorar = null);
    bool PossuiJogos(uint id);
    uint Inserir(Plataforma plataforma);
    void Alterar(Plataforma plataforma);
    void Excluir(uint id);
}
