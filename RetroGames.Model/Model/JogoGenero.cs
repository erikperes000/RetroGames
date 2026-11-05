namespace RetroGames.Model.Model{
    public class JogoGenero{
        // Coluna: co_jogo (parte da chave primária e chave estrangeira)
        public uint IdJogo { get; set; }

        // Coluna: co_genero (parte da chave primária e chave estrangeira)
        public uint IdGenero { get; set; }
    }
}