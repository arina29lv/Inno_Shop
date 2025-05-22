using MediatR;

namespace UserControl.Application.Commands.AuthCommands;

public class RegisterUserCommand : IRequest<(int id, bool isRegistered)>
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string Role { get; set; }
}