using System.Security.Cryptography.X509Certificates;
using DFeSigner.Core.Signers;

namespace DFeSigner.Tests
{
    public class EventoNFeXmlSignerTests
    {
        private readonly string _nfePath = Path.Combine(AppContext.BaseDirectory, "Xml", "cancelamento-nfe.xml");
        private readonly string _certificatePath = Path.Combine(AppContext.BaseDirectory, "Certificates", "certificate.pfx");
        private readonly string _certificatePassword = "123";

        [Fact]
        public void Sign_ValidEventoCancelamentoNFeXmlAndCertificate_ReturnsSignedXml()
        {
            string xmlContent = File.ReadAllText(_nfePath);
            Assert.False(string.IsNullOrWhiteSpace(xmlContent), "O conteúdo do XML de exemplo da NF-e não pode ser vazio.");

            X509Certificate2 certificate = new X509Certificate2(_certificatePath, _certificatePassword, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);
            Assert.NotNull(certificate);

            EventoNFeXmlSigner signer = new EventoNFeXmlSigner();

            string signedXml = signer.Sign(xmlContent, certificate);

            Assert.False(string.IsNullOrWhiteSpace(signedXml));
            Assert.Contains("<Signature", signedXml);
        }
    }
}
