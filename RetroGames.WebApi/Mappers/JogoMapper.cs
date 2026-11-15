using RetroGames.Services;
using RetroGames.WebApi.ApiModels.Responses;

namespace RetroGames.WebApi.Mappers;

public static class JogoMapper
{
    public static ListarJogoResponseApiModel ParaListagem(
        this JogoDetalheServiceModel detalhe)
    {
        return new ListarJogoResponseApiModel
        {
            Id = detalhe.Jogo.Id,
            Nome = detalhe.Jogo.Nome,
            AnoLancamento = detalhe.Jogo.AnoLancamento,
            Preco = detalhe.Jogo.Preco,
            Estoque = detalhe.Jogo.Estoque,
            Ativo = detalhe.Jogo.Ativo,
            NomePlataforma = detalhe.NomePlataforma,
            DescricaoCondicao = detalhe.DescricaoCondicao,
            Generos = detalhe.Generos
                .Select(genero => genero.Nome)
                .ToList()
        };
    }

    public static ListarPorIdJogoResponseApiModel ParaDetalhe(
        this JogoDetalheServiceModel detalhe)
    {
        return new ListarPorIdJogoResponseApiModel
        {
            Id = detalhe.Jogo.Id,
            Nome = detalhe.Jogo.Nome,
            Descricao = detalhe.Jogo.Descricao,
            AnoLancamento = detalhe.Jogo.AnoLancamento,
            Preco = detalhe.Jogo.Preco,
            Estoque = detalhe.Jogo.Estoque,
            Ativo = detalhe.Jogo.Ativo,
            IdPlataforma = detalhe.Jogo.IdPlataforma,
            NomePlataforma = detalhe.NomePlataforma,
            IdCondicaoJogo = detalhe.Jogo.IdCondicaoJogo,
            DescricaoCondicao = detalhe.DescricaoCondicao,
            Generos = detalhe.Generos
                .Select(genero => new ListarGeneroResponseApiModel
                {
                    Id = genero.Id,
                    Nome = genero.Nome
                })
                .ToList()
        };
    }
}