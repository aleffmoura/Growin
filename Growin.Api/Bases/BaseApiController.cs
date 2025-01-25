namespace Growin.Api.Bases;

using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using FunctionalConcepts.Errors;
using FunctionalConcepts.Results;
using Growin.Api.Helpers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Extensions;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Newtonsoft.Json;
using System.Net;

public class BaseApiController(IMediator mediator, IMapper mapper) : ControllerBase
{
    private readonly IMediator _mediator = mediator;
    private readonly IMapper _mapper = mapper;

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

    protected async Task<IActionResult> HandleQueryable<TSource, TDestiny>(
        IRequest<Result<IQueryable<TSource>>> query,
        ODataQueryOptions<TDestiny> queryOptions)
    {
        var result = await _mediator.Send(query);

        return result.Match(succ => Ok(HandlePage(succ, queryOptions)), HandleFailure)!;
    }

    private PageResult<TView> HandlePage<TDomain, TView>
            (IQueryable<TDomain> query,
            ODataQueryOptions<TView> queryOptions)
    {
        var projectTo = query.ProjectTo<TView>(_mapper.ConfigurationProvider);

        var queryResults = queryOptions.ApplyTo(projectTo);
        var oDataFeature = Request.HttpContext.ODataFeature();
        var queryFilter = queryResults.Provider.CreateQuery<TView>(queryResults.Expression);

        return new PageResult<TView>(queryFilter,
                                     oDataFeature.NextLink,
                                     oDataFeature.TotalCount);
    }
    private IActionResult MakePayload(BaseError error)
    {
        var payload = ErrorPayload.New(error.Exception, error.Message, error.Code);

        return Problem(title: $"{error.Exception?.GetType().Name}",
                       detail: payload.ErrorMessage,
                       statusCode: error.Code.GetHashCode());
    }
}
