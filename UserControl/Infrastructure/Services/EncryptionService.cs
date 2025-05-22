using UserControl.Infrastructure.Interfaces;

namespace UserControl.Infrastructure.Services;

public class EncryptionService : IEncryptionService
{
    public string Hash(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public bool Verify(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }
}