using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Azure.KeyVault.Core;
using Microsoft.Azure.KeyVault;

namespace Table.CSE
{
    public class LocalKeyResolver : IKeyResolver
    {
        private Dictionary<string, IKey> keys = new Dictionary<string, IKey>();

        public void AddKey(IKey key)
        {
            keys[key.Kid] = key;
        }
        
        public string AddKey(string key)
        {
            var rsaKey = new RsaKey(key);
            keys[rsaKey.Kid] = rsaKey;
            return rsaKey.Kid;
        }

        public async Task<IKey> ResolveKeyAsync(string kid, CancellationToken token)
        {
            keys.TryGetValue(kid, out IKey result);
            return await Task.FromResult(result);
        }

        public async Task<IKey> ResolveKeyAsync(string kid)
        {
            keys.TryGetValue(kid, out IKey result);
            return await Task.FromResult(result);
        }
    }
}