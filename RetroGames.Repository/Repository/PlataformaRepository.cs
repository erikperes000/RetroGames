using RetroGames.Model.Model;
using RetroGames.Repository.Base;
using RetroGames.Repository.Config;
using RetroGames.Repository.Interfaces;

namespace RetroGames.Repository.Repository;

public class PlataformaRepository : RepositoryBase, IPlataformaRepository{
    public PlataformaRepository(RepositoryConfig config)
        : base(config)
    {
    }

    public async Task<List<Plataforma>> ListarAsync(){
        const string sql = @"SELECT co_seq_plataforma, no_plataforma, no_fabricante, nu_ano_lancamento FROM tb_plataforma ORDER BY no_plataforma;";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(sql, conexao);
        await using var leitor = await comando.ExecuteReaderAsync();

        var plataformas = new List<Plataforma>();

        while (await leitor.ReadAsync()){
            plataformas.Add(new Plataforma{
                Id = leitor.GetUInt32("co_seq_plataforma"),
                Nome = leitor.GetString("no_plataforma"),
                Fabricante = leitor.GetString("no_fabricante"),
                AnoLancamento = leitor.GetUInt16("nu_ano_lancamento")
            });
        }

        return plataformas;
    }

    public async Task<Plataforma?> BuscarPorIdAsync(uint id){
        const string sql = @"SELECT co_seq_plataforma, no_plataforma, no_fabricante, nu_ano_lancamento FROM tb_plataforma WHERE co_seq_plataforma = @id;";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(sql, conexao);

        AdicionarParametro(comando, "@id", id);

        await using var leitor = await comando.ExecuteReaderAsync();

        if (!await leitor.ReadAsync())
            return null;

        return new Plataforma{
            Id = leitor.GetUInt32("co_seq_plataforma"),
            Nome = leitor.GetString("no_plataforma"),
            Fabricante = leitor.GetString("no_fabricante"),
            AnoLancamento = leitor.GetUInt16("nu_ano_lancamento")
        };
    }

    public async Task<bool> ExisteComNomeAsync(string nome, uint? idIgnorar = null){
        string sql;

        if (idIgnorar.HasValue){
            sql = @"SELECT COUNT(*) FROM tb_plataforma WHERE no_plataforma = @nome AND co_seq_plataforma <> @idIgnorar;";
        }else{
            sql = @"SELECT COUNT(*) FROM tb_plataforma WHERE no_plataforma = @nome;";
        }

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(sql, conexao);

        AdicionarParametro(comando, "@nome", nome);

        if (idIgnorar.HasValue)
            AdicionarParametro(comando, "@idIgnorar", idIgnorar.Value);

        var quantidade = Convert.ToInt32(await comando.ExecuteScalarAsync());

        return quantidade > 0;
    }

    public async Task<bool> PossuiJogosAsync(uint id){
        const string sql = @"SELECT COUNT(*) FROM tb_jogo WHERE co_plataforma = @id;";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(sql, conexao);

        AdicionarParametro(comando, "@id", id);

        var quantidade = Convert.ToInt32(await comando.ExecuteScalarAsync());

        return quantidade > 0;
    }

    public async Task<uint> InserirAsync(Plataforma plataforma){
        const string sql = @"INSERT INTO tb_plataforma (no_plataforma, no_fabricante, nu_ano_lancamento) VALUES (@nome, @fabricante, @anoLancamento);";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(sql, conexao);

        AdicionarParametro(comando, "@nome", plataforma.Nome);
        AdicionarParametro(comando, "@fabricante", plataforma.Fabricante);
        AdicionarParametro(comando, "@anoLancamento", plataforma.AnoLancamento);

        await ExecutarGravacaoAsync(comando);

        return checked((uint)comando.LastInsertedId);
    }

    public async Task AlterarAsync(Plataforma plataforma){
        const string sql = @"UPDATE tb_plataforma SET no_plataforma = @nome, no_fabricante = @fabricante, nu_ano_lancamento = @anoLancamento WHERE co_seq_plataforma = @id;";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(sql, conexao);

        AdicionarParametro(comando, "@nome", plataforma.Nome);
        AdicionarParametro(comando, "@fabricante", plataforma.Fabricante);
        AdicionarParametro(comando, "@anoLancamento", plataforma.AnoLancamento);
        AdicionarParametro(comando, "@id", plataforma.Id);

        await ExecutarGravacaoAsync(comando);
    }

    public async Task ExcluirAsync(uint id){
        const string sql = @"DELETE FROM tb_plataforma WHERE co_seq_plataforma = @id;";

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(sql, conexao);

        AdicionarParametro(comando, "@id", id);

        await ExecutarGravacaoAsync(comando);
    }
}