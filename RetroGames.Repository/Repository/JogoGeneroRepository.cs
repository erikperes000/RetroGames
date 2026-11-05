using MySqlConnector;
using RetroGames.Repository.Base;
using RetroGames.Repository.Config;
using RetroGames.Repository.Interfaces;

namespace RetroGames.Repository.Repository;

public class JogoGeneroRepository : RepositoryBase, IJogoGeneroRepository{
    public JogoGeneroRepository(RepositoryConfig config)
        : base(config)
    {
    }

    public List<(uint IdJogo, uint IdGenero)> ListarTodos(){
        const string sql = @"SELECT co_jogo, co_genero FROM tb_jogo_genero;";

        using var conexao = AbrirConexao();
        using var comando = CriarComando(sql, conexao);
        using var leitor = comando.ExecuteReader();

        var vinculos = new List<(uint IdJogo, uint IdGenero)>();

        while (leitor.Read()){
            vinculos.Add((leitor.GetUInt32("co_jogo"), leitor.GetUInt32("co_genero")));
        }

        return vinculos;
    }

    public List<(uint IdJogo, uint IdGenero)> ListarPorJogos(List<uint> idsJogos){
        if (idsJogos == null || idsJogos.Count == 0)
            return new List<(uint IdJogo, uint IdGenero)>();

        string parametros = string.Join(", ", idsJogos.Select((_, i) => $"@id{i}"));

        string sql = $@"SELECT co_jogo, co_genero FROM tb_jogo_genero WHERE co_jogo IN ({parametros});";

        using var conexao = AbrirConexao();
        using var comando = CriarComando(sql, conexao);

        for (int i = 0; i < idsJogos.Count; i++)
            AdicionarParametro(comando, $"@id{i}", idsJogos[i]);

        using var leitor = comando.ExecuteReader();

        var vinculos = new List<(uint IdJogo, uint IdGenero)>();

        while (leitor.Read()){
            vinculos.Add((leitor.GetUInt32("co_jogo"),leitor.GetUInt32("co_genero")));
        }

        return vinculos;
    }

    public void Inserir(uint idJogo, uint idGenero)
    {
        const string sql = @"INSERT INTO tb_jogo_genero (co_jogo, co_genero) VALUES (@idJogo, @idGenero);";

        using var conexao = AbrirConexao();
        using var comando = CriarComando(sql, conexao);

        AdicionarParametro(comando, "@idJogo", idJogo);
        AdicionarParametro(comando, "@idGenero", idGenero);
        ExecutarGravacao(comando);
    }

    public void RemoverPorJogo(uint idJogo){
        const string sql = @"DELETE FROM tb_jogo_genero WHERE co_jogo = @idJogo;";

        using var conexao = AbrirConexao();
        using var comando = CriarComando(sql, conexao);

        AdicionarParametro(comando, "@idJogo", idJogo);
        ExecutarGravacao(comando);
    }
}