using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Application.Interfaces;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using Pardakht.PardakhtPay.Shared.Models.WebService;
using System.Linq;

namespace Pardakht.PardakhtPay.Application.Services
{
    public class RiskyKeywordService : DatabaseServiceBase<RiskyKeyword, IRiskyKeywordManager>, IRiskyKeywordService
    {
        public RiskyKeywordService(IRiskyKeywordManager manager, ILogger<RiskyKeywordService> logger):base(manager, logger)
        {

        }

        public async Task<WebResponse<string[]>> GetAll()
        {
            try
            {
                var items = await Manager.GetAllAsync();

                var keywords = items.Select(t => t.Keyword).ToArray();

                return new WebResponse<string[]>(keywords);
            }
            catch (Exception ex)
            {
                return new WebResponse<string[]>(ex);
            }
        }

        public async Task<WebResponse<string[]>> Update(string[] values)
        {
            try
            {
                var items = await Manager.GetAllAsync();

                values = values.Distinct().ToArray();

                var deleteds = items.Where(p => !values.Contains(p.Keyword)).ToList();

                for (int i = 0; i < deleteds.Count; i++)
                {
                    await Manager.DeleteAsync(deleteds[i]);
                }

                var news = values.Where(t => !items.Any(p => t == p.Keyword)).ToList();

                for (int i = 0; i < news.Count; i++)
                {
                    var item = new RiskyKeyword();
                    item.Keyword = news[i];

                    await Manager.AddAsync(item);
                }

                await Manager.SaveAsync();

                return new WebResponse<string[]>(values);
            }
            catch (Exception ex)
            {
                return new WebResponse<string[]>(ex);
            }
        }
    }
}
