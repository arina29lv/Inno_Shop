using AutoMapper;
using ProductControl.Application.Command;
using ProductControl.Application.DTOs;
using ProductControl.Domain.Models;

namespace ProductControl.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Product, ProductDto>();
        CreateMap<CreateProductCommand, Product>()
            .ForMember(dest => dest.Id, opt => opt.Ignore()) 
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()) 
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore()) 
            .ForMember(dest => dest.UserId, opt =>
            {
                opt.PreCondition(src => src.UserId.HasValue);
                opt.MapFrom(src => src.UserId!.Value);
            });
        CreateMap<UpdateProductCommand, Product>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()) 
            .ForMember(dest => dest.UserId, opt =>
            {
                opt.PreCondition(src => src.UserId.HasValue);
                opt.MapFrom(src => src.UserId!.Value);
            });
    }
}