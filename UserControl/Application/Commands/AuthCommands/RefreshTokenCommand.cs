using MediatR;
using UserControl.Application.DTOs.AuthDTOs;

namespace UserControl.Application.Commands.AuthCommands;

public class RefreshTokenCommand : IRequest<LoginDto?>
{
    public string RefreshToken { get; set; }
}