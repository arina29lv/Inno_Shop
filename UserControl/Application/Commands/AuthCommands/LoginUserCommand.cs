using MediatR;
using UserControl.Application.DTOs.AuthDTOs;

namespace UserControl.Application.Commands.AuthCommands;

public class LoginUserCommand : IRequest<LoginDto?>
{
    public string Email { get; set; }
    public string Password { get; set; }
}