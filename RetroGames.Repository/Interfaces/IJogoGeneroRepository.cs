namespace RetroGames.Repository.Interfaces;

public interface IJogoGeneroRepository{
    Task<List<(uint IdJogo, uint IdGenero)>> ListarTodosAsync();

    Task<List<(uint IdJogo, uint IdGenero)>> ListarPorJogosAsync(
        List<uint> idsJogos);

    Task InserirAsync(uint idJogo, uint idGenero);
    Task RemoverPorJogoAsync(uint idJogo);
}