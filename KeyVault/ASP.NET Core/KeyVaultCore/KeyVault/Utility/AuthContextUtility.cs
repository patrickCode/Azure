using System.Threading.Tasks;
using Microsoft.KeyVault.Core.Spec;
using Microsoft.KeyVault.Core.Spec.Internal;
using System.Security.Cryptography.X509Certificates;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace Microsoft.KeyVault.Core.Utility
{
    public class AuthContextUtility: IAuthorizationContext
    {
        //Azure AD
        private readonly string _azureAdClientId;
        private readonly string _azureAdClientSecret;
        private readonly string _token;

        //Certificate
        private readonly string _appCertificateThumbprint;
        private readonly StoreLocation _certificateStoreLocation;
        private readonly StoreName _certificateStoreName;

        private readonly ICertificateService _certificateService;

        public AuthContextUtility(string adClientId, string adClientSecret)
        {
            _azureAdClientId = adClientId;
            _azureAdClientSecret = adClientSecret;
        }

        public AuthContextUtility(string adClientId, string certificateThumbprint, ICertificateService certificateService, StoreLocation certificateStoreLocation = StoreLocation.LocalMachine, StoreName certificateStore = StoreName.My)
        {
            _azureAdClientId = adClientId;
            _appCertificateThumbprint = certificateThumbprint;
            _certificateStoreLocation = certificateStoreLocation;
            _certificateStoreName = certificateStore;
            _certificateService = certificateService;
        }

        public async Task<string> GetTokenUsingCertificateAsync(string authority, string resource, string scope)
        {
            var authContext = new AuthenticationContext(authority);
            var certificate = _certificateService.FindCertificateByThhumbprint(_appCertificateThumbprint, _certificateStoreLocation, _certificateStoreName);
            IClientAssertionCertificate certificateAssertion = new ClientAssertionCertificate(_azureAdClientId, certificate);
            var authResult = await authContext.AcquireTokenAsync(resource, certificateAssertion);
            return authResult.AccessToken;
        }

        public async Task<string> GetTokenUsingClientSecretAsync(string authority, string resource, string scope)
        {
            var authContext = new AuthenticationContext(authority);
            var authResult = await authContext.AcquireTokenAsync(resource, new ClientCredential(_azureAdClientId, _azureAdClientSecret));
            return authResult.AccessToken;
        }
    }
}