using MediatR;
using ProductControl.Application.DTOs;

namespace ProductControl.Application.Queries;

public class GetAllProductsQuery : IRequest<IEnumerable<ProductDto>>
{
    public int UserId { get; set; }
    public string UserRole { get; set; }
    
    public bool? IsAvailable { get; set; }
    public decimal? PriceMin { get; set; }
    public decimal? PriceMax { get; set; }
    public string? NameContains { get; set; }
}