using RetroGames.Model.Model;
using RetroGames.Repository.Base;
using RetroGames.Repository.Config;
using RetroGames.Repository.Interfaces;

namespace RetroGames.Repository.Repository;

public class CondicaoJogoRepository : RepositoryBase, ICondicaoJogoRepository
{
    public CondicaoJogoRepository(RepositoryConfig config)
        : base(config)
    {
    }

    public async Task<List<CondicaoJogo>> ListarAsync()
    {
        const string sql = @"
            SELECT co_condicao_jogo, ds_condicao_jogo
            FROM tb_condicao_jogo;
        ";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(sql, conexao);
        await using var leitor = await comando.ExecuteReaderAsync();

        var condicoes = new List<CondicaoJogo>();

        while (await leitor.ReadAsync())
        {
            condicoes.Add(new CondicaoJogo
            {
                Id = leitor.GetUInt16("co_condicao_jogo"),
                Descricao = leitor.GetString("ds_condicao_jogo")
            });
        }

        return condicoes;
    }
}