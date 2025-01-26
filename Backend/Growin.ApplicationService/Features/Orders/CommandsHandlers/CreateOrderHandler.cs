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
    IProductReadRepository productReadRepository,
    IProductWriteRepository productWriteRepository,
    IMapper mapper)
    : IRequestHandler<CreateOrderCommand, Result<long>>
{
    public async Task<Result<long>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var productOpt =
                await productReadRepository.GetAsync(request.ProductId, cancellationToken);

            return await productOpt.MatchAsync(async product =>
                product switch
                {
                    { QuantityInStock: var quantity } when quantity >= request.Quantity
                      => await AddOrder(product),
                    _ => await ProductOutOfStock()
                }, NotFound);

            #region nested function
            async Task<Result<long>> AddOrder(Product product)
            {
                Order order = mapper.Map<Order>(request);

                var createdEntity = await orderWriteRepository.AddAsync(order, cancellationToken);

                _ = productWriteRepository.UpdateAsync(product with
                {
                    QuantityInStock = product.QuantityInStock - request.Quantity
                }, cancellationToken);

                return Result.Of(createdEntity.Id);
            }

            Task<Result<long>> NotFound()
                => Task.Run(() => Result.Of<long>(NotFoundError.New("ProductNotFound")));

            Task<Result<long>> ProductOutOfStock()
                => Task.Run(() => Result.Of<long>(InvalidObjectError.New("ProductOutOfStock")));
            #endregion
        }
        catch (Exception ex)
        {
            UnhandledError error = ("Handled exception on create order", ex);
            return error;
        }
    }
}
