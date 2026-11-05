namespace RetroGames.Model.Model
{
    public class Usuario{
        // Coluna: co_seq_usuario
        public uint CoSeqUsuario { get; set; }

        // Coluna: no_usuario
        public string NoUsuario { get; set; } = string.Empty;

        // Coluna: ds_email
        public string DsEmail { get; set; } = string.Empty;

        // Coluna: ds_senha_hash
        public string DsSenhaHash { get; set; } = string.Empty;

        // Coluna: st_ativo
        public bool StAtivo { get; set; }

        // Coluna: dt_cadastro
        public DateTime DtCadastro { get; set; }

        // Coluna: co_perfil
        public ushort CoPerfil { get; set; }
    }
}