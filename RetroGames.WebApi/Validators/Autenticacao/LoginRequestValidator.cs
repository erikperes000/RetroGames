using FluentValidation;
using RetroGames.WebApi.ApiModels.Requests;

namespace RetroGames.WebApi.Validators.Autenticacao;

public class LoginRequestValidator
    : AbstractValidator<LoginRequestApiModel>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Informe o e-mail.")
            .EmailAddress()
            .WithMessage("Informe um e-mail válido.")
            .MaximumLength(254)
            .WithMessage("O e-mail deve ter até 254 caracteres.");

        RuleFor(x => x.Senha)
            .NotEmpty()
            .WithMessage("Informe a senha.");
    }
}