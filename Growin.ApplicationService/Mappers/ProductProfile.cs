namespace Growin.ApplicationService.Mappers;
using AutoMapper;
using Growin.ApplicationService.ViewModels;

using Growin.Domain.Features;

internal class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<Product, ProductResumeViewModel>()
            .ForMember(ds => ds.Id, m => m.MapFrom(src => src.Id))
            .ForMember(ds => ds.Name, m => m.MapFrom(src => src.Name))
            .ForMember(ds => ds.QuantityInStock, m => m.MapFrom(src => src.QuantityInStock));
    }
}