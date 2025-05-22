using MediatR;

namespace UserControl.Application.Commands.AuthCommands;

public class ConfirmEmailCommand : IRequest<bool>
{
    public string Email { get; set; }
    public string Token { get; set; }
}