using MediatR;
using UserControl.Application.Commands.UserCommands;
using UserControl.Domain.Interfaces;
using UserControl.Infrastructure.Interfaces;

namespace UserControl.Application.Handlers.UserHandlers;

public class ActivateUserHandler : IRequestHandler<ActivateUserCommand, bool>
{
    private readonly IUserRepository _userRepository;
    private readonly IEventBus _eventBus;

    public ActivateUserHandler(IUserRepository userRepository, IEventBus eventBus)
    {
        _userRepository = userRepository;
        _eventBus = eventBus;
    }

    public async Task<bool> Handle(ActivateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByIdAsync(request.UserId);
        if (user == null)
            return false;
        
        if (user.IsActive)
            return true;
        
        user.IsActive = true;
        await _userRepository.UpdateUserAsync(user);
        
        _eventBus.PublishUserActivated(user.Id);
        return true;
    }
}