using MediatR;

namespace UserControl.Application.Commands.AuthCommands;

public class ResetPasswordCommand : IRequest<bool>
{
    public string Email { get; set; }
    public string Token { get; set; }
    public string NewPassword { get; set; }
}