using System.Security.Cryptography.X509Certificates;

namespace Microsoft.KeyVault.Core.Spec.Internal
{
    public interface ICertificateService
    {
        X509Certificate2 FindCertificateByThhumbprint(string certificateThumbprint, StoreLocation certificateStoreLocation = StoreLocation.LocalMachine, StoreName certificateStore = StoreName.My);
        X509Certificate2 FindCertificateByName(string certificateName, StoreLocation certificateStoreLocation = StoreLocation.LocalMachine, StoreName certificateStore = StoreName.My);
    }
}