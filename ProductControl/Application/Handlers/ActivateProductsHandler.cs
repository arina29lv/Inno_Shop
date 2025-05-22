using MediatR;
using ProductControl.Application.Command;
using ProductControl.Domain.Interfaces;

namespace ProductControl.Application.Handlers;

public class ActivateProductsHandler : IRequestHandler<ActivateProductsCommand>
{
    private readonly IProductRepository _productRepository;

    public ActivateProductsHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<Unit> Handle(ActivateProductsCommand request, CancellationToken cancellationToken)
    {
        var products = await _productRepository.GetProductsByUserIdAsync(request.UserId);
        foreach (var product in products)
            product.IsDeleted = false;

        await _productRepository.SaveChangesAsync();
        return Unit.Value;
    }
}