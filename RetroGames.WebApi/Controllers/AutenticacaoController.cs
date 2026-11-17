using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetroGames.Model.Constantes;
using RetroGames.Services.Interfaces;
using RetroGames.WebApi.ApiModels.Constantes;
using RetroGames.WebApi.ApiModels.Requests;
using RetroGames.WebApi.ApiModels.Responses;
using RetroGames.WebApi.Extensions;

namespace RetroGames.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AutenticacaoController : ControllerBase
{
    private readonly IAutenticacaoService _autenticacaoService;

    public AutenticacaoController(
        IAutenticacaoService autenticacaoService)
    {
        _autenticacaoService = autenticacaoService;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequestApiModel request)
    {
        var usuario = await _autenticacaoService.AutenticarAsync(
            request.Email,
            request.Senha);

        if (usuario == null)
        {
            var respostaInvalida = new RetroGamesResponseApiModel
            {
                ResponseCode = ResponseCodes.CREDENCIAIS_INVALIDAS,
                ErrorMessage = "E-mail ou senha inválidos."
            };

            return Unauthorized(respostaInvalida);
        }

        var identidade = usuario.ParaClaimsPrincipal();

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            identidade);

        var dados = new UsuarioLogadoResponseApiModel
        {
            Id = usuario.Id,
            Nome = usuario.Nome,
            Email = usuario.Email,
            Perfil = usuario.IdPerfil == Perfis.Administrador
                ? nameof(Perfis.Administrador)
                : nameof(Perfis.Cliente)
        };

        var resposta = new RetroGamesResponseApiModel<UsuarioLogadoResponseApiModel>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = dados
        };

        return Ok(resposta);
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme);

        var resposta = new RetroGamesResponseApiModel
        {
            ResponseCode = ResponseCodes.SUCESSO
        };

        return Ok(resposta);
    }

    [HttpGet("eu")]
    [Authorize]
    public IActionResult Eu()
    {
        var idUsuario = User.ObterIdUsuario();

        var nome = User.Identity?.Name ?? string.Empty;
        var email = User.FindFirst(
            System.Security.Claims.ClaimTypes.Email)?.Value
            ?? string.Empty;

        var perfil = User.FindFirst(
            System.Security.Claims.ClaimTypes.Role)?.Value
            ?? string.Empty;

        var dados = new UsuarioLogadoResponseApiModel
        {
            Id = idUsuario,
            Nome = nome,
            Email = email,
            Perfil = perfil
        };

        var resposta =
            new RetroGamesResponseApiModel<UsuarioLogadoResponseApiModel>
            {
                ResponseCode = ResponseCodes.SUCESSO,
                Data = dados
            };

        return Ok(resposta);
    }
}