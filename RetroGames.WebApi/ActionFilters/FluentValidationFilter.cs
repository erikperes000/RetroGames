using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.Json;
using RetroGames.WebApi.ApiModels.Constantes;

namespace RetroGames.WebApi.ActionFilters;

public class FluentValidationFilter : IAsyncActionFilter, IOrderedFilter
{
    private readonly IServiceProvider _serviceProvider;

    public int Order => -3000;

    public FluentValidationFilter(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        // Verifica se algum parâmetro obrigatório do corpo
        // da requisição veio nulo.
        foreach (var parametro in context.ActionDescriptor.Parameters)
        {
            if (parametro.BindingInfo?.BindingSource == BindingSource.Body
                && context.ActionArguments.TryGetValue(
                    parametro.Name, out var valor)
                && valor == null)
            {
                context.Result = new BadRequestObjectResult(new
                {
                    responseCode = ResponseCodes.MODEL_NULL,
                    errorMessage = "O modelo veio nulo."
                });

                return;
            }
        }

        // Executa os validators encontrados para cada argumento.
        foreach (var argumento in context.ActionArguments)
        {
            if (argumento.Value == null)
                continue;

            var tipoValidator = typeof(IValidator<>)
                .MakeGenericType(argumento.Value.GetType());

            var validator = _serviceProvider.GetService(tipoValidator)
                as IValidator;

            if (validator == null)
                continue;

            var resultado = await validator.ValidateAsync(
                new ValidationContext<object>(argumento.Value),
                context.HttpContext.RequestAborted);

            if (resultado.IsValid)
                continue;

            // Agrupa os erros por propriedade.
            // Os nomes são convertidos para camelCase.
            var errosPorCampo = resultado.Errors
                .GroupBy(erro =>
                    JsonNamingPolicy.CamelCase.ConvertName(
                        erro.PropertyName))
                .ToDictionary(
                    grupo => grupo.Key,
                    grupo => grupo
                        .Select(erro => erro.ErrorMessage)
                        .ToArray());

            context.Result = new BadRequestObjectResult(new
            {
                responseCode = ResponseCodes.MODEL_STATE_INVALIDO,
                data = errosPorCampo,
                errorMessage = "Há um erro com o modelo enviado."
            });

            return;
        }

        await next();
    }
}