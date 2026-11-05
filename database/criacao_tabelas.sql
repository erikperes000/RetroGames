-- Tabela de condições do jogo (domínio)
CREATE TABLE tb_condicao_jogo (
    co_condicao_jogo SMALLINT UNSIGNED NOT NULL,
    ds_condicao_jogo VARCHAR(100) NOT NULL,

    CONSTRAINT pk_tb_condicao_jogo
        PRIMARY KEY (co_condicao_jogo)
) ENGINE = InnoDB DEFAULT CHARSET = utf8mb4;


-- Tabela de perfis (domínio)
CREATE TABLE tb_perfil (
    co_perfil SMALLINT UNSIGNED NOT NULL,
    ds_perfil VARCHAR(50) NOT NULL,

    CONSTRAINT pk_tb_perfil
        PRIMARY KEY (co_perfil)
) ENGINE = InnoDB DEFAULT CHARSET = utf8mb4;


-- Tabela de plataformas
CREATE TABLE tb_plataforma (
    co_seq_plataforma INT UNSIGNED NOT NULL AUTO_INCREMENT,
    no_plataforma VARCHAR(100) NOT NULL,
    no_fabricante VARCHAR(100) NOT NULL,
    nu_ano_lancamento SMALLINT UNSIGNED NOT NULL,

    CONSTRAINT pk_tb_plataforma
        PRIMARY KEY (co_seq_plataforma),

    CONSTRAINT uq_tb_plataforma_no_plataforma
        UNIQUE (no_plataforma)
) ENGINE = InnoDB DEFAULT CHARSET = utf8mb4;


-- Tabela de gêneros (domínio / somente leitura)
CREATE TABLE tb_genero (
    co_seq_genero INT UNSIGNED NOT NULL AUTO_INCREMENT,
    no_genero VARCHAR(100) NOT NULL,

    CONSTRAINT pk_tb_genero
        PRIMARY KEY (co_seq_genero)
) ENGINE = InnoDB DEFAULT CHARSET = utf8mb4;


-- Tabela principal de jogos
CREATE TABLE tb_jogo (
    co_seq_jogo INT UNSIGNED NOT NULL AUTO_INCREMENT,
    no_jogo VARCHAR(150) NOT NULL,
    ds_jogo TEXT NULL,
    nu_ano_lancamento SMALLINT UNSIGNED NOT NULL,
    vl_preco DECIMAL(10,2) NOT NULL,
    qt_estoque INT UNSIGNED NOT NULL,
    st_ativo TINYINT(1) NOT NULL,
    dt_cadastro DATETIME NOT NULL,
    co_plataforma INT UNSIGNED NOT NULL,
    co_condicao_jogo SMALLINT UNSIGNED NOT NULL,

    CONSTRAINT pk_tb_jogo
        PRIMARY KEY (co_seq_jogo),

    CONSTRAINT fk_tb_jogo_plataforma
        FOREIGN KEY (co_plataforma)
        REFERENCES tb_plataforma (co_seq_plataforma),

    CONSTRAINT fk_tb_jogo_condicao
        FOREIGN KEY (co_condicao_jogo)
        REFERENCES tb_condicao_jogo (co_condicao_jogo)
) ENGINE = InnoDB DEFAULT CHARSET = utf8mb4;


-- Tabela associativa entre jogos e gêneros
CREATE TABLE tb_jogo_genero (
    co_jogo INT UNSIGNED NOT NULL,
    co_genero INT UNSIGNED NOT NULL,

    CONSTRAINT pk_tb_jogo_genero
        PRIMARY KEY (co_jogo, co_genero),

    CONSTRAINT fk_tb_jogo_genero_jogo
        FOREIGN KEY (co_jogo)
        REFERENCES tb_jogo (co_seq_jogo),

    CONSTRAINT fk_tb_jogo_genero_genero
        FOREIGN KEY (co_genero)
        REFERENCES tb_genero (co_seq_genero)
) ENGINE = InnoDB DEFAULT CHARSET = utf8mb4;


-- Tabela de usuários
CREATE TABLE tb_usuario (
    co_seq_usuario INT UNSIGNED NOT NULL AUTO_INCREMENT,
    no_usuario VARCHAR(120) NOT NULL,
    ds_email VARCHAR(254) NOT NULL,
    ds_senha_hash VARCHAR(255) NOT NULL,
    st_ativo TINYINT(1) NOT NULL,
    dt_cadastro DATETIME NOT NULL,
    co_perfil SMALLINT UNSIGNED NOT NULL,

    CONSTRAINT pk_tb_usuario
        PRIMARY KEY (co_seq_usuario),

    CONSTRAINT uq_tb_usuario_ds_email
        UNIQUE (ds_email),

    CONSTRAINT fk_tb_usuario_perfil
        FOREIGN KEY (co_perfil)
        REFERENCES tb_perfil (co_perfil)
) ENGINE = InnoDB DEFAULT CHARSET = utf8mb4;
