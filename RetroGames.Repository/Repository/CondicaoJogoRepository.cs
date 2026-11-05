using RetroGames.Model.Model;
using RetroGames.Repository.Base;
using RetroGames.Repository.Config;
using RetroGames.Repository.Interfaces;

namespace RetroGames.Repository.Repository;

public class GeneroRepository : RepositoryBase, ICondicaoJogoRepository{
    public GeneroRepository(RepositoryConfig config)
        : base(config)
    {
    }

    public List<Condicao> Listar(){
        const string sql = @"SELECT co_condicao_jogo, ds_condicao_jogo FROM tb_condicao_jogo;";

        using var conexao = AbrirConexao();
        using var comando = CriarComando(sql, conexao);
        using var leitor = comando.ExecuteReader();

        var codicao = new List<Condicao>();

        while (leitor.Read()){
            condicao.Add(new Condico{
                Id = leitor.GetUInt32("cco_condicao_jogo"),
                Nome = leitor.GetString("ds_condicao_jogo")
            });
        }

        return generos;
    }
}