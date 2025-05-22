using MediatR;

namespace UserControl.Application.Commands.AuthCommands;

public class RequestPasswordResetCommand : IRequest<bool>
{
    public string Email { get; set; }
}