namespace Domain.Interfaces.Services
{
    public interface IEncryptionService
    {
        string EncryptDynamic(string textToEncrypt, string key);
        string DecryptDynamic(string textToDecrypt, string key);
        string EncryptDeterministic(byte[] key, byte[] textToDecrypt);
    }
}
