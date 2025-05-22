using MediatR;

namespace UserControl.Application.Commands.UserCommands;

public class UpdateUserCommand : IRequest<bool>
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
}