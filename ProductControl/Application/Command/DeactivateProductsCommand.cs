using MediatR;

namespace ProductControl.Application.Command;

public class DeactivateProductsCommand : IRequest
{
    public int UserId { get; set; }
}