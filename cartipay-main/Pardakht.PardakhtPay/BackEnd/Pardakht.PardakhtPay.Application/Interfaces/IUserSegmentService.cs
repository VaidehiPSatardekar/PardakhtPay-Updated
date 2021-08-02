using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;

namespace Pardakht.PardakhtPay.Application.Interfaces
{
    public interface IUserSegmentService : IServiceBase<UserSegment, IUserSegmentManager>
    {
    }
}
