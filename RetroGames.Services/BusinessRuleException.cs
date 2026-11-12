
namespace RetroGames.Services;

public class BusinessRuleException : Exception{
    public (int Code, string DefaultMessage, System.Net.HttpStatusCode Status) Erro { get; }

    public BusinessRuleException(
        (int Code, string DefaultMessage, System.Net.HttpStatusCode Status) erro)
        : base(erro.DefaultMessage)
    {
        Erro = erro;
    }
}
