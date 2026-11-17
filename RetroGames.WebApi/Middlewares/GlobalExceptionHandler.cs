using Microsoft.AspNetCore.Diagnostics;
using RetroGames.Services;
using RetroGames.WebApi.ApiModels.Constantes;
using RetroGames.WebApi.ApiModels.Responses;
using System.Text.Json;

namespace RetroGames.WebApi.Middlewares;

public class GlobalExceptionHandler : IExceptionHandler{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        RetroGamesResponseApiModel resposta;

        if (exception is BusinessRuleException erroNegocio){
            httpContext.Response.StatusCode = (int)erroNegocio.Erro.Status;

            resposta = new RetroGamesResponseApiModel{
                ResponseCode = erroNegocio.Erro.Code,
                ErrorMessage = erroNegocio.Message
            };
        }else if (exception is JsonException || exception.InnerException is JsonException){
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

            resposta = new RetroGamesResponseApiModel{
                ResponseCode = 3,
                ErrorMessage = "JSON inválido ou malformado."
            };
        }else{_logger.LogError(exception, "Ocorreu um erro inesperado na API.");

            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

            resposta = new RetroGamesResponseApiModel{
                ResponseCode = ResponseCodes.ERRO,
                ErrorMessage = "Ocorreu um erro interno no servidor."
            };
        }

        await httpContext.Response.WriteAsJsonAsync(resposta, cancellationToken);

        return true;
    }
}