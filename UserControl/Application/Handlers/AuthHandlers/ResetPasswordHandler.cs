using MediatR;
using UserControl.Application.Commands.AuthCommands;
using UserControl.Domain.Interfaces;
using UserControl.Infrastructure.Interfaces;

namespace UserControl.Application.Handlers.AuthHandlers;

public class ResetPasswordHandler : IRequestHandler<ResetPasswordCommand, bool>
{
    private readonly IUserRepository _userRepository;
    private readonly IEncryptionService _encryptionService;

    public ResetPasswordHandler(IUserRepository userRepository, IEncryptionService encryptionService)
    {
        _userRepository = userRepository;
        _encryptionService = encryptionService;
    }

    public async Task<bool> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByEmailAsync(request.Email);
        if (user == null || user.PasswordResetToken != request.Token || user.PasswordResetTokenExpiresAt < DateTime.UtcNow)
            return false;

        user.PasswordHash = _encryptionService.Hash(request.NewPassword);
        user.PasswordResetToken = null;
        user.PasswordResetTokenExpiresAt = null;

        await _userRepository.UpdateUserAsync(user);
        return true;
    }
}