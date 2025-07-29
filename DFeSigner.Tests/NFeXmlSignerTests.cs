using System.Security.Cryptography.X509Certificates;
using DFeSigner.Core.Signers;

namespace DFeSigner.Tests
{
    public class NFeXmlSignerTests
    {
        private readonly string _nfePath = Path.Combine(AppContext.BaseDirectory, "nfe.xml");
        private readonly string _nfcePath = Path.Combine(AppContext.BaseDirectory, "nfce.xml");
        private readonly string _certificatePath = Path.Combine(AppContext.BaseDirectory, "certificate.pfx");
        private readonly string _certificatePassword = "123";

        [Fact]
        public void Sign_ValidNFeXmlAndCertificate_ReturnsSignedXml()
        {
            string xmlContent = File.ReadAllText(_nfePath);
            Assert.False(string.IsNullOrWhiteSpace(xmlContent), "O conteúdo do XML de exemplo da NF-e não pode ser vazio.");

            X509Certificate2 certificate = new X509Certificate2(_certificatePath, _certificatePassword, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);
            Assert.NotNull(certificate);

            NFeXmlSigner signer = new NFeXmlSigner();

            string signedXml = signer.Sign(xmlContent, certificate);

            Assert.False(string.IsNullOrWhiteSpace(signedXml));
            Assert.Contains("<Signature", signedXml);
        }

        [Fact]
        public void Sign_InvalidXmlContent_ThrowsArgumentException()
        {
            string invalidXml = "";
            X509Certificate2 certificate = new X509Certificate2(_certificatePath, _certificatePassword, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);
            NFeXmlSigner signer = new NFeXmlSigner();

            Assert.Throws<ArgumentException>(() => signer.Sign(invalidXml, certificate));
        }

        [Fact]
        public void Sign_NullCertificate_ThrowsArgumentNullException()
        {
            string xmlContent = File.ReadAllText(_nfePath);
            X509Certificate2 nullCertificate = null;
            NFeXmlSigner signer = new NFeXmlSigner();

            Assert.Throws<ArgumentNullException>(() => signer.Sign(xmlContent, nullCertificate));
        }

        [Fact]
        public void Sign_NFCeXmlPassedToNFeSigner_ThrowsInvalidOperationException()
        {
            string nfceXmlContent = File.ReadAllText(_nfcePath);
            X509Certificate2 certificate = new X509Certificate2(_certificatePath, _certificatePassword, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);
            NFeXmlSigner signer = new NFeXmlSigner();

            Assert.Throws<InvalidOperationException>(() => signer.Sign(nfceXmlContent, certificate));
        }
    }
}