using MediatR;

namespace UserControl.Application.Commands.UserCommands;

public class DeleteUserCommand : IRequest<bool>
{
    public int Id { get; set; }
}