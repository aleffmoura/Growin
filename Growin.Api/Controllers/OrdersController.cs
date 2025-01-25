namespace Growin.Api.Controllers;

using Growin.Api.Bases;
using Growin.ApplicationService.Features.Orders.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class OrdersController : BaseApiController
{
    public OrdersController(IMediator mediator) : base(mediator) { }

    [HttpPost]
    [ProducesResponseType<long>(statusCode: 201)]
    public async Task<IActionResult> Post([FromBody] CreateOrderCommand createCmd)
            => await HandleCommand(createCmd);
}
