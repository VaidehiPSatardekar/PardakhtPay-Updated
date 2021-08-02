//using System;
//using System.Collections.Generic;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;
//using Pardakht.PardakhtPay.Enterprise.Utilities.Models.Shared;

//namespace Pardakht.PardakhtPay.Enterprise.Utilities.Models.Tenant
//{
//    public class TenantConfig : ConfigBase
//    {
//        public int Id { get; set; }
//        public int? Status { get; set; }

//        public string TenantPlatformMapGuid { get; set; }
//        public TenantDto Tenant { get; set; }
//        public string TenantName { get; set; }
//        public string BrandName { get; set; }
//        public string PlatformGuid { get; set; }
//        public string PrimaryDomainName { get; set; }
//        public string SubDomain { get; set; }
//        //public string ActiveDomainName { get; set; }
//        //public ICollection<MappedDomain> Domains { get; set; }
//        public string PreferenceConfig { get; set; }
//        public ICollection<MappedProduct> Products { get; set; }
//        public ICollection<MappingBase> Countries { get; set; }
//        public ICollection<CurrencyMappingBase> Currencies { get; set; }
//        public ICollection<DocumentsMappingBase> Documents { get; set; }
//        public ICollection<MappingBase> FinancialActionDocumentSettings { get; set; }

//        public ICollection<MappingBase> Languages { get; set; }
//        public ICollection<MappingBase> TimeZones { get; set; }
//        //public ICollection<MappedPaymentSetting> PaymentSettings { get; set; }
//        public ICollection<MappedBonus> Bonuses { get; set; }
//        public ICollection<TenantPlatformMapBrandDto> TenantPlatformMapBrands { get; set; }
//        public string AccountContext { get; set; }
//        public TenantPlatformMapBrandDto ActiveTenantPlatformMapBrand { get; set; }

//        public string ThisProductConnectionString { get; set; }
//        public FieldPreferenceSection FieldPreferenceSection
//        {
//            get
//            {
//                return JsonConvert.DeserializeObject<JObject>(PreferenceConfig).SelectToken("$.sections[?(@.name == 'customerRegistrationMandatoryFields')]").ToObject<FieldPreferenceSection>();
//            }
//        }
//        public PasswordPreferenceSection PasswordPreferenceSection
//        {
//            get
//            {
//                return JsonConvert.DeserializeObject<JObject>(PreferenceConfig).SelectToken("$.sections[?(@.name == 'passwordSettings')]").ToObject<PasswordPreferenceSection>();
//            }
//        }
//        public LoginPreferenceSection LoginPreferenceSection
//        {
//            get
//            {
//                return JsonConvert.DeserializeObject<JObject>(PreferenceConfig).SelectToken("$.sections[?(@.name == 'loginSettings')]").ToObject<LoginPreferenceSection>();
//            }
//        }
//        public CommunicationPreferenceSection CommunicationPreferenceSection
//        {
//            get
//            {
//                return JsonConvert.DeserializeObject<JObject>(PreferenceConfig).SelectToken("$.sections[?(@.name == 'communicationSettings')]").ToObject<CommunicationPreferenceSection>();
//            }
//        }
//        public OtherPreferenceSection OtherPreferenceSection
//        {
//            get
//            {
//                return JsonConvert.DeserializeObject<JObject>(PreferenceConfig).SelectToken("$.sections[?(@.name == 'otherSettings')]").ToObject<OtherPreferenceSection>();
//            }
//        }

//        public bool IsPopulated
//        {
//            get { return !string.IsNullOrWhiteSpace(ThisProductConnectionString); }
//        }

//        public void Clone(TenantConfig copyFrom)
//        {
//            TenantPlatformMapGuid = copyFrom.TenantPlatformMapGuid;
//            ConnectionString = copyFrom.ConnectionString;
//            Id = copyFrom.Id;
//            Tenant = copyFrom.Tenant;
//            TenantName = copyFrom.TenantName;
//            BrandName = copyFrom.BrandName;
//            PreferenceConfig = copyFrom.PreferenceConfig;
//            Currencies = copyFrom.Currencies;
//            Documents = copyFrom.Documents;
//            FinancialActionDocumentSettings = copyFrom.FinancialActionDocumentSettings;
//            Languages = copyFrom.Languages;
//            //Domains = copyFrom.Domains;
//            Countries = copyFrom.Countries;
//            TimeZones = copyFrom.TimeZones;
//            //ActiveDomainName = copyFrom.ActiveDomainName;
//            PrimaryDomainName = copyFrom.PrimaryDomainName;
//            SubDomain = copyFrom.SubDomain;
//            PlatformGuid = copyFrom.PlatformGuid;
//            Products = copyFrom.Products;
//            ThisProductConnectionString = copyFrom.ThisProductConnectionString;
//            //PaymentSettings = copyFrom.PaymentSettings;
//            Bonuses = copyFrom.Bonuses;
//            TenantPlatformMapBrands = copyFrom.TenantPlatformMapBrands;
//            AccountContext = copyFrom.AccountContext;
//            ActiveTenantPlatformMapBrand = copyFrom.ActiveTenantPlatformMapBrand;
//            Status = copyFrom.Status;
//        }

//        public class TenantDto
//        {
//            public string Email { get; set; }
//        }

//        public void Reset()
//        {
//            TenantPlatformMapGuid = string.Empty;
//            ConnectionString = string.Empty;
//            Id = 0;
//            TenantName = string.Empty;
//            Tenant = null;
//            BrandName = string.Empty;
//            PreferenceConfig = string.Empty;
//            //ActiveDomainName = string.Empty;
//            PrimaryDomainName = string.Empty;
//            SubDomain = string.Empty;
//            PlatformGuid = string.Empty;
//            Products = null;
//            Currencies = null;
//            Documents = null;
//            FinancialActionDocumentSettings = null;
//            Languages = null;
//            //Domains = null;
//            Countries = null;
//            TimeZones = null;
//            ThisProductConnectionString = string.Empty;
//            //PaymentSettings = null;
//            Bonuses = null;
//            TenantPlatformMapBrands = null;
//            Status = null;
//        }
//    }

//    public class TenantPlatformMapBrandDto 
//    {
//        public int Id { get; set; }
//        public int TenantPlatformMapId { get; set; }
//        public string Name { get; set; }
//        public string DomainGuid { get; set; }
//        public string TenantUid { get; set; }
//        public string Domain { get; set; }
//        public string Group { get; set; }
//        public string SubDomain { get; set; }
//        public string BrandName { get; set; }

//    }
//    public class MappingBase
//    {
//        public int Id { get; set; }
//        public string Code { get; set; }
//        public string Description { get; set; }
//        public bool IsDefault { get; set; }
//    }

//    public class CurrencyMappingBase : MappingBase
//    {
//        public decimal MinDeposit { get; set; }
//        public decimal MaxDeposit { get; set; }
//        public decimal MinWithdrawal { get; set; }
//        public decimal MaxWithdrawal { get; set; }
//        public decimal WithdrawalLimitAmount { get; set; }
//        public int WithdrawalLimitDuration { get; set; }
//    }

//    public class DocumentsMappingBase : MappingBase
//    {
//        public int DocumentId { get; set; }
//        public bool IsActive { get; set; }
//    }

//    public enum TenantPaymentSettingType
//    {
//        Withdraw = 1,
//        Deposit = 2,
//        Both = 3
//    }

//    public class MappedPaymentSetting
//    {
//        public int Id { get; set; }
//        public string CustomFields { get; set; }
//        public int PaymentSettingId { get; set; }
//        public int PaymentSettingStatus { get; set; }
//        public string Currency { get; set; }
//        public string PaymentIdentifier { get; set; }
//        public string GatewayKey { get; set; }
//        public bool IsDefault { get; set; }
//        public string DepositParamatersJson { get; set; }
//        public string WithdrawParamatersJson { get; set; }
//        public TenantPaymentSettingType TenantPaymentSettingType { get; set; }
//        public ICollection<PaymentCustomerSegmentRelationDto> PaymentSettingCustomerSegmentationRelations { get; set; }
//    }

//    public class PaymentCustomerSegmentRelationDto
//    {
//        public int Id { get; set; }
//        public int CustomerSegmentationGroupId { get; set; }
//        public CustomerSegmentationGroupModelDto CustomerSegmentationGroup { get; set; }
//        public int TenantPlatformMapPaymentSettingId { get; set; }
//    }
  
//    public class MappedBonus
//    {
//        public int Id { get; set; }
//        public string Title { get; set; }
//        public int Status { get; set; }
//        public int BonusType { get; set; }
//        public DateTime CreatedAt { get; set; }
//        public DateTime? UpdatedAt { get; set; }
//        public DateTime StartDate { get; set; }
//        public DateTime EndDate { get; set; }
//        public decimal BonusPercentage { get; set; }
//        public bool IsFirstTransactionOnly { get; set; }
//        public bool IsBonusForfeitForWithdrawal { get; set; }
//        public double QualifyingDuration { get; set; }
//        public int Priority { get; set; }
//        public int? BrandId { get; set; }
//        public ICollection<MappedBonusAmount> BonusAmounts { get; set; }
//        public ICollection<MappedBonusProduct> BonusProducts { get; set; }
//        public ICollection<MappedBonusRule> BonusRules { get; set; }
//        public ICollection<MappedBonusPaymentSystem> BonusPaymentSystems { get; set; }
//        public ICollection<MappedBonusGameProvider> BonusGameProviders { get; set; }
//    }

//    public class MappedBonusAmount
//    {
//        public int BonusId { get; set; }
//        public string CurrencyCode { get; set; }
//        public decimal MinimumAmount { get; set; }
//        public decimal MaxAmount { get; set; }
//    }

//    public class MappedBonusProduct
//    {
//        public int BonusId { get; set; }
//        public string ProductCode { get; set; }
//        public ICollection<MappedBonusProductRule> BonusProductRules { get; set; }
//    }

//    public class MappedBonusPaymentSystem
//    {
//        public int BonusId { get; set; }
//        public int TenantPlatformMapPaymentSettingId { get; set; }
//        public MappedPaymentSetting PaymentSetting { get; set; }
//    }

//    public class MappedBonusGameProvider
//    {
//        public int BonusId { get; set; }
//        public int TenantPlatformMapGameProviderId { get; set; }
//    }

//    public class MappedBonusRule
//    {
//        public int Id { get; set; }
//        public int BonusId { get; set; }
//        public int BonusCompareTypeId { get; set; }
//        public int BonusRuleTypeId { get; set; }
//        public string Value { get; set; }
//        public bool IsTurnover { get; set; }
//    }

//    public class MappedBonusProductRule
//    {
//        public int Id { get; set; }
//        public int TenantPlatformMapBonusProductId { get; set; }
//        public int CategoryTypeId { get; set; }
//        public int ItemId { get; set; }
//        public int? ParentLinkId { get; set; }
//    }


//    public class MappedProduct : ConfigBase
//    {
//        public int Id { get; set; }
//        public string Name { get; set; }
//        public string Code { get; set; }
//    }
//    public class MappedDomain
//    {
//        public int Id { get; set; }
//        public string DomainGuid { get; set; }
//        public string DomainAddress { get; set; }
//        public string ZoneId { get; set; }
//        public string ServerIpAddress { get; set; }
//        public string TenantPlatformMapGuid { get; set; }
//        public DomainStatus DomainStatus { get; set; }
//        public string DomainStatusStr => $"{DomainStatus}";
//        public DateTime CreatedAt { get; set; }
//        public DateTime? UpdatedAt { get; set; }
//        public DateTime? RegistrationDate { get; set; }
//        public bool IsPrimary { get; set; }
//        public bool IsProviderDomain { get; set; }
//        public bool IsAutoRenew { get; set; }
//        public int PurchasePeriod { get; set; }
//        public bool IsPrivateDomain { get; set; }
//        public bool IsTenantDomain { get; set; }
//        public bool IsTenantContactInfo { get; set; }
//        public bool UseExistingDomain { get; set; }
//        public bool HasDomainChangedOnEdit { get; set; }
//        public int? TenantPlatformMapBrandId { get; set; }

//    }
//    public enum DomainStatus
//    {
//        InActive = 0,
//        Active = 1,
//        Blocked = 2,
//        Pending = 3,
//        Disabled = 4
//    }
//    public abstract class ConfigBase
//    {
//        public string ConnectionString { get; set; }

//        public string GetConnectionStringWithoutPassword()
//        {
//            return GetConnectionStringWithoutPassword(ConnectionString);
//        }

//        public string GetConnectionStringWithoutPassword(string input)
//        {
//            if (!string.IsNullOrEmpty(input))
//            {
//                var startIndex = input.ToLower().IndexOf("password=");
//                if (startIndex > -1)
//                {
//                    var endIndex = input.IndexOf(';', startIndex);
//                    if (endIndex > startIndex)
//                    {
//                        var replace = input.Substring(startIndex, endIndex - startIndex);

//                        return input.Replace(replace, "**********");
//                    }
//                }
//            }

//            return string.Empty;
//        }
//    }

//    public class PreferenceConfig
//    {
//        public ICollection<FieldPreferenceSection> Sections { get; set; }
//    }

//    public class FieldPreferenceSection
//    {
//        public string Name { get; set; }
//        public string Label { get; set; }
//        public ConfigType ConfigType { get; set; }
//        public ICollection<FieldItem> Items { get; set; }
//    }

//    public class PasswordPreferenceSection
//    {
//        public string Name { get; set; }
//        public string Label { get; set; }
//        public ConfigType ConfigType { get; set; }
//        public ICollection<PasswordItem> Items { get; set; }
//    }

//    public class LoginPreferenceSection : PasswordPreferenceSection { }

//    public class CommunicationPreferenceSection : FieldPreferenceSection { }

//    public class OtherPreferenceSection : PasswordPreferenceSection { }

//    public class FieldItem
//    {
//        public string Name { get; set; }
//        public string Label { get; set; }
//        public ICollection<string> Value { get; set; }
//    }
//    public class PasswordItem
//    {
//        public string Name { get; set; }
//        public string Label { get; set; }
//        public string Value { get; set; }
//    }

//    public enum ConfigType
//    {
//        Preference = 1
//    }
//}
