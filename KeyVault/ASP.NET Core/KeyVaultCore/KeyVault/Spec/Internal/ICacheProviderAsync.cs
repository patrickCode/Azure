using System.Threading.Tasks;

namespace Microsoft.KeyVault.Core.Spec.Internal
{
    public interface ICacheProviderAsync
    {
        Task<bool> ExistsAsync(string key);
        Task<string> GetAsync(string key);
        Task SetAsync(string key, string value);
    }
}