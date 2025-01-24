namespace Growin.ApplicationService.Features.Orders.CommandsHandlers;

using AutoMapper;
using FunctionalConcepts.Errors;
using FunctionalConcepts.Results;
using Growin.ApplicationService.Features.Orders.Commands;
using Growin.Domain.Features;
using Growin.Domain.Interfaces.WriteRepositories;
using MediatR;
using System;
using System.Threading.Tasks;

public class CreateOrderHandler(
    IOrderWriteRepository orderWriteRepository,
    IMapper mapper)
    : IRequestHandler<CreateOrderCommand, Result<long>>
{
    public async Task<Result<long>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        try
        {
            Order order = mapper.Map<Order>(request);

            var createdEntity = await orderWriteRepository.AddAsync(order, cancellationToken);

            return createdEntity.Id;
        }
        catch (Exception ex)
        {
            UnhandledError error = ("Handled exception on create order", ex);
            return error;
        }
    }
}
