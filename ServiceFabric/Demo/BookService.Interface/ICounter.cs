using Microsoft.ServiceFabric.Services.Remoting;
using System.Threading.Tasks;

namespace BookService.Interface
{
    public interface ICounter: IService
    {
        Task<long> GetCounterAsync();
    }
}