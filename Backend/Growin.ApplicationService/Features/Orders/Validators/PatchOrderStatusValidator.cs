namespace Growin.ApplicationService.Features.Orders.Validators;

using FluentValidation;
using Growin.ApplicationService.Features.Orders.Commands;

internal class PatchOrderStatusValidator : AbstractValidator<PatchOrderStatusCommand>
{
    public PatchOrderStatusValidator()
    {
        RuleFor(a => a.OrderId)
        .GreaterThan(0)
        .WithMessage($"{nameof(PatchOrderStatusCommand.OrderId)} should be greater than 0");
    }
}
