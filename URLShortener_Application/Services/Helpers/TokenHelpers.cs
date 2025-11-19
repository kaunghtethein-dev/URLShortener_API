using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace URLShortener_Application.Services.Helpers
{
    public static class TokenHelpers
    {
        public static string GenerateRandomToken(int size = 64)
        {
            var randomBytes = new byte[size];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }

            return Convert.ToBase64String(randomBytes);
        }

        public static string ComputeSha256Hash(string raw)
        {
            using (var sha = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(raw);
                var hash = sha.ComputeHash(bytes);
                return Convert.ToHexString(hash);
            }
        }
    }
}
