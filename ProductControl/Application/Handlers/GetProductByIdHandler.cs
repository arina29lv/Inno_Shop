using AutoMapper;
using MediatR;
using ProductControl.Application.DTOs;
using ProductControl.Application.Queries;
using ProductControl.Domain.Interfaces;

namespace ProductControl.Application.Handlers;

public class GetProductByIdHandler : IRequestHandler<GetProductByIdQuery, ProductDto?>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public GetProductByIdHandler(IProductRepository productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }
    
    public async Task<ProductDto?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetProductByIdAsync(request.Id);
        if (product == null)
            return null;

        var hasAccess = request.UserRole == "Admin" || request.UserId == product.UserId;
        if (!hasAccess)
            return null;

        return _mapper.Map<ProductDto>(product);
    }
}