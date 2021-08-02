using System.Collections.Generic;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using Pardakht.PardakhtPay.Shared.Models.Models;
using Pardakht.PardakhtPay.Shared.Models.WebService;
using System.Linq;

namespace Pardakht.PardakhtPay.Shared.Models.Configuration
{
    [Setting(Key = ApplicationSettingKeys.MobileApi)]
    public class MobileApiConfiguration : BaseEntityDTO
    {
        public bool UseAsanPardakhtApi { get; set; }

        public int AsandPardakhtApiOrder { get; set; }

        public int AsanpardakhtWithdrawalOrder { get; set; }

        public bool UseAsanpardahkhtForWithdrawals { get; set; }

        public bool UseHamrahCardApi { get; set; }

        public int HamrahCardApiOrder { get; set; }

        public int HamrahCardWithdrawalOrder { get; set; }

        public int HamrahMaximumTryCount { get; set; }

        public bool UseHamrahCardForWithdrawals { get; set; }

        public bool UseSekeh { get; set; }

        public int SekehApiOrder { get; set; }

        public int SekehWithdrawalOrder { get; set; }

        public int SekehMaximumTryCount { get; set; }

        public bool UseSekehForWithdrawals { get; set; }

        public bool UseSes { get; set; }

        public int SesApiOrder { get; set; }

        public int SesWithdrawalOrder { get; set; }

        public int SesMaximumTryCount { get; set; }

        public bool UseSesForWithdrawals { get; set; }

        public decimal SesLimit { get; set; }

        public bool UseSadadPsp { get; set; }

        public int SadadPspOrder { get; set; }

        public bool UseSadadPspForWithdrawals { get; set; }

        public int SadadPspWithdrawalOrder { get; set; }

        public int SadadPspMaxTryCount { get; set; }

        public bool UseMydigi { get; set; }

        public int MydigiOrder { get; set; }

        public bool UseMydigiForWithdrawals { get; set; }

        public int MydigiWithdrawalOrder { get; set; }

        public int MydigiMaxTryCount { get; set; }

        public int DeviceRegistrationApi { get; set; }

        public bool UsePayment780 { get; set; }

        public int Payment780ApiOrder { get; set; }

        public int Payment780WithdrawalOrder { get; set; }

        public int Payment780MaximumTryCount { get; set; }

        public bool UsePayment780ForWithdrawals { get; set; }

        public List<MobileApiItem> GetItems(bool onlyActives = true)
        {
            var items = new List<MobileApiItem>();

            if(UseAsanPardakhtApi || UseAsanpardahkhtForWithdrawals || !onlyActives)
            {
                items.Add(new MobileApiItem()
                {
                    ApiType = ApiType.AsanPardakht,
                    InUse = UseAsanPardakhtApi,
                    Order = AsandPardakhtApiOrder,
                    WithdrawalOrder = AsanpardakhtWithdrawalOrder > 0 ? AsanpardakhtWithdrawalOrder : AsandPardakhtApiOrder,
                    UseForWithdrawals = UseAsanpardahkhtForWithdrawals
                });
            }

            if(UseHamrahCardApi || UseHamrahCardForWithdrawals || !onlyActives)
            {
                items.Add(new MobileApiItem()
                {
                    ApiType = ApiType.HamrahCard,
                    InUse = UseHamrahCardApi,
                    Order = HamrahCardApiOrder,
                    WithdrawalOrder = HamrahCardWithdrawalOrder > 0 ? HamrahCardWithdrawalOrder : HamrahCardApiOrder,
                    UseForWithdrawals = UseHamrahCardForWithdrawals
                });
            }

            if(UseSekeh || UseSekehForWithdrawals || !onlyActives)
            {
                items.Add(new MobileApiItem()
                {
                    ApiType = ApiType.Sekeh,
                    InUse = UseSekeh,
                    Order = SekehApiOrder,
                    WithdrawalOrder = SekehWithdrawalOrder > 0 ? SekehWithdrawalOrder : SekehApiOrder,
                    UseForWithdrawals = UseSekehForWithdrawals
                });
            }

            if(UseSes || UseSesForWithdrawals || !onlyActives)
            {
                items.Add(new MobileApiItem()
                {
                    ApiType = ApiType.Ses,
                    InUse = UseSes,
                    Order = SesApiOrder,
                    WithdrawalOrder = SesWithdrawalOrder > 0 ? SesWithdrawalOrder : SesApiOrder,
                    UseForWithdrawals = UseSesForWithdrawals,
                    Limit = SesLimit
                });
            }

            if (UseSadadPsp || UseSadadPspForWithdrawals || !onlyActives)
            {
                items.Add(new MobileApiItem()
                {
                    ApiType = ApiType.SadadPsp,
                    InUse = UseSadadPsp,
                    Order = SadadPspOrder,
                    WithdrawalOrder = SadadPspWithdrawalOrder > 0 ? SadadPspWithdrawalOrder : SadadPspOrder,
                    UseForWithdrawals = UseSadadPspForWithdrawals
                });
            }

            if (UseMydigi || UseMydigiForWithdrawals || !onlyActives)
            {
                items.Add(new MobileApiItem()
                {
                    ApiType = ApiType.Mydigi,
                    InUse = UseMydigi,
                    Order = MydigiOrder,
                    WithdrawalOrder = MydigiWithdrawalOrder > 0 ? MydigiWithdrawalOrder : MydigiOrder,
                    UseForWithdrawals = UseMydigiForWithdrawals
                });
            }

            if (UsePayment780 || UsePayment780ForWithdrawals || !onlyActives)
            {
                items.Add(new MobileApiItem()
                {
                    ApiType = ApiType.Payment780,
                    InUse = UsePayment780,
                    Order = Payment780ApiOrder,
                    WithdrawalOrder = Payment780WithdrawalOrder > 0 ? Payment780WithdrawalOrder : Payment780ApiOrder,
                    UseForWithdrawals = UsePayment780ForWithdrawals
                });
            }

            return items.OrderBy(t => t.Order).ToList();
        }
    }

    public class MobileApiItem
    {
        public ApiType ApiType { get; set; }

        public bool InUse { get; set; }

        public int Order { get; set; }

        public int WithdrawalOrder { get; set; }

        public bool UseForWithdrawals { get; set; }

        public decimal Limit { get; set; }
    }
}
