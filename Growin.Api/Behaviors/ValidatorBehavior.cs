namespace Growin.Api.Behaviors;

using FluentValidation;
using FunctionalConcepts.Errors;
using FunctionalConcepts.Results;
using MediatR;

public class ValidatorBehavior<TRequest, TResponse>(IValidator<TRequest>[] validators)
    : IPipelineBehavior<TRequest, Result<TResponse>> where TRequest : notnull
{
    private readonly IValidator<TRequest>[] _validators = validators;

    public async Task<Result<TResponse>> Handle(TRequest request, RequestHandlerDelegate<Result<TResponse>> next, CancellationToken cancellationToken)
    {
        List<FluentValidation.Results.ValidationFailure> failures = _validators
            .Select(v => v.Validate(request))
            .SelectMany(result => result.Errors)
            .Where(error => error != null)
            .ToList();

        return failures.Count != 0
               ? (InvalidObjectError)("Fail on validate request", new ValidationException(failures))
               : await next();
    }
}
