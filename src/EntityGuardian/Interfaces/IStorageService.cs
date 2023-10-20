using System.Threading.Tasks;

namespace EntityGuardian.Interfaces
{
    public interface IStorageService
    {
        void Install();

        Task CreateAsync();
    }
}
