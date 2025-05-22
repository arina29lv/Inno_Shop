using AutoMapper;
using MediatR;
using UserControl.Application.Commands.AuthCommands;
using UserControl.Domain.Interfaces;
using UserControl.Domain.Models;
using UserControl.Infrastructure.Interfaces;

namespace UserControl.Application.Handlers.AuthHandlers;

public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, (int id, bool isRegistered)>
{
    private readonly IUserRepository _userRepository;
    private readonly IEncryptionService _encryptionService;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;
    private readonly IMapper _mapper;

    public RegisterUserHandler(
        IUserRepository userRepository,
        IEncryptionService encryptionService,
        IEmailService emailService,
        IConfiguration configuration,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _encryptionService = encryptionService;
        _emailService = emailService;
        _configuration = configuration;
        _mapper = mapper;
    }

    public async Task<(int id, bool isRegistered)> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await _userRepository.GetUserByEmailAsync(request.Email);
        if (existingUser is not null)
            return (existingUser.Id, false);
        
        var token = Guid.NewGuid().ToString();
        
        var user = _mapper.Map<User>(request);
        user.PasswordHash = _encryptionService.Hash(request.Password);
        user.EmailConfirmationToken = token;

        var userId = await _userRepository.AddUserAsync(user);

        var baseUrl = _configuration["App:UserControlUrl"];
        var confirmationLink = $"{baseUrl}/api/auths/confirm-email?email={user.Email}&token={token}";
        await _emailService.SendConfirmationEmailAsync(user.Email, confirmationLink);

        return (userId, true);
    }
}