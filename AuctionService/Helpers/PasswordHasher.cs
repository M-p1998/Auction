using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AuctionService.Helpers
{
    public class PasswordHasher
    {

        public static string HashPassword(string password)
    {
        // using var sha256 = SHA256.Create();
        // var bytes = Encoding.UTF8.GetBytes(password);
        // var hash = sha256.ComputeHash(bytes);
        // return Convert.ToBase64String(hash);
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

        internal static bool VerifyPassword(string password, string passwordHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }
    }
}