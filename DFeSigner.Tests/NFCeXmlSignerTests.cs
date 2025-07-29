using System.Security.Cryptography.X509Certificates;
using DFeSigner.Core.Signers;

namespace DFeSigner.Tests
{
    public class NFCeXmlSignerTests
    {
        private readonly string _nfcePath = Path.Combine(AppContext.BaseDirectory, "nfce.xml");
        private readonly string _certificatePath = Path.Combine(AppContext.BaseDirectory, "certificate.pfx");
        private readonly string _certificatePassword = "123";

        [Fact]
        public void Sign_ValidNFCeXmlAndCertificate_ReturnsSignedXml()
        {
            string xmlContent = File.ReadAllText(_nfcePath);
            Assert.False(string.IsNullOrWhiteSpace(xmlContent));

            X509Certificate2 certificate = new X509Certificate2(_certificatePath, _certificatePassword, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);
            Assert.NotNull(certificate);

            NFCeXmlSigner signer = new NFCeXmlSigner();

            string signedXml = signer.Sign(xmlContent, certificate);

            Assert.False(string.IsNullOrWhiteSpace(signedXml));
            Assert.Contains("<Signature", signedXml);
        }

        [Fact]
        public void Sign_InvalidNFCeXmlContent_ThrowsArgumentException()
        {
            string invalidXml = "";
            X509Certificate2 certificate = new X509Certificate2(_certificatePath, _certificatePassword, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);
            NFCeXmlSigner signer = new NFCeXmlSigner();

            Assert.Throws<ArgumentException>(() => signer.Sign(invalidXml, certificate));
        }

        [Fact]
        public void Sign_NFeXmlPassedToNFCeSigner_ThrowsInvalidOperationException()
        {
            string nfeXmlContent = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "nfe.xml"));
            X509Certificate2 certificate = new X509Certificate2(_certificatePath, _certificatePassword, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);
            NFCeXmlSigner signer = new NFCeXmlSigner();

            Assert.Throws<InvalidOperationException>(() => signer.Sign(nfeXmlContent, certificate));
        }
    }
}