using Microsoft.AspNetCore.Mvc;
using RetroGames.Services.Interfaces;
using RetroGames.WebApi.ApiModels.Constantes;
using RetroGames.WebApi.ApiModels.Responses;
using RetroGames.WebApi.Mappers;
using Microsoft.AspNetCore.Authorization;

namespace RetroGames.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CondicaoJogoController : ControllerBase
{
    private readonly ICondicaoJogoService _condicaoJogoService;

    public CondicaoJogoController(
        ICondicaoJogoService condicaoJogoService)
    {
        _condicaoJogoService = condicaoJogoService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Listar()
    {
        var condicoes = await _condicaoJogoService.ListarAsync();

        var dados = condicoes
            .Select(condicao => condicao.ParaResponse())
            .ToList();

        var resposta =
            new RetroGamesResponseApiModel<List<ListarCondicaoJogoResponseApiModel>>
            {
                ResponseCode = ResponseCodes.SUCESSO,
                Data = dados
            };

        return Ok(resposta);
    }
}