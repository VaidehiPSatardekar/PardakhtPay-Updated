using System;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Shared.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.MobileTransfer;

namespace Pardakht.PardakhtPay.Application.Mock
{
    public class MobileTransferMockService : IMobileTransferService
    {
        public async Task<MobileTransferResponse> ActivateDeviceAsync(MobileTransferActivateDeviceModel model)
        {
            var mod = 0;// new Random().Next() % 2;

            if (mod == 0)
            {
                return new MobileTransferResponse()
                {
                    Error = null,
                    Result = new MobileTransferResult()
                    {
                        Id = 1,
                        Msg = "Success"
                    }
                };
            }
            else
            {
                return new MobileTransferResponse()
                {
                    Error = new MobileTransferError()
                    {
                        Code = new Random().Next(),
                        Desc = new Random().Next().ToString()
                    },
                    Result = null
                };
            }
        }

        public async Task<MobileTransferResponse> CheckDeviceStatusAsync(MobileTransferSendSmsModel model)
        {
            return new MobileTransferResponse()
            {
                Error = null,
                Result = new MobileTransferResult()
                {
                    Id = 1,
                    Msg = "Registered"
                }
            };
        }

        public async Task<MobileTransferResponse> CheckStatusAsync(MobileTransferCheckStatusModel model)
        {
            return new MobileTransferResponse()
            {
                Error = null,
                Result = new MobileTransferResult()
                {
                    Id = 1,
                    Msg = "Registered"
                }
            };
        }

        public async Task<MobileTransferResponse> CheckTransferStatusAsync(MobileTransferStartTransferModel model)
        {
            return new MobileTransferResponse()
            {
                Error = null,
                Result = new MobileTransferResult()
                {
                    Id = 1,
                    Msg = "Success"
                }
            };
        }

        public Task<MobileTransferResponse> GetCardOwnerNameAsync(MobileTransferStartTransferModel model)
        {
            throw new NotImplementedException();
        }

        public async Task<MobileTransferResponse> RemoveDeviceAsync(MobileTransferRemoveDeviceModel model)
        {
            return new MobileTransferResponse()
            {
                Error = null,
                Result = new MobileTransferResult()
                {
                    Id = 1,
                    Msg = "Success"
                }
            };
        }

        public async Task<MobileTransferResponse> SendOtpPinAsync(MobileTransferStartTransferModel model)
        {
            return new MobileTransferResponse()
            {
                Result = new MobileTransferResult()
                {
                    
                }
            };
        }

        public async Task<MobileTransferResponse> SendSMSAsync(MobileTransferSendSmsModel model)
        {
            return new MobileTransferResponse()
            {
                Error = null,
                Result = new MobileTransferResult()
                {
                    Id = 1,
                    Msg = "Success"
                }
            };
        }

        public async Task<MobileTransferResponse> StartTransferAsync(MobileTransferStartTransferModel model)
        {
            //Thread.Sleep(3000);
            var mod = 1;// new Random().Next() % 2;

            if (mod != 0)
            {
                return new MobileTransferResponse()
                {
                    Error = null,
                    Result = new MobileTransferResult()
                    {
                        Id = new Random().Next(),
                        Msg = new Random().Next().ToString()
                    }
                };
            }
            else
            {
                return new MobileTransferResponse()
                {
                    Error = new MobileTransferError()
                    {
                        Code = new Random().Next(),
                        Desc = "Unsuccessful"
                    },
                    Result = null
                };
            }
        }
    }
}
