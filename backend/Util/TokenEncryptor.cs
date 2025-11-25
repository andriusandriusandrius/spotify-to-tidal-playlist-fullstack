using System.Security.Cryptography;
using System.Text;

namespace backend.Util
{
    public class TokenEncryptor
    {
        private readonly byte[] key;

        public TokenEncryptor(){
            string? keyBase64 = Environment.GetEnvironmentVariable("AES_KEY");
            if (string.IsNullOrEmpty(keyBase64))
                throw new InvalidOperationException("AES_KEY environment variable not set");

            key = Convert.FromBase64String(keyBase64);
            if (key.Length != 32)
                throw new InvalidOperationException("AES-256 key must be 32 bytes");
        }
        public  string Encrypt(string plainText)
        {
            using var aes = new AesGcm(key, tagSizeInBytes:16);
            byte[] nonce = RandomNumberGenerator.GetBytes(12);
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            byte[] encryptedDataBytes = new byte [plainTextBytes.Length];
            byte[] tag = new byte[16];
            aes.Encrypt(nonce, plainTextBytes, encryptedDataBytes,tag);

            byte[] combined = new byte[nonce.Length + tag.Length + encryptedDataBytes.Length];
            Buffer.BlockCopy(nonce,0,combined,0,nonce.Length);
            Buffer.BlockCopy(tag, 0, combined, nonce.Length, tag.Length );
            Buffer.BlockCopy(encryptedDataBytes, 0, combined, nonce.Length+tag.Length, encryptedDataBytes.Length );
             
            return Convert.ToBase64String(combined);
        }
        public string Decrypt(string encriptyedDataString)
        {
            byte[] combined = Convert.FromBase64String(encriptyedDataString);
            byte[] nonce = new byte[12];
            byte[] tag = new byte[16];
            byte[] cipherBytes = new byte[combined.Length - nonce.Length - tag.Length];

            Buffer.BlockCopy(combined, 0, nonce, 0, nonce.Length);
            Buffer.BlockCopy(combined, nonce.Length, tag, 0, tag.Length);
            Buffer.BlockCopy(combined, nonce.Length + tag.Length, cipherBytes, 0, cipherBytes.Length);

            using var aes = new AesGcm(key, tagSizeInBytes:16);
            byte[] decrypted = new byte[cipherBytes.Length];
            aes.Decrypt(nonce, cipherBytes, tag, decrypted);

            return Encoding.UTF8.GetString(decrypted);
        }
    }
}