using MediatR;
using UserControl.Application.Commands.AuthCommands;
using UserControl.Application.DTOs.AuthDTOs;
using UserControl.Domain.Interfaces;
using UserControl.Infrastructure.Interfaces;

namespace UserControl.Application.Handlers.AuthHandlers;

public class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, LoginDto?>
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;

    public RefreshTokenHandler(IUserRepository userRepository, ITokenService tokenService)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
    }
    
    public async Task<LoginDto?> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByRefreshTokenAsync(request.RefreshToken);

        if (user == null)
            return null;

        var newAccessToken = _tokenService.GenerateAccessToken(user);
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        await _userRepository.UpdateUserAsync(user);

        return new LoginDto
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken
        };
    }
}