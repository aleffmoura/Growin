namespace Growin.Api.Bases;

using FunctionalConcepts.Results;
using FunctionalConcepts;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using FunctionalConcepts.Errors;
using Newtonsoft.Json;
using System.Net;
using Growin.Api.Helpers;
using FluentValidation;

public class BaseApiController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    protected async Task<IActionResult> HandleCommand(IRequest<Result<Success>> cmd)
    {
        var result = await _mediator.Send(cmd);
        return result.Match(succ => Ok(succ), HandleFailure)!;
    }

    private IActionResult HandleFailure(BaseError error)
        => error.Exception is ValidationException validationError
            ? Problem(title: "ValidationError",
                              detail: JsonConvert.SerializeObject(validationError.Errors),
                              statusCode: HttpStatusCode.BadRequest.GetHashCode())
            : MakePayload(error);

    private IActionResult MakePayload(BaseError error)
    {
        var payload = ErrorPayload.New(error.Exception, error.Message, error.Code);

        return Problem(title: $"{error.Exception?.GetType().Name}",
                       detail: payload.ErrorMessage,
                       statusCode: error.Code.GetHashCode());
    }
}
