using System;

namespace Pardakht.PardakhtPay.Shared.Models.Configuration
{
    public class AesEncryptionSettings
    {
        string _Key = string.Empty;
        string _IV = string.Empty;

        public byte[] KeyArray { get; private set; } = null;

        public byte[] IVArray { get; private set; } = null;

        public string Key
        {
            get
            {
                return _Key;
            }
            set
            {
                _Key = value;
                KeyArray = Convert.FromBase64String(value);
            }
        }

        public string IV
        {
            get
            {
                return _IV;
            }
            set
            {
                _IV = value;
                IVArray = Convert.FromBase64String(value);
            }
        }
    }
}
