using System.Text;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography.X509Certificates;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace Microsoft.KeyVault.Core.Utility
{
    public class ClientAssertionCertificate : IClientAssertionCertificate
    {
        private X509Certificate2 _certificate;
        public string ClientId { get; private set; }

        public ClientAssertionCertificate(string clientId, X509Certificate2 certificate)
        {
            ClientId = clientId;
            _certificate = certificate;
        }

        public string Thumbprint
        {
            get
            {
                return Base64UrlEncoder.Encode(_certificate.GetCertHash());
            }
        }

        public byte[] Sign(string message)
        {
            using (var key = _certificate.GetRSAPrivateKey())
            {
                return key.SignData(Encoding.UTF8.GetBytes(message), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            }
        }
    }
}