using System.Threading.Tasks;

namespace Table.CSE
{
    public interface IAadAuthService
    {
        Task<string> GetAccessTokenAsync(string authority, string resource, string scope);
    }
}