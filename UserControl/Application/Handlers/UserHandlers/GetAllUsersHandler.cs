using AutoMapper;
using MediatR;
using UserControl.Application.DTOs.UserDTOs;
using UserControl.Application.Queries;
using UserControl.Application.Queries.UserQueries;
using UserControl.Domain.Interfaces;

namespace UserControl.Application.Handlers.UserHandlers;

public class GetAllUsersHandler : IRequestHandler<GetAllUsersQuery, IEnumerable<UserDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public GetAllUsersHandler(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }
    
    public async Task<IEnumerable<UserDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _userRepository.GetAllUsersAsync();
        return _mapper.Map<IEnumerable<UserDto>>(users);
    }
}