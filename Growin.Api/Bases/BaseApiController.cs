namespace Growin.Api.Bases;

using FluentValidation;
using FunctionalConcepts.Errors;
using FunctionalConcepts.Results;
using Growin.Api.Helpers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;

public class BaseApiController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    protected async Task<IActionResult> HandleCommand<TR>(IRequest<Result<TR>> cmd)
        where TR : struct
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
