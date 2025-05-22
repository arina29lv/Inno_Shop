using MediatR;

namespace ProductControl.Application.Command;

public class ActivateProductsCommand : IRequest
{
    public int UserId { get; set; }
}