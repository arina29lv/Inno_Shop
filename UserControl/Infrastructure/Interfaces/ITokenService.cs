using UserControl.Domain.Models;

namespace UserControl.Infrastructure.Interfaces;

public interface ITokenService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
}
