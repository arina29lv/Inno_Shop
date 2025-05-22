using MediatR;
using ProductControl.Application.Command;
using ProductControl.Domain.Interfaces;

namespace ProductControl.Application.Handlers;

public class DeleteProductHandler : IRequestHandler<DeleteProductCommand, bool>
{
    private readonly IProductRepository _productRepository;

    public DeleteProductHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }
    
    public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetProductByIdAsync(request.Id);
        
        if (product == null)
            return false;
        
        if (request.UserRole != "Admin")
        {
            if (product.UserId != request.UserId)
                return false;
        }

        await _productRepository.DeleteProductAsync(product);
        return true;
    }
}