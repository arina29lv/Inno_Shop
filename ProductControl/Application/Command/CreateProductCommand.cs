using MediatR;

namespace ProductControl.Application.Command;

public class CreateProductCommand : IRequest<int>
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public bool IsAvailable { get; set; }
    public int? UserId { get; set; }
}