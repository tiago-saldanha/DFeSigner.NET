using System.Security.Cryptography.X509Certificates;
using DFeSigner.Core.Signers;

namespace DFeSigner.Tests
{
    public class NFeXmlSignerTests
    {
        private readonly string _nfePath = Path.Combine(AppContext.BaseDirectory, "nfe.xml");
        private readonly string _certificatePath = Path.Combine(AppContext.BaseDirectory, "certificate.pfx");
        private readonly string _certificatePassword = "123";

        [Fact]
        public void Sign_ValidNFeXmlAndCertificate_ReturnsSignedXml()
        {
            string xmlContent = File.ReadAllText(_nfePath);
            Assert.False(string.IsNullOrWhiteSpace(xmlContent));

            X509Certificate2 x509Certificate2 = new X509Certificate2(_certificatePath, _certificatePassword, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);
            Assert.NotNull(x509Certificate2);

            NFeXmlSigner nfeXmlSigner = new NFeXmlSigner();
            string signedXml = nfeXmlSigner.Sign(xmlContent, x509Certificate2);

            Assert.False(string.IsNullOrWhiteSpace(signedXml));
            Assert.Contains("<Signature", signedXml);
        }
    }
}