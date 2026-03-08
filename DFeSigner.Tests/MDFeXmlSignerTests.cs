using DFeSigner.Core.Exceptions;
using DFeSigner.Core.Signers;
using System.Security.Cryptography.X509Certificates;

namespace DFeSigner.Tests
{
    public class MDFeXmlSignerTests
    {
        private readonly string _mdfePath = Path.Combine(AppContext.BaseDirectory, "Xml", "mdfe.xml");
        private readonly string _dfeValidPath = Path.Combine(AppContext.BaseDirectory, "Xml", "dfe-valid.xml");
        private readonly string _dfeInvalidPath = Path.Combine(AppContext.BaseDirectory, "Xml", "dfe-invalid.xml");
        private readonly string _certificatePath = Path.Combine(AppContext.BaseDirectory, "Certificates", "certificate.pfx");
        private readonly string _certificateInvalidPath = Path.Combine(AppContext.BaseDirectory, "Certificates", "certificate.cer");
        private readonly string _certificatePassword = "123";

        private const string InvalidXmlWithoutReferenceId = "<MDFe xmlns=\"http://www.portalfiscal.inf.br/mdfe\"><infMDFe versao=\"3.00\" ><ide><cUF>43</cUF><tpAmb>2</tpAmb><tpEmit>2</tpEmit><mod>58</mod><serie>1</serie><nMDF>417</nMDF><cMDF>94986538</cMDF><cDV>6</cDV><modal>1</modal><dhEmi>2023-09-25T14:25:00-03:00</dhEmi><tpEmis>1</tpEmis><procEmi>0</procEmi><verProc>_5_55</verProc><UFIni>RS</UFIni><UFFim>SP</UFFim><infMunCarrega><cMunCarrega>4314902</cMunCarrega><xMunCarrega>Porto Alegre</xMunCarrega></infMunCarrega><infPercurso><UFPer>SC</UFPer></infPercurso><infPercurso><UFPer>PR</UFPer></infPercurso><dhIniViagem>2021-07-21T00:00:00-03:00</dhIniViagem></ide><emit><CNPJ>12345678901234</CNPJ><IE>9999999999</IE><xNome>TRANSPORTADORA FICTICIA LTDA</xNome><xFant>TRANSFICT</xFant><enderEmit><xLgr>Rua Ficticia do Emissor</xLgr><nro>1234</nro><xBairro>Bairro Central</xBairro><cMun>4314902</cMun><xMun>Porto Alegre</xMun><CEP>90000000</CEP><UF>RS</UF><fone>51999999999</fone><email>contato@ficticia.com.br</email></enderEmit></emit><infModal versaoModal=\"3.00\"><rodo><veicTracao><placa>ABC5678</placa><tara>400</tara><condutor><xNome>MOTORISTA FICTICIO</xNome><CPF>99999999999</CPF></condutor><tpRod>01</tpRod><tpCar>01</tpCar><UF>RS</UF></veicTracao></rodo></infModal><infDoc><infMunDescarga><cMunDescarga>4314902</cMunDescarga><xMunDescarga>Porto Alegre</xMunDescarga><infNFe><chNFe>43210712345678901234550030000015091684574043</chNFe></infNFe></infMunDescarga></infDoc><tot><qNFe>1</qNFe><vCarga>100.00</vCarga><cUnid>01</cUnid><qCarga>12.0000</qCarga></tot></infMDFe><infMDFeSupl><qrCodMDFe>https://dfe-portal.svrs.rs.gov.br/mdfe/qrCode?chMDFe=43230912345678901234580010000004171949865386&amp;tpAmb=2</qrCodMDFe></infMDFeSupl></MDFe>";
        private const string InvalidXmlWithoutInfMDFeElement = "<MDFe xmlns=\"http://www.portalfiscal.inf.br/mdfe\"><infMDFa Id=\"MDFe43230912345678901234580010000004171949865386\" versao=\"3.00\" ><ide><cUF>43</cUF><tpAmb>2</tpAmb><tpEmit>2</tpEmit><mod>58</mod><serie>1</serie><nMDF>417</nMDF><cMDF>94986538</cMDF><cDV>6</cDV><modal>1</modal><dhEmi>2023-09-25T14:25:00-03:00</dhEmi><tpEmis>1</tpEmis><procEmi>0</procEmi><verProc>_5_55</verProc><UFIni>RS</UFIni><UFFim>SP</UFFim><infMunCarrega><cMunCarrega>4314902</cMunCarrega><xMunCarrega>Porto Alegre</xMunCarrega></infMunCarrega><infPercurso><UFPer>SC</UFPer></infPercurso><infPercurso><UFPer>PR</UFPer></infPercurso><dhIniViagem>2021-07-21T00:00:00-03:00</dhIniViagem></ide><emit><CNPJ>12345678901234</CNPJ><IE>9999999999</IE><xNome>TRANSPORTADORA FICTICIA LTDA</xNome><xFant>TRANSFICT</xFant><enderEmit><xLgr>Rua Ficticia do Emissor</xLgr><nro>1234</nro><xBairro>Bairro Central</xBairro><cMun>4314902</cMun><xMun>Porto Alegre</xMun><CEP>90000000</CEP><UF>RS</UF><fone>51999999999</fone><email>contato@ficticia.com.br</email></enderEmit></emit><infModal versaoModal=\"3.00\"><rodo><veicTracao><placa>ABC5678</placa><tara>400</tara><condutor><xNome>MOTORISTA FICTICIO</xNome><CPF>99999999999</CPF></condutor><tpRod>01</tpRod><tpCar>01</tpCar><UF>RS</UF></veicTracao></rodo></infModal><infDoc><infMunDescarga><cMunDescarga>4314902</cMunDescarga><xMunDescarga>Porto Alegre</xMunDescarga><infNFe><chNFe>43210712345678901234550030000015091684574043</chNFe></infNFe></infMunDescarga></infDoc><tot><qNFe>1</qNFe><vCarga>100.00</vCarga><cUnid>01</cUnid><qCarga>12.0000</qCarga></tot></infMDFa><infMDFeSupl><qrCodMDFe>https://dfe-portal.svrs.rs.gov.br/mdfe/qrCode?chMDFe=43230912345678901234580010000004171949865386&amp;tpAmb=2</qrCodMDFe></infMDFeSupl></MDFe>";
        private const string InvalidXmlWithoutIdeElement = "<MDFe xmlns=\"http://www.portalfiscal.inf.br/mdfe\"><infMDFe Id=\"MDFe43230912345678901234580010000004171949865386\" versao=\"3.00\" ><ida><cUF>43</cUF><tpAmb>2</tpAmb><tpEmit>2</tpEmit><mod>58</mod><serie>1</serie><nMDF>417</nMDF><cMDF>94986538</cMDF><cDV>6</cDV><modal>1</modal><dhEmi>2023-09-25T14:25:00-03:00</dhEmi><tpEmis>1</tpEmis><procEmi>0</procEmi><verProc>_5_55</verProc><UFIni>RS</UFIni><UFFim>SP</UFFim><infMunCarrega><cMunCarrega>4314902</cMunCarrega><xMunCarrega>Porto Alegre</xMunCarrega></infMunCarrega><infPercurso><UFPer>SC</UFPer></infPercurso><infPercurso><UFPer>PR</UFPer></infPercurso><dhIniViagem>2021-07-21T00:00:00-03:00</dhIniViagem></ida><emit><CNPJ>12345678901234</CNPJ><IE>9999999999</IE><xNome>TRANSPORTADORA FICTICIA LTDA</xNome><xFant>TRANSFICT</xFant><enderEmit><xLgr>Rua Ficticia do Emissor</xLgr><nro>1234</nro><xBairro>Bairro Central</xBairro><cMun>4314902</cMun><xMun>Porto Alegre</xMun><CEP>90000000</CEP><UF>RS</UF><fone>51999999999</fone><email>contato@ficticia.com.br</email></enderEmit></emit><infModal versaoModal=\"3.00\"><rodo><veicTracao><placa>ABC5678</placa><tara>400</tara><condutor><xNome>MOTORISTA FICTICIO</xNome><CPF>99999999999</CPF></condutor><tpRod>01</tpRod><tpCar>01</tpCar><UF>RS</UF></veicTracao></rodo></infModal><infDoc><infMunDescarga><cMunDescarga>4314902</cMunDescarga><xMunDescarga>Porto Alegre</xMunDescarga><infNFe><chNFe>43210712345678901234550030000015091684574043</chNFe></infNFe></infMunDescarga></infDoc><tot><qNFe>1</qNFe><vCarga>100.00</vCarga><cUnid>01</cUnid><qCarga>12.0000</qCarga></tot></infMDFe><infMDFeSupl><qrCodMDFe>https://dfe-portal.svrs.rs.gov.br/mdfe/qrCode?chMDFe=43230912345678901234580010000004171949865386&amp;tpAmb=2</qrCodMDFe></infMDFeSupl></MDFe>";
        private const string InvalidXmlWithModElementIncorret = "<MDFe xmlns=\"http://www.portalfiscal.inf.br/mdfe\"><infMDFe Id=\"MDFe43230912345678901234580010000004171949865386\" versao=\"3.00\" ><ide><cUF>43</cUF><tpAmb>2</tpAmb><tpEmit>2</tpEmit><mod>55</mod><serie>1</serie><nMDF>417</nMDF><cMDF>94986538</cMDF><cDV>6</cDV><modal>1</modal><dhEmi>2023-09-25T14:25:00-03:00</dhEmi><tpEmis>1</tpEmis><procEmi>0</procEmi><verProc>_5_55</verProc><UFIni>RS</UFIni><UFFim>SP</UFFim><infMunCarrega><cMunCarrega>4314902</cMunCarrega><xMunCarrega>Porto Alegre</xMunCarrega></infMunCarrega><infPercurso><UFPer>SC</UFPer></infPercurso><infPercurso><UFPer>PR</UFPer></infPercurso><dhIniViagem>2021-07-21T00:00:00-03:00</dhIniViagem></ide><emit><CNPJ>12345678901234</CNPJ><IE>9999999999</IE><xNome>TRANSPORTADORA FICTICIA LTDA</xNome><xFant>TRANSFICT</xFant><enderEmit><xLgr>Rua Ficticia do Emissor</xLgr><nro>1234</nro><xBairro>Bairro Central</xBairro><cMun>4314902</cMun><xMun>Porto Alegre</xMun><CEP>90000000</CEP><UF>RS</UF><fone>51999999999</fone><email>contato@ficticia.com.br</email></enderEmit></emit><infModal versaoModal=\"3.00\"><rodo><veicTracao><placa>ABC5678</placa><tara>400</tara><condutor><xNome>MOTORISTA FICTICIO</xNome><CPF>99999999999</CPF></condutor><tpRod>01</tpRod><tpCar>01</tpCar><UF>RS</UF></veicTracao></rodo></infModal><infDoc><infMunDescarga><cMunDescarga>4314902</cMunDescarga><xMunDescarga>Porto Alegre</xMunDescarga><infNFe><chNFe>43210712345678901234550030000015091684574043</chNFe></infNFe></infMunDescarga></infDoc><tot><qNFe>1</qNFe><vCarga>100.00</vCarga><cUnid>01</cUnid><qCarga>12.0000</qCarga></tot></infMDFe><infMDFeSupl><qrCodMDFe>https://dfe-portal.svrs.rs.gov.br/mdfe/qrCode?chMDFe=43230912345678901234580010000004171949865386&amp;tpAmb=2</qrCodMDFe></infMDFeSupl></MDFe>";

        [Fact]
        public void Sign_ValidMDFeXmlAndCertificate_ReturnsSignedXml()
        {
            var xmlContent = File.ReadAllText(_mdfePath);
            var certificate = new X509Certificate2(_certificatePath, _certificatePassword, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);
            var sut = new MDFeXmlSigner();

            var expected = sut.Sign(xmlContent, certificate);

            Assert.False(string.IsNullOrWhiteSpace(expected));
            Assert.Contains("<Signature", expected);
        }

        [Fact]
        public void Sign_InvalidXmlContentWithoutReferenceId_ThrowsInvalidOperationException()
        {
            var invalidXml = InvalidXmlWithoutReferenceId;
            var certificate = new X509Certificate2(_certificatePath, _certificatePassword, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);
            
            var sut = new MDFeXmlSigner();

            Assert.Throws<MissingReferenceIdException>(() => sut.Sign(invalidXml, certificate));
        }

        [Fact]
        public void Sign_InvalidXmlContentWithoutElementInfMDFe_ThrowsInvalidOperationException()
        {
            var invalidXml = InvalidXmlWithoutInfMDFeElement;
            var certificate = new X509Certificate2(_certificatePath, _certificatePassword, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);
            
            var sut = new MDFeXmlSigner();

            Assert.Throws<InvalidXmlFormatException>(() => sut.Sign(invalidXml, certificate));
        }

        [Fact]
        public void Sign_InvalidXmlContentWithoutElementIde_ThrowsInvalidOperationException()
        {
            var invalidXml = InvalidXmlWithoutIdeElement;
            var certificate = new X509Certificate2(_certificatePath, _certificatePassword, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);
            
            var sut = new MDFeXmlSigner();

            Assert.Throws<MissingXmlElementException>(() => sut.Sign(invalidXml, certificate));
        }

        [Fact]
        public void Sign_InvalidXmlContent_ThrowsArgumentException()
        {
            var invalidXml = string.Empty;
            var certificate = new X509Certificate2(_certificatePath, _certificatePassword, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);
            
            var sut = new MDFeXmlSigner();

            Assert.Throws<InvalidXmlFormatException>(() => sut.Sign(invalidXml, certificate));
        }

        [Fact]
        public void Sign_NullCertificate_ThrowsArgumentNullException()
        {
            var xmlContent = File.ReadAllText(_mdfePath);
            
            var sut = new MDFeXmlSigner();

            Assert.Throws<InvalidCertificateException>(() => sut.Sign(xmlContent, null));
        }

        [Fact]
        public void Sign_NFeXmlPassedToMDFeSigner_ThrowsInvalidOperationException()
        {
            var nfeXmlContent = InvalidXmlWithModElementIncorret;
            var certificate = new X509Certificate2(_certificatePath, _certificatePassword, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);
            
            var sut = new MDFeXmlSigner();

            Assert.Throws<UnexpectedDocumentTypeException>(() => sut.Sign(nfeXmlContent, certificate));
        }

        [Fact]
        public void Sign_CertificateWithoutPrivateKey_ThrowsInvalidOperationException()
        {
            var xmlContent = File.ReadAllText(_mdfePath);
            var certificate = new X509Certificate2(_certificateInvalidPath);

            var sut = new MDFeXmlSigner();

            Assert.Throws<InvalidCertificateException>(() => sut.Sign(xmlContent, certificate));
        }

        [Fact]
        public void Sign_ValidMDFeXmlAndCertificate_IsSignatureValidReturnsTrue()
        {
            var xmlContent = File.ReadAllText(_mdfePath);
            var certificate = new X509Certificate2(_certificatePath, _certificatePassword, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);
            var sut = new MDFeXmlSigner();
            
            var expected = sut.Sign(xmlContent, certificate);

            Assert.False(string.IsNullOrWhiteSpace(expected));
            Assert.Contains("<Signature", expected);
            Assert.True(sut.IsSignatureValid(expected));
        }

        [Fact]
        public void Sign_ValidXmlWithValidCertificate_ReturnsSignedXml()
        {
            var xmlContent = File.ReadAllText(_dfeValidPath);
            var sut = new MDFeXmlSigner();

            var expected = sut.IsSignatureValid(xmlContent);
            
            Assert.True(expected);
        }

        [Fact]
        public void Sign_ValidXmlWithInvalidCertificate_ReturnsSignedXml()
        {
            var xmlContent = File.ReadAllText(_dfeInvalidPath);
            var sut = new MDFeXmlSigner();

            var expected = sut.IsSignatureValid(xmlContent);
            
            Assert.False(expected);
        }

        [Fact]
        public void Sign_NullOrEmptyXmlContent_ThrowsArgumentException()
        {
            var xmlContent = string.Empty;
            
            var sut = new MDFeXmlSigner();

            Assert.Throws<InvalidXmlFormatException>(() => sut.IsSignatureValid(xmlContent));
        }

        [Fact]
        public void IsSignatureValid_XmlWithoutSignatureElement_ThrowsMissingSignatureElementException()
        {
            var xmlContent = File.ReadAllText(_mdfePath);
            
            var sut = new MDFeXmlSigner();

            Assert.Throws<MissingSignatureElementException>(() => sut.IsSignatureValid(xmlContent));
        }
    }
}
