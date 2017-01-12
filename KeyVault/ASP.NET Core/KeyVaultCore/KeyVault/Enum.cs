using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.KeyVault.Core
{
    public enum AuthorizationMode
    {
        ClientSecret = 1,
        Certificate,
        Token
    }
}
