using AutoMapper;
using MediatR;
using UserControl.Application.DTOs.UserDTOs;
using UserControl.Application.Queries;
using UserControl.Application.Queries.UserQueries;
using UserControl.Domain.Interfaces;

namespace UserControl.Application.Handlers.UserHandlers;

public class GetUserByIdHandler : IRequestHandler<GetUserByIdQuery, UserDto?>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public GetUserByIdHandler(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }
    
    public async Task<UserDto?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByIdAsync(request.Id);
        if (user == null)
            return null;
        
        return _mapper.Map<UserDto>(user);
    }
}