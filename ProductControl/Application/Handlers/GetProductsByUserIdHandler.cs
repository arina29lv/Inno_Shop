using AutoMapper;
using MediatR;
using ProductControl.Application.DTOs;
using ProductControl.Application.Queries;
using ProductControl.Domain.Interfaces;

namespace ProductControl.Application.Handlers;

public class GetProductsByUserIdHandler : IRequestHandler<GetProductsByUserIdQuery, IEnumerable<ProductDto>>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public GetProductsByUserIdHandler(IProductRepository productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }
    
    public async Task<IEnumerable<ProductDto>> Handle(GetProductsByUserIdQuery request, CancellationToken cancellationToken)
    {
        var products = await _productRepository.GetProductsByUserIdAsync(request.UserId);
        return _mapper.Map<IEnumerable<ProductDto>>(products);
    }
}