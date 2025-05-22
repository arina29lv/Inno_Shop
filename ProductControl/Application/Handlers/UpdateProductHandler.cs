using AutoMapper;
using MediatR;
using ProductControl.Application.Command;
using ProductControl.Domain.Interfaces;

namespace ProductControl.Application.Handlers;

public class UpdateProductHandler : IRequestHandler<UpdateProductCommand, bool>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public UpdateProductHandler(IProductRepository productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }
    
    public async Task<bool> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetProductByIdAsync(request.Id);
        if (product == null)
            return false;

        if (request.UpdatorRole != "Admin" && product.UserId != request.UpdatorId)
            return false;

        _mapper.Map(request, product);

        await _productRepository.UpdateProductAsync(product);
        return true;
    }
}