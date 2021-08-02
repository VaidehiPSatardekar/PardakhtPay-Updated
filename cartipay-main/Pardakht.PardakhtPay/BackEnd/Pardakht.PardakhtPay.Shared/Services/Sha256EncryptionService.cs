using System;
using System.Security.Cryptography;
using System.Text;
using Pardakht.PardakhtPay.Shared.Interfaces;

namespace Pardakht.PardakhtPay.Shared.Services
{
    /// <summary>
    /// Encrypts parameters which is given
    /// </summary>
    public class Sha256EncryptionService : ISha256EncryptionService
    {
        /// <summary>
        /// UTF8 is our default encoding. If any encoding information is not given to methods, we will use this encoding
        /// </summary>
        public readonly Encoding DefaultEncoding = Encoding.UTF8;

        public Sha256EncryptionService()
        {
        }

        /// <summary>
        /// Encrypts given string input
        /// </summary>
        /// <param name="input"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public byte[] Encrypt(string input, Encoding encoding = null)
        {
            var bytes = GetBytes(input, encoding);

            return Encrypt(bytes);
        }

        /// <summary>
        /// Encrypts given byte array
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public byte[] Encrypt(byte[] input)
        {
            using (var crypt = new SHA256Managed())
            {
                return crypt.ComputeHash(input);
            }
        }

        /// <summary>
        /// Encrypts given input string and converts to base64 string
        /// </summary>
        /// <param name="input"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public string EncryptToBase64(string input, Encoding encoding = null)
        {
            return Convert.ToBase64String(Encrypt(input, encoding));
        }

        /// <summary>
        /// Encrypts given byte array and converts to base64 string
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public string EncryptToBase64(byte[] input)
        {
            return Convert.ToBase64String(Encrypt(input));
        }

        /// <summary>
        /// Encrypts the given input parameter and converts it to string using x2
        /// </summary>
        /// <param name="input"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public string EncryptToString(string input, Encoding encoding = null)
        {
            var data = GetBytes(input, encoding);

            return EncryptToString(data);
        }

        /// <summary>
        /// Encrypts this given byte array parameter and  converts it to string using x2
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public string EncryptToString(byte[] input)
        {
            var encryptedData = Encrypt(input);

            StringBuilder data = new StringBuilder();

            foreach (var b in encryptedData)
            {
                data.Append(b.ToString("x2"));
            }

            return data.ToString();
        }

        private byte[] GetBytes(string input, Encoding encoding)
        {
            encoding = encoding ?? DefaultEncoding;

            return encoding.GetBytes(input);
        }
    }
}
