using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Application.Interfaces;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Shared.Interfaces;
using Pardakht.PardakhtPay.Shared.Models;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using Pardakht.PardakhtPay.Shared.Models.MobileTransfer;
using Pardakht.PardakhtPay.Shared.Models.WebService;
using Pardakht.PardakhtPay.Shared.Models.WebService.MobileTransfer;

namespace Pardakht.PardakhtPay.Application.Services
{
    public class MobileTransferDeviceService: DatabaseServiceBase<MobileTransferDevice, IMobileTransferDeviceManager>, IMobileTransferDeviceService
    {
        IMobileTransferService _MobileTransferService = null;

        public MobileTransferDeviceService(IMobileTransferDeviceManager manager, 
            ILogger<MobileTransferDeviceService> logger,
            IMobileTransferService mobileTransferService):base(manager, logger)
        {
            _MobileTransferService = mobileTransferService;
        }

        public async Task<WebResponse<List<MobileTransferDeviceDTO>>> GetAllItemsAsync()
        {
            try
            {
                var result = await GetAllAsync();

                if (!result.Success)
                {
                    throw new Exception(result.Message);
                }

                var dtos = AutoMapper.Mapper.Map<List<MobileTransferDeviceDTO>>(result.Payload);

                return new WebResponse<List<MobileTransferDeviceDTO>>(true, string.Empty, dtos);
            }
            catch (Exception ex)
            {
                return new WebResponse<List<MobileTransferDeviceDTO>>(ex);
            }
        }

        public async Task<WebResponse<MobileTransferDeviceDTO>> GetItemById(int id)
        {
            try
            {
                var result = await GetEntityByIdAsync(id);

                if (!result.Success)
                {
                    throw result.Exception;
                }

                var dto = AutoMapper.Mapper.Map<MobileTransferDeviceDTO>(result.Payload);

                return new WebResponse<MobileTransferDeviceDTO>(dto);
            }
            catch (Exception ex)
            {
                return new WebResponse<MobileTransferDeviceDTO>(ex);
            }
        }

        public async Task<WebResponse<MobileTransferDeviceDTO>> InsertAsync(MobileTransferDeviceDTO item)
        {
            try
            {
                var entity = AutoMapper.Mapper.Map<MobileTransferDevice>(item);
                entity.Status = (int)MobileTransferDeviceStatus.Created;

                var result = await Manager.AddAsync(entity);

                await Manager.SaveAsync();

                var dto = AutoMapper.Mapper.Map<MobileTransferDeviceDTO>(result);

                return new WebResponse<MobileTransferDeviceDTO>(true, string.Empty, dto);
            }
            catch (Exception ex)
            {
                return new WebResponse<MobileTransferDeviceDTO>(ex);
            }
        }

        public async Task<WebResponse<MobileTransferDeviceDTO>> UpdateAsync(MobileTransferDeviceDTO item)
        {
            try
            {
                var entity = AutoMapper.Mapper.Map<MobileTransferDevice>(item);

                var oldItem = await Manager.GetEntityByIdAsync(item.Id);

                if (oldItem.Status != (int)MobileTransferDeviceStatus.Created)
                {
                    entity.PhoneNumber = oldItem.PhoneNumber;
                }
                entity.ExternalId = oldItem.ExternalId;
                entity.ExternalStatus = oldItem.ExternalStatus;
                entity.Status = oldItem.Status;
                entity.VerificationCode = oldItem.VerificationCode;
                entity.VerifiedDate = oldItem.VerifiedDate;
                entity.VerifyCodeSendDate = oldItem.VerifyCodeSendDate;
                entity.LastBlockDate = oldItem.LastBlockDate;

                var result = await Manager.UpdateAsync(entity);

                await Manager.SaveAsync();

                return new WebResponse<MobileTransferDeviceDTO>(AutoMapper.Mapper.Map<MobileTransferDeviceDTO>(result));
            }
            catch (Exception ex)
            {
                return new WebResponse<MobileTransferDeviceDTO>(ex);
            }
        }

        public async Task<WebResponse<MobileTransferDeviceDTO>> SendSmsAsync(int id)
        {
            try
            {
                var entity = await Manager.GetEntityByIdAsync(id);

                var response = await _MobileTransferService.SendSMSAsync(new MobileTransferSendSmsModel()
                {
                    MobileNo = entity.PhoneNumber,
                    ApiType = (int)ApiType.AsanPardakht
                });

                response.CheckError();

                entity.Status = (int)MobileTransferDeviceStatus.VerifyCodeSended;
                entity.VerifyCodeSendDate = DateTime.UtcNow;
                entity.ExternalId = response.Result.Id;
                entity.ExternalStatus = response.Result.Msg;

                var result = await Manager.UpdateAsync(entity);

                await Manager.SaveAsync();

                return new WebResponse<MobileTransferDeviceDTO>(AutoMapper.Mapper.Map<MobileTransferDeviceDTO>(result));
            }
            catch (Exception ex)
            {
                return new WebResponse<MobileTransferDeviceDTO>(ex);
            }
        }

        public async Task<WebResponse<MobileTransferDeviceDTO>> ActivateAsync(int id, string verificationCode)
        {
            try
            {
                var entity = await Manager.GetEntityByIdAsync(id);

                var response = await _MobileTransferService.ActivateDeviceAsync(new MobileTransferActivateDeviceModel
                {
                    MobileNo = entity.PhoneNumber,
                    VerificationCode = verificationCode,
                    ApiType = (int)ApiType.AsanPardakht
                });

                response.CheckError();

                entity.Status = (int)MobileTransferDeviceStatus.PhoneNumberVerified;
                entity.VerifiedDate = DateTime.UtcNow;
                entity.VerificationCode = verificationCode;

                var result = await Manager.UpdateAsync(entity);

                await Manager.SaveAsync();

                return new WebResponse<MobileTransferDeviceDTO>(AutoMapper.Mapper.Map<MobileTransferDeviceDTO>(result));
            }
            catch (Exception ex)
            {
                return new WebResponse<MobileTransferDeviceDTO>(ex);
            }
        }

        public async Task<WebResponse<MobileTransferDeviceDTO>> CheckStatusAsync(int id)
        {
            try
            {
                var entity = await Manager.GetEntityByIdAsync(id);

                var response = await _MobileTransferService.CheckStatusAsync(new MobileTransferCheckStatusModel
                {
                    MobileNo = entity.PhoneNumber,
                    ApiType = (int)ApiType.AsanPardakht
                });

                if (response.IsSuccess)
                {
                    if (response.Result.Msg == Helper.Registered)
                    {
                        entity.Status = (int)MobileTransferDeviceStatus.PhoneNumberVerified;
                    }
                    else if(entity.Status == (int)MobileTransferDeviceStatus.PhoneNumberVerified)
                    {
                        entity.Status = (int)MobileTransferDeviceStatus.Created;
                    }
                    entity.ExternalId = response.Result.Id;
                    entity.ExternalStatus = response.Result.Msg;
                }
                else
                {
                    entity.Status = (int)MobileTransferDeviceStatus.Error;
                    //entity.ExternalId = response.Result.Id;
                    entity.ExternalStatus = response.Error.Desc;
                }

                var result = await Manager.UpdateAsync(entity);

                await Manager.SaveAsync();

                response.CheckError();

                return new WebResponse<MobileTransferDeviceDTO>(AutoMapper.Mapper.Map<MobileTransferDeviceDTO>(result));
            }
            catch (Exception ex)
            {
                return new WebResponse<MobileTransferDeviceDTO>(ex);
            }
        }

        public async Task<WebResponse<MobileTransferDeviceDTO>> RemoveAsync(int id)
        {
            try
            {
                var entity = await Manager.GetEntityByIdAsync(id);

                var response = await _MobileTransferService.RemoveDeviceAsync(new MobileTransferRemoveDeviceModel
                {
                    MobileNo = entity.PhoneNumber,
                    ApiType = (int)ApiType.AsanPardakht
                });

                response.CheckError();

                entity.Status = (int)MobileTransferDeviceStatus.Removed;
                entity.ExternalStatus = response.Result.Msg;

                var result = await Manager.UpdateAsync(entity);

                await Manager.SaveAsync();

                return new WebResponse<MobileTransferDeviceDTO>(AutoMapper.Mapper.Map<MobileTransferDeviceDTO>(result));
            }
            catch (Exception ex)
            {
                return new WebResponse<MobileTransferDeviceDTO>(ex);
            }
        }
    }
}
