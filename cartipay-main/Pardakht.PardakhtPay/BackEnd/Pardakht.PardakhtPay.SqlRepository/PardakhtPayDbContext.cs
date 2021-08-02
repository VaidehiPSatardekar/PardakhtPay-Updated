using Microsoft.EntityFrameworkCore;
using System;
using Pardakht.PardakhtPay.Shared.Models.Entities;

namespace Pardakht.PardakhtPay.SqlRepository
{
    /// <summary>
    /// Database context of PardakhtPay Payments
    /// </summary>
    public class PardakhtPayDbContext : DbContext
    {
        public PardakhtPayDbContext(string connectionString) : base(GetOptions(connectionString)) { }
        public PardakhtPayDbContext(DbContextOptions<PardakhtPayDbContext> options) : base(options)
        {

        }

        private static DbContextOptions GetOptions(string connectionString)
        {
            var dbOptionsBuilder = new DbContextOptionsBuilder<PardakhtPayDbContext>();
            dbOptionsBuilder.UseSqlServer(connectionString, builder => {
                builder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(30), null);
            });

            return dbOptionsBuilder.Options;
        }

        public DbSet<Merchant> Merchants { get; set; }

        public DbSet<Transaction> Transactions { get; set; }

        public DbSet<TransactionQueryHistory> TransactionQueryHistories { get; set; }

        public DbSet<Withdrawal> Withdrawals { get; set; }

        public DbSet<WithdrawalQueryHistory> WithdrawalQueryHistories { get; set; }

        public DbSet<TransferAccount> TransferAccounts { get; set; }

        public DbSet<CardToCardAccount> CardToCardAccounts { get; set; }

        public DbSet<OwnerBankLogin> OwnerBankLogins { get; set; }

        public DbSet<BankStatementItem> BankStatementItems { get; set; }

        public DbSet<TenantApi> TenantsApis { get; set; }

        public DbSet<AutoTransfer> AutoTransfers { get; set; }

        public DbSet<MerchantCustomer> MerchantCustomers { get; set; }

        public DbSet<CardToCardAccountGroup> CardToCardAccountGroups { get; set; }

        public DbSet<CardToCardAccountGroupItem> CardToCardAccountGroupItems { get; set; }

        public DbSet<UserSegmentGroup> UserSegmentGroups { get; set; }

        public DbSet<UserSegment> UserSegments { get; set; }

        public DbSet<UserSegmentGroupHistory> UserSegmentGroupHistories { get; set; }

        public DbSet<CardToCardUserSegmentRelation> CardToCardUserSegmentRelations { get; set; }

        public DbSet<ApplicationSetting> ApplicationSettings { get; set; }

        public DbSet<RiskyKeyword> RiskyKeywords { get; set; }

        public DbSet<DeviceMerchantCustomerRelation> DeviceMerchantCustomerRelations { get; set; }

        public DbSet<ManualTransfer> ManualTransfers { get; set; }

        public DbSet<ManualTransferDetail> ManualTransferDetails { get; set; }

        public DbSet<ManualTransferSourceCardDetails> ManualTransferSourceCardDetails { get; set; }

        public DbSet<MobileTransferDevice> MobileTransferDevices { get; set; }

        public DbSet<MobileTransferCardAccount> MobileTransferCardAccounts { get; set; }

        public DbSet<MobileTransferCardAccountGroup> MobileTransferCardAccountGroups { get; set; }

        public DbSet<MobileTransferCardAccountGroupItem> MobileTransferCardAccountGroupItems { get; set; }

        public DbSet<MobileCardAccountUserSegmentRelation> MobileCardAccountUserSegmentRelations { get; set; }

        public DbSet<TransactionWithdrawalRelation> TransactionWithdrawalRelations { get; set; }

        public DbSet<TransactionWithdrawalHistory> TransactionWithdrawalHistories { get; set; }

        public DbSet<BlockedPhoneNumber> BlockedPhoneNumbers { get; set; }

        public DbSet<WithdrawalTransferHistory> WithdrawalTransferHistories { get; set; }

        public DbSet<CardHolderName> CardHolderNames { get; set; }

        public DbSet<SekehDevice> SekehDevices { get; set; }

        public DbSet<UnsupportedBin> UnsupportedBins { get; set; }

        public DbSet<Invoice> Invoices { get; set; }

        public DbSet<InvoiceDetail> InvoiceDetails { get; set; }

        public DbSet<InvoiceOwnerSetting> InvoiceOwnerSettings { get; set; }

        public DbSet<InvoicePayment> InvoicePayments { get; set; }

        public DbSet<SesDevice> SesDevices { get; set; }

        public DbSet<TransactionRequestContent> TransactionRequestContents { get; set; }

        public DbSet<WithdrawalRequestContent> WithdrawalRequestContents { get; set; }

        public DbSet<OwnerSetting> OwnerSettings { get; set; }

        public DbSet<SadadPspDevice> SadadPspDevices { get; set; }

        public DbSet<MydigiDevice> MydigiDevices { get; set; }

        public DbSet<IZMobileDevice> IZMobileDevices { get; set; }

        public DbSet<BlockedCardNumber> BlockedCardNumbers { get; set; }

        public DbSet<Payment780Device> Payment780Devices { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<Merchant>().HasIndex(p => p.Title).IsUnique().HasName("UIX_MerchantTitle");
            modelBuilder.Entity<Merchant>().HasIndex(p => p.ApiKey).IsUnique().HasName("UIX_Merchant_ApiKey");

            modelBuilder.Entity<CardToCardAccount>().HasIndex(p => p.CardNumber).IsUnique().HasName("UIX_CardToCardAccount_CardNumber");

            modelBuilder.Entity<BankStatementItem>().HasIndex(p => p.AccountGuid).IsUnique(false).HasName("IX_BankStatementItem_AccountGuid");

            modelBuilder.Entity<Transaction>().HasIndex(p => p.TenantGuid).IsUnique(false).HasName("IX_Transaction_TenantGuid");

            modelBuilder.Entity<Transaction>().HasIndex(p => p.Token).IsUnique(true).HasName("UIX_Transaction_Token");

            modelBuilder.Entity<BankStatementItem>().HasIndex(p => new { p.AccountGuid, p.TransactionDateTime }).IsUnique(false).HasName("IX_BankStatement_AccountGuidTransactionDate");

            modelBuilder.Entity<Transaction>().Property(p => p.PaymentType).HasDefaultValue((int)PaymentType.CardToCard);

            modelBuilder.Entity<TransactionWithdrawalRelation>().HasIndex(p => p.WithdrawalId).IsUnique(true).HasName("UIX_TransactionWithdrawalRelation_WithdrawalId");

            modelBuilder.Entity<Withdrawal>().Property(p => p.WithdrawalProcessType).HasDefaultValue((int)WithdrawalProcessType.Transfer);

            modelBuilder.Entity<BlockedPhoneNumber>().HasIndex(p => p.PhoneNumber).IsUnique(true).HasName("UIX_BlockedPhoneNumber_PhoneNumber");

            modelBuilder.Entity<CardHolderName>().HasIndex(p => p.RecordId).IsUnique(false).HasName("UIX_CardHolderName_RecordId");

            modelBuilder.Entity<CardHolderName>().HasIndex(p => p.CardNumber).HasName("IX_CardHolderName_CardNumber");

            modelBuilder.Entity<OwnerSetting>().Property(p => p.WaitAmountForCurrentWithdrawal).HasDefaultValue(true);

            modelBuilder.Entity<OwnerSetting>().HasIndex(p => p.OwnerGuid).HasName("UIX_OwnerSettingGuid").IsUnique(true);

            modelBuilder.Entity<Payment780Device>().HasIndex(p => p.MerchantCustomerId).HasName("IX_Payment780DeviceMerchantCustomerId");

            modelBuilder.Entity<Payment780Device>().HasIndex(p => p.PhoneNumber).HasName("IX_Payment780DevicePhoneNumber");
        }
    }
}
