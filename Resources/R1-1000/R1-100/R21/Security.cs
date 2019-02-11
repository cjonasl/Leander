using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Leander.Nr1
{
    public partial class Utility
    {

        /// <summary>
        /// From: https://www.ktlsolutions.com/how-tos/effective-password-hashing/
        /// </summary>
        public static void GenerateSaltedHash(string password, out string hash, out string salt)
        {
            var saltBytes = new byte[64];
            var provider = new RNGCryptoServiceProvider();
            provider.GetNonZeroBytes(saltBytes);
            salt = Convert.ToBase64String(saltBytes);

            var rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, saltBytes, 10000);
            hash = Convert.ToBase64String(rfc2898DeriveBytes.GetBytes(256));
        }

        /// <summary>
        /// From: https://www.ktlsolutions.com/how-tos/effective-password-hashing/
        /// </summary>
        public static bool VerifyPassword(string enteredPassword, string storedHash, string storedSalt)
        {
            var saltBytes = Convert.FromBase64String(storedSalt);
            var rfc2898DeriveBytes = new Rfc2898DeriveBytes(enteredPassword, saltBytes, 10000);
            return Convert.ToBase64String(rfc2898DeriveBytes.GetBytes(256)) == storedHash;
        }
    }
}
