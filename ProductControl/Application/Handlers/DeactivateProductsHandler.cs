using MediatR;
using ProductControl.Application.Command;
using ProductControl.Domain.Interfaces;

namespace ProductControl.Application.Handlers;

public class DeactivateProductsHandler : IRequestHandler<DeactivateProductsCommand>
{ 
    private readonly IProductRepository _productRepository;

    public DeactivateProductsHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<Unit> Handle(DeactivateProductsCommand request, CancellationToken cancellationToken)
    {
        var products = await _productRepository.GetProductsByUserIdAsync(request.UserId);
        foreach (var product in products)
            product.IsDeleted = true;
        
        await _productRepository.SaveChangesAsync();
        return Unit.Value;
    }
}