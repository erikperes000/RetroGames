using MySqlConnector;
using RetroGames.Model.Model;
using RetroGames.Repository.Base;
using RetroGames.Repository.Config;
using RetroGames.Repository.Interfaces;

namespace RetroGames.Repository.Repository;

public class PlataformaRepository : RepositoryBase, IPlataformaRepository
{
    public PlataformaRepository(RepositoryConfig config)
        : base(config)
    {
    }

    public List<Plataforma> Listar(){
        const string sql = @"SELECT co_seq_plataforma, no_plataforma, no_fabricante, nu_ano_lancamento FROM tb_plataforma ORDER BY no_plataforma;";

        using var conexao = AbrirConexao();
        using var comando = CriarComando(sql, conexao);
        using var leitor = comando.ExecuteReader();

        var plataformas = new List<Plataforma>();

        while (leitor.Read()){
            plataformas.Add(new Plataforma{
                Id = leitor.GetUInt32("co_seq_plataforma"),
                Nome = leitor.GetString("no_plataforma"),
                Fabricante = leitor.GetString("no_fabricante"),
                AnoLancamento = leitor.GetUInt16("nu_ano_lancamento")
            });
        }

        return plataformas;
    }

    public Plataforma? BuscarPorId(uint id){
        const string sql = @"SELECT co_seq_plataforma, no_plataforma, no_fabricante, nu_ano_lancamento FROM tb_plataforma WHERE co_seq_plataforma = @id;";

        using var conexao = AbrirConexao();
        using var comando = CriarComando(sql, conexao);

        AdicionarParametro(comando, "@id", id);

        using var leitor = comando.ExecuteReader();

        if (!leitor.Read())
            return null;

        return new Plataforma{
            Id = leitor.GetUInt32("co_seq_plataforma"),
            Nome = leitor.GetString("no_plataforma"),
            Fabricante = leitor.GetString("no_fabricante"),
            AnoLancamento = leitor.GetUInt16("nu_ano_lancamento")
        };
    }

    public bool ExisteComNome(string nome, uint? idIgnorar = null){
        string sql;

        if (idIgnorar.HasValue){
            sql = @"SELECT EXISTS (SELECT 1 FROM tb_plataforma WHERE no_plataforma = @nome AND co_seq_plataforma <> @idIgnorar);";
        }else{
            sql = @"SELECT EXISTS (SELECT 1 FROM tb_plataforma WHERE no_plataforma = @nome);";
        }

        using var conexao = AbrirConexao();
        using var comando = CriarComando(sql, conexao);

        AdicionarParametro(comando, "@nome", nome);

        if (idIgnorar.HasValue)
            AdicionarParametro(comando, "@idIgnorar", idIgnorar.Value);

        return Convert.ToBoolean(comando.ExecuteScalar());
    }

    public bool PossuiJogos(uint id){
        const string sql = @"SELECT EXISTS (SELECT 1 FROM tb_jogo WHERE co_plataforma = @id);";

        using var conexao = AbrirConexao();
        using var comando = CriarComando(sql, conexao);

        AdicionarParametro(comando, "@id", id);

        return Convert.ToBoolean(comando.ExecuteScalar());
    }

    public uint Inserir(Plataforma plataforma){
        const string sql = @"INSERT INTO tb_plataforma (no_plataforma, no_fabricante, nu_ano_lancamento) VALUES (@nome, @fabricante, @anoLancamento);";

        using var conexao = AbrirConexao();
        using var comando = CriarComando(sql, conexao);

        AdicionarParametro(comando, "@nome", plataforma.Nome);
        AdicionarParametro(comando, "@fabricante", plataforma.Fabricante);
        AdicionarParametro(comando, "@anoLancamento", plataforma.AnoLancamento);

        ExecutarGravacao(comando);

        return checked((uint)comando.LastInsertedId);
    }

    public void Alterar(Plataforma plataforma)
    {
        const string sql = @"UPDATE tb_plataforma SET no_plataforma = @nome, no_fabricante = @fabricante, nu_ano_lancamento = @anoLancamento WHERE co_seq_plataforma = @id;";

        using var conexao = AbrirConexao();
        using var comando = CriarComando(sql, conexao);

        AdicionarParametro(comando, "@nome", plataforma.Nome);
        AdicionarParametro(comando, "@fabricante", plataforma.Fabricante);
        AdicionarParametro(comando, "@anoLancamento", plataforma.AnoLancamento);
        AdicionarParametro(comando, "@id", plataforma.Id);

        ExecutarGravacao(comando);
    }

    public void Excluir(uint id)
    {
        const string sql = @"DELETE FROM tb_plataforma WHERE co_seq_plataforma = @id;";

        using var conexao = AbrirConexao();
        using var comando = CriarComando(sql, conexao);

        AdicionarParametro(comando, "@id", id);

        ExecutarGravacao(comando);
    }
}