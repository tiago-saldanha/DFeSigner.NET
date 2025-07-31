using DFeSigner.Core.Exceptions;
using DFeSigner.Core.Signers;
using System.Security.Cryptography.X509Certificates;

namespace DFeSigner.Tests
{
    public class MDFeXmlSignerTests
    {
        private readonly string _mdfePath = Path.Combine(AppContext.BaseDirectory, "Xml", "mdfe.xml");
        private readonly string _nfePath = Path.Combine(AppContext.BaseDirectory, "Xml", "nfe.xml");
        private readonly string _dfeValidPath = Path.Combine(AppContext.BaseDirectory, "Xml", "dfe-valid.xml");
        private readonly string _certificatePath = Path.Combine(AppContext.BaseDirectory, "Certificates", "certificate.pfx");
        private readonly string _certificateInvalidPath = Path.Combine(AppContext.BaseDirectory, "Certificates", "certificate.cer");
        private readonly string _certificatePassword = "123";

        private const string InvalidXmlWithoutReferenceId = "<MDFe xmlns=\"http://www.portalfiscal.inf.br/mdfe\"><infMDFe versao=\"3.00\" ><ide><cUF>43</cUF><tpAmb>2</tpAmb><tpEmit>2</tpEmit><mod>58</mod><serie>1</serie><nMDF>417</nMDF><cMDF>94986538</cMDF><cDV>6</cDV><modal>1</modal><dhEmi>2023-09-25T14:25:00-03:00</dhEmi><tpEmis>1</tpEmis><procEmi>0</procEmi><verProc>_5_55</verProc><UFIni>RS</UFIni><UFFim>SP</UFFim><infMunCarrega><cMunCarrega>4314902</cMunCarrega><xMunCarrega>Porto Alegre</xMunCarrega></infMunCarrega><infPercurso><UFPer>SC</UFPer></infPercurso><infPercurso><UFPer>PR</UFPer></infPercurso><dhIniViagem>2021-07-21T00:00:00-03:00</dhIniViagem></ide><emit><CNPJ>12345678901234</CNPJ><IE>9999999999</IE><xNome>TRANSPORTADORA FICTICIA LTDA</xNome><xFant>TRANSFICT</xFant><enderEmit><xLgr>Rua Ficticia do Emissor</xLgr><nro>1234</nro><xBairro>Bairro Central</xBairro><cMun>4314902</cMun><xMun>Porto Alegre</xMun><CEP>90000000</CEP><UF>RS</UF><fone>51999999999</fone><email>contato@ficticia.com.br</email></enderEmit></emit><infModal versaoModal=\"3.00\"><rodo><veicTracao><placa>ABC5678</placa><tara>400</tara><condutor><xNome>MOTORISTA FICTICIO</xNome><CPF>99999999999</CPF></condutor><tpRod>01</tpRod><tpCar>01</tpCar><UF>RS</UF></veicTracao></rodo></infModal><infDoc><infMunDescarga><cMunDescarga>4314902</cMunDescarga><xMunDescarga>Porto Alegre</xMunDescarga><infNFe><chNFe>43210712345678901234550030000015091684574043</chNFe></infNFe></infMunDescarga></infDoc><tot><qNFe>1</qNFe><vCarga>100.00</vCarga><cUnid>01</cUnid><qCarga>12.0000</qCarga></tot></infMDFe><infMDFeSupl><qrCodMDFe>https://dfe-portal.svrs.rs.gov.br/mdfe/qrCode?chMDFe=43230912345678901234580010000004171949865386&amp;tpAmb=2</qrCodMDFe></infMDFeSupl></MDFe>";
        private const string InvalidXmlWithoutInfMDFeElement = "<MDFe xmlns=\"http://www.portalfiscal.inf.br/mdfe\"><infMDFa Id=\"MDFe43230912345678901234580010000004171949865386\" versao=\"3.00\" ><ide><cUF>43</cUF><tpAmb>2</tpAmb><tpEmit>2</tpEmit><mod>58</mod><serie>1</serie><nMDF>417</nMDF><cMDF>94986538</cMDF><cDV>6</cDV><modal>1</modal><dhEmi>2023-09-25T14:25:00-03:00</dhEmi><tpEmis>1</tpEmis><procEmi>0</procEmi><verProc>_5_55</verProc><UFIni>RS</UFIni><UFFim>SP</UFFim><infMunCarrega><cMunCarrega>4314902</cMunCarrega><xMunCarrega>Porto Alegre</xMunCarrega></infMunCarrega><infPercurso><UFPer>SC</UFPer></infPercurso><infPercurso><UFPer>PR</UFPer></infPercurso><dhIniViagem>2021-07-21T00:00:00-03:00</dhIniViagem></ide><emit><CNPJ>12345678901234</CNPJ><IE>9999999999</IE><xNome>TRANSPORTADORA FICTICIA LTDA</xNome><xFant>TRANSFICT</xFant><enderEmit><xLgr>Rua Ficticia do Emissor</xLgr><nro>1234</nro><xBairro>Bairro Central</xBairro><cMun>4314902</cMun><xMun>Porto Alegre</xMun><CEP>90000000</CEP><UF>RS</UF><fone>51999999999</fone><email>contato@ficticia.com.br</email></enderEmit></emit><infModal versaoModal=\"3.00\"><rodo><veicTracao><placa>ABC5678</placa><tara>400</tara><condutor><xNome>MOTORISTA FICTICIO</xNome><CPF>99999999999</CPF></condutor><tpRod>01</tpRod><tpCar>01</tpCar><UF>RS</UF></veicTracao></rodo></infModal><infDoc><infMunDescarga><cMunDescarga>4314902</cMunDescarga><xMunDescarga>Porto Alegre</xMunDescarga><infNFe><chNFe>43210712345678901234550030000015091684574043</chNFe></infNFe></infMunDescarga></infDoc><tot><qNFe>1</qNFe><vCarga>100.00</vCarga><cUnid>01</cUnid><qCarga>12.0000</qCarga></tot></infMDFa><infMDFeSupl><qrCodMDFe>https://dfe-portal.svrs.rs.gov.br/mdfe/qrCode?chMDFe=43230912345678901234580010000004171949865386&amp;tpAmb=2</qrCodMDFe></infMDFeSupl></MDFe>";
        private const string InvalidXmlWithoutIdeElement = "<MDFe xmlns=\"http://www.portalfiscal.inf.br/mdfe\"><infMDFe Id=\"MDFe43230912345678901234580010000004171949865386\" versao=\"3.00\" ><ida><cUF>43</cUF><tpAmb>2</tpAmb><tpEmit>2</tpEmit><mod>58</mod><serie>1</serie><nMDF>417</nMDF><cMDF>94986538</cMDF><cDV>6</cDV><modal>1</modal><dhEmi>2023-09-25T14:25:00-03:00</dhEmi><tpEmis>1</tpEmis><procEmi>0</procEmi><verProc>_5_55</verProc><UFIni>RS</UFIni><UFFim>SP</UFFim><infMunCarrega><cMunCarrega>4314902</cMunCarrega><xMunCarrega>Porto Alegre</xMunCarrega></infMunCarrega><infPercurso><UFPer>SC</UFPer></infPercurso><infPercurso><UFPer>PR</UFPer></infPercurso><dhIniViagem>2021-07-21T00:00:00-03:00</dhIniViagem></ida><emit><CNPJ>12345678901234</CNPJ><IE>9999999999</IE><xNome>TRANSPORTADORA FICTICIA LTDA</xNome><xFant>TRANSFICT</xFant><enderEmit><xLgr>Rua Ficticia do Emissor</xLgr><nro>1234</nro><xBairro>Bairro Central</xBairro><cMun>4314902</cMun><xMun>Porto Alegre</xMun><CEP>90000000</CEP><UF>RS</UF><fone>51999999999</fone><email>contato@ficticia.com.br</email></enderEmit></emit><infModal versaoModal=\"3.00\"><rodo><veicTracao><placa>ABC5678</placa><tara>400</tara><condutor><xNome>MOTORISTA FICTICIO</xNome><CPF>99999999999</CPF></condutor><tpRod>01</tpRod><tpCar>01</tpCar><UF>RS</UF></veicTracao></rodo></infModal><infDoc><infMunDescarga><cMunDescarga>4314902</cMunDescarga><xMunDescarga>Porto Alegre</xMunDescarga><infNFe><chNFe>43210712345678901234550030000015091684574043</chNFe></infNFe></infMunDescarga></infDoc><tot><qNFe>1</qNFe><vCarga>100.00</vCarga><cUnid>01</cUnid><qCarga>12.0000</qCarga></tot></infMDFe><infMDFeSupl><qrCodMDFe>https://dfe-portal.svrs.rs.gov.br/mdfe/qrCode?chMDFe=43230912345678901234580010000004171949865386&amp;tpAmb=2</qrCodMDFe></infMDFeSupl></MDFe>";

        [Fact]
        public void Sign_ValidMDFeXmlAndCertificate_ReturnsSignedXml()
        {
            string xmlContent = File.ReadAllText(_mdfePath);
            Assert.False(string.IsNullOrWhiteSpace(xmlContent), "O conteúdo do XML de exemplo da MDF-e não pode ser vazio.");

            X509Certificate2 certificate = new X509Certificate2(_certificatePath, _certificatePassword, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);
            Assert.NotNull(certificate);

            MDFeXmlSigner signer = new MDFeXmlSigner();

            string signedXml = signer.Sign(xmlContent, certificate);

            Assert.False(string.IsNullOrWhiteSpace(signedXml));
            Assert.Contains("<Signature", signedXml);
        }

        [Fact]
        public void Sign_InvalidXmlContentWithoutReferenceId_ThrowsInvalidOperationException()
        {
            string invalidXml = InvalidXmlWithoutReferenceId;

            X509Certificate2 certificate = new X509Certificate2(_certificatePath, _certificatePassword, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);
            MDFeXmlSigner signer = new MDFeXmlSigner();

            var ex = Assert.Throws<MissingReferenceIdException>(() => signer.Sign(invalidXml, certificate));
            Assert.Contains("O atributo 'Id' (referenceId) não foi encontrado ou está vazio no elemento 'mdfe:infMDFe'.", ex.Message);
        }

        [Fact]
        public void Sign_InvalidXmlContentWithoutElementInfMDFe_ThrowsInvalidOperationException()
        {
            string invalidXml = InvalidXmlWithoutInfMDFeElement;

            X509Certificate2 certificate = new X509Certificate2(_certificatePath, _certificatePassword, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);
            MDFeXmlSigner signer = new MDFeXmlSigner();

            var ex = Assert.Throws<InvalidXmlFormatException>(() => signer.Sign(invalidXml, certificate));
            Assert.Contains("O XML fornecido não contém a tag raiz esperada para assinatura digital: 'mdfe:infMDFe'.", ex.Message);
        }

        [Fact]
        public void Sign_InvalidXmlContentWithoutElementIde_ThrowsInvalidOperationException()
        {
            string invalidXml = InvalidXmlWithoutIdeElement;

            X509Certificate2 certificate = new X509Certificate2(_certificatePath, _certificatePassword, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);
            MDFeXmlSigner signer = new MDFeXmlSigner();

            var ex = Assert.Throws<MissingXmlElementException>(() => signer.Sign(invalidXml, certificate));
            Assert.Contains("Elemento 'ide' não encontrado no XML dentro de 'infMDFe'.", ex.Message);
        }

        [Fact]
        public void Sign_InvalidXmlContent_ThrowsArgumentException()
        {
            string invalidXml = "";
            X509Certificate2 certificate = new X509Certificate2(_certificatePath, _certificatePassword, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);
            MDFeXmlSigner signer = new MDFeXmlSigner();

            var ex = Assert.Throws<InvalidXmlFormatException>(() => signer.Sign(invalidXml, certificate));
            Assert.Contains("O XML fornecido não está no formato esperado ou é nulo/vazio.", ex.Message);
        }

        [Fact]
        public void Sign_NullCertificate_ThrowsArgumentNullException()
        {
            string xmlContent = File.ReadAllText(_mdfePath);
            X509Certificate2 nullCertificate = null;
            MDFeXmlSigner signer = new MDFeXmlSigner();

            var ex = Assert.Throws<InvalidCertificateException>(() => signer.Sign(xmlContent, nullCertificate));
            Assert.Contains("O certificado digital fornecido é inválido ou não possui uma chave privada acessível.", ex.Message);
        }

        [Fact]
        public void Sign_NFeXmlPassedToMDFeSigner_ThrowsInvalidOperationException()
        {
            string nfeXmlContent = File.ReadAllText(_nfePath);
            X509Certificate2 certificate = new X509Certificate2(_certificatePath, _certificatePassword, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);
            MDFeXmlSigner signer = new MDFeXmlSigner();

            var ex = Assert.Throws<MissingXmlElementException>(() => signer.Sign(nfeXmlContent, certificate));
            Assert.Contains("Elemento 'ide' não encontrado no XML dentro de 'infMDFe'.", ex.Message);
        }

        [Fact]
        public void Sign_CertificateWithoutPrivateKey_ThrowsInvalidOperationException()
        {
            string xmlContent = File.ReadAllText(_mdfePath);

            X509Certificate2 certificate = new X509Certificate2(_certificateInvalidPath);
            Assert.Null(certificate.GetRSAPrivateKey());

            MDFeXmlSigner signer = new MDFeXmlSigner();

            var ex = Assert.Throws<InvalidCertificateException>(() => signer.Sign(xmlContent, certificate));
            Assert.Contains("O certificado digital fornecido é inválido ou não possui uma chave privada acessível.", ex.Message);
        }

        [Fact]
        public void Sign_ValidMDFeXmlAndCertificate_IsSignatureValidReturnsTrue()
        {
            string xmlContent = File.ReadAllText(_mdfePath);
            X509Certificate2 certificate = new X509Certificate2(_certificatePath, _certificatePassword, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);

            MDFeXmlSigner signer = new MDFeXmlSigner();
            string signedXml = signer.Sign(xmlContent, certificate);

            Assert.False(string.IsNullOrWhiteSpace(signedXml), "O XML assinado não pode ter seu conteúdo vazio");
            Assert.Contains("<Signature", signedXml);

            Assert.True(signer.IsSignatureValid(signedXml), "O XML foi assinado com sucesso!");
        }

        [Fact]
        public void Sign_ValidXmlWithValidCertificate_ReturnsSignedXml()
        {
            string xmlContent = File.ReadAllText(_dfeValidPath);
            MDFeXmlSigner signer = new MDFeXmlSigner();

            var expected = signer.IsSignatureValid(xmlContent);
            Assert.True(expected, "A assinatura digital do XML assinado deve ser válida.");
        }

        [Fact]
        public void Sign_NullOrEmptyXmlContent_ThrowsArgumentException()
        {
            string xmlContent = string.Empty;
            MDFeXmlSigner signer = new MDFeXmlSigner();

            var ex = Assert.Throws<InvalidXmlFormatException>(() => signer.IsSignatureValid(xmlContent));
            Assert.Contains("O XML fornecido não está no formato esperado ou é nulo/vazio.", ex.Message);
        }

        [Fact]
        public void IsSignatureValid_XmlWithoutSignatureElement_ThrowsMissingSignatureElementException()
        {
            string xmlContent = File.ReadAllText(_mdfePath);
            MDFeXmlSigner signer = new MDFeXmlSigner();

            var ex = Assert.Throws<MissingSignatureElementException>(() => signer.IsSignatureValid(xmlContent));
            Assert.Contains("O XML fornecido não contém a tag Signature necessária para a validação da assinatura digital.", ex.Message);
        }
    }
}
