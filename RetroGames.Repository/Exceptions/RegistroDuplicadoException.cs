namespace RetroGames.Repository.Exceptions;

public class RegistroDuplicadoException : Exception{
    public RegistroDuplicadoException(string message)
        : base(message)
    {
    }
}
