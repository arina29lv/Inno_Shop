using AutoMapper;
using MediatR;
using ProductControl.Application.Command;
using ProductControl.Domain.Interfaces;
using ProductControl.Domain.Models;

namespace ProductControl.Application.Handlers;

public class CreateProductHandler : IRequestHandler<CreateProductCommand, int>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public CreateProductHandler(IProductRepository productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }
    
    public async Task<int> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = _mapper.Map<Product>(request);
        return await _productRepository.AddProductAsync(product);
    }
}