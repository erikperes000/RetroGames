using MySqlConnector;
using RetroGames.Model.Model;
using RetroGames.Repository.Base;
using RetroGames.Repository.Config;
using RetroGames.Repository.Interfaces;

namespace RetroGames.Repository.Repository;

public class JogoRepository : RepositoryBase, IJogoRepository{
    public JogoRepository(RepositoryConfig config)
        : base(config)
    {
    }

    public List<Jogo> Listar(int pagina, int tamanhoPagina, out int total, string? nome = null, uint? idPlataforma = null, List<uint>? ids = null, bool incluirInativos = false){
        var filtros = new List<string>();

        if (!incluirInativos)
            filtros.Add("st_ativo = 1");

        if (!string.IsNullOrWhiteSpace(nome))
            filtros.Add("no_jogo LIKE @nome");

        if (idPlataforma.HasValue)
            filtros.Add("co_plataforma = @idPlataforma");

        if (ids != null){
            if (ids.Count == 0){
                total = 0;
                return new List<Jogo>();
            }
            filtros.Add($"co_seq_jogo IN ({string.Join(", ", ids.Select((_, i) => $"@id{i}"))})");
        }

        string where = filtros.Count > 0 ? "WHERE " + string.Join(" AND ", filtros) : "";

        string sql = $@"SELECT co_seq_jogo, no_jogo, ds_jogo, nu_ano_lancamento, vl_preco, qt_estoque, st_ativo, dt_cadastro, co_plataforma, co_condicao_jogo
                         FROM tb_jogo {where} ORDER BY no_jogo LIMIT @limite OFFSET @offset;";

        string sqlTotal = $@"SELECT COUNT(*) FROM tb_jogo {where};";

        using var conexao = AbrirConexao();

        using (var comando = CriarComando(sql, conexao)){
            if (!string.IsNullOrWhiteSpace(nome))
                AdicionarParametro(comando, "@nome", $"%{nome}%");

            if (idPlataforma.HasValue)
                AdicionarParametro(comando, "@idPlataforma", idPlataforma.Value);

            if (ids != null){
                for (int i = 0; i < ids.Count; i++)
                    AdicionarParametro(comando, $"@id{i}", ids[i]);
            }

            AdicionarParametro(comando, "@limite", tamanhoPagina);
            AdicionarParametro(comando, "@offset", (pagina - 1) * tamanhoPagina);

            var jogos = new List<Jogo>();

            using var leitor = comando.ExecuteReader();

            while (leitor.Read()){
                jogos.Add(new Jogo{
                    Id = leitor.GetUInt32("co_seq_jogo"),
                    Nome = leitor.GetString("no_jogo"),
                    Descricao = leitor.IsDBNull(leitor.GetOrdinal("ds_jogo")) ? null : leitor.GetString("ds_jogo"),
                    AnoLancamento = leitor.GetUInt16("nu_ano_lancamento"),
                    Preco = leitor.GetDecimal("vl_preco"),
                    Estoque = leitor.GetUInt32("qt_estoque"),
                    Ativo = leitor.GetBoolean("st_ativo"),
                    DataCadastro = leitor.GetDateTime("dt_cadastro"),
                    IdPlataforma = leitor.GetUInt32("co_plataforma"),
                    IdCondicaoJogo = leitor.GetUInt16("co_condicao_jogo")
                }
                );
            }

            using var comandoTotal = CriarComando(sqlTotal, conexao);

            if (!string.IsNullOrWhiteSpace(nome))
                AdicionarParametro(comandoTotal, "@nome", $"%{nome}%");

            if (idPlataforma.HasValue)
                AdicionarParametro(comandoTotal, "@idPlataforma", idPlataforma.Value);

            if (ids != null){
                for (int i = 0; i < ids.Count; i++)
                    AdicionarParametro(comandoTotal, $"@id{i}", ids[i]);
            }

            total = Convert.ToInt32(comandoTotal.ExecuteScalar());

            return jogos;
        }
    }

    public Jogo? BuscarPorId(uint id){
        const string sql = @"SELECT co_seq_jogo, no_jogo, ds_jogo, nu_ano_lancamento, vl_preco, qt_estoque, st_ativo, dt_cadastro, co_plataforma, co_condicao_jogo
                             FROM tb_jogo WHERE co_seq_jogo = @id;";

        using var conexao = AbrirConexao();
        using var comando = CriarComando(sql, conexao);

        AdicionarParametro(comando, "@id", id);

        using var leitor = comando.ExecuteReader();

        if (!leitor.Read())
            return null;

        return new Jogo{
            Id = leitor.GetUInt32("co_seq_jogo"),
            Nome = leitor.GetString("no_jogo"),
            Descricao = leitor.IsDBNull(leitor.GetOrdinal("ds_jogo")) ? null : leitor.GetString("ds_jogo"),
            AnoLancamento = leitor.GetUInt16("nu_ano_lancamento"),
            Preco = leitor.GetDecimal("vl_preco"),
            Estoque = leitor.GetUInt32("qt_estoque"),
            Ativo = leitor.GetBoolean("st_ativo"),
            DataCadastro = leitor.GetDateTime("dt_cadastro"),
            IdPlataforma = leitor.GetUInt32("co_plataforma"),
            IdCondicaoJogo = leitor.GetUInt16("co_condicao_jogo")
        };
    }

    public uint Inserir(Jogo jogo){
        const string sql = @"INSERT INTO tb_jogo (no_jogo, ds_jogo, nu_ano_lancamento, vl_preco, qt_estoque, st_ativo, dt_cadastro, co_plataforma, co_condicao_jogo)
                             VALUES (@nome, @descricao, @anoLancamento, @preco, @estoque, @ativo, @dataCadastro, @idPlataforma, @idCondicao);";

        using var conexao = AbrirConexao();
        using var comando = CriarComando(sql, conexao);

        object descricao = jogo.Descricao is null ? DBNull.Value : jogo.Descricao;

        AdicionarParametro(comando, "@nome", jogo.Nome);
        AdicionarParametro(comando, "@descricao", descricao);
        AdicionarParametro(comando, "@anoLancamento", jogo.AnoLancamento);
        AdicionarParametro(comando, "@preco", jogo.Preco);
        AdicionarParametro(comando, "@estoque", jogo.Estoque);
        AdicionarParametro(comando, "@ativo", jogo.Ativo);
        AdicionarParametro(comando, "@dataCadastro", jogo.DataCadastro);
        AdicionarParametro(comando, "@idPlataforma", jogo.IdPlataforma);
        AdicionarParametro(comando, "@idCondicao", jogo.IdCondicaoJogo);
        ExecutarGravacao(comando);

        return checked((uint)comando.LastInsertedId);
    }

    public void Alterar(Jogo jogo){
        const string sql = @"UPDATE tb_jogo SET no_jogo = @nome, ds_jogo = @descricao, nu_ano_lancamento = @anoLancamento, vl_preco = @preco, qt_estoque = @estoque, co_plataforma = @idPlataforma, co_condicao_jogo = @idCondicao
                             WHERE co_seq_jogo = @id;";

        using var conexao = AbrirConexao();
        using var comando = CriarComando(sql, conexao);

        object descricao = jogo.Descricao is null ? DBNull.Value : jogo.Descricao;

        AdicionarParametro(comando, "@nome", jogo.Nome);
        AdicionarParametro(comando, "@descricao", descricao);
        AdicionarParametro(comando, "@anoLancamento", jogo.AnoLancamento);
        AdicionarParametro(comando, "@preco", jogo.Preco);
        AdicionarParametro(comando, "@estoque", jogo.Estoque);
        AdicionarParametro(comando, "@idPlataforma", jogo.IdPlataforma);
        AdicionarParametro(comando, "@idCondicao", jogo.IdCondicaoJogo);
        AdicionarParametro(comando, "@id", jogo.Id);
        ExecutarGravacao(comando);
    }

    public void Desativar(uint id){
        const string sql = @"UPDATE tb_jogo SET st_ativo = 0 WHERE co_seq_jogo = @id;";

        using var conexao = AbrirConexao();
        using var comando = CriarComando(sql, conexao);

        AdicionarParametro(comando, "@id", id);
        ExecutarGravacao(comando);
    }
}