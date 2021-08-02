using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Application.Interfaces;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using Pardakht.PardakhtPay.Shared.Models.WebService;
using System.Linq;
using Pardakht.PardakhtPay.Shared.Interfaces;

namespace Pardakht.PardakhtPay.Application.Services
{
    public class CardToCardAccountGroupService: DatabaseServiceBase<CardToCardAccountGroup, ICardToCardAccountGroupManager>, ICardToCardAccountGroupService
    {
        ICardToCardAccountGroupItemManager _GroupItemManager = null;
        ICardToCardAccountManager _CardToCardAccountManager = null;

        public CardToCardAccountGroupService(ICardToCardAccountGroupManager manager,
            ILogger<CardToCardAccountGroupService> logger,
            ICardToCardAccountGroupItemManager groupItemManager,
            ICardToCardAccountManager cardToCardAccountManager)
            :base(manager, logger)
        {
            _GroupItemManager = groupItemManager;
            _CardToCardAccountManager = cardToCardAccountManager;
        }

        public async Task<WebResponse<List<CardToCardAccountGroupDTO>>> GetAllItemsAsync()
        {
            try
            {
                var result = await GetAllAsync();

                if (!result.Success)
                {
                    throw new Exception(result.Message);
                }

                var dtos = AutoMapper.Mapper.Map<List<CardToCardAccountGroupDTO>>(result.Payload);

                var items = await _GroupItemManager.GetAllAsync();

                var cardToCardAccounts = await _CardToCardAccountManager.GetAllAsync();

                for (int i = 0; i < dtos.Count; i++)
                {
                    StringBuilder str = new StringBuilder();

                    var accountItems = items.Where(t => t.CardToCardAccountGroupId == dtos[i].Id).ToList();

                    dtos[i].Accounts = str.ToString();
                }

                return new WebResponse<List<CardToCardAccountGroupDTO>>(true, string.Empty, dtos);
            }
            catch (Exception ex)
            {
                return new WebResponse<List<CardToCardAccountGroupDTO>>(ex);
            }
        }

        public async Task<WebResponse<CardToCardAccountGroupDTO>> GetItemById(int id)
        {

            try
            {
                var result = await GetEntityByIdAsync(id);

                if (!result.Success)
                {
                    throw result.Exception;
                }

                var dto = AutoMapper.Mapper.Map<CardToCardAccountGroupDTO>(result.Payload);

                var items = await _GroupItemManager.GetItemsAsync(id);

                items = items.OrderBy(t => t.Status).ToList();

                dto.Items = items.ConvertAll(t => AutoMapper.Mapper.Map<CardToCardAccountGroupItemDTO>(t));

                for (int i = 0; i < dto.Items.Count; i++)
                {
                    dto.Items[i].UserSegmentGroups = await _GroupItemManager.GetUserSegments(dto.Items[i].Id);
                }

                return new WebResponse<CardToCardAccountGroupDTO>(dto);
            }
            catch (Exception ex)
            {
                return new WebResponse<CardToCardAccountGroupDTO>(ex);
            }
        }

        public async Task<WebResponse<CardToCardAccountGroupDTO>> InsertAsync(CardToCardAccountGroupDTO item)
        {
            try
            {
                var entity = AutoMapper.Mapper.Map<CardToCardAccountGroup>(item);

                var result = await Manager.AddAsync(entity);

                await Manager.SaveAsync();

                for (int i = 0; i < item.Items.Count; i++)
                {
                    var groupItem = AutoMapper.Mapper.Map<CardToCardAccountGroupItem>(item.Items[i]);

                    groupItem.CardToCardAccountGroupId = result.Id;

                    await _GroupItemManager.AddAsync(groupItem, item.Items[i].UserSegmentGroups ?? new List<int>());
                }

                await _GroupItemManager.SaveAsync();

                var dto = AutoMapper.Mapper.Map<CardToCardAccountGroupDTO>(result);

                return new WebResponse<CardToCardAccountGroupDTO>(true, string.Empty, dto);
            }
            catch (Exception ex)
            {
                return new WebResponse<CardToCardAccountGroupDTO>(ex);
            }
        }

        public async Task<WebResponse<CardToCardAccountGroupDTO>> UpdateAsync(CardToCardAccountGroupDTO item)
        {
            try
            {
                var entity = AutoMapper.Mapper.Map<CardToCardAccountGroup>(item);

                var result = await Manager.UpdateAsync(entity);

                var oldItems = await _GroupItemManager.GetItemsAsync(item.Id);

                var deletedItems = oldItems.Where(t => !item.Items.Select(p => p.Id).Contains(t.Id)).ToList();

                for (int i = 0; i < deletedItems.Count; i++)
                {
                    await _GroupItemManager.DeleteAsync(deletedItems[i]);
                }

                for (int i = 0; i < item.Items.Count; i++)
                {
                    var groupItem = AutoMapper.Mapper.Map<CardToCardAccountGroupItem>(item.Items[i]);

                    if (groupItem.Id == 0)
                    {
                        groupItem.CardToCardAccountGroupId = result.Id;
                        await _GroupItemManager.AddAsync(groupItem, item.Items[i].UserSegmentGroups ?? new List<int>());
                    }
                    else
                    {
                        await _GroupItemManager.UpdateAsync(groupItem, item.Items[i].UserSegmentGroups ?? new List<int>());
                    }
                }

                await Manager.SaveAsync();

                return new WebResponse<CardToCardAccountGroupDTO>(AutoMapper.Mapper.Map<CardToCardAccountGroupDTO>(result));
            }
            catch (Exception ex)
            {
                return new WebResponse<CardToCardAccountGroupDTO>(ex);
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

        public async Task<WebResponse> CheckPausedAccounts()
        {
            try
            {
                await _GroupItemManager.CheckPausedAccounts();

                return new WebResponse();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);

                return new WebResponse(ex);
            }
        }
    }
}
