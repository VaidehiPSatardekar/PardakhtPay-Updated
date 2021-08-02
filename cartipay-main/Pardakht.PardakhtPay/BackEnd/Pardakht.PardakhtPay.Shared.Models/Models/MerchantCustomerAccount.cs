using Pardakht.PardakhtPay.Shared.Models.Entities;

namespace Pardakht.PardakhtPay.Shared.Models.Models
{
    public class MerchantCustomerAccount
    {
        public MerchantCustomer Customer { get; set; }

        public CardToCardAccount Account { get; set; }

        public UserSegmentGroup UserSegmentGroup { get; set; }

        public CardToCardAccountGroupItem GroupItem { get; set; }
    }

    public class MerchantCustomerMobileAccount
    {
        public MerchantCustomer Customer { get; set; }

        public MobileTransferCardAccount Account { get; set; }

        public UserSegmentGroup UserSegmentGroup { get; set; }

        public MobileTransferCardAccountGroupItem GroupItem { get; set; }
    }
}
