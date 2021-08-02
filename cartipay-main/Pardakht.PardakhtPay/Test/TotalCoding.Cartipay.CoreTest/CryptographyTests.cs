using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using TotalCoding.Cartipay.Shared.Interfaces;
using TotalCoding.Cartipay.Shared.Models.Configuration;
using TotalCoding.Cartipay.Shared.Services;

namespace TotalCoding.Cartipay.CoreTest
{
    [TestClass]
    public class CryptographyTests
    {
        private IServiceProvider _Provider;

        [TestInitialize]
        public void Initialize()
        {
            IServiceCollection collection = new ServiceCollection();

            var configuration = InitConfiguration();

            collection.Configure<CacheConfiguration>(configuration.GetSection(nameof(CacheConfiguration)));
            collection.Configure<AesEncryptionSettings>(configuration.GetSection(nameof(AesEncryptionSettings)));

            collection.AddScoped(typeof(ICacheService), typeof(MemoryCacheService));
            collection.AddScoped(typeof(IAesEncryptionService), typeof(AesEncryptionService));

            _Provider = collection.BuildServiceProvider();
        }

        [TestMethod]
        public void TestEncyptAndDescriptWithAes256()
        {

            var settings = _Provider.GetRequiredService<IOptions<AesEncryptionSettings>>();

            var plainText = "Total Coding";

            var decryptedText = string.Empty;

            var key = settings.Value.KeyArray;// Convert.FromBase64String("Q/JzK2BvnXy+c0/pNZG67ZF0HG4LnSRnw0D/7oU0KMA=");
            var iv = settings.Value.IVArray;// Convert.FromBase64String("Y7U6YhwvileZY3+EY85Ydg==");

            using (AesManaged aes = new AesManaged())
            {
                //aes.GenerateKey();

                //aes.GenerateIV();

                //var key = Convert.ToBase64String(aes.Key);
                //var iv = Convert.ToBase64String(aes.IV);

                aes.Key = key;
                aes.IV = iv;

                byte[] encrypted = new byte[0];

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }

                var base64 = Convert.ToBase64String(encrypted);

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(encrypted))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            decryptedText = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            Assert.AreEqual(plainText, decryptedText);
        }

        [TestMethod]
        public void TestEncryptionWithAesService()
        {
            var service = _Provider.GetRequiredService<IAesEncryptionService>();

            var decryptedData = service.DecryptToString("tsyUpdmz3OAf3h7+/ZJ/Nqca2zWGCIQpxUVl1uIkEZcWUHBYb5sGtTziOtHGx1dsENsMtX9J7DnTA5EUcwveVQ==");
        }

        [TestMethod]
        public void Decrypt()
        {
            var service = _Provider.GetRequiredService<IAesEncryptionService>();

            string plainText = "totalcoding";

            var encryptedData = service.Encrypt(plainText);

            var decryptedData = service.DecryptToString(encryptedData);

            Assert.AreEqual(plainText, decryptedData);
        }

        [TestMethod]
        public void SpeedTest()
        {
            int count = 100000;

            int excepted = 1000;

            var service = _Provider.GetRequiredService<IAesEncryptionService>();

            string plainText = "totalcoding";

            Stopwatch watch = new Stopwatch();
            watch.Start();

            for (int i = 0; i < count; i++)
            {
                var encryptedData = service.Encrypt(plainText);

                var decryptedData = service.DecryptToString(encryptedData);
            }

            watch.Stop();

            Debug.WriteLine(watch.ElapsedMilliseconds);

            if(watch.ElapsedMilliseconds > excepted)
            {
                Assert.Fail(string.Format("Takes too much time : {0}", watch.ElapsedMilliseconds));
            }
        }

        public static IConfiguration InitConfiguration()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                              .AddJsonFile($"appsettings.{Environment.MachineName}.json", optional: true, reloadOnChange: true)
                .Build();
            return config;
        }
    }
}
