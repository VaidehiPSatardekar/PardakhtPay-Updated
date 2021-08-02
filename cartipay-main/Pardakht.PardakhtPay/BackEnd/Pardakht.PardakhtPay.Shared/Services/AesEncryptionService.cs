using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Pardakht.PardakhtPay.Shared.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Configuration;
using Pardakht.PardakhtPay.Shared.Models.Models;

namespace Pardakht.PardakhtPay.Shared.Services
{
    public class AesEncryptionService : IAesEncryptionService
    {
        AesEncryptionSettings _Settings = null;
        IReflectionService _ReflectionService = null;

        public AesEncryptionService(IOptions<AesEncryptionSettings> options,
            IReflectionService reflectionService)
        {
            _Settings = options.Value;
            _ReflectionService = reflectionService;
        }

        public byte[] Encrypt(byte[] data)
        {
            using (AesManaged aes = new AesManaged())
            {

                aes.Key = _Settings.KeyArray;
                aes.IV = _Settings.IVArray;

                byte[] encrypted = new byte[0];

                using (ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                {
                    using (MemoryStream msEncrypt = new MemoryStream())
                    {
                        using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        {
                            csEncrypt.Write(data);
                        }
                        return msEncrypt.ToArray();
                    }
                }
            }
        }

        public string EncryptToBase64(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
            {
                return string.Empty;
            }
            var encrptedData = Encrypt(plainText);

            return Convert.ToBase64String(encrptedData);
        }

        public string EncryptToBase64(byte[] data)
        {
            if(data == null || data.Length == 0)
            {
                return string.Empty;
            }
            var encryptedData = Encrypt(data);

            return Convert.ToBase64String(encryptedData);
        }

        public byte[] Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
            {
                return new byte[0];
            }

            var data = Encoding.UTF8.GetBytes(plainText);

            var encryptedData = Encrypt(data);

            return encryptedData;
        }

        public byte[] Decrypt(string encryptedText)
        {
            var data = Convert.FromBase64String(encryptedText);

            return Decrypt(data);
        }

        public byte[] Decrypt(byte[] data)
        {
            using (AesManaged aes = new AesManaged())
            {
                aes.Key = _Settings.KeyArray;
                aes.IV = _Settings.IVArray;

                using (ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                {
                    using (MemoryStream msDecrypt = new MemoryStream(data))
                    {
                        using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (MemoryStream ms = new MemoryStream())
                            {
                                csDecrypt.CopyTo(ms);

                                return ms.ToArray();
                            }
                        }
                    }
                }
            }
        }

        public string DecryptToString(string encryptedText)
        {
            if (string.IsNullOrEmpty(encryptedText))
            {
                return encryptedText;
            }
            var decryptedData = Decrypt(encryptedText);

            return Encoding.UTF8.GetString(decryptedData);
        }

        public string DecryptToString(byte[] data)
        {
            var decryptedData = Decrypt(data);

            return Encoding.UTF8.GetString(decryptedData);
        }

        public string Encrypt<T>(T item)
        {
            Dictionary<string, string> values = new Dictionary<string, string>();

            var properties = _ReflectionService.GetProperties<T>();

            properties.ForEach(property =>
            {
                var encrypt = property.Attributes.Exists(p => p is EncryptAttribute);

                var value = property.Info.GetValue(item);

                if (value == null)
                {
                    values[property.Info.Name] = string.Empty;
                }

                else
                {
                    if (encrypt)
                    {
                        values[property.Info.Name] = EncryptToBase64(JsonConvert.SerializeObject(value));
                    }
                    else
                    {
                        values[property.Info.Name] = JsonConvert.SerializeObject(value);
                    }
                }
            });

            var text = JsonConvert.SerializeObject(values);

            return text;
        }

        public T Decrypt<T>(string text) where T : new()
        {
            T t = new T();

            Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(text);

            var properties = _ReflectionService.GetProperties<T>();

            properties.ForEach(property =>
            {
                var encrypt = property.Attributes.Exists(p => p is EncryptAttribute);

                if (values.ContainsKey(property.Info.Name))
                {
                    var value = values[property.Info.Name];

                    if (encrypt)
                    {
                        value = DecryptToString(value);
                    }

                    property.Info.SetValue(t, JsonConvert.DeserializeObject(value, property.Info.PropertyType));
                }
            });

            return t;
        }
    }
}
