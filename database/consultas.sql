-- Catálogo: jogos ativos com o nome da plataforma e a descrição da condição, ordenados por nome.
SELECT
    j.no_jogo,
    p.no_plataforma,
    cj.ds_condicao_jogo,
    j.nu_ano_lancamento,
    j.vl_preco,
    j.qt_estoque
FROM tb_jogo j INNER JOIN tb_plataforma p ON p.co_seq_plataforma = j.co_plataforma INNER JOIN tb_condicao_jogo cj
ON cj.co_condicao_jogo = j.co_condicao_jogo WHERE j.st_ativo = 1 ORDER BY j.no_jogo ASC;

-- Gêneros de um jogo informado.
SET @nome_jogo = 'Super Mario Bros.';
SELECT j.no_jogo, g.no_genero FROM tb_jogo j INNER JOIN tb_jogo_genero jg ON jg.co_jogo = j.co_seq_jogo INNER JOIN tb_genero g
ON g.co_seq_genero = jg.co_genero WHERE j.no_jogo = @nome_jogo ORDER BY g.no_genero;

-- Jogos de um gênero informado.
SET @nome_genero = 'Plataforma';

SELECT g.no_genero, j.no_jogo, p.no_plataforma, j.nu_ano_lancamento FROM tb_genero g INNER JOIN tb_jogo_genero jg
ON jg.co_genero = g.co_seq_genero INNER JOIN tb_jogo j ON j.co_seq_jogo = jg.co_jogo INNER JOIN tb_plataforma p
ON p.co_seq_plataforma = j.co_plataforma WHERE g.no_genero = @nome_genero ORDER BY j.no_jogo;

-- Quantidade de jogos por plataforma.
SELECT p.no_plataforma, COUNT(j.co_seq_jogo) AS qt_jogos FROM tb_plataforma p LEFT JOIN tb_jogo j 
ON j.co_plataforma = p.co_seq_plataforma GROUP BY p.co_seq_plataforma, p.no_plataforma ORDER BY p.no_plataforma;

-- Busca por parte do nome do jogo, com 5 itens por página.
SET @termo = 'Mario';
SELECT
    j.no_jogo,
    p.no_plataforma,
    j.nu_ano_lancamento,
    j.vl_preco,
    j.qt_estoque
FROM tb_jogo j INNER JOIN tb_plataforma p ON p.co_seq_plataforma = j.co_plataforma WHERE j.no_jogo LIKE CONCAT('%', @termo, '%')
ORDER BY j.no_jogo LIMIT 5 OFFSET 0;

/*
==========================================================
TESTES DE INTEGRIDADE - ESPECIFICAÇÕES 4.2 E 4.3
==========================================================

1. Inserir jogo com co_plataforma inexistente:
   Resultado: ERRO 1452.
   Status: Conforme esperado.

2. Inserir plataforma com nome já cadastrado:
   Resultado: ERRO 1062.
   Status: Conforme esperado.

3. Excluir plataforma que possui jogos:
   Resultado: ERRO 1451.
   Status: Conforme esperado.

4. Inserir jogo com no_jogo nulo:
   Resultado: ERRO 1048.
   Status: Conforme esperado.

5. Inserir jogo com vl_preco igual a 0:
   Resultado: ACEITO.
   Status: Conforme esperado.
   Regra de negócio: validação no Service (RN06).

==========================================================
*/