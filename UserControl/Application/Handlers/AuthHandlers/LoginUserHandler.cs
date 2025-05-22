using MediatR;
using UserControl.Application.Commands.AuthCommands;
using UserControl.Application.DTOs.AuthDTOs;
using UserControl.Domain.Interfaces;
using UserControl.Infrastructure.Interfaces;

namespace UserControl.Application.Handlers.AuthHandlers;

public class LoginUserHandler : IRequestHandler<LoginUserCommand, LoginDto?>
{
    private readonly ITokenService _tokenService;
    private readonly IEncryptionService _encryptionService;
    private readonly IUserRepository _userRepository;

    public LoginUserHandler(ITokenService tokenService, IUserRepository userRepository, IEncryptionService encryptionService)
    {
        _tokenService = tokenService;
        _userRepository = userRepository;
        _encryptionService = encryptionService;
    }

    public async Task<LoginDto?> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByEmailAsync(request.Email);
        if (user == null || !_encryptionService.Verify(request.Password, user.PasswordHash) || !user.IsEmailConfirmed)
            return null;
        
        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken();
        
        user.RefreshToken = refreshToken;
        await _userRepository.UpdateUserAsync(user);

        return new LoginDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
        };
    }
}