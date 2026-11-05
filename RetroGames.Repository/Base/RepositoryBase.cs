using MySqlConnector;
using RetroGames.Repository.Config;
using RetroGames.Repository.Exceptions;

namespace RetroGames.Repository.Base;

public abstract class RepositoryBase{ //abstract para que a classe seja a base e não seja instanciada diretamente.
    private readonly RepositoryConfig _config;

    protected RepositoryBase(RepositoryConfig config){
        _config = config; //guarda a configuração recebida no construtor.
    }

    protected MySqlConnection AbrirConexao(){
        var conexao = new MySqlConnection(_config.ConnectionString);
        conexao.Open();
        return conexao;
    }

    protected MySqlCommand CriarComando(string sql, MySqlConnection conexao){
        return new MySqlCommand(sql, conexao);
    }

    //insere parâmetros na query evitando concatenar valores diretamente no SQL.
    protected void AdicionarParametro(MySqlCommand comando, string nome, object valor){
        comando.Parameters.AddWithValue(nome, valor);
    }

    //executa INSERT ou UPDATE e captura o erro 1062.
    protected int ExecutarGravacao(MySqlCommand comando){
        try{
            return comando.ExecuteNonQuery();
        }catch (MySqlException ex) when (ex.Number == 1062){
            throw new RegistroDuplicadoException("Já existe um registro com esse valor único.");
        }
    }
}