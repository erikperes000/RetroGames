using Microsoft.AspNetCore.Mvc;
using RetroGames.Services.Interfaces;
using RetroGames.WebApi.ApiModels.Constantes;
using RetroGames.WebApi.ApiModels.Responses;
using RetroGames.WebApi.Mappers;
using Microsoft.AspNetCore.Authorization;

namespace RetroGames.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GeneroController : ControllerBase
{
    private readonly IGeneroService _generoService;

    public GeneroController(IGeneroService generoService)
    {
        _generoService = generoService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Listar()
    {
        var generos = await _generoService.ListarAsync();

        var dados = generos
            .Select(genero => genero.ParaResponse())
            .ToList();

        var resposta =
            new RetroGamesResponseApiModel<List<ListarGeneroResponseApiModel>>
            {
                ResponseCode = ResponseCodes.SUCESSO,
                Data = dados
            };

        return Ok(resposta);
    }
}