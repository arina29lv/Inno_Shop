namespace UserControl.Infrastructure.Interfaces;

public interface IEncryptionService
{
    string Hash(string password);
    bool Verify(string password, string hash);
}