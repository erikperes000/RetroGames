-- Etapa 3:

-- Tabela de condições do jogo (domínio)
CREATE TABLE tb_condicao_jogo (
    co_condicao_jogo SMALLINT UNSIGNED NOT NULL,
    ds_condicao_jogo VARCHAR(100) NOT NULL,
    CONSTRAINT pk_tb_condicao_jogo PRIMARY KEY (co_condicao_jogo)
) ENGINE = InnoDB DEFAULT CHARSET = utf8mb4;


-- Tabela de perfis (domínio)
CREATE TABLE tb_perfil (
    co_perfil SMALLINT UNSIGNED NOT NULL,
    ds_perfil VARCHAR(50) NOT NULL,
    CONSTRAINT pk_tb_perfil PRIMARY KEY (co_perfil)
) ENGINE = InnoDB DEFAULT CHARSET = utf8mb4;


-- Tabela de plataformas
CREATE TABLE tb_plataforma (
    co_seq_plataforma INT UNSIGNED NOT NULL AUTO_INCREMENT,
    no_plataforma VARCHAR(100) NOT NULL,
    no_fabricante VARCHAR(100) NOT NULL,
    nu_ano_lancamento SMALLINT UNSIGNED NOT NULL,
    CONSTRAINT pk_tb_plataforma PRIMARY KEY (co_seq_plataforma),
    CONSTRAINT uq_tb_plataforma_no_plataforma UNIQUE (no_plataforma)
) ENGINE = InnoDB DEFAULT CHARSET = utf8mb4;


-- Tabela de gêneros (domínio / somente leitura)
CREATE TABLE tb_genero (
    co_seq_genero INT UNSIGNED NOT NULL AUTO_INCREMENT,
    no_genero VARCHAR(100) NOT NULL,
    CONSTRAINT pk_tb_genero PRIMARY KEY (co_seq_genero)
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
    CONSTRAINT pk_tb_jogo PRIMARY KEY (co_seq_jogo),
    CONSTRAINT fk_tb_jogo_plataforma FOREIGN KEY (co_plataforma) REFERENCES tb_plataforma (co_seq_plataforma),
    CONSTRAINT fk_tb_jogo_condicao FOREIGN KEY (co_condicao_jogo) REFERENCES tb_condicao_jogo (co_condicao_jogo)
) ENGINE = InnoDB DEFAULT CHARSET = utf8mb4;


-- Tabela associativa entre jogos e gêneros
CREATE TABLE tb_jogo_genero (
    co_jogo INT UNSIGNED NOT NULL,
    co_genero INT UNSIGNED NOT NULL,
    CONSTRAINT pk_tb_jogo_genero PRIMARY KEY (co_jogo, co_genero),
    CONSTRAINT fk_tb_jogo_genero_jogo FOREIGN KEY (co_jogo) REFERENCES tb_jogo (co_seq_jogo),
    CONSTRAINT fk_tb_jogo_genero_genero FOREIGN KEY (co_genero) REFERENCES tb_genero (co_seq_genero)
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
    CONSTRAINT pk_tb_usuario PRIMARY KEY (co_seq_usuario),
    CONSTRAINT uq_tb_usuario_ds_email UNIQUE (ds_email),
    CONSTRAINT fk_tb_usuario_perfil FOREIGN KEY (co_perfil) REFERENCES tb_perfil (co_perfil)
) ENGINE = InnoDB DEFAULT CHARSET = utf8mb4;

-- Etapa 4:

-- Condições dos jogos
INSERT INTO tb_condicao_jogo (co_condicao_jogo, ds_condicao_jogo) VALUES (1, 'Novo'), (2, 'Usado'), (3, 'Recondicionado');

-- Perfis de usuários
INSERT INTO tb_perfil (co_perfil, ds_perfil) VALUES (1, 'Administrador'), (2, 'Cliente');

-- Plataformas
INSERT INTO tb_plataforma
    (no_plataforma, no_fabricante, nu_ano_lancamento)
VALUES
    ('Playstation', 'Sony', 1994),
    ('Playstation 2', 'Sony', 2000),
    ('Xbox', 'Microsoft', 2001),
    ('Dreamcast', 'Sega', 1998),
    ('Nintendo 64', 'Nintendo', 1996),
    ('Nintendo GameCube', 'Nintendo', 2001);
    
-- Gênero
INSERT INTO tb_genero (no_genero) VALUES ('Ação'), ('Aventura'), ('Plataforma'), ('RPG'), ('Corrida'), ('Luta'), ('Tiro'), ('Terror');

-- Jogos
INSERT INTO tb_jogo (
    no_jogo,
    ds_jogo,
    nu_ano_lancamento,
    vl_preco,
    qt_estoque,
    st_ativo,
    dt_cadastro,
    co_plataforma,
    co_condicao_jogo
)
SELECT
    dados.no_jogo,
    dados.ds_jogo,
    dados.nu_ano_lancamento,
    dados.vl_preco,
    dados.qt_estoque,
    dados.st_ativo,
    NOW(),
    p.co_seq_plataforma,
    dados.co_condicao_jogo
FROM (
    SELECT 'Crash Bandicoot' AS no_jogo,
           'O marsupial que rodopia no Playstation.' AS ds_jogo,
           1996 AS nu_ano_lancamento, 50.00 AS vl_preco,
           5 AS qt_estoque, 1 AS st_ativo, 1 AS co_condicao_jogo,
           'Playstation' AS plataforma

    UNION ALL SELECT 'Crash Bandicoot 2: Cortex Strikes Back',
           'O marsupial que rodopiava continua rodopiando.',
           1997, 50.00, 3, 1, 2, 'Playstation'

    UNION ALL SELECT 'Crash Bandicoot: Warped',
           'Esse marsupial não para de rodopiar.',
           1998, 50.00, 2, 1, 2, 'Playstation'

    UNION ALL SELECT 'Crash Team Racing / CTR',
           'Agora o marsupial rodopia em seu dirigível.',
           1999, 50.00, 4, 1, 2, 'Playstation'

    UNION ALL SELECT 'Crash Bandicoot: The Wrath of Cortex',
           'É o mesmo marsupial do PS1, mas agora também está no PS2.',
           2001, 100.00, 0, 1, 1, 'Playstation 2'

    UNION ALL SELECT 'Crash Nitro Cart',
           'O marsupial ainda sabe pilotar.',
           2003, 110.00, 6, 1, 1, 'Playstation 2'

    UNION ALL SELECT 'Crash Twinsanity',
           'O nosso marsupial que rodopia precisa salvar sua casa.',
           2004, 120.00, 3, 1, 2, 'Playstation 2'

    UNION ALL SELECT 'Crash of the Titans',
           'O mesmo marsupial, mas de cara nova!.',
           2007, 120.00, 2, 1, 2, 'Playstation 2'

    UNION ALL SELECT 'Halo: Combat Evolved',
           'Uma missão, um soldado. Tiro em primeira pessoa.',
           2001, 130.00, 2, 1, 2, 'Xbox'

    UNION ALL SELECT 'Halo 2',
           'Tiro em primeira pessoa.',
           2004, 130.00, 1, 1, 1, 'Xbox'

    UNION ALL SELECT 'Ninja Gaiden',
           'Jogo de combate com ninjas.',
           2004, 130.00, 4, 1, 2, 'Xbox'

    UNION ALL SELECT 'IIIbleed',
           'Terror e sobrevivência.',
           2001, 150.00, 2, 1, 2, 'Dreamcast'

    UNION ALL SELECT 'Blue Stinger',
           'Ação e aventura no qual os jogadores controlam Eliot Ballade ou Dogs Bower na Ilha dos Dinossauros.',
           1999, 155.00, 5, 1, 1, 'Dreamcast'

    UNION ALL SELECT 'D2',
           'Terror e sobrevivência.',
           1991, 160.00, 3, 1, 2, 'Dreamcast'

    UNION ALL SELECT 'Super Mario 64',
           'Clássico das pltaformas 2d, agora no 3D.',
           1996, 160.00, 4, 1, 1, 'Nintendo 64'

    UNION ALL SELECT 'The Legend of Zelda: Ocarina of Time',
           'Plataforma de combate e aventura protagonizado por Link, POR LINK!.',
           1998, 170.00, 3, 1, 2, 'Nintendo 64'

    UNION ALL SELECT 'Mario Kart 64',
           'Corrida com mario e seus amigos.',
           1996, 160.00, 2, 1, 2, 'Nintendo 64'

    UNION ALL SELECT 'Paper Mario: The Thousand-Year Door',
           'Aventuras com Mario, só um pouco mais dobrável.',
           2004, 100.00, 5, 1, 1, 'Nintendo GameCube'

    UNION ALL SELECT 'The Legend of Zelda The Wind Waker',
           'RPG de aventura e combate... protagonizado por LINK!',
           2002, 120.00, 0, 1, 2, 'Nintendo GameCube'

    UNION ALL SELECT 'Mario Kart: Double Dash',
           'Aventura motorizada com o encanador favorito.',
           2003, 110.00, 2, 0, 2, 'Nintendo GameCube'
) AS dados INNER JOIN tb_plataforma p ON p.no_plataforma = dados.plataforma;

INSERT INTO tb_jogo_genero (co_jogo, co_genero)
SELECT
    j.co_seq_jogo,
    g.co_seq_genero
FROM (
    SELECT 'Crash Bandicoot' AS jogo, 'Plataforma' AS genero
    UNION ALL SELECT 'Crash Bandicoot', 'Aventura'

    UNION ALL SELECT 'Crash Bandicoot 2: Cortex Strikes Back', 'Plataforma'
    UNION ALL SELECT 'Crash Bandicoot 2: Cortex Strikes Back', 'Aventura'

    UNION ALL SELECT 'Crash Bandicoot: Warped', 'Plataforma'
    UNION ALL SELECT 'Crash Bandicoot: Warped', 'Aventura'

    UNION ALL SELECT 'Crash Team Racing / CTR', 'Corrida'
    UNION ALL SELECT 'Crash Team Racing / CTR', 'Aventura'

    UNION ALL SELECT 'Crash Bandicoot: The Wrath of Cortex', 'Plataforma'
    UNION ALL SELECT 'Crash Bandicoot: The Wrath of Cortex', 'Aventura'

    UNION ALL SELECT 'Crash Nitro Cart', 'Corrida'
    UNION ALL SELECT 'Crash Nitro Cart', 'Aventura'

    UNION ALL SELECT 'Crash Twinsanity', 'Plataforma'
    UNION ALL SELECT 'Crash Twinsanity', 'Aventura'

    UNION ALL SELECT 'Crash of the Titans', 'Plataforma'
    UNION ALL SELECT 'Crash of the Titans', 'Aventura'

    UNION ALL SELECT 'Halo: Combat Evolved', 'Tiro'
    UNION ALL SELECT 'Halo: Combat Evolved', 'Ação'

    UNION ALL SELECT 'Halo 2', 'Tiro'
    UNION ALL SELECT 'Halo 2', 'Ação'

    UNION ALL SELECT 'Ninja Gaiden', 'Luta'
    UNION ALL SELECT 'Ninja Gaiden', 'Ação'

    UNION ALL SELECT 'IIIbleed', 'Terror'
    UNION ALL SELECT 'IIIbleed', 'Ação'

    UNION ALL SELECT 'Blue Stinger', 'Aventura'
    UNION ALL SELECT 'Blue Stingerd', 'Ação'

    UNION ALL SELECT 'D2', 'Terror'
    UNION ALL SELECT 'D2', 'Ação'

    UNION ALL SELECT 'Super Mario 64', 'Plataforma'
    UNION ALL SELECT 'Super Mario 64', 'Aventura'

    UNION ALL SELECT 'The Legend of Zelda: Ocarina of Time', 'RPG'

    UNION ALL SELECT 'Mario Kart 64', 'Corrida'
    UNION ALL SELECT 'Mario Kart 64', 'Aventura'

    UNION ALL SELECT 'Paper Mario: The Thousand-Year Door', 'Aventura'

    UNION ALL SELECT 'The Legend of Zelda The Wind Waker', 'RPG'
    UNION ALL SELECT 'The Legend of Zelda The Wind Waker', 'Aventura'

    UNION ALL SELECT 'Mario Kart: Double Dash', 'Corrida'
    UNION ALL SELECT 'Mario Kart: Double Dash', 'Aventura'
) AS relacionamentos
INNER JOIN tb_jogo j
    ON j.no_jogo = relacionamentos.jogo
INNER JOIN tb_genero g
    ON g.no_genero = relacionamentos.genero;