namespace Growin.Api.Controllers;

using AutoMapper;
using Growin.Api.Bases;
using Growin.ApplicationService.Features.Orders.Commands;
using Growin.ApplicationService.Features.Orders.Queries;
using Growin.ApplicationService.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;

[ApiController]
[Route("[controller]")]
public class OrdersController(IMediator mediator, IMapper mapper)
    : BaseApiController(mediator, mapper)
{
    [HttpPost]
    [ProducesResponseType<long>(statusCode: 201)]
    public async Task<IActionResult> Post([FromBody] CreateOrderCommand createCmd)
            => await HandleCommand(createCmd);

    [HttpGet]
    [ProducesResponseType<PageResult<OrderResumeViewModel>>(statusCode: 200)]
    public async Task<IActionResult> Get(ODataQueryOptions<OrderResumeViewModel> queryOptions)
        => await HandleQueryable(new OrderCollectionQuery(), queryOptions);
}
