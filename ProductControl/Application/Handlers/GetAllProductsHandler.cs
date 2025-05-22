using AutoMapper;
using MediatR;
using ProductControl.Application.DTOs;
using ProductControl.Application.Queries;
using ProductControl.Domain.Interfaces;
using ProductControl.Domain.Models;

namespace ProductControl.Application.Handlers;

public class GetAllProductsHandler : IRequestHandler<GetAllProductsQuery, IEnumerable<ProductDto>>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public GetAllProductsHandler(IProductRepository productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }
    
    public async Task<IEnumerable<ProductDto>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        var products = new List<Product>();
        
        if (request.UserRole == "Admin")
        {
            products = (await _productRepository.GetAllProductsAsync()).Where(p => !p.IsDeleted).ToList();
        }

        else
        {
            products = (await _productRepository.GetProductsByUserIdAsync(request.UserId)).ToList();
        }
        
        if (request.IsAvailable.HasValue)
            products = products.Where(p => p.IsAvailable == request.IsAvailable.Value).ToList();

        if (request.PriceMin.HasValue)
            products = products.Where(p => p.Price >= request.PriceMin.Value).ToList();

        if (request.PriceMax.HasValue)
            products = products.Where(p => p.Price <= request.PriceMax.Value).ToList();

        if (!string.IsNullOrWhiteSpace(request.NameContains))
            products = products.Where(p => p.Name.Contains(request.NameContains, StringComparison.OrdinalIgnoreCase)).ToList();
        
        
        return _mapper.Map<IEnumerable<ProductDto>>(products);
    }
}