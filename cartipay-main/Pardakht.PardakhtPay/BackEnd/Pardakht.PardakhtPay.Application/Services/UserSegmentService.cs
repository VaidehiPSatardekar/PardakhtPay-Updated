using Microsoft.Extensions.Logging;
using Pardakht.PardakhtPay.Application.Interfaces;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;

namespace Pardakht.PardakhtPay.Application.Services
{
    public class UserSegmentService : DatabaseServiceBase<UserSegment, IUserSegmentManager>, IUserSegmentService
    {
        public UserSegmentService(IUserSegmentManager manager, ILogger<UserSegmentService> logger)
            :base(manager, logger)
        {

        }
    }
}
