using MediatR;

namespace UserControl.Application.Commands.UserCommands;

public class ActivateUserCommand : IRequest<bool>
{
    public int UserId { get; set; }
}