using MediatR;
using UserControl.Application.Commands.AuthCommands;
using UserControl.Domain.Interfaces;

namespace UserControl.Application.Handlers.AuthHandlers;

public class ConfirmEmailHandler : IRequestHandler<ConfirmEmailCommand, bool>
{
    private readonly IUserRepository _userRepository;

    public ConfirmEmailHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<bool> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByEmailAsync(request.Email);

        if (user == null || user.EmailConfirmationToken != request.Token)
            return false;

        user.IsEmailConfirmed = true;
        user.EmailConfirmationToken = null;

        await _userRepository.UpdateUserAsync(user);
        return true;
    }
}