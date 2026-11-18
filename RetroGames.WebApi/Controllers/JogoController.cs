using Microsoft.AspNetCore.Mvc;
using RetroGames.Services.Interfaces;
using RetroGames.WebApi.ApiModels.Constantes;
using RetroGames.WebApi.ApiModels.Requests;
using RetroGames.WebApi.ApiModels.Responses;
using RetroGames.WebApi.Mappers;
using RetroGames.Model.Model;
using RetroGames.WebApi.Validators.Jogo;
using Microsoft.AspNetCore.Authorization;
using RetroGames.WebApi.Configuration;

namespace RetroGames.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JogoController : ControllerBase
{
    private readonly IJogoService _jogoService;

    public JogoController(IJogoService jogoService)
    {
        _jogoService = jogoService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Listar(
        [FromQuery] ListarJogoRequestApiModel request)
    {
        var (jogos, total) = await _jogoService.ListarAsync(
            pagina: request.Pagina,
            tamanhoPagina: request.TamanhoPagina,
            nome: request.Busca,
            idPlataforma: request.IdPlataforma,
            idGenero: request.IdGenero,
            incluirInativos: request.IncluirInativos);

        var itens = jogos
            .Select(jogo => jogo.ParaListagem())
            .ToList();

        var dados = new PaginaResponseApiModel<ListarJogoResponseApiModel>
        {
            Itens = itens,
            Pagina = request.Pagina,
            TamanhoPagina = request.TamanhoPagina,
            TotalItens = total,
            TotalPaginas = (int)Math.Ceiling(
                total / (double)request.TamanhoPagina)
        };

        var resposta =
            new RetroGamesResponseApiModel<
                PaginaResponseApiModel<ListarJogoResponseApiModel>>
            {
                ResponseCode = ResponseCodes.SUCESSO,
                Data = dados
            };

        return Ok(resposta);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> ListarPorId(uint id)
    {
        var jogo = await _jogoService.BuscarPorIdAsync(id);

        var dados = jogo.ParaDetalhe();

        var resposta =
            new RetroGamesResponseApiModel<ListarPorIdJogoResponseApiModel>
            {
                ResponseCode = ResponseCodes.SUCESSO,
                Data = dados
            };

        return Ok(resposta);
    }

    [HttpPost]
    [Authorize(Policy = Politicas.Administrador)]
    public async Task<IActionResult> Inserir(
        [FromBody] InserirJogoRequestApiModel request)
    {
        var jogo = await _jogoService.InserirAsync(
            request.ParaModel(),
            request.IdsGeneros);

        var dados = jogo.ParaDetalhe();

        var resposta =
            new RetroGamesResponseApiModel<ListarPorIdJogoResponseApiModel>
            {
                ResponseCode = ResponseCodes.SUCESSO,
                Data = dados
            };

        return CreatedAtAction(
            nameof(ListarPorId),
            new { id = jogo.Jogo.Id },
            resposta);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = Politicas.Administrador)]
    public async Task<IActionResult> Alterar(
        uint id,
        [FromBody] AlterarJogoRequestApiModel request)
    {
        var jogo = await _jogoService.AlterarAsync(
            request.ParaModel(id),
            request.IdsGeneros);

        var dados = jogo.ParaDetalhe();

        var resposta =
            new RetroGamesResponseApiModel<ListarPorIdJogoResponseApiModel>
            {
                ResponseCode = ResponseCodes.SUCESSO,
                Data = dados
            };

        return Ok(resposta);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = Politicas.Administrador)]
    public async Task<IActionResult> Excluir(uint id)
    {
        await _jogoService.DesativarAsync(id);

        var resposta = new RetroGamesResponseApiModel
        {
            ResponseCode = ResponseCodes.SUCESSO
        };

        return Ok(resposta);
    }
}