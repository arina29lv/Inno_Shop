using MediatR;
using ProductControl.Application.DTOs;

namespace ProductControl.Application.Queries;

public class GetProductsByUserIdQuery : IRequest<IEnumerable<ProductDto>>
{
    public int UserId { get; set; }

    public GetProductsByUserIdQuery(int userId)
    {
        UserId = userId;
    }
}