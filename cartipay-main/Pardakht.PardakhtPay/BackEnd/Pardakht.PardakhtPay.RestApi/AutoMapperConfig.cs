using Newtonsoft.Json;
using System.Collections.Generic;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using Pardakht.PardakhtPay.Shared.Models.WebService;
using Pardakht.PardakhtPay.Shared.Models.WebService.Bot;
using Pardakht.PardakhtPay.Shared.Models.WebService.Invoice;
using Pardakht.PardakhtPay.Shared.Models.WebService.MobileTransfer;

namespace Pardakht.PardakhtPay.RestApi
{
    /// <summary>
    /// Represents a class to manage auto mapper configuration
    /// </summary>
    public class AutoMapperConfig
    {
        /// <summary>
        /// Initialize mappers
        /// </summary>
        public static void Initialize()
        {
            AutoMapper.Mapper.Initialize(config =>
            {
                config.CreateMap<Merchant, MerchantDTO>();
                config.CreateMap<MerchantDTO, Merchant>();

                config.CreateMap<MerchantUpdateDTO, Merchant>();
                config.CreateMap<Merchant, MerchantUpdateDTO>();

                config.CreateMap<MerchantCreateDTO, Merchant>();
                config.CreateMap<Merchant, MerchantCreateDTO>();

                //config.CreateMap<MerchantBankAccount, MerchantBankAccountDTO>();
                //config.CreateMap<MerchantBankAccountDTO, MerchantBankAccount>();

                //config.CreateMap<Tenant, TenantDTO>();
                //config.CreateMap<TenantDTO, Tenant>();

                //config.CreateMap<Tenant, TenantCreateDTO>();
                //config.CreateMap<TenantCreateDTO, Tenant>();

                config.CreateMap<Language, LanguageDTO>();
                config.CreateMap<LanguageDTO, Language>();

                config.CreateMap<Currency, CurrencyDTO>();
                config.CreateMap<CurrencyDTO, Currency>();

                config.CreateMap<Withdrawal, WithdrawalDTO>();
                config.CreateMap<WithdrawalDTO, Withdrawal>();

                config.CreateMap<TransferAccount, TransferAccountDTO>();
                config.CreateMap<TransferAccountDTO, TransferAccount>();

                config.CreateMap<PaymentGateway, PaymentGatewayDTO>().ForMember(p => p.ParameterItems, t => t.MapFrom(m => JsonConvert.DeserializeObject<List<PaymentGatewayParameterDTO>>(m.Parameters)));
                config.CreateMap<PaymentGatewayDTO, PaymentGateway>().ForMember(p => p.Parameters, t => t.MapFrom(m => JsonConvert.SerializeObject(m.ParameterItems)));

                config.CreateMap<PaymentGatewayConfiguration, PaymentGatewayConfigurationDTO>().ForMember(p => p.ParameterItems, t => t.MapFrom(m => JsonConvert.DeserializeObject<List<PaymentGatewayConfigurationParameterDTO>>(m.Configuration)));
                config.CreateMap<PaymentGatewayConfigurationDTO, PaymentGatewayConfiguration>().ForMember(p => p.Configuration, t => t.MapFrom(m => JsonConvert.SerializeObject(m.ParameterItems)));

                config.CreateMap<CardToCardAccount, CardToCardAccountDTO>();
                config.CreateMap<CardToCardAccountDTO, CardToCardAccount>();

                config.CreateMap<OwnerBankLogin, OwnerBankLoginDTO>();
                config.CreateMap<OwnerBankLoginDTO, OwnerBankLogin>();

                config.CreateMap<BankStatementItem, BankStatementItemDTO>();
                config.CreateMap<BankStatementItemDTO, BankStatementItem>();

                config.CreateMap<TenantApi, TenantApiDTO>();
                config.CreateMap<TenantApiDTO, TenantApi>();

                config.CreateMap<CardToCardAccountGroup, CardToCardAccountGroupDTO>();
                config.CreateMap<CardToCardAccountGroupDTO, CardToCardAccountGroup>();

                config.CreateMap<CardToCardAccountGroupItem, CardToCardAccountGroupItemDTO>();
                config.CreateMap<CardToCardAccountGroupItemDTO, CardToCardAccountGroupItem>();

                config.CreateMap<MerchantCustomer, MerchantCustomerDTO>();
                config.CreateMap<MerchantCustomerDTO, MerchantCustomer>();

                config.CreateMap<UserSegmentGroup, UserSegmentGroupDTO>();
                config.CreateMap<UserSegmentGroupDTO, UserSegmentGroup>();

                config.CreateMap<UserSegment, UserSegmentDTO>();
                config.CreateMap<UserSegmentDTO, UserSegment>();

                config.CreateMap<ManualTransfer, ManualTransferDTO>();
                config.CreateMap<ManualTransferDTO, ManualTransfer>();

                config.CreateMap<ManualTransferDetail, ManualTransferDetailDTO>();
                config.CreateMap<ManualTransferDetailDTO, ManualTransferDetail>();

                config.CreateMap<ManualTransferSourceCardDetails, ManualTransferSourceCardDetailsDTO>();
                config.CreateMap<ManualTransferDetailDTO, ManualTransferSourceCardDetails>();

                config.CreateMap<MobileTransferDevice, MobileTransferDeviceDTO>();
                config.CreateMap<MobileTransferDeviceDTO, MobileTransferDevice>();

                config.CreateMap<MobileTransferCardAccount, MobileTransferCardAccountDTO>();
                config.CreateMap<MobileTransferCardAccountDTO, MobileTransferCardAccount>();

                config.CreateMap<MobileTransferCardAccountGroup, MobileTransferCardAccountGroupDTO>();
                config.CreateMap<MobileTransferCardAccountGroupDTO, MobileTransferCardAccountGroup>();

                config.CreateMap<MobileTransferCardAccountGroupItem, MobileTransferCardAccountGroupItemDTO>();
                config.CreateMap<MobileTransferCardAccountGroupItemDTO, MobileTransferCardAccountGroupItem>();

                config.CreateMap<CompleteMobilePaymentRequest, CompletePaymentRequest>();

                config.CreateMap<BlockedPhoneNumber, BlockedPhoneNumberDTO>();
                config.CreateMap<BlockedPhoneNumberDTO, BlockedPhoneNumber>();

                config.CreateMap<Invoice, InvoiceDTO>();
                config.CreateMap<InvoiceDTO, Invoice>();

                config.CreateMap<InvoiceDetail, InvoiceDetailDTO>();
                config.CreateMap<InvoiceDetailDTO, InvoiceDetail>();

                config.CreateMap<InvoiceOwnerSetting, InvoiceOwnerSettingDTO>();
                config.CreateMap<InvoiceOwnerSettingDTO, InvoiceOwnerSetting>();

                config.CreateMap<InvoicePayment, InvoicePaymentDTO>();
                config.CreateMap<InvoicePaymentDTO, InvoicePayment>();

                config.CreateMap<OwnerSetting, OwnerSettingDTO>();
                config.CreateMap<OwnerSettingDTO, OwnerSetting>();

                config.CreateMap<BlockedCardNumber, BlockedCardNumberDTO>();
                config.CreateMap<BlockedCardNumberDTO, BlockedCardNumber>();
            });
        }
    }
}
