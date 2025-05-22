using MediatR;
using UserControl.Application.Commands.UserCommands;
using UserControl.Domain.Interfaces;
using UserControl.Infrastructure.Interfaces;

namespace UserControl.Application.Handlers.UserHandlers;

public class DeactivateUserHandler : IRequestHandler<DeactivateUserCommand, bool>
{
    private readonly IUserRepository _userRepository;
    private readonly IEventBus _eventBus;

    public DeactivateUserHandler(IUserRepository userRepository, IEventBus eventBus)
    {
        _userRepository = userRepository;
        _eventBus = eventBus;
    }

    public async Task<bool> Handle(DeactivateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByIdAsync(request.UserId);
        if (user == null)
            return false;

        if (!user.IsActive)
            return true;
        
        user.IsActive = false;
        await _userRepository.UpdateUserAsync(user);
        
        _eventBus.PublishUserDeactivated(user.Id);
        return true;
    }
}