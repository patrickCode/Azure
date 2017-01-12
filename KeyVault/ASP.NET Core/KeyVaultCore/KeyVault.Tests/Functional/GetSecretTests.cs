using Microsoft.KeyVault.Core.Spec;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Microsoft.KeyVault.Core.Tests.Functional
{
    [TestClass]
    public class GetSecretTests
    {
        //Vault Name
        private readonly string _vaultName;
        

        //Azure AD
        private readonly string _azureAdClientId;
        private readonly string _azureAdClientSecret;
        private readonly string _token;

        //Certificate
        private readonly string _appCertificateThumbprint;
        private readonly StoreLocation _certificateStoreLocation;
        private readonly StoreName _certificateStoreName;

        public GetSecretTests()
        {
            //Configure Here
            _vaultName = "kv-phoenix-identity-dev";

            _azureAdClientId = "118223c1-2281-4edd-8f0d-0877d2958bef";
            _azureAdClientSecret = "Jvj33AJ1mwCsgCuDDMQFaNnKz35aRzEm+IWR3cdKwZc=";

            _appCertificateThumbprint = "11A40AE48EE41325A8E5FDA45D3A2B7897196CDD";
            _certificateStoreLocation = StoreLocation.LocalMachine;
            _certificateStoreName = StoreName.My;
        }

        #region Positive Test Cases
        #region Certificate
        
        [TestMethod]
        public async Task Secret_GetByCertificate()
        {
            //Arrange
            IKeyVaultSecretReaderAsync keyVaultProvider = new KeyVaultProvider(_vaultName, _azureAdClientId, _appCertificateThumbprint, _certificateStoreLocation, _certificateStoreName);
            var secretKey = "secret-storage-key";
            var expectedSecret = "";

            //Act
            var secret = await keyVaultProvider.GetSecretAsync(secretKey, false);

            //Assert
            Assert.IsNotNull(secret);
            Assert.IsNotNull(secret.Value);
            Assert.AreEqual(expectedSecret, secret.Value);
        }

        #endregion
        #endregion

    }
}
