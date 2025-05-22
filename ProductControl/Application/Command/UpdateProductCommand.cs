using MediatR;

namespace ProductControl.Application.Command;

public class UpdateProductCommand : IRequest<bool>
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public bool IsAvailable { get; set; }
    public int? UserId { get; set; }
    
    public int UpdatorId { get; set; }
    public string UpdatorRole { get; set; }
}