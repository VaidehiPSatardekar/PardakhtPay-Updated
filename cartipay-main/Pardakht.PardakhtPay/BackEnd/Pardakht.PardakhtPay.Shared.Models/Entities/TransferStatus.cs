namespace Pardakht.PardakhtPay.Shared.Models.Entities
{
    public enum TransferStatus
    {
        NotSent = -1,
        Incomplete = 0,
        Complete = 1,
        AccountBalanceLow = 2,
        AccountBalanceLowerThanAccountTransferMinimumLimit = 3,
        AccountBalanceHigerThanAccountTransferMaximumLimit = 4,
        AccountBalanceHigherThanAccountTransferWithdrawalLimitForDay = 5,
        AccountBalanceHigherThanAccountTransferWithdrawalLimitForMonth = 6,
        Invalid = 7,
        Pending = 8,
        InSufficientTime = 9,
        DetailRecorded = 10,
        RejectedDueToBlokedAccount = 11,
        Cancelled = 12,
        AwaitingConfirmation = 13,
        InsufficientBalance = 14,
        FailedFromBank = 15,
        RefundFromBank = 16,
        BankSubmitted = 17,
        InvalidIBANNumber = 18,
        CompletedWithNoReceipt = 19,
        TransferFailedDueToIncorrectAuthentication = 20,
        DownloadingReceipt = 21,
        TargetPasswordRequired = 22,
        AccountNumberInvalid = 23,
        OneTimePasswordRequired = 24,
        SecondPasswordRequiredButNotSet = 25
    }
}
