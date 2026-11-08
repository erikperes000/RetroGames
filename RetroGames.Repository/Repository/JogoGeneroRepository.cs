using RetroGames.Repository.Base;
using RetroGames.Repository.Config;
using RetroGames.Repository.Interfaces;

namespace RetroGames.Repository.Repository;

public class JogoGeneroRepository : RepositoryBase, IJogoGeneroRepository
{
    public JogoGeneroRepository(RepositoryConfig config)
        : base(config)
    {
    }

    public async Task<List<(uint IdJogo, uint IdGenero)>> ListarTodosAsync()
    {
        const string sql = @"
            SELECT co_jogo, co_genero
            FROM tb_jogo_genero;
        ";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(sql, conexao);
        await using var leitor = await comando.ExecuteReaderAsync();

        var vinculos = new List<(uint IdJogo, uint IdGenero)>();

        while (await leitor.ReadAsync())
        {
            vinculos.Add((
                leitor.GetUInt32("co_jogo"),
                leitor.GetUInt32("co_genero")
            ));
        }

        return vinculos;
    }

    public async Task<List<(uint IdJogo, uint IdGenero)>> ListarPorJogosAsync(
        List<uint> idsJogos)
    {
        if (idsJogos == null || idsJogos.Count == 0)
            return new List<(uint IdJogo, uint IdGenero)>();

        string parametros = string.Join(
            ", ",
            idsJogos.Select((_, i) => $"@id{i}")
        );

        string sql = $@"
            SELECT co_jogo, co_genero
            FROM tb_jogo_genero
            WHERE co_jogo IN ({parametros});
        ";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(sql, conexao);

        for (int i = 0; i < idsJogos.Count; i++)
            AdicionarParametro(comando, $"@id{i}", idsJogos[i]);

        await using var leitor = await comando.ExecuteReaderAsync();

        var vinculos = new List<(uint IdJogo, uint IdGenero)>();

        while (await leitor.ReadAsync())
        {
            vinculos.Add((
                leitor.GetUInt32("co_jogo"),
                leitor.GetUInt32("co_genero")
            ));
        }

        return vinculos;
    }

    public async Task InserirAsync(uint idJogo, uint idGenero)
    {
        const string sql = @"
            INSERT INTO tb_jogo_genero (co_jogo, co_genero)
            VALUES (@idJogo, @idGenero);
        ";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(sql, conexao);

        AdicionarParametro(comando, "@idJogo", idJogo);
        AdicionarParametro(comando, "@idGenero", idGenero);

        await ExecutarGravacaoAsync(comando);
    }

    public async Task RemoverPorJogoAsync(uint idJogo)
    {
        const string sql = @"
            DELETE FROM tb_jogo_genero
            WHERE co_jogo = @idJogo;
        ";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(sql, conexao);

        AdicionarParametro(comando, "@idJogo", idJogo);

        await ExecutarGravacaoAsync(comando);
    }
}