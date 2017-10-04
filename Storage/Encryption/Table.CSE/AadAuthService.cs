using System;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace Table.CSE
{
    public class AadAuthService : IAadAuthService
    {
        private readonly string _clientId;
        private readonly string _certificateThumbprint;

        public AadAuthService(string clientId, string thumbprint)
        {
            _clientId = clientId;
            _certificateThumbprint = thumbprint;
        }

        public async Task<string> GetAccessTokenAsync(string authority, string resource, string scope)
        {
            var clientCertificate = GetClientCertificate();
            var certificateCredential = new ClientAssertionCertificate(_clientId, clientCertificate);
            var authenticationContext = new AuthenticationContext(authority, false);
            var result = await authenticationContext.AcquireTokenAsync(resource, certificateCredential);
            return result.AccessToken;
        }

        private X509Certificate2 GetClientCertificate()
        {
            var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadOnly);
            if (store.Certificates == null || store.Certificates.Count == 0)
                throw new Exception(string.Format("Certificate for thumbprint {0} not found !!", _certificateThumbprint));

            var x509CertificateCollection = store.Certificates.Find(X509FindType.FindByThumbprint, _certificateThumbprint, false);

            if (x509CertificateCollection == null || x509CertificateCollection.Count == 0)
                throw new Exception(string.Format("Certificate for thumbprint {0} not found !!", _certificateThumbprint));

            return x509CertificateCollection[0];
        }
    }
}