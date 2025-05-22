using MediatR;
using UserControl.Application.DTOs.UserDTOs;

namespace UserControl.Application.Queries.UserQueries;

public class GetUserByIdQuery : IRequest<UserDto>
{
    public int Id { get;}

    public GetUserByIdQuery(int id)
    {
        Id = id;
    }
}