using MediatR;
using UserControl.Application.Commands.UserCommands;
using UserControl.Domain.Interfaces;

namespace UserControl.Application.Handlers.UserHandlers;

public class DeleteUserHandler : IRequestHandler<DeleteUserCommand, bool>
{
    private readonly IUserRepository _userRepository;

    public DeleteUserHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<bool> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByIdAsync(request.Id);
        if(user == null)
            return false;
        
        await _userRepository.DeleteUserAsync(user);
        return true;
    }
}