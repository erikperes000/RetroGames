using Microsoft.AspNetCore.Mvc;
using RetroGames.Services.Interfaces;
using RetroGames.WebApi.ApiModels.Constantes;
using RetroGames.WebApi.ApiModels.Requests;
using RetroGames.WebApi.ApiModels.Responses;
using RetroGames.WebApi.Mappers;
using Microsoft.AspNetCore.Authorization;
using RetroGames.WebApi.Configuration;

namespace RetroGames.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlataformaController : ControllerBase
{
    private readonly IPlataformaService _plataformaService;

    public PlataformaController(IPlataformaService plataformaService)
    {
        _plataformaService = plataformaService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Listar()
    {
        var plataformas = await _plataformaService.ListarAsync();

        var dados = plataformas
            .Select(plataforma => plataforma.ParaResponse())
            .ToList();

        var resposta =
            new RetroGamesResponseApiModel<List<ListarPlataformaResponseApiModel>>
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
        var plataforma = await _plataformaService.BuscarPorIdAsync(id);

        var dados = plataforma.ParaResponse();

        var resposta =
            new RetroGamesResponseApiModel<ListarPlataformaResponseApiModel>
            {
                ResponseCode = ResponseCodes.SUCESSO,
                Data = dados
            };

        return Ok(resposta);
    }

    [HttpPost]
    [Authorize(Policy = Politicas.Administrador)]
    public async Task<IActionResult> Inserir(
        [FromBody] InserirPlataformaRequestApiModel request)
    {
        var plataforma = request.ParaModel();

        var criada = await _plataformaService.InserirAsync(plataforma);

        var dados = criada.ParaResponse();

        var resposta =
            new RetroGamesResponseApiModel<ListarPlataformaResponseApiModel>
            {
                ResponseCode = ResponseCodes.SUCESSO,
                Data = dados
            };

        return CreatedAtAction(
            nameof(ListarPorId),
            new { id = criada.Id },
            resposta
        );
    }

    [HttpPut("{id}")]
    [Authorize(Policy = Politicas.Administrador)]
    public async Task<IActionResult> Alterar(
        uint id,
        [FromBody] AlterarPlataformaRequestApiModel request)
    {
        var plataforma = request.ParaModel(id);

        await _plataformaService.AlterarAsync(plataforma);

        var resposta = new RetroGamesResponseApiModel
        {
            ResponseCode = ResponseCodes.SUCESSO
        };

        return Ok(resposta);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = Politicas.Administrador)]
    public async Task<IActionResult> Excluir(uint id)
    {
        await _plataformaService.ExcluirAsync(id);

        var resposta = new RetroGamesResponseApiModel
        {
            ResponseCode = ResponseCodes.SUCESSO
        };

        return Ok(resposta);
    }
}