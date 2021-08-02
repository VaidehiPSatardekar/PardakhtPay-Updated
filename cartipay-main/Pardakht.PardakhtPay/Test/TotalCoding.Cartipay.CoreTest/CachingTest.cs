using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TotalCoding.Cartipay.Shared.Interfaces;
using TotalCoding.Cartipay.Shared.Models.Configuration;
using TotalCoding.Cartipay.Shared.Services;

namespace TotalCoding.Cartipay.CoreTest
{
    [TestClass]

    public class CachingTest
    {
        private IServiceProvider _Provider;
        const string CacheKey = "K1";
        const string CacheValue = "K2";

        [TestInitialize]
        public void Start()
        {
            IServiceCollection collection = new ServiceCollection();

            var configuration = InitConfiguration();

            collection.Configure<CacheConfiguration>(configuration.GetSection(nameof(CacheConfiguration)));

            collection.AddScoped(typeof(ICacheService), typeof(MemoryCacheService));

            _Provider = collection.BuildServiceProvider();

            var cacheService = _Provider.GetRequiredService<ICacheService>();

            cacheService.Set(CacheKey, CacheValue);
        }

        [TestMethod]
        public void CheckCacheValue()
        {
            var cacheService = _Provider.GetRequiredService<ICacheService>();

            var value = cacheService.Get<string>(CacheKey);

            Assert.AreEqual(CacheValue, value);
        }

        public static IConfiguration InitConfiguration()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                              .AddJsonFile($"appsettings.{Environment.MachineName}.json", optional: true, reloadOnChange: true)
                .Build();
            return config;
        }

        [TestMethod]
        public async Task HttpClientTest()
        {
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync("https://www.google.com");
            }
        }
    }
}
