using MediatR;
using UserControl.Application.Commands.AuthCommands;
using UserControl.Domain.Interfaces;
using UserControl.Infrastructure.Interfaces;

namespace UserControl.Application.Handlers.AuthHandlers;

public class RequestPasswordResetHandler : IRequestHandler<RequestPasswordResetCommand, bool>
{
    private readonly IUserRepository _userRepository;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;

    public RequestPasswordResetHandler(IUserRepository userRepository, IEmailService emailService, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _emailService = emailService;
        _configuration = configuration;
    }

    public async Task<bool> Handle(RequestPasswordResetCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByEmailAsync(request.Email);
        if (user == null || !user.IsEmailConfirmed)
            return false;

        var token = Guid.NewGuid().ToString();
        user.PasswordResetToken = token;
        user.PasswordResetTokenExpiresAt = DateTime.UtcNow.AddHours(1);

        await _userRepository.UpdateUserAsync(user);

        var baseUrl = _configuration["App:UserControlUrl"];
        var resetLink = $"{baseUrl}/api/auths/reset-password?token={token}&email={user.Email}";
        await _emailService.SendPasswordResetEmailAsync(user.Email, resetLink);

        return true;
    }
}