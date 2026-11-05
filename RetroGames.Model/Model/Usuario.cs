namespace RetroGames.Model.Model{
    public class Usuario{
        // Coluna: co_seq_usuario (chave primária)
        public uint Id { get; set; }

        // Coluna: no_usuario
        public string Nome { get; set; } = string.Empty;

        // Coluna: ds_email
        public string Email { get; set; } = string.Empty;

        // Coluna: ds_senha_hash
        public string SenhaHash { get; set; } = string.Empty;

        // Coluna: st_ativo
        public bool Ativo { get; set; }

        // Coluna: dt_cadastro
        public DateTime DataCadastro { get; set; }

        // Coluna: co_perfil (chave estrangeira)
        public ushort IdPerfil { get; set; }
    }
}