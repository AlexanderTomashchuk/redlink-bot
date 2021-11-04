using System.Threading.Tasks;

namespace Application.Common.Interfaces
{
    public interface ICurrentUserService
    {
        Task<long> GetUserIdAsync();
    }
}