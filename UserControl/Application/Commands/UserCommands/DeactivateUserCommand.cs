using MediatR;

namespace UserControl.Application.Commands.UserCommands;

public class DeactivateUserCommand : IRequest<bool>
{
    public int UserId { get; set; }
}