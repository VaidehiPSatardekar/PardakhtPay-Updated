using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Application.Interfaces;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Shared.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using Pardakht.PardakhtPay.Shared.Models.WebService;
using Pardakht.PardakhtPay.Shared.Models.WebService.MobileTransfer;

namespace Pardakht.PardakhtPay.Application.Services
{
    public class MobileTransferCardAccountGroupService: DatabaseServiceBase<MobileTransferCardAccountGroup, IMobileTransferCardAccountGroupManager>, IMobileTransferCardAccountGroupService
    {
        IMobileTransferCardAccountGroupItemManager _GroupItemManager = null;
        ICachedObjectManager _CachedObjectManager = null;

        public MobileTransferCardAccountGroupService(IMobileTransferCardAccountGroupManager manager,
            ILogger<MobileTransferCardAccountGroup> logger,
            IMobileTransferCardAccountGroupItemManager groupItemManager,
            ICachedObjectManager cachedObjectManager):base(manager, logger)
        {
            _GroupItemManager = groupItemManager;
            _CachedObjectManager = cachedObjectManager;
        }

        public async Task<WebResponse<List<MobileTransferCardAccountGroupDTO>>> GetAllItemsAsync()
        {
            try
            {
                var result = await GetAllAsync();

                if (!result.Success)
                {
                    throw new Exception(result.Message);
                }

                var dtos = AutoMapper.Mapper.Map<List<MobileTransferCardAccountGroupDTO>>(result.Payload);

                return new WebResponse<List<MobileTransferCardAccountGroupDTO>>(true, string.Empty, dtos);
            }
            catch (Exception ex)
            {
                return new WebResponse<List<MobileTransferCardAccountGroupDTO>>(ex);
            }
        }

        public async Task<WebResponse<MobileTransferCardAccountGroupDTO>> GetItemById(int id)
        {

            try
            {
                var result = await GetEntityByIdAsync(id);

                if (!result.Success)
                {
                    throw result.Exception;
                }

                var dto = AutoMapper.Mapper.Map<MobileTransferCardAccountGroupDTO>(result.Payload);

                var items = await _GroupItemManager.GetItemsAsync(t => t.GroupId == id);

                items = items.OrderByDescending(t => t.Status).ToList();

                dto.Items = items.ConvertAll(t => AutoMapper.Mapper.Map<MobileTransferCardAccountGroupItemDTO>(t));

                for (int i = 0; i < dto.Items.Count; i++)
                {
                    dto.Items[i].UserSegmentGroups = await _GroupItemManager.GetUserSegments(dto.Items[i].Id);
                }

                return new WebResponse<MobileTransferCardAccountGroupDTO>(dto);
            }
            catch (Exception ex)
            {
                return new WebResponse<MobileTransferCardAccountGroupDTO>(ex);
            }
        }

        public async Task<WebResponse<MobileTransferCardAccountGroupDTO>> InsertAsync(MobileTransferCardAccountGroupDTO item)
        {
            try
            {
                var entity = AutoMapper.Mapper.Map<MobileTransferCardAccountGroup>(item);

                var result = await Manager.AddAsync(entity);

                await Manager.SaveAsync();

                for (int i = 0; i < item.Items.Count; i++)
                {
                    var groupItem = AutoMapper.Mapper.Map<MobileTransferCardAccountGroupItem>(item.Items[i]);

                    groupItem.GroupId = result.Id;

                    await _GroupItemManager.AddAsync(groupItem, item.Items[i].UserSegmentGroups ?? new List<int>());
                }

                await _GroupItemManager.SaveAsync();

                var dto = AutoMapper.Mapper.Map<MobileTransferCardAccountGroupDTO>(result);

                return new WebResponse<MobileTransferCardAccountGroupDTO>(true, string.Empty, dto);
            }
            catch (Exception ex)
            {
                return new WebResponse<MobileTransferCardAccountGroupDTO>(ex);
            }
        }

        public async Task<WebResponse<MobileTransferCardAccountGroupDTO>> UpdateAsync(MobileTransferCardAccountGroupDTO item)
        {
            try
            {
                var entity = AutoMapper.Mapper.Map<MobileTransferCardAccountGroup>(item);

                var result = await Manager.UpdateAsync(entity);

                var oldItems = await _GroupItemManager.GetItemsAsync(t => t.GroupId == item.Id);

                var deletedItems = oldItems.Where(t => !item.Items.Select(p => p.Id).Contains(t.Id)).ToList();

                for (int i = 0; i < deletedItems.Count; i++)
                {
                    await _GroupItemManager.DeleteAsync(deletedItems[i]);
                }

                for (int i = 0; i < item.Items.Count; i++)
                {
                    var groupItem = AutoMapper.Mapper.Map<MobileTransferCardAccountGroupItem>(item.Items[i]);

                    if (groupItem.Id == 0)
                    {
                        groupItem.GroupId = result.Id;
                        await _GroupItemManager.AddAsync(groupItem, item.Items[i].UserSegmentGroups ?? new List<int>());
                        //await _GroupItemManager.AddAsync(groupItem);
                    }
                    else
                    {
                        //await _GroupItemManager.UpdateAsync(groupItem);
                        await _GroupItemManager.UpdateAsync(groupItem, item.Items[i].UserSegmentGroups ?? new List<int>());
                    }
                }

                await Manager.SaveAsync();

                return new WebResponse<MobileTransferCardAccountGroupDTO>(AutoMapper.Mapper.Map<MobileTransferCardAccountGroupDTO>(result));
            }
            catch (Exception ex)
            {
                return new WebResponse<MobileTransferCardAccountGroupDTO>(ex);
            }
        }

        public override async Task<WebResponse> DeleteAsync(int id)
        {
            try
            {
                await Manager.SoftDeleteAsync(id);

                await Manager.SaveAsync();

                return new WebResponse();
            }
            catch (Exception ex)
            {
                return new WebResponse(ex);
            }
        }

        public async Task ClearCache()
        {
            await _CachedObjectManager.ClearCachedItems<MobileTransferCardAccountGroupItem>();
            await _CachedObjectManager.ClearCachedItems<MobileCardAccountUserSegmentRelation>();
        }
    }
}
