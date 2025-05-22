using MediatR;

namespace ProductControl.Application.Command;

public class DeleteProductCommand : IRequest<bool>
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string UserRole { get; set; }
}