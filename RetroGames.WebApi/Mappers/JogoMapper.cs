using RetroGames.Services;
using RetroGames.WebApi.ApiModels.Responses;
using RetroGames.Model.Model;
using RetroGames.WebApi.ApiModels.Requests;

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

    public static Jogo ParaModel(
        this InserirJogoRequestApiModel request)
    {
        return new Jogo
        {
            Nome = request.Nome.Trim(),

            Descricao = string.IsNullOrWhiteSpace(request.Descricao)
                ? null
                : request.Descricao.Trim(),

            AnoLancamento = request.AnoLancamento,

            Preco = request.Preco,

            Estoque = request.Estoque,

            IdPlataforma = request.IdPlataforma,

            IdCondicaoJogo = request.IdCondicaoJogo
        };
    }

    public static Jogo ParaModel(
        this AlterarJogoRequestApiModel request,
        uint id)
    {
        return new Jogo
        {
            Id = id,

            Nome = request.Nome.Trim(),

            Descricao = string.IsNullOrWhiteSpace(request.Descricao)
                ? null
                : request.Descricao.Trim(),

            AnoLancamento = request.AnoLancamento,

            Preco = request.Preco,

            Estoque = request.Estoque,

            Ativo = request.Ativo,

            IdPlataforma = request.IdPlataforma,

            IdCondicaoJogo = request.IdCondicaoJogo
        };
    }
}