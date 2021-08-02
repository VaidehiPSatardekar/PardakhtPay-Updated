using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Pardakht.PardakhtPay.SqlRepository.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApplicationSettings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Key = table.Column<string>(nullable: true),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AutoTransfers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantGuid = table.Column<string>(maxLength: 70, nullable: true),
                    OwnerGuid = table.Column<string>(maxLength: 70, nullable: true),
                    Amount = table.Column<decimal>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    StatusDescription = table.Column<string>(nullable: true),
                    RequestId = table.Column<int>(nullable: false),
                    RequestGuid = table.Column<string>(nullable: true),
                    CardToCardAccountId = table.Column<int>(nullable: false),
                    AccountGuid = table.Column<string>(maxLength: 70, nullable: true),
                    TransferFromAccount = table.Column<string>(nullable: true),
                    TransferToAccount = table.Column<string>(nullable: true),
                    Priority = table.Column<int>(nullable: false),
                    TransferRequestDate = table.Column<DateTime>(nullable: false),
                    TransferredDate = table.Column<DateTime>(nullable: true),
                    CancelDate = table.Column<DateTime>(nullable: true),
                    IsCancelled = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AutoTransfers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BankStatementItems",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RecordId = table.Column<int>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    AccountId = table.Column<int>(nullable: false),
                    AccountGuid = table.Column<string>(nullable: false),
                    LoginId = table.Column<int>(nullable: false),
                    LoginGuid = table.Column<string>(maxLength: 50, nullable: false),
                    TransactionNo = table.Column<string>(maxLength: 50, nullable: true),
                    CheckNo = table.Column<string>(maxLength: 50, nullable: true),
                    TransactionDateTime = table.Column<DateTime>(nullable: false),
                    Debit = table.Column<decimal>(nullable: false),
                    Credit = table.Column<decimal>(nullable: false),
                    Balance = table.Column<decimal>(nullable: false),
                    Description = table.Column<string>(maxLength: 500, nullable: true),
                    InsertDateTime = table.Column<DateTime>(nullable: false),
                    ConfirmedTransactionId = table.Column<int>(nullable: false),
                    Notes = table.Column<string>(maxLength: 100, nullable: true),
                    WithdrawalId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankStatementItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BlockedCardNumbers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CardNumber = table.Column<string>(maxLength: 20, nullable: false),
                    BlockedDate = table.Column<DateTime>(nullable: false),
                    InsertUserId = table.Column<string>(maxLength: 70, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlockedCardNumbers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BlockedPhoneNumbers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PhoneNumber = table.Column<string>(maxLength: 20, nullable: true),
                    BlockedDate = table.Column<DateTime>(nullable: false),
                    InsertUserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlockedPhoneNumbers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CardHolderNames",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RecordId = table.Column<int>(nullable: false),
                    CardNumber = table.Column<string>(maxLength: 200, nullable: true),
                    Name = table.Column<string>(maxLength: 500, nullable: true),
                    CreateDate = table.Column<DateTime>(nullable: false),
                    UpdateDate = table.Column<DateTime>(nullable: true),
                    Type = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    CallbackUrl = table.Column<string>(nullable: true),
                    AccountContext = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardHolderNames", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CardToCardAccountGroupItems",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CardToCardAccountGroupId = table.Column<int>(nullable: false),
                    CardToCardAccountId = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    LoginType = table.Column<int>(nullable: false),
                    AllowCardToCard = table.Column<bool>(nullable: false),
                    AllowWithdrawal = table.Column<bool>(nullable: false),
                    HideCardNumber = table.Column<bool>(nullable: false),
                    BlockedDate = table.Column<DateTime>(nullable: true),
                    PausedDate = table.Column<DateTime>(nullable: true),
                    TempGroupItemId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardToCardAccountGroupItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CardToCardAccountGroups",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    TenantGuid = table.Column<string>(nullable: true),
                    OwnerGuid = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardToCardAccountGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CardToCardAccounts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoginGuid = table.Column<string>(nullable: false),
                    AccountGuid = table.Column<string>(nullable: false),
                    CardNumber = table.Column<string>(nullable: true),
                    CardHolderName = table.Column<string>(nullable: false),
                    SafeAccountNumber = table.Column<string>(nullable: true),
                    TransferThreshold = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    IsTransferThresholdActive = table.Column<bool>(nullable: false),
                    TransferThresholdLimit = table.Column<decimal>(type: "decimal(18, 2)", nullable: true),
                    TenantGuid = table.Column<string>(nullable: true),
                    OwnerGuid = table.Column<string>(nullable: true),
                    LoginType = table.Column<int>(nullable: false),
                    SwitchOnLimit = table.Column<bool>(nullable: false),
                    SwitchIfHasReserveAccount = table.Column<bool>(nullable: false),
                    SwitchLimitAmount = table.Column<decimal>(type: "decimal(18, 2)", nullable: true),
                    SwitchCreditDailyLimit = table.Column<decimal>(type: "decimal(18, 2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardToCardAccounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CardToCardUserSegmentRelations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CardToCardAccountGroupItemId = table.Column<int>(nullable: false),
                    UserSegmentGroupId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardToCardUserSegmentRelations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DeviceMerchantCustomerRelations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DeviceKey = table.Column<string>(maxLength: 200, nullable: false),
                    MerchantCustomerId = table.Column<int>(nullable: false),
                    CreateDate = table.Column<DateTime>(nullable: false),
                    TransactionId = table.Column<int>(nullable: false),
                    TenantGuid = table.Column<string>(maxLength: 200, nullable: false),
                    OwnerGuid = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceMerchantCustomerRelations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InvoiceDetails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreateDate = table.Column<DateTime>(nullable: false),
                    InvoiceId = table.Column<int>(nullable: false),
                    MerchantId = table.Column<int>(nullable: false),
                    ItemTypeId = table.Column<int>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: false),
                    TotalCount = table.Column<int>(nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    Rate = table.Column<decimal>(type: "decimal(18, 4)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18, 2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceDetails", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InvoiceOwnerSettings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsActive = table.Column<bool>(nullable: false),
                    OwnerGuid = table.Column<string>(maxLength: 70, nullable: true),
                    StartDate = table.Column<DateTime>(nullable: false),
                    InvoicePeriod = table.Column<int>(nullable: false),
                    PardakhtPayDepositRate = table.Column<decimal>(type: "decimal(18, 4)", nullable: false),
                    PardakhtPalDepositRate = table.Column<decimal>(type: "decimal(18, 4)", nullable: false),
                    PardakhtPalWithdrawalRate = table.Column<decimal>(type: "decimal(18, 4)", nullable: false),
                    WithdrawalRate = table.Column<decimal>(type: "decimal(18, 4)", nullable: false),
                    TenantGuid = table.Column<string>(maxLength: 70, nullable: true),
                    CreateDate = table.Column<DateTime>(nullable: false),
                    UpdateDate = table.Column<DateTime>(nullable: true),
                    CreateUserId = table.Column<string>(nullable: true),
                    UpdateUserId = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceOwnerSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InvoicePayments",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OwnerGuid = table.Column<string>(maxLength: 70, nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18, 4)", nullable: false),
                    PaymentDate = table.Column<DateTime>(nullable: false),
                    PaymentReference = table.Column<string>(maxLength: 100, nullable: true),
                    CreateDate = table.Column<DateTime>(nullable: false),
                    CreateUserId = table.Column<string>(maxLength: 70, nullable: true),
                    UpdateDate = table.Column<DateTime>(nullable: true),
                    UpdateUserId = table.Column<string>(maxLength: 70, nullable: true),
                    TenantGuid = table.Column<string>(maxLength: 70, nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoicePayments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Invoices",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreateDate = table.Column<DateTime>(nullable: false),
                    DueDate = table.Column<DateTime>(nullable: false),
                    OwnerGuid = table.Column<string>(maxLength: 70, nullable: true),
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    TenantGuid = table.Column<string>(maxLength: 70, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IZMobileDevices",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MerchantCustomerId = table.Column<int>(nullable: false),
                    ExternalId = table.Column<int>(nullable: false),
                    PhoneNumber = table.Column<string>(nullable: true),
                    IsRegistered = table.Column<bool>(nullable: false),
                    TryCount = table.Column<int>(nullable: false),
                    RegistrationDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IZMobileDevices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ManualTransferDetails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ManualTransferId = table.Column<int>(nullable: false),
                    CreateDate = table.Column<DateTime>(nullable: false),
                    UpdateDate = table.Column<DateTime>(nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    TransferGuid = table.Column<string>(nullable: true),
                    TransferId = table.Column<int>(nullable: true),
                    TransferNotes = table.Column<string>(nullable: true),
                    TrackingNumber = table.Column<string>(nullable: true),
                    TransferStatus = table.Column<int>(nullable: false),
                    TransferDate = table.Column<DateTime>(nullable: true),
                    TransferRequestDate = table.Column<DateTime>(nullable: true),
                    BankStatementId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ManualTransferDetails", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ManualTransfers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    UpdateDate = table.Column<DateTime>(nullable: true),
                    TenantGuid = table.Column<string>(maxLength: 70, nullable: true),
                    OwnerGuid = table.Column<string>(maxLength: 70, nullable: true),
                    CardToCardAccountId = table.Column<int>(nullable: false),
                    AccountGuid = table.Column<string>(maxLength: 4000, nullable: true),
                    TransferType = table.Column<int>(nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    TransferAccountId = table.Column<int>(nullable: false),
                    ToAccountNo = table.Column<string>(nullable: true),
                    Iban = table.Column<string>(nullable: true),
                    FirstName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    Priority = table.Column<int>(nullable: false),
                    ProcessedDate = table.Column<DateTime>(nullable: true),
                    CancelledDate = table.Column<DateTime>(nullable: true),
                    ImmediateTransfer = table.Column<bool>(nullable: false),
                    ExpectedTransferDate = table.Column<DateTime>(nullable: true),
                    Comment = table.Column<string>(nullable: true),
                    CreatorId = table.Column<string>(nullable: true),
                    UpdaterId = table.Column<string>(nullable: true),
                    CancellerId = table.Column<string>(nullable: true),
                    TransferWholeAmount = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ManualTransfers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ManualTransferSourceCardDetails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ManualTransferId = table.Column<int>(nullable: false),
                    CreateDate = table.Column<DateTime>(nullable: false),
                    UpdateDate = table.Column<DateTime>(nullable: true),
                    CardToCardAccountId = table.Column<int>(nullable: false),
                    AccountGuid = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ManualTransferSourceCardDetails", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MerchantCustomers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantGuid = table.Column<string>(maxLength: 50, nullable: false),
                    OwnerGuid = table.Column<string>(maxLength: 50, nullable: false),
                    MerchantId = table.Column<int>(nullable: false),
                    WebsiteName = table.Column<string>(maxLength: 200, nullable: false),
                    UserId = table.Column<string>(maxLength: 1000, nullable: false),
                    RegisterDate = table.Column<DateTime>(nullable: true),
                    TotalDeposit = table.Column<decimal>(type: "decimal(18, 2)", nullable: true),
                    TotalWithdraw = table.Column<decimal>(type: "decimal(18, 2)", nullable: true),
                    DepositNumber = table.Column<int>(nullable: true),
                    WithdrawNumber = table.Column<int>(nullable: true),
                    ActivityScore = table.Column<int>(nullable: true),
                    GroupName = table.Column<string>(nullable: true),
                    LastActivity = table.Column<DateTime>(nullable: true),
                    CardToCardAccountId = table.Column<int>(nullable: true),
                    UserSegmentGroupId = table.Column<int>(nullable: true),
                    WithdrawalAccountId = table.Column<int>(nullable: true),
                    UserTotalSportbook = table.Column<decimal>(type: "decimal(18, 2)", nullable: true),
                    UserSportbookNumber = table.Column<int>(nullable: true),
                    UserTotalCasino = table.Column<decimal>(type: "decimal(18, 2)", nullable: true),
                    UserCasinoNumber = table.Column<int>(nullable: true),
                    PhoneNumber = table.Column<string>(maxLength: 20, nullable: true),
                    IsConfirmed = table.Column<bool>(nullable: true),
                    ConfirmDate = table.Column<DateTime>(nullable: true),
                    ConfirmCode = table.Column<string>(nullable: true),
                    ConfirmCodeValidationEndDate = table.Column<DateTime>(nullable: true),
                    PhoneNumberRelatedCustomers = table.Column<string>(nullable: true),
                    DifferentCardNumberCount = table.Column<int>(nullable: false),
                    DeviceRelatedCustomers = table.Column<string>(nullable: true),
                    CardNumberRelatedCustomers = table.Column<string>(nullable: true),
                    SmsVerificationType = table.Column<int>(nullable: false),
                    SmsVerificationTryCount = table.Column<int>(nullable: false),
                    ConfirmedPhoneNumber = table.Column<string>(maxLength: 20, nullable: true),
                    PardakhtPayDepositCount = table.Column<int>(nullable: false),
                    PardakhtPayDepositAmount = table.Column<decimal>(nullable: false),
                    PardakhtPayWithdrawalCount = table.Column<int>(nullable: false),
                    PardakhtPayWithdrawalAmount = table.Column<decimal>(nullable: false),
                    HamrahCardTryCount = table.Column<int>(nullable: false),
                    HamrahCardVerifiedPhoneNumber = table.Column<string>(maxLength: 20, nullable: true),
                    IsHamrahCardPhoneVerified = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MerchantCustomers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Merchants",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantGuid = table.Column<string>(nullable: true),
                    Title = table.Column<string>(maxLength: 2000, nullable: false),
                    DomainAddress = table.Column<string>(maxLength: 200, nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    ApiKey = table.Column<string>(nullable: true),
                    CardToCardAccountGroupId = table.Column<int>(nullable: true),
                    MobileTransferAccountGroupId = table.Column<int>(nullable: true),
                    OwnerGuid = table.Column<string>(nullable: true),
                    MinimumTransactionAmount = table.Column<decimal>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    UseCardtoCardPaymentForWithdrawal = table.Column<bool>(nullable: false),
                    AllowPartialPaymentForWithdrawals = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Merchants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MobileCardAccountUserSegmentRelations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MobileTransferCardAccountGroupItemId = table.Column<int>(nullable: false),
                    UserSegmentGroupId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MobileCardAccountUserSegmentRelations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MobileTransferCardAccountGroups",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantGuid = table.Column<string>(nullable: true),
                    OwnerGuid = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MobileTransferCardAccountGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MobileTransferCardAccounts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PaymentProviderType = table.Column<int>(nullable: false),
                    CardNumber = table.Column<string>(nullable: true),
                    CardHolderName = table.Column<string>(nullable: true),
                    MerchantId = table.Column<string>(nullable: true),
                    MerchantPassword = table.Column<string>(maxLength: 200, nullable: true),
                    TerminalId = table.Column<string>(maxLength: 200, nullable: true),
                    Title = table.Column<string>(nullable: true),
                    TenantGuid = table.Column<string>(nullable: true),
                    OwnerGuid = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    ThresholdAmount = table.Column<long>(nullable: false),
                    CardToCardAccountGuid = table.Column<string>(maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MobileTransferCardAccounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MobileTransferDevices",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PhoneNumber = table.Column<string>(maxLength: 20, nullable: true),
                    VerificationCode = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    VerifyCodeSendDate = table.Column<DateTime>(nullable: true),
                    VerifiedDate = table.Column<DateTime>(nullable: true),
                    ExternalId = table.Column<int>(nullable: true),
                    ExternalStatus = table.Column<string>(nullable: true),
                    TenantGuid = table.Column<string>(nullable: true),
                    LastBlockDate = table.Column<DateTime>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    MerchantCustomerId = table.Column<int>(nullable: false),
                    TryCount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MobileTransferDevices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MydigiDevices",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MerchantCustomerId = table.Column<int>(nullable: false),
                    ExternalId = table.Column<int>(nullable: false),
                    PhoneNumber = table.Column<string>(nullable: true),
                    IsRegistered = table.Column<bool>(nullable: false),
                    TryCount = table.Column<int>(nullable: false),
                    RegistrationDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MydigiDevices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OwnerBankLogins",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FriendlyName = table.Column<string>(nullable: true),
                    OwnerGuid = table.Column<string>(nullable: true),
                    BankLoginGuid = table.Column<string>(nullable: true),
                    BankLoginId = table.Column<int>(nullable: false),
                    TenantGuid = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    LoginRequestId = table.Column<int>(nullable: false),
                    AccountNumbers = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    LoginType = table.Column<int>(nullable: false),
                    BankId = table.Column<int>(nullable: false),
                    LastPasswordChangeDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OwnerBankLogins", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OwnerSettings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OwnerGuid = table.Column<string>(maxLength: 70, nullable: true),
                    CreateDate = table.Column<DateTime>(nullable: false),
                    CreateUserId = table.Column<string>(maxLength: 70, nullable: true),
                    UpdateDate = table.Column<DateTime>(nullable: false),
                    UpdateUserId = table.Column<string>(maxLength: 70, nullable: true),
                    WaitAmountForCurrentWithdrawal = table.Column<bool>(nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OwnerSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Payment780Devices",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MerchantCustomerId = table.Column<int>(nullable: false),
                    ExternalId = table.Column<int>(nullable: false),
                    PhoneNumber = table.Column<string>(maxLength: 100, nullable: true),
                    IsRegistered = table.Column<bool>(nullable: false),
                    TryCount = table.Column<int>(nullable: false),
                    RegistrationDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payment780Devices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RiskyKeywords",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Keyword = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RiskyKeywords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SadadPspDevices",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MerchantCustomerId = table.Column<int>(nullable: false),
                    ExternalId = table.Column<int>(nullable: false),
                    PhoneNumber = table.Column<string>(nullable: true),
                    IsRegistered = table.Column<bool>(nullable: false),
                    TryCount = table.Column<int>(nullable: false),
                    RegistrationDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SadadPspDevices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SekehDevices",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MerchantCustomerId = table.Column<int>(nullable: false),
                    ExternalId = table.Column<int>(nullable: false),
                    PhoneNumber = table.Column<string>(nullable: true),
                    IsRegistered = table.Column<bool>(nullable: false),
                    TryCount = table.Column<int>(nullable: false),
                    RegistrationDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SekehDevices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SesDevices",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MerchantCustomerId = table.Column<int>(nullable: false),
                    ExternalId = table.Column<int>(nullable: false),
                    PhoneNumber = table.Column<string>(nullable: true),
                    IsRegistered = table.Column<bool>(nullable: false),
                    TryCount = table.Column<int>(nullable: false),
                    RegistrationDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SesDevices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TenantsApis",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantGuid = table.Column<string>(nullable: true),
                    MerchantId = table.Column<int>(nullable: false),
                    ApiUrl = table.Column<string>(nullable: true),
                    IsServiceUrl = table.Column<bool>(nullable: false),
                    IsPaymentUrl = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantsApis", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TransactionQueryHistories",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreateDate = table.Column<DateTime>(nullable: false),
                    UpdateDate = table.Column<DateTime>(nullable: true),
                    TransactionId = table.Column<int>(nullable: false),
                    RequestContent = table.Column<string>(nullable: false),
                    ResponseContent = table.Column<string>(nullable: true),
                    IsCompleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionQueryHistories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TransactionRequestContents",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransactionId = table.Column<int>(nullable: false),
                    RequestContent = table.Column<string>(maxLength: 4000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionRequestContents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    TransactionAmount = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    MerchantId = table.Column<int>(nullable: false),
                    CardNumber = table.Column<string>(maxLength: 200, nullable: false),
                    AccountNumber = table.Column<string>(maxLength: 200, nullable: false),
                    TransferredDate = table.Column<DateTime>(nullable: true),
                    Token = table.Column<string>(maxLength: 200, nullable: false),
                    CustomerCardNumber = table.Column<string>(maxLength: 1000, nullable: true),
                    BankNumber = table.Column<string>(maxLength: 500, nullable: true),
                    IpAddress = table.Column<string>(maxLength: 50, nullable: false),
                    ReturnUrl = table.Column<string>(maxLength: 500, nullable: true),
                    Status = table.Column<int>(nullable: false),
                    CardHolderName = table.Column<string>(maxLength: 300, nullable: true),
                    CardToCardAccountId = table.Column<int>(nullable: true),
                    MobileTransferAccountId = table.Column<int>(nullable: true),
                    TenantGuid = table.Column<string>(maxLength: 70, nullable: false),
                    OwnerGuid = table.Column<string>(maxLength: 70, nullable: false),
                    AccountGuid = table.Column<string>(maxLength: 70, nullable: true),
                    LoginGuid = table.Column<string>(maxLength: 70, nullable: true),
                    Description = table.Column<string>(maxLength: 100, nullable: true),
                    MerchantCustomerId = table.Column<int>(nullable: true),
                    IsMaliciousCustomer = table.Column<bool>(nullable: false),
                    UserSegmentGroupId = table.Column<int>(nullable: true),
                    HideCardNumber = table.Column<bool>(nullable: false),
                    ExternalId = table.Column<int>(nullable: true),
                    ExternalMessage = table.Column<string>(maxLength: 2000, nullable: true),
                    Reference = table.Column<string>(maxLength: 100, nullable: true),
                    PaymentType = table.Column<int>(nullable: false, defaultValue: 1),
                    MobileDeviceNumber = table.Column<string>(maxLength: 50, nullable: true),
                    WithdrawalId = table.Column<int>(nullable: true),
                    IsPhoneNumberBlocked = table.Column<bool>(nullable: false),
                    UpdateUserId = table.Column<string>(maxLength: 70, nullable: true),
                    ApiType = table.Column<int>(nullable: false),
                    ExternalReference = table.Column<string>(maxLength: 100, nullable: true),
                    ProxyPaymentAccountId = table.Column<int>(nullable: true),
                    ProcessDurationInMiliseconds = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TransactionWithdrawalHistories",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransactionId = table.Column<int>(nullable: false),
                    WithdrawalId = table.Column<int>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    IsCompleted = table.Column<bool>(nullable: false),
                    Message = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionWithdrawalHistories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TransactionWithdrawalRelations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransactionId = table.Column<int>(nullable: false),
                    WithdrawalId = table.Column<int>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionWithdrawalRelations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TransferAccounts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    TenantGuid = table.Column<string>(nullable: true),
                    AccountNo = table.Column<string>(nullable: true),
                    AccountHolderFirstName = table.Column<string>(nullable: true),
                    AccountHolderLastName = table.Column<string>(nullable: true),
                    Iban = table.Column<string>(nullable: true),
                    FriendlyName = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    OwnerGuid = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransferAccounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UnsupportedBins",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApiType = table.Column<int>(nullable: false),
                    Bin = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnsupportedBins", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserSegmentGroupHistories",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    MerchantCustomerId = table.Column<int>(nullable: false),
                    OldSegmentGroupId = table.Column<int>(nullable: true),
                    NewSegmentGroupId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSegmentGroupHistories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserSegmentGroups",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    IsDefault = table.Column<bool>(nullable: false),
                    IsMalicious = table.Column<bool>(nullable: false),
                    CreateDate = table.Column<DateTime>(nullable: false),
                    TenantGuid = table.Column<string>(nullable: true),
                    OwnerGuid = table.Column<string>(nullable: true),
                    Order = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSegmentGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserSegments",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserSegmentGroupId = table.Column<int>(nullable: false),
                    UserSegmentCompareTypeId = table.Column<int>(nullable: false),
                    UserSegmentTypeId = table.Column<int>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSegments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WithdrawalQueryHistories",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    WithdrawalId = table.Column<int>(nullable: false),
                    RequestContent = table.Column<string>(nullable: true),
                    ResponseContent = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WithdrawalQueryHistories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WithdrawalRequestContents",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WithdrawalId = table.Column<int>(nullable: false),
                    RequestContent = table.Column<string>(maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WithdrawalRequestContents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Withdrawals",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Amount = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    RemainingAmount = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    TenantGuid = table.Column<string>(maxLength: 70, nullable: false),
                    TransferAccountId = table.Column<int>(nullable: false),
                    FromAccountNumber = table.Column<string>(maxLength: 2000, nullable: true),
                    ToAccountNumber = table.Column<string>(maxLength: 2000, nullable: true),
                    ToIbanNumber = table.Column<string>(maxLength: 2000, nullable: true),
                    CardNumber = table.Column<string>(maxLength: 2000, nullable: true),
                    Priority = table.Column<int>(nullable: false),
                    FirstName = table.Column<string>(maxLength: 2000, nullable: true),
                    LastName = table.Column<string>(maxLength: 2000, nullable: true),
                    TransferRequestGuid = table.Column<string>(maxLength: 50, nullable: true),
                    TransferNotes = table.Column<string>(maxLength: 50, nullable: true),
                    TransferStatus = table.Column<int>(nullable: false),
                    TransferStatusDescription = table.Column<string>(maxLength: 2000, nullable: true),
                    TransferType = table.Column<int>(nullable: false),
                    ExpectedTransferDate = table.Column<DateTime>(nullable: false),
                    TransferRequestDate = table.Column<DateTime>(nullable: true),
                    TransferDate = table.Column<DateTime>(nullable: true),
                    TransferId = table.Column<int>(nullable: true),
                    OwnerGuid = table.Column<string>(maxLength: 70, nullable: false),
                    WithdrawalStatus = table.Column<int>(nullable: false),
                    MerchantId = table.Column<int>(nullable: false),
                    MerchantCustomerId = table.Column<int>(nullable: true),
                    ReturnUrl = table.Column<string>(maxLength: 500, nullable: true),
                    Reference = table.Column<string>(maxLength: 100, nullable: true),
                    CreateDate = table.Column<DateTime>(nullable: false),
                    UpdateDate = table.Column<DateTime>(nullable: true),
                    CancelDate = table.Column<DateTime>(nullable: true),
                    TrackingNumber = table.Column<string>(maxLength: 50, nullable: true),
                    AccountGuid = table.Column<string>(maxLength: 50, nullable: true),
                    Description = table.Column<string>(maxLength: 100, nullable: true),
                    BankStatementItemId = table.Column<int>(nullable: true),
                    WithdrawalProcessType = table.Column<int>(nullable: false, defaultValue: 1),
                    CardToCardTryCount = table.Column<int>(nullable: false),
                    UpdateUserId = table.Column<string>(maxLength: 70, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Withdrawals", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WithdrawalTransferHistories",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WithdrawalId = table.Column<int>(nullable: false),
                    TransferId = table.Column<int>(nullable: false),
                    TransferNotes = table.Column<string>(nullable: true),
                    Amount = table.Column<long>(nullable: false),
                    TransferStatus = table.Column<int>(nullable: false),
                    TransferStatusDescription = table.Column<string>(nullable: true),
                    RequestedDate = table.Column<DateTime>(nullable: false),
                    LastCheckDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WithdrawalTransferHistories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MobileTransferCardAccountGroupItems",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GroupId = table.Column<int>(nullable: false),
                    ItemId = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    AccountId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MobileTransferCardAccountGroupItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MobileTransferCardAccountGroupItems_MobileTransferCardAccounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "MobileTransferCardAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BankStatementItem_AccountGuid",
                table: "BankStatementItems",
                column: "AccountGuid");

            migrationBuilder.CreateIndex(
                name: "IX_BankStatement_AccountGuidTransactionDate",
                table: "BankStatementItems",
                columns: new[] { "AccountGuid", "TransactionDateTime" });

            migrationBuilder.CreateIndex(
                name: "UIX_BlockedPhoneNumber_PhoneNumber",
                table: "BlockedPhoneNumbers",
                column: "PhoneNumber",
                unique: true,
                filter: "[PhoneNumber] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CardHolderName_CardNumber",
                table: "CardHolderNames",
                column: "CardNumber");

            migrationBuilder.CreateIndex(
                name: "UIX_CardHolderName_RecordId",
                table: "CardHolderNames",
                column: "RecordId");

            migrationBuilder.CreateIndex(
                name: "UIX_CardToCardAccount_CardNumber",
                table: "CardToCardAccounts",
                column: "CardNumber",
                unique: true,
                filter: "[CardNumber] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UIX_Merchant_ApiKey",
                table: "Merchants",
                column: "ApiKey",
                unique: true,
                filter: "[ApiKey] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UIX_MerchantTitle",
                table: "Merchants",
                column: "Title",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MobileTransferCardAccountGroupItems_AccountId",
                table: "MobileTransferCardAccountGroupItems",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "UIX_OwnerSettingGuid",
                table: "OwnerSettings",
                column: "OwnerGuid",
                unique: true,
                filter: "[OwnerGuid] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Payment780DeviceMerchantCustomerId",
                table: "Payment780Devices",
                column: "MerchantCustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Payment780DevicePhoneNumber",
                table: "Payment780Devices",
                column: "PhoneNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_TenantGuid",
                table: "Transactions",
                column: "TenantGuid");

            migrationBuilder.CreateIndex(
                name: "UIX_Transaction_Token",
                table: "Transactions",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UIX_TransactionWithdrawalRelation_WithdrawalId",
                table: "TransactionWithdrawalRelations",
                column: "WithdrawalId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationSettings");

            migrationBuilder.DropTable(
                name: "AutoTransfers");

            migrationBuilder.DropTable(
                name: "BankStatementItems");

            migrationBuilder.DropTable(
                name: "BlockedCardNumbers");

            migrationBuilder.DropTable(
                name: "BlockedPhoneNumbers");

            migrationBuilder.DropTable(
                name: "CardHolderNames");

            migrationBuilder.DropTable(
                name: "CardToCardAccountGroupItems");

            migrationBuilder.DropTable(
                name: "CardToCardAccountGroups");

            migrationBuilder.DropTable(
                name: "CardToCardAccounts");

            migrationBuilder.DropTable(
                name: "CardToCardUserSegmentRelations");

            migrationBuilder.DropTable(
                name: "DeviceMerchantCustomerRelations");

            migrationBuilder.DropTable(
                name: "InvoiceDetails");

            migrationBuilder.DropTable(
                name: "InvoiceOwnerSettings");

            migrationBuilder.DropTable(
                name: "InvoicePayments");

            migrationBuilder.DropTable(
                name: "Invoices");

            migrationBuilder.DropTable(
                name: "IZMobileDevices");

            migrationBuilder.DropTable(
                name: "ManualTransferDetails");

            migrationBuilder.DropTable(
                name: "ManualTransfers");

            migrationBuilder.DropTable(
                name: "ManualTransferSourceCardDetails");

            migrationBuilder.DropTable(
                name: "MerchantCustomers");

            migrationBuilder.DropTable(
                name: "Merchants");

            migrationBuilder.DropTable(
                name: "MobileCardAccountUserSegmentRelations");

            migrationBuilder.DropTable(
                name: "MobileTransferCardAccountGroupItems");

            migrationBuilder.DropTable(
                name: "MobileTransferCardAccountGroups");

            migrationBuilder.DropTable(
                name: "MobileTransferDevices");

            migrationBuilder.DropTable(
                name: "MydigiDevices");

            migrationBuilder.DropTable(
                name: "OwnerBankLogins");

            migrationBuilder.DropTable(
                name: "OwnerSettings");

            migrationBuilder.DropTable(
                name: "Payment780Devices");

            migrationBuilder.DropTable(
                name: "RiskyKeywords");

            migrationBuilder.DropTable(
                name: "SadadPspDevices");

            migrationBuilder.DropTable(
                name: "SekehDevices");

            migrationBuilder.DropTable(
                name: "SesDevices");

            migrationBuilder.DropTable(
                name: "TenantsApis");

            migrationBuilder.DropTable(
                name: "TransactionQueryHistories");

            migrationBuilder.DropTable(
                name: "TransactionRequestContents");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "TransactionWithdrawalHistories");

            migrationBuilder.DropTable(
                name: "TransactionWithdrawalRelations");

            migrationBuilder.DropTable(
                name: "TransferAccounts");

            migrationBuilder.DropTable(
                name: "UnsupportedBins");

            migrationBuilder.DropTable(
                name: "UserSegmentGroupHistories");

            migrationBuilder.DropTable(
                name: "UserSegmentGroups");

            migrationBuilder.DropTable(
                name: "UserSegments");

            migrationBuilder.DropTable(
                name: "WithdrawalQueryHistories");

            migrationBuilder.DropTable(
                name: "WithdrawalRequestContents");

            migrationBuilder.DropTable(
                name: "Withdrawals");

            migrationBuilder.DropTable(
                name: "WithdrawalTransferHistories");

            migrationBuilder.DropTable(
                name: "MobileTransferCardAccounts");
        }
    }
}
