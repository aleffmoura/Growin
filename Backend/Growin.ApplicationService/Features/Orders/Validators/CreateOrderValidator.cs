namespace Growin.ApplicationService.Features.Orders.Validators;

using FluentValidation;
using Growin.ApplicationService.Features.Orders.Commands;

internal class CreateOrderValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderValidator()
    {
        RuleFor(a => a.ProductId)
        .GreaterThan(0)
        .WithMessage($"{nameof(CreateOrderCommand.ProductId)} should be greater than 0");

        RuleFor(a => a.Quantity)
        .GreaterThan(0)
        .WithMessage($"{nameof(CreateOrderCommand.Quantity)} should be greater than 0");
    }
}
