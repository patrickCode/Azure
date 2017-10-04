using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Core;
using System.Threading.Tasks;
using System.Threading;
using System.Security.Cryptography;

namespace Table.CSE
{
    public class GlobalKeyResolver : IKeyResolver
    {
        private readonly KeyVaultKeyResolver _resolver;
        private readonly KeyVaultProvider _provider;

        public GlobalKeyResolver(KeyVaultProvider provider)
        {
            _provider = provider;
            _resolver = new KeyVaultKeyResolver(_provider.Client);
        }

        public async Task<string> AddKey(string keyName, string key)
        {
            return await _provider.CreateSymmetricSecretAsync(keyName, key);
        }

        public async Task<IKey> ResolveKeyAsync(string kid, CancellationToken token)
        {
            var key = await _resolver.ResolveKeyAsync(kid, token);
            return key;
        }
    }
}