using System.Collections.Generic;

namespace Pardakht.PardakhtPay.Enterprise.Utilities.Models.Shared
{
    public enum TenantPaymentSettingStatus
    {
        Acive = 1,
        Passive = 2,
        Deleted = 3
    }

    public enum PaymentSettingStatus
    {
        Acive = 1,
        Passive = 2,
        Deleted = 3
    }

    public class CustomFieldDto
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public string Label { get; set; }
        public string Value { get; set; }
        public bool Required { get; set; }
        public bool IsReadonly { get; set; }

    }
    public class CustomerSegmentationGroupModelDto
    {
        public int Id { get; set; }
        public int TenantPlatformMapId { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsDefault { get; set; }
        public bool IsMalicious { get; set; }
        public int Order { get; set; }
        public List<CustomerSegmentationRuleModelDto> Rules { get; set; }
    }

    public class CustomerSegmentationRuleModelDto
    {
        public int Id { get; set; }
        public int CustomerSegmentGroupId { get; set; }
        public int CustomerSegmentCompareTypeId { get; set; }
        public int CustomerSegmentRuleTypeId { get; set; }
        public string Value { get; set; }
    }

}
