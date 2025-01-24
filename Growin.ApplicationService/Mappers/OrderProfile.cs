namespace Growin.ApplicationService.Mappers;

using AutoMapper;
using Growin.ApplicationService.Features.Orders.Commands;
using Growin.Domain.Enums;
using Growin.Domain.Features;
using System;

public class OrderProfile : Profile
{
    public OrderProfile()
    {
        CreateMap<CreateOrderCommand, Order>()
            .ForMember(ds => ds.Id, m => m.MapFrom(_ => 0))
            .ForMember(ds => ds.Status, m => m.MapFrom(_ => EOrderStatus.Reserved))
            .ForMember(ds => ds.CreatedAt, m => m.MapFrom(_ => DateTime.UtcNow))
            .ForMember(ds => ds.ProductId, m => m.MapFrom(src => src.ProductId))
            .ForMember(ds => ds.Quantity, m => m.MapFrom(src => src.Quantity));
    }
}