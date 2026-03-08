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
            var xmlContent = File.ReadAllText(_cancelamentoNFePath);
            var certificate = new X509Certificate2(_certificatePath, _certificatePassword, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);
            var sut = new EventoNFeXmlSigner();

            var expected = sut.Sign(xmlContent, certificate);

            Assert.False(string.IsNullOrWhiteSpace(expected));
            Assert.Contains("<Signature", expected);
            Assert.True(sut.IsSignatureValid(expected));
        }

        [Fact]
        public void Sign_InvalidXmlContentWithoutReferenceId_ThrowsInvalidOperationException()
        {
            var invalidXml = InvalidXmlWithoutReferenceId;
            var certificate = new X509Certificate2(_certificatePath, _certificatePassword, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);
            
            var sut = new EventoNFeXmlSigner();

            Assert.Throws<MissingReferenceIdException>(() => sut.Sign(invalidXml, certificate));
        }

        [Fact]
        public void Sign_InvalidXmlContentWithoutElementInfNFe_ThrowsInvalidOperationException()
        {
            var invalidXml = InvalidXmlWithoutInfNFeElement;
            var certificate = new X509Certificate2(_certificatePath, _certificatePassword, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);
            
            var sut = new EventoNFeXmlSigner();

            Assert.Throws<InvalidXmlFormatException>(() => sut.Sign(invalidXml, certificate));
        }

        [Fact]
        public void Sign_CertificateWithoutPrivateKey_ThrowsInvalidOperationException()
        {
            var xmlContent = File.ReadAllText(_cancelamentoNFePath);
            var certificate = new X509Certificate2(_certificateInvalidPath);

            var sut = new EventoNFeXmlSigner();

            Assert.Throws<InvalidCertificateException>(() => sut.Sign(xmlContent, certificate));
        }

        [Fact]
        public void Sign_ValidXmlWithValidCertificate_ReturnsSignedXml()
        {
            var xmlContent = File.ReadAllText(_dfeValidPath);
            var sut = new EventoNFeXmlSigner();

            var expected = sut.IsSignatureValid(xmlContent);
            
            Assert.True(expected);
        }

        [Fact]
        public void Sign_ValidXmlWithInvalidCertificate_ReturnsSignedXml()
        {
            var xmlContent = File.ReadAllText(_dfeInvalidPath);
            var sut = new EventoNFeXmlSigner();

            var expected = sut.IsSignatureValid(xmlContent);
            
            Assert.False(expected);
        }
    }
}
