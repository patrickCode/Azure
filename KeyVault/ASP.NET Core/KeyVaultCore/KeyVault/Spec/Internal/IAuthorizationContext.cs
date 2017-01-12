using System.Threading.Tasks;

namespace Microsoft.KeyVault.Core.Spec.Internal
{
    public interface IAuthorizationContext
    {
        Task<string> GetTokenUsingCertificateAsync(string authority, string resource, string scope);
        Task<string> GetTokenUsingClientSecretAsync(string authority, string resource, string scope);
    }
}