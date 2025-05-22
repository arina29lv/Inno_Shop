using MediatR;
using UserControl.Application.DTOs.UserDTOs;

namespace UserControl.Application.Queries.UserQueries;

public class GetAllUsersQuery : IRequest<IEnumerable<UserDto>> {}