using System.Threading.Tasks;

namespace Pardakht.UserManagement.Infrastructure.SqlRepository.SeedData
{
    public interface ISeedData
    {
        Task Seed(bool clearExistingData);
    }
}
