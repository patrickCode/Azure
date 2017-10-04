using System;
using System.Threading.Tasks;
using Microsoft.Azure.KeyVault;
using System.Text;
using System.Security.Cryptography;

namespace Table.CSE
{
    public class KeyVaultProvider
    {
        private readonly string _vaultUri;
        private readonly IAadAuthService _authService;
        private static readonly byte[] Salt = new byte[] { 10, 20, 30, 40, 50, 60, 70, 80 };

        public KeyVaultClient Client { get; set; }

        public KeyVaultProvider(string vaultUri, IAadAuthService authService)
        {
            _vaultUri = vaultUri;
            _authService = authService;
            Client = new KeyVaultClient(authService.GetAccessTokenAsync);
        }

        public async Task<string> CreateSymmetricSecretAsync(string secretName, string secretValue)
        {
            try
            {
                //await Client.DeleteSecretAsync(_vaultUri, secretName);
                var secret = await Client.GetSecretAsync(_vaultUri, secretName);
                return secret.SecretIdentifier.BaseIdentifier;
            }
            catch (Exception error)
            {
                if (error.Message == $"Secret not found: {secretName}")
                {
                    var keyBytes = CreateKey(secretValue, 32);
                    var keyStr = Convert.ToBase64String(keyBytes);
                    
                    //var cloudSecret = await Client.SetSecretAsync(_vaultUri, secretName, "qwerttyuiop12134567890", null, "application/octet-stream");
                    var cloudSecret = await Client.SetSecretAsync(_vaultUri, secretName, keyStr, null, "application/octet-stream");
                    return cloudSecret.SecretIdentifier.BaseIdentifier;
                }
                throw;
            }
        }

        public async Task<string> GetSecretIdAsync(string secretName)
        {
            var secret = await Client.GetSecretAsync(_vaultUri, secretName);
            return secret.SecretIdentifier.BaseIdentifier;
        }

        private static byte[] CreateKey(string password, int keyBytes = 32)
        {
            const int Iterations = 300;
            var keyGenerator = new Rfc2898DeriveBytes(password, Salt, Iterations);
            return keyGenerator.GetBytes(keyBytes);
        }
    }
}