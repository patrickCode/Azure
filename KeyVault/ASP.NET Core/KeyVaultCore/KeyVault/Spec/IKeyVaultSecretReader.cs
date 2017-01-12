using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.KeyVault.Core.Model;

namespace Microsoft.KeyVault.Core.Spec
{
    public interface IKeyVaultSecretReaderAsync
    {
        Task<Secret> GetSecretAsync(string secretKey, bool useCache = true);
        Task<List<Secret>> GetSecretsAsync(bool cacheSecrets = true);
    }
}