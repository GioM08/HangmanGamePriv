using System.Security.Cryptography;

namespace HangmanGameBusiness.Security
{
    public static class VerificationCodeGenerator
    {
        public static string GenerateSixDigitCode()
        {
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                byte[] bytes = new byte[4];
                rng.GetBytes(bytes);

                int value = System.BitConverter.ToInt32(bytes, 0) & int.MaxValue;
                int code = value % 900000 + 100000;

                return code.ToString();
            }
        }
    }
}
