using RetroGames.Model.Model;
using RetroGames.Repository.Base;
using RetroGames.Repository.Config;
using RetroGames.Repository.Interfaces;

namespace RetroGames.Repository.Repository;

public class JogoRepository : RepositoryBase, IJogoRepository
{
    public JogoRepository(RepositoryConfig config)
        : base(config)
    {
    }

    public async Task<(List<Jogo> Jogos, int Total)> ListarAsync(
        int pagina,
        int tamanhoPagina,
        string? nome = null,
        uint? idPlataforma = null,
        List<uint>? ids = null,
        bool incluirInativos = false)
    {
        var filtros = new List<string>();

        if (!incluirInativos)
            filtros.Add("st_ativo = 1");

        if (!string.IsNullOrWhiteSpace(nome))
            filtros.Add("no_jogo LIKE @nome");

        if (idPlataforma.HasValue)
            filtros.Add("co_plataforma = @idPlataforma");

        if (ids != null)
        {
            if (ids.Count == 0)
                return (new List<Jogo>(), 0);

            filtros.Add(
                $"co_seq_jogo IN ({string.Join(", ", ids.Select((_, i) => $"@id{i}"))})");
        }

        string where = filtros.Count > 0
            ? "WHERE " + string.Join(" AND ", filtros)
            : "";

        string sql = $@"
            SELECT co_seq_jogo, no_jogo, ds_jogo, nu_ano_lancamento,
                   vl_preco, qt_estoque, st_ativo, dt_cadastro,
                   co_plataforma, co_condicao_jogo
            FROM tb_jogo
            {where}
            ORDER BY no_jogo
            LIMIT @limite OFFSET @offset;
        ";

        string sqlTotal = $@"
            SELECT COUNT(*)
            FROM tb_jogo
            {where};
        ";

        await using var conexao = await AbrirConexaoAsync();

        var jogos = new List<Jogo>();

        await using (var comando = CriarComando(sql, conexao))
        {
            AdicionarFiltros(comando, nome, idPlataforma, ids);

            AdicionarParametro(comando, "@limite", tamanhoPagina);
            AdicionarParametro(comando, "@offset", (pagina - 1) * tamanhoPagina);

            await using var leitor = await comando.ExecuteReaderAsync();

            while (await leitor.ReadAsync())
            {
                jogos.Add(new Jogo
                {
                    Id = leitor.GetUInt32("co_seq_jogo"),
                    Nome = leitor.GetString("no_jogo"),
                    Descricao = leitor.IsDBNull(leitor.GetOrdinal("ds_jogo"))
                        ? null
                        : leitor.GetString("ds_jogo"),
                    AnoLancamento = leitor.GetUInt16("nu_ano_lancamento"),
                    Preco = leitor.GetDecimal("vl_preco"),
                    Estoque = leitor.GetUInt32("qt_estoque"),
                    Ativo = leitor.GetBoolean("st_ativo"),
                    DataCadastro = leitor.GetDateTime("dt_cadastro"),
                    IdPlataforma = leitor.GetUInt32("co_plataforma"),
                    IdCondicaoJogo = leitor.GetUInt16("co_condicao_jogo")
                });
            }
        }

        // O leitor já foi descartado. Agora podemos executar a contagem.
        int total;

        await using (var comandoTotal = CriarComando(sqlTotal, conexao))
        {
            AdicionarFiltros(comandoTotal, nome, idPlataforma, ids);

            total = Convert.ToInt32(
                await comandoTotal.ExecuteScalarAsync());
        }

        return (jogos, total);
    }

    public async Task<Jogo?> BuscarPorIdAsync(uint id)
    {
        const string sql = @"
            SELECT co_seq_jogo, no_jogo, ds_jogo, nu_ano_lancamento,
                   vl_preco, qt_estoque, st_ativo, dt_cadastro,
                   co_plataforma, co_condicao_jogo
            FROM tb_jogo
            WHERE co_seq_jogo = @id;
        ";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(sql, conexao);

        AdicionarParametro(comando, "@id", id);

        await using var leitor = await comando.ExecuteReaderAsync();

        if (!await leitor.ReadAsync())
            return null;

        return new Jogo
        {
            Id = leitor.GetUInt32("co_seq_jogo"),
            Nome = leitor.GetString("no_jogo"),
            Descricao = leitor.IsDBNull(leitor.GetOrdinal("ds_jogo"))
                ? null
                : leitor.GetString("ds_jogo"),
            AnoLancamento = leitor.GetUInt16("nu_ano_lancamento"),
            Preco = leitor.GetDecimal("vl_preco"),
            Estoque = leitor.GetUInt32("qt_estoque"),
            Ativo = leitor.GetBoolean("st_ativo"),
            DataCadastro = leitor.GetDateTime("dt_cadastro"),
            IdPlataforma = leitor.GetUInt32("co_plataforma"),
            IdCondicaoJogo = leitor.GetUInt16("co_condicao_jogo")
        };
    }

    public async Task<uint> InserirAsync(Jogo jogo)
    {
        const string sql = @"
            INSERT INTO tb_jogo
                (no_jogo, ds_jogo, nu_ano_lancamento, vl_preco,
                 qt_estoque, st_ativo, dt_cadastro, co_plataforma,
                 co_condicao_jogo)
            VALUES
                (@nome, @descricao, @anoLancamento, @preco,
                 @estoque, @ativo, @dataCadastro, @idPlataforma,
                 @idCondicao);
        ";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(sql, conexao);

        object descricao = jogo.Descricao is null
            ? DBNull.Value
            : jogo.Descricao;

        AdicionarParametro(comando, "@nome", jogo.Nome);
        AdicionarParametro(comando, "@descricao", descricao);
        AdicionarParametro(comando, "@anoLancamento", jogo.AnoLancamento);
        AdicionarParametro(comando, "@preco", jogo.Preco);
        AdicionarParametro(comando, "@estoque", jogo.Estoque);
        AdicionarParametro(comando, "@ativo", jogo.Ativo);
        AdicionarParametro(comando, "@dataCadastro", jogo.DataCadastro);
        AdicionarParametro(comando, "@idPlataforma", jogo.IdPlataforma);
        AdicionarParametro(comando, "@idCondicao", jogo.IdCondicaoJogo);

        await ExecutarGravacaoAsync(comando);

        return checked((uint)comando.LastInsertedId);
    }

    public async Task AlterarAsync(Jogo jogo)
    {
        const string sql = @"
            UPDATE tb_jogo
            SET
                no_jogo = @nome,
                ds_jogo = @descricao,
                nu_ano_lancamento = @anoLancamento,
                vl_preco = @preco,
                qt_estoque = @estoque,
                co_plataforma = @idPlataforma,
                co_condicao_jogo = @idCondicao
            WHERE co_seq_jogo = @id;
        ";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(sql, conexao);

        object descricao = jogo.Descricao is null
            ? DBNull.Value
            : jogo.Descricao;

        AdicionarParametro(comando, "@nome", jogo.Nome);
        AdicionarParametro(comando, "@descricao", descricao);
        AdicionarParametro(comando, "@anoLancamento", jogo.AnoLancamento);
        AdicionarParametro(comando, "@preco", jogo.Preco);
        AdicionarParametro(comando, "@estoque", jogo.Estoque);
        AdicionarParametro(comando, "@idPlataforma", jogo.IdPlataforma);
        AdicionarParametro(comando, "@idCondicao", jogo.IdCondicaoJogo);
        AdicionarParametro(comando, "@id", jogo.Id);

        await ExecutarGravacaoAsync(comando);
    }

    public async Task DesativarAsync(uint id)
    {
        const string sql = @"
            UPDATE tb_jogo
            SET st_ativo = 0
            WHERE co_seq_jogo = @id;
        ";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(sql, conexao);

        AdicionarParametro(comando, "@id", id);

        await ExecutarGravacaoAsync(comando);
    }

    private void AdicionarFiltros(
        MySqlConnector.MySqlCommand comando,
        string? nome,
        uint? idPlataforma,
        List<uint>? ids)
    {
        if (!string.IsNullOrWhiteSpace(nome))
            AdicionarParametro(comando, "@nome", $"%{nome}%");

        if (idPlataforma.HasValue)
            AdicionarParametro(comando, "@idPlataforma", idPlataforma.Value);

        if (ids != null)
        {
            for (int i = 0; i < ids.Count; i++)
                AdicionarParametro(comando, $"@id{i}", ids[i]);
        }
    }
}