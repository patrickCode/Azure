using System.Threading.Tasks;
using Microsoft.Azure.KeyVault;
using System.Collections.Generic;
using Microsoft.KeyVault.Core.Spec;
using Microsoft.KeyVault.Core.Model;
using Microsoft.KeyVault.Core.Utility;
using Microsoft.KeyVault.Core.Spec.Internal;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.KeyVault.Core
{
    public class KeyVaultProvider : IKeyVaultSecretReaderAsync
    {
        //Vault Name
        private readonly string _vaultName;
        private readonly string _vaultUri;
        private readonly KeyVaultClient _keyVaultClient;
        private readonly AuthorizationMode _authorizationMode;

        //Azure AD
        private readonly string _azureAdClientId;
        private readonly string _azureAdClientSecret;
        private readonly string _token;

        //Certificate
        private readonly string _appCertificateThumbprint;
        private readonly StoreLocation _certificateStoreLocation;
        private readonly StoreName _certificateStoreName;

        //Internal 
        private readonly ICertificateService _certificateService;
        private readonly IAuthorizationContext _authorizationContext;
        private readonly ICacheProviderAsync _cache;


        public KeyVaultProvider(string keyVaultName, string adClientId, string adClientSecret)
        {
            _vaultName = keyVaultName;
            _vaultUri = string.Format("https://{0}.vault.azure.net", _vaultName);

            _azureAdClientId = adClientId;
            _azureAdClientSecret = adClientSecret;
            _authorizationMode = AuthorizationMode.ClientSecret;

            _cache = new InMemoryCache();
            _authorizationContext = new AuthContextUtility(_azureAdClientId, _azureAdClientSecret);
            _keyVaultClient = new KeyVaultClient(_authorizationContext.GetTokenUsingClientSecretAsync);
        }

        public KeyVaultProvider(string keyVaultName, string adClientId, string certificateThumbprint, StoreLocation certificateStoreLocation = StoreLocation.LocalMachine, StoreName certificateStore = StoreName.My)
        {
            _vaultName = keyVaultName;
            _vaultUri = string.Format("https://{0}.vault.azure.net", _vaultName);

            _azureAdClientId = adClientId;
            _appCertificateThumbprint = certificateThumbprint;
            _certificateStoreLocation = certificateStoreLocation;
            _certificateStoreName = certificateStore;
            _authorizationMode = AuthorizationMode.Certificate;

            _cache = new InMemoryCache();
            _certificateService = new CertificateService();
            _authorizationContext = new AuthContextUtility(_azureAdClientId, _appCertificateThumbprint, _certificateService, _certificateStoreLocation, _certificateStoreName);
            _keyVaultClient = new KeyVaultClient(_authorizationContext.GetTokenUsingCertificateAsync);
        }

        public KeyVaultProvider(string keyVaultName, string authToken)
        {
            _vaultName = keyVaultName;
            _token = authToken;
            _authorizationMode = AuthorizationMode.Token;

            _cache = new InMemoryCache();
            _keyVaultClient = new KeyVaultClient(GetTokenAsync);
        }

        public async Task<Secret> GetSecretAsync(string secretKey, bool useCache = true)
        {
            var secret = string.Empty;
            if (useCache)
            {
                if (await _cache.ExistsAsync(secretKey))
                {
                    secret = await _cache.GetAsync(secretKey);
                    return new Secret(secretKey, secret);
                }

            }

            secret = (await _keyVaultClient.GetSecretAsync(_vaultUri, secretKey)).Value;
            if (useCache)
                await _cache.SetAsync(secretKey, secret);

            return new Secret(secretKey, secret);
        }

        public async Task<List<Secret>> GetSecretsAsync(bool cacheSecrets = true)
        {
            var secrets = new List<Secret>();
            var nextPageLink = string.Empty;
            do
            {
                var page = string.IsNullOrEmpty(nextPageLink)
                    ? await _keyVaultClient.GetSecretsAsync(_vaultUri)
                    : await _keyVaultClient.GetSecretsNextAsync(nextPageLink);

                foreach (var secret in page)
                {
                    var model = new Secret(secret.Id, secret.Identifier.Name);
                    secrets.Add(model);
                    if (cacheSecrets)
                        await _cache.SetAsync(model.Key, model.Value);
                }

            } while (!string.IsNullOrEmpty(nextPageLink));

            return secrets;
        }

        private async Task<string> GetTokenAsync(string authority, string resource, string scope)
        {
            return await Task.Run(() => {
                return _token;
            });
        }
    }
}