using Domain.Interfaces.Services;
using System.Security.Cryptography;
using System.Text;

namespace Infra.CrossCutting.Security
{
    public class EncryptionService : IEncryptionService
    {
        private const int Keysize = 128;
        private const int DerivationIterations = 1000;

        public string EncryptDynamic(string textToEncrypt, string key)
        {
            var saltStringBytes = Generate128BitsOfRandomEntropy();
            var ivStringBytes = Generate128BitsOfRandomEntropy();

            var textToEncryptBytes = Encoding.UTF8.GetBytes(textToEncrypt);

            using var password = new Rfc2898DeriveBytes(key, saltStringBytes, DerivationIterations, HashAlgorithmName.SHA256);
            var keyBytes = password.GetBytes(Keysize / 8);

            using var symmetricKey = Aes.Create();
            symmetricKey.BlockSize = Keysize;
            symmetricKey.Mode = CipherMode.CBC;
            symmetricKey.Padding = PaddingMode.PKCS7;
            using var encryptor = symmetricKey.CreateEncryptor(keyBytes, ivStringBytes);

            using var memoryStream = new MemoryStream();
            using var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
            cryptoStream.Write(textToEncryptBytes, 0, textToEncryptBytes.Length);
            cryptoStream.FlushFinalBlock();

            var encryptedTextBytes = saltStringBytes;
            encryptedTextBytes = encryptedTextBytes.Concat(ivStringBytes).ToArray();
            encryptedTextBytes = encryptedTextBytes.Concat(memoryStream.ToArray()).ToArray();

            memoryStream.Close();
            cryptoStream.Close();

            return Convert.ToBase64String(encryptedTextBytes);
        }

        public string DecryptDynamic(string textToDecrypt, string key)
        {
            var textToDecryptBytesWithSaltAndIv = Convert.FromBase64String(textToDecrypt);
            var saltStringBytes = textToDecryptBytesWithSaltAndIv.Take(Keysize / 8).ToArray();
            var ivStringBytes = textToDecryptBytesWithSaltAndIv.Skip(Keysize / 8).Take(Keysize / 8).ToArray();
            var textToDecryptBytes = textToDecryptBytesWithSaltAndIv.Skip(Keysize / 8 * 2).ToArray();

            using var password = new Rfc2898DeriveBytes(key, saltStringBytes, DerivationIterations, HashAlgorithmName.SHA256);
            var keyBytes = password.GetBytes(Keysize / 8);

            using var symmetricKey = Aes.Create();
            symmetricKey.BlockSize = Keysize;
            symmetricKey.Mode = CipherMode.CBC;
            symmetricKey.Padding = PaddingMode.PKCS7;
            using var decryptor = symmetricKey.CreateDecryptor(keyBytes, ivStringBytes);

            using var memoryStream = new MemoryStream(textToDecryptBytes);
            using var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);

            using var decryptedMemoryStream = new MemoryStream();
            cryptoStream.CopyTo(decryptedMemoryStream);

            return Encoding.UTF8.GetString(decryptedMemoryStream.ToArray());
        }

        public string EncryptDeterministic(byte[] key, byte[] byteToEncrypt)
        {
            var hash = new HMACSHA256(key);

            return Convert.ToBase64String(hash.ComputeHash(byteToEncrypt));
        }

        private static byte[] Generate128BitsOfRandomEntropy()
        {
            var randomBytes = new byte[16];

            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);

            return randomBytes;
        }
    }
}
