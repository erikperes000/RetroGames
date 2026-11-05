namespace RetroGames.Repository.Interfaces;

public interface IJogoGeneroRepository{
    List<(uint IdJogo, uint IdGenero)> ListarTodos();
    List<(uint IdJogo, uint IdGenero)> ListarPorJogos(List<uint> idsJogos);
    void Inserir(uint idJogo, uint idGenero);
    void RemoverPorJogo(uint idJogo);
}
