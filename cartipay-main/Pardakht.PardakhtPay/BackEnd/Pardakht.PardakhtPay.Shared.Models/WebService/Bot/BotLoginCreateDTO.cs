using System;
using System.ComponentModel.DataAnnotations;

namespace Pardakht.PardakhtPay.Shared.Models.WebService.Bot
{
    public class BotLoginCreateDTO
    {
        public int BankId { get; set; }
                
        public string Username { get; set; }
                
        public string Password { get; set; }
        public string MobileUsername { get; set; }

        public string MobilePassword { get; set; }

        [Required]
        public string OwnerGuid { get; set; }

        [Required]
        public string FriendlyName { get; set; }

        [Required]
        public string TenantGuid { get; set; }

        public bool IsMobileLogin { get; set; }

        public long? MobileNumber { get; set; }

        public int OTP { get; set; }

        //[Required]
        //public string AccountNo { get; set; }
    }

    public class BankBotLoginStatus
    {

        public int Id { get; set; }

        public string LoginGuid { get; set; }

        public bool IsLoggedIn { get; set; }

        public bool IsBlocked { get; set; }

        public bool IsActive { get; set; }

        public bool IsWrongPassword { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsPasswordChanged { get; set; }

        //public string LoginGuid { get; set; }

        //public bool Success { get; set; }
    }

    public class BankBotUpdateLoginDTO
    {
        public int Id { get; set; }

        //[Required]
        public string Username { get; set; }

        //[Required]
        public string Password { get; set; }
        public string MobileUsername { get; set; }

        //[Required]
        public string MobilePassword { get; set; }

        public string SecondPassword { get; set; }

        public string MobileNumber { get; set; }

        public string EmailAddress { get; set; }

        public string EmailPassword { get; set; }
        public int? ProcessCountIn24Hrs { get; set; }


    }

    public class CreateLoginFromLoginRequestDTO
    {
        public int LoginRequestId { get; set; }

        public int LoginType { get; set; }

        public string AccountNumber { get; set; }

        public bool LoadPreviousStatements { get; set; }

        public bool IsBlockCard { get; set; }

        public string SecondPassword { get; set; }

        public string MobileNumber { get; set; }

        public string EmailAddress { get; set; }

        public string EmailPassword { get; set; }

        public int Otp { get; set; }

        public int? ProcessCountIn24Hrs { get; set; }
    }

    public class QRRegisterLoginDTO
    {
        public int Id { get; set; }
        public DateTime RequestDate { get; set; }


        [Required]
        public int LoginId { get; set; }
        public int Status { get; set; }
        public string QRImagePath { get; set; }
        public DateTime? QRCodeExpiryDateTime { get; set; }
        public DateTime? UpdateTimeStamp { get; set; }

        public string QRRegistrationStatus { get; set; }
        public string OTP { get; set; }
    }

    public class QrCodeRegistrationRequest
    {
        public int BankLoginId { get; set; }
        public string OTP { get; set; }
    }

    public class LoginDeviceResult
    {
        public int LoginId { get; set; }
        public bool Status { get; set; }
        public int DeviceStatus { get; set; }
    }

    public class DeviceOTP
    {
        public string GeneratedCode { get; set; }
    }

    public enum LoginDeviceStatuses
    {
        Active = 1,
        InActive = 2,
        MobileNotConfigured = 3,
        Error = 4
    }

    public enum QRRegisterRequestStatus
    {
        Incomplete = 0,
        InProgress = 1,
        Complete = 2,
        Pending = 3,
        Failed = 4,
        SessionOut = 5,
        QrImageCaptured = 6
    }


    public class LoginDeviceStatus
    {
        public int Id { get; set; }
        public int LoginId { get; set; }
        public int Status { get; set; }
        public DateTime? UpdateTimeStamp { get; set; }

    }

    public class RegisterLogin
    {
        public int LoginRequestId { get; set; }
        public int Otp { get; set; }

        public int BankId { get; set; }
    }
}