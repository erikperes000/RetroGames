using RetroGames.Model.Model;
using RetroGames.Repository.Base;
using RetroGames.Repository.Config;
using RetroGames.Repository.Interfaces;

namespace RetroGames.Repository.Repository;

public class GeneroRepository : RepositoryBase, IGeneroRepository{
    public GeneroRepository(RepositoryConfig config)
        : base(config)
    {
    }

    public List<Genero> Listar(){
        const string sql = @"SELECT co_seq_genero, no_genero FROM tb_genero ORDER BY no_genero;";

        using var conexao = AbrirConexao();
        using var comando = CriarComando(sql, conexao);
        using var leitor = comando.ExecuteReader();

        var generos = new List<Genero>();

        while (leitor.Read()){
            generos.Add(new Genero{
                Id = leitor.GetUInt32("co_seq_genero"),
                Nome = leitor.GetString("no_genero")
            });
        }

        return generos;
    }
}