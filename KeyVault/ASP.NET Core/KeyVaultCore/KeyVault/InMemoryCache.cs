using Microsoft.KeyVault.Core.Spec.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.KeyVault.Core
{
    public class InMemoryCache : ICacheProviderAsync
    {
        public Task<bool> ExistsAsync(string key)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetAsync(string key)
        {
            throw new NotImplementedException();
        }

        public Task SetAsync(string key, string value)
        {
            throw new NotImplementedException();
        }
    }
}
