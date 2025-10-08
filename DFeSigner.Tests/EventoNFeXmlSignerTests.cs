using System.Security.Cryptography.X509Certificates;
using DFeSigner.Core.Exceptions;
using DFeSigner.Core.Signers;

namespace DFeSigner.Tests
{
    public class EventoNFeXmlSignerTests
    {
        private readonly string _cancelamentoNFePath = Path.Combine(AppContext.BaseDirectory, "Xml", "cancelamento-nfe.xml");
        private readonly string _certificatePath = Path.Combine(AppContext.BaseDirectory, "Certificates", "certificate.pfx");
        private readonly string _dfeValidPath = Path.Combine(AppContext.BaseDirectory, "Xml", "dfe-valid.xml");
        private readonly string _dfeInvalidPath = Path.Combine(AppContext.BaseDirectory, "Xml", "dfe-invalid.xml");
        private readonly string _certificateInvalidPath = Path.Combine(AppContext.BaseDirectory, "Certificates", "certificate.cer");
        private readonly string _certificatePassword = "123";
        
        private const string InvalidXmlWithoutReferenceId = "<evento versao=\"1.00\"><infEvento><cOrgao>43</cOrgao><tpAmb>2</tpAmb><CNPJ>12345678901234</CNPJ><chNFe>43250912345678901234550150000003821896145097</chNFe><dhEvento>2025-09-01T14:40:03-03:00</dhEvento><tpEvento>110111</tpEvento><nSeqEvento>1</nSeqEvento><verEvento>1.00</verEvento><detEvento versao=\"1.00\"><descEvento>Cancelamento</descEvento><nProt>143250002598645</nProt><xJust>Justificativa de Cancelamento</xJust></detEvento></infEvento></evento>";
        private const string InvalidXmlWithoutInfNFeElement = "<evento versao=\"1.00\"><infEvent Id=\"ID1101114325091234567890123455015000000382189614509701\"><cOrgao>43</cOrgao><tpAmb>2</tpAmb><CNPJ>12345678901234</CNPJ><chNFe>43250912345678901234550150000003821896145097</chNFe><dhEvento>2025-09-01T14:40:03-03:00</dhEvento><tpEvento>110111</tpEvento><nSeqEvento>1</nSeqEvento><verEvento>1.00</verEvento><detEvento versao=\"1.00\"><descEvento>Cancelamento</descEvento><nProt>143250002598645</nProt><xJust>Justificativa de Cancelamento</xJust></detEvento></infEvent></evento>";

        [Fact]
        public void Sign_ValidNFeXmlAndCertificate_IsSignatureValidReturnsTrue()
        {
            string xmlContent = File.ReadAllText(_cancelamentoNFePath);
            Assert.False(string.IsNullOrWhiteSpace(xmlContent), "O conteúdo do XML de exemplo da NF-e não pode ser vazio.");

            X509Certificate2 certificate = new X509Certificate2(_certificatePath, _certificatePassword, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);
            Assert.NotNull(certificate);

            EventoNFeXmlSigner signer = new EventoNFeXmlSigner();

            string signedXml = signer.Sign(xmlContent, certificate);

            Assert.False(string.IsNullOrWhiteSpace(signedXml));
            Assert.Contains("<Signature", signedXml);

            bool isSignatureValid = signer.IsSignatureValid(signedXml);
            Assert.True(isSignatureValid, "A assinatura digital do XML assinado deve ser válida.");
        }

        [Fact]
        public void Sign_InvalidXmlContentWithoutReferenceId_ThrowsInvalidOperationException()
        {
            string invalidXml = InvalidXmlWithoutReferenceId;

            X509Certificate2 certificate = new X509Certificate2(_certificatePath, _certificatePassword, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);
            EventoNFeXmlSigner signer = new EventoNFeXmlSigner();

            var ex = Assert.Throws<MissingReferenceIdException>(() => signer.Sign(invalidXml, certificate));
            Assert.Contains("O atributo 'Id' (referenceId) não foi encontrado ou está vazio no elemento 'infEvento'.", ex.Message);
        }

        [Fact]
        public void Sign_InvalidXmlContentWithoutElementInfNFe_ThrowsInvalidOperationException()
        {
            string invalidXml = InvalidXmlWithoutInfNFeElement;

            X509Certificate2 certificate = new X509Certificate2(_certificatePath, _certificatePassword, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);
            EventoNFeXmlSigner signer = new EventoNFeXmlSigner();

            var ex = Assert.Throws<InvalidXmlFormatException>(() => signer.Sign(invalidXml, certificate));
            Assert.Contains($"O XML fornecido não contém a tag raiz esperada para assinatura digital: 'nfe:infEvento'.", ex.Message);
        }

        [Fact]
        public void Sign_CertificateWithoutPrivateKey_ThrowsInvalidOperationException()
        {
            string xmlContent = File.ReadAllText(_cancelamentoNFePath);

            X509Certificate2 certificate = new X509Certificate2(_certificateInvalidPath);
            Assert.Null(certificate.GetRSAPrivateKey());

            EventoNFeXmlSigner signer = new EventoNFeXmlSigner();

            var ex = Assert.Throws<InvalidCertificateException>(() => signer.Sign(xmlContent, certificate));
            Assert.Contains("O certificado digital fornecido é inválido ou não possui uma chave privada acessível.", ex.Message);
        }

        [Fact]
        public void Sign_ValidXmlWithValidCertificate_ReturnsSignedXml()
        {
            string xmlContent = File.ReadAllText(_dfeValidPath);
            EventoNFeXmlSigner signer = new EventoNFeXmlSigner();

            var expected = signer.IsSignatureValid(xmlContent);
            Assert.True(expected, "A assinatura digital do XML assinado deve ser válida.");
        }

        [Fact]
        public void Sign_ValidXmlWithInvalidCertificate_ReturnsSignedXml()
        {
            string xmlContent = File.ReadAllText(_dfeInvalidPath);
            EventoNFeXmlSigner signer = new EventoNFeXmlSigner();

            var expected = signer.IsSignatureValid(xmlContent);
            Assert.False(expected, "A assinatura digital do XML assinado deve ser válida.");
        }
    }
}
