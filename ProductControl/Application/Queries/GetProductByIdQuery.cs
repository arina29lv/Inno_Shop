using MediatR;
using ProductControl.Application.DTOs;

namespace ProductControl.Application.Queries;

public class GetProductByIdQuery : IRequest<ProductDto>
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string UserRole { get; set; }

    public GetProductByIdQuery(int id)
    {
        Id = id;
    }
}