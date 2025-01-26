namespace Growin.Api.Controllers;

using AutoMapper;
using Growin.Api.Bases;
using Growin.ApplicationService.Features.Products.Queries;
using Growin.ApplicationService.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;

[ApiController]
[Route("[controller]")]
public class Products(IMediator mediator, IMapper mapper)
    : BaseApiController(mediator, mapper)
{
    [HttpGet]
    [ProducesResponseType<PageResult<ProductResumeViewModel>>(statusCode: 200)]
    public async Task<IActionResult> Get(ODataQueryOptions<ProductResumeViewModel> queryOptions)
        => await HandleQueryable(new ProductCollectionQuery(), queryOptions);
}
