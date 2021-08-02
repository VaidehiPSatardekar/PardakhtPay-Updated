using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TotalCoding.Cartipay.Application.Interfaces;
using TotalCoding.Cartipay.Shared.Interfaces;

namespace TotalCoding.Cartipay.CoreTest
{
    [TestClass]
    public class BankBotTests
    {
        private IServiceProvider _Provider;

        [TestInitialize]
        public void Initialize()
        {
            IServiceCollection collection = new ServiceCollection();

            var configuration = CommonMethods.InitConfiguration();

            collection.SetupConfigurations(configuration);
            collection.AddDependencyInjections();
            collection.AddCacheService();

            collection.AddContext(configuration);

            _Provider = collection.BuildServiceProvider();
        }

        [TestMethod]
        public void GetBankInformations()
        {
            try
            {
                var service = _Provider.GetRequiredService<IBankBotService>();

                var task = service.GetBanks();

                task.Wait();

                var banks = task.Result;

                Assert.IsNotNull(banks);

                banks.ForEach(bank =>
                {
                    Assert.IsFalse(string.IsNullOrEmpty(bank.BankGuid));
                    Assert.IsFalse(string.IsNullOrEmpty(bank.BankName));
                });
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public void GetAccountInformations()
        {
            try
            {
                var service = _Provider.GetRequiredService<IBankBotService>();

                var task = service.GetAccountsAsync();

                task.Wait();

                var accounts = task.Result;

                Assert.IsNotNull(accounts);

                accounts.ForEach(account =>
                {
                    Assert.IsFalse(string.IsNullOrEmpty(account.AccountGuid), "Account Guid Is Null");
                    Assert.IsFalse(string.IsNullOrEmpty(account.AccountNo), "Account No Is Null");
                    Assert.IsFalse(string.IsNullOrEmpty(account.LoginGuid), "Login Guid Is Null");
                });
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public void GetLoginInformations()
        {
            try
            {
                var service = _Provider.GetRequiredService<IBankBotService>();

                var task = service.GetLogins();

                task.Wait();

                var logins = task.Result;

                Assert.IsNotNull(logins);

                logins.ForEach(login =>
                {
                    Assert.IsFalse(string.IsNullOrEmpty(login.Username), "Username Is Null");
                    Assert.IsFalse(string.IsNullOrEmpty(login.LoginGuid), "Login Guid Is Null");
                    Assert.IsFalse(login.Id == 0, "Login id is empty");
                });
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public void GetLoginSelectInformations()
        {
            try
            {
                var service = _Provider.GetRequiredService<IBankBotService>();

                var task = service.GetLoginSelect();

                task.Wait();

                var logins = task.Result;

                Assert.IsNotNull(logins);

                logins.ForEach(login =>
                {
                    //Assert.IsFalse(string.IsNullOrEmpty(login.FriendlyName), "Username Is Null");
                    Assert.IsFalse(string.IsNullOrEmpty(login.LoginGuid), "Login Guid Is Null");
                    Assert.IsFalse(string.IsNullOrEmpty(login.BankName), "Bank Name Is Null");
                    Assert.IsFalse(login.Id == 0, "Login id is empty");
                });
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public void AccountCacheTest()
        {
            var service = _Provider.GetRequiredService<IBankBotService>();

            for (int i = 0; i < 100; i++)
            {
                var task = service.GetAccountsAsync();

                task.Wait();

                Thread.Sleep(10);
            }

            Assert.AreEqual(1, service.AccountReadCount);
        }

        [TestMethod]
        public void AccountCacheMultipleTest()
        {
            var expected = 1;
            var service = _Provider.GetRequiredService<IBankBotService>();

            var tasks = new List<Task>();

            for (int i = 0; i < 10; i++)
            {
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    for (int j = 0; j < 100; j++)
                    {
                        var task = service.GetAccountsAsync();

                        task.Wait();

                        Thread.Sleep(10);
                    }
                }));
            }

            Task.WaitAll(tasks.ToArray());

            Assert.AreEqual(expected, service.AccountReadCount);
        }

        [TestMethod]
        public void BankCacheTest()
        {
            var service = _Provider.GetRequiredService<IBankBotService>();

            for (int i = 0; i < 100; i++)
            {
                var task = service.GetBanks();

                task.Wait();

                Thread.Sleep(10);
            }

            Assert.AreEqual(1, service.BankReadCount);
        }

        [TestMethod]
        public void BankCacheMultipleTest()
        {
            var expected = 1;
            var service = _Provider.GetRequiredService<IBankBotService>();

            var tasks = new List<Task>();

            for (int i = 0; i < 10; i++)
            {
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    for (int j = 0; j < 100; j++)
                    {
                        var task = service.GetBanks();

                        task.Wait();

                        Thread.Sleep(10);
                    }
                }));
            }

            Task.WaitAll(tasks.ToArray());

            Assert.AreEqual(expected, service.BankReadCount);
        }

        [TestMethod]
        public void LoginCacheTest()
        {
            var service = _Provider.GetRequiredService<IBankBotService>();

            for (int i = 0; i < 100; i++)
            {
                var task = service.GetLogins();

                task.Wait();

                Thread.Sleep(10);
            }

            Assert.AreEqual(1, service.LoginReadCount);
        }

        [TestMethod]
        public void LoginCacheMultipleTest()
        {
            var expected = 1;
            var service = _Provider.GetRequiredService<IBankBotService>();

            var tasks = new List<Task>();

            for (int i = 0; i < 10; i++)
            {
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    for (int j = 0; j < 100; j++)
                    {
                        var task = service.GetLogins();

                        task.Wait();

                        Thread.Sleep(10);
                    }
                }));
            }

            Task.WaitAll(tasks.ToArray());

            Assert.AreEqual(expected, service.LoginReadCount);
        }
    }
}
