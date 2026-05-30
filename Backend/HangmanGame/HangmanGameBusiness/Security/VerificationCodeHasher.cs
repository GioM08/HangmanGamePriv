using System;
using System.Security.Cryptography;
using System.Text;

namespace HangmanGameBusiness.Security
{
    public static class VerificationCodeHasher
    {
        public static string HashCode(string code)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(code);
                byte[] hashBytes = sha256.ComputeHash(bytes);

                return Convert.ToBase64String(hashBytes);
            }
        }

        public static bool VerifyCode(string code, string storedCodeHash)
        {
            string codeHash = HashCode(code);
            return codeHash == storedCodeHash;
        }
    }
}
