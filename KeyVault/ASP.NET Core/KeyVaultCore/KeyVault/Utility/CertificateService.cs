using System;
using Microsoft.KeyVault.Core.Spec.Internal;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.KeyVault.Core.Utility
{
    public class CertificateService : ICertificateService
    {
        public X509Certificate2 FindCertificateByName(string certificateName, StoreLocation certificateStoreLocation = StoreLocation.LocalMachine, StoreName certificateStore = StoreName.My)
        {
            var store = new X509Store(certificateStore, certificateStoreLocation);
            store.Open(OpenFlags.ReadOnly);
            if (store.Certificates == null || store.Certificates.Count == 0)
                throw new Exception("No certificate found in the store");

            var certCollection = store.Certificates.Find(X509FindType.FindBySubjectName, certificateName, true);
            if (certCollection == null || certCollection.Count == 0)
                throw new Exception($"No certificate found by subject name {certificateName}");

            return certCollection[0];
        }

        public X509Certificate2 FindCertificateByThhumbprint(string certificateThumbprint, StoreLocation certificateStoreLocation = StoreLocation.LocalMachine, StoreName certificateStore = StoreName.My)
        {
            var store = new X509Store(certificateStore, certificateStoreLocation);
            store.Open(OpenFlags.ReadOnly);
            if (store.Certificates == null || store.Certificates.Count == 0)
                throw new Exception("No certificate found in the store");

            var certCollection = store.Certificates.Find(X509FindType.FindByThumbprint, certificateThumbprint, true);
            if (certCollection == null || certCollection.Count == 0)
                throw new Exception($"No certificate found by thumbprint {certificateThumbprint}");

            return certCollection[0];
        }
    }
}