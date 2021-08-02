using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Application.Interfaces;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using Pardakht.PardakhtPay.Shared.Models.WebService;

namespace Pardakht.PardakhtPay.Application.Services
{
    public class UserSegmentGroupService : DatabaseServiceBase<UserSegmentGroup, IUserSegmentGroupManager>, IUserSegmentGroupService
    {
        IUserSegmentManager _SegmentManager = null;

        public UserSegmentGroupService(IUserSegmentGroupManager manager,
            ILogger<UserSegmentGroupService> logger,
            IUserSegmentManager segmentManager)
            :base(manager, logger)
        {
            _SegmentManager = segmentManager;
        }

        public async Task<WebResponse<List<UserSegmentGroupDTO>>> GetAllItemsAsync()
        {
            try
            {
                var result = await GetAllAsync();

                if (!result.Success)
                {
                    throw new Exception(result.Message);
                }

                var dtos = AutoMapper.Mapper.Map<List<UserSegmentGroupDTO>>(result.Payload);

                return new WebResponse<List<UserSegmentGroupDTO>>(true, string.Empty, dtos);
            }
            catch (Exception ex)
            {
                return new WebResponse<List<UserSegmentGroupDTO>>(ex);
            }
        }

        public async Task<WebResponse<UserSegmentGroupDTO>> GetItemById(int id)
        {
            try
            {
                var result = await GetEntityByIdAsync(id);

                if (!result.Success)
                {
                    throw result.Exception;
                }

                var dto = AutoMapper.Mapper.Map<UserSegmentGroupDTO>(result.Payload);

                var items = await _SegmentManager.GetItemsAsync(id);

                dto.Items = items.ConvertAll(t => AutoMapper.Mapper.Map<UserSegmentDTO>(t));

                return new WebResponse<UserSegmentGroupDTO>(dto);
            }
            catch (Exception ex)
            {
                return new WebResponse<UserSegmentGroupDTO>(ex);
            }
        }

        public async Task<WebResponse<UserSegmentGroupDTO>> InsertAsync(UserSegmentGroupDTO item)
        {
            try
            {
                var entity = AutoMapper.Mapper.Map<UserSegmentGroup>(item);
                entity.CreateDate = DateTime.UtcNow;

                var result = await Manager.AddAsync(entity);

                await Manager.SaveAsync();

                for (int i = 0; i < item.Items.Count; i++)
                {
                    var groupItem = AutoMapper.Mapper.Map<UserSegment>(item.Items[i]);

                    groupItem.UserSegmentGroupId = result.Id;

                    await _SegmentManager.AddAsync(groupItem);
                }

                await _SegmentManager.SaveAsync();

                var dto = AutoMapper.Mapper.Map<UserSegmentGroupDTO>(result);

                return new WebResponse<UserSegmentGroupDTO>(true, string.Empty, dto);
            }
            catch (Exception ex)
            {
                return new WebResponse<UserSegmentGroupDTO>(ex);
            }
        }

        public async Task<WebResponse<UserSegmentGroupDTO>> UpdateAsync(UserSegmentGroupDTO item)
        {
            try
            {
                var oldEntity = await Manager.GetEntityByIdAsync(item.Id);

                if (oldEntity.IsMalicious || oldEntity.IsDefault)
                {
                    throw new Exception("You can't edit the 'Malicious User Group' or 'Default Group'");
                }

                var entity = AutoMapper.Mapper.Map<UserSegmentGroup>(item);

                entity.IsMalicious = oldEntity.IsMalicious;
                entity.IsDefault = oldEntity.IsDefault;

                var result = await Manager.UpdateAsync(entity);

                var oldItems = await _SegmentManager.GetItemsAsync(item.Id);

                var deletedItems = oldItems.Where(t => !item.Items.Select(p => p.Id).Contains(t.Id)).ToList();

                for (int i = 0; i < deletedItems.Count; i++)
                {
                    await _SegmentManager.DeleteAsync(deletedItems[i]);
                }

                for (int i = 0; i < item.Items.Count; i++)
                {
                    var groupItem = AutoMapper.Mapper.Map<UserSegment>(item.Items[i]);

                    if (groupItem.Id == 0)
                    {
                        groupItem.UserSegmentGroupId = result.Id;
                        await _SegmentManager.AddAsync(groupItem);
                    }
                    else
                    {
                        await _SegmentManager.UpdateAsync(groupItem);
                    }
                }

                await Manager.SaveAsync();

                return new WebResponse<UserSegmentGroupDTO>(AutoMapper.Mapper.Map<UserSegmentGroupDTO>(result));
            }
            catch (Exception ex)
            {
                return new WebResponse<UserSegmentGroupDTO>(ex);
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
    }
}
