using System;

namespace Pardakht.PardakhtPay.Shared.Models.Models
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SettingAttribute : Attribute
    {
        public string Key { get; set; }
    }
}
