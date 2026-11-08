using MySqlConnector;
using RetroGames.Repository.Config;
using RetroGames.Repository.Exceptions;

namespace RetroGames.Repository.Base;

public abstract class RepositoryBase{
    private readonly RepositoryConfig _config;

    protected RepositoryBase(RepositoryConfig config){
        _config = config;
    }

    // Abre uma conexão de forma assíncrona.
    protected async Task<MySqlConnection> AbrirConexaoAsync(){
        var conexao = new MySqlConnection(_config.ConnectionString);
        try{
            await conexao.OpenAsync();
            return conexao;
        }catch{
            await conexao.DisposeAsync();
            throw;
        }
    }

    // Cria um comando SQL.
    protected MySqlCommand CriarComando(string sql, MySqlConnection conexao){
        return new MySqlCommand(sql, conexao);
    }

    // Adiciona parâmetros ao comando.
    protected void AdicionarParametro(MySqlCommand comando, string nome, object valor){
        comando.Parameters.AddWithValue(nome, valor);
    }

    // Executa INSERT ou UPDATE de forma assíncrona.
    protected async Task<int> ExecutarGravacaoAsync(MySqlCommand comando){
        try{
            return await comando.ExecuteNonQueryAsync();
        }catch (MySqlException ex) when (ex.Number == 1062){
            throw new RegistroDuplicadoException("Já existe um registro com esse valor único.");
        }
    }
}