using System.Security.Cryptography;
using System.Text;

namespace ProjectFlow_FNS.Helpers
{
    /// <summary>
    /// Вспомогательный класс для хэширования паролей (SHA-256).
    /// В production-среде рекомендуется использовать BCrypt.
    /// </summary>
    public static class PasswordHasher
    {
        public static string Hash(string input)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder sb = new StringBuilder();
                foreach (byte b in bytes)
                    sb.Append(b.ToString("X2"));
                return sb.ToString();
            }
        }

        public static bool Verify(string input, string storedHash)
        {
            return Hash(input) == storedHash;
        }
    }
}