using DFeSigner.Core.Exceptions;
using DFeSigner.Core.Signers;
using System.Security.Cryptography.X509Certificates;

namespace DFeSigner.Tests
{
    public class CTeXmlSignerTests
    {
        private readonly string _ctePath = Path.Combine(AppContext.BaseDirectory, "Xml", "cte.xml");
        private readonly string _dfeValidPath = Path.Combine(AppContext.BaseDirectory, "Xml", "dfe-valid.xml");
        private readonly string _dfeInvalidPath = Path.Combine(AppContext.BaseDirectory, "Xml", "dfe-invalid.xml");
        private readonly string _certificatePath = Path.Combine(AppContext.BaseDirectory, "Certificates", "certificate.pfx");
        private readonly string _certificateInvalidPath = Path.Combine(AppContext.BaseDirectory, "Certificates", "certificate.cer");
        private readonly string _certificatePassword = "123";

        private const string InvalidXmlWithoutReferenceId = "<CTe xmlns=\"http://www.portalfiscal.inf.br/cte\"><infCte versao=\"4.00\"><ide><cUF>43</cUF><cCT>05734384</cCT><CFOP>5353</CFOP><natOp>5353 - PREST SERVICO TRANSP A ESTAB COM (Dentro do Estado)</natOp><mod>57</mod><serie>1</serie><nCT>5</nCT><dhEmi>2025-07-29T20:31:54-03:00</dhEmi><tpImp>1</tpImp><tpEmis>1</tpEmis><cDV>2</cDV><tpAmb>2</tpAmb><tpCTe>0</tpCTe><procEmi>0</procEmi><verProc>5.55</verProc><cMunEnv>4314902</cMunEnv><xMunEnv>Porto Alegre</xMunEnv><UFEnv>RS</UFEnv><modal>01</modal><tpServ>0</tpServ><cMunIni>4314902</cMunIni><xMunIni>Porto Alegre</xMunIni><UFIni>RS</UFIni><cMunFim>4314902</cMunFim><xMunFim>Porto Alegre</xMunFim><UFFim>RS</UFFim><retira>1</retira><indIEToma>2</indIEToma><toma3><toma>3</toma></toma3></ide><emit><CNPJ>12345678901234</CNPJ><IE>0470027053</IE><xNome>SIGE CLOUD SISTEMA DE GESTAO LTDA - ME</xNome><enderEmit><xLgr>Rua Tiradentes</xLgr><nro>195</nro><xBairro>Centro</xBairro><cMun>4314902</cMun><xMun>Porto Alegre</xMun><CEP>95770000</CEP><UF>RS</UF><fone>080000041</fone></enderEmit><CRT>3</CRT></emit><rem><CPF>99999999999</CPF><IE>ISENTO</IE><xNome>TRANSPORTADORA ABC LTDA</xNome><fone>51987654321</fone><enderReme><xLgr>Av. Principal</xLgr><nro>1000</nro><xBairro>Centro Urbano</xBairro><cMun>4314902</cMun><xMun>Porto Alegre</xMun><CEP>95760000</CEP><UF>RS</UF><cPais>1058</cPais><xPais>BRASIL</xPais></enderReme></rem><dest><CPF>99999999999</CPF><IE>ISENTO</IE><xNome>DISTRIBUIDORA XYZ S.A.</xNome><fone>51912345678</fone><enderDest><xLgr>Rua das Flores</xLgr><nro>50</nro><xBairro>Jardim Amarelo</xBairro><cMun>4314902</cMun><xMun>Porto Alegre</xMun><CEP>95760000</CEP><UF>RS</UF><cPais>1058</cPais><xPais>BRASIL</xPais></enderDest></dest><vPrest><vTPrest>50.00</vTPrest><vRec>50.00</vRec><Comp><xNome>Frete</xNome><vComp>50.00</vComp></Comp></vPrest><imp><ICMS><ICMS45><CST>40</CST></ICMS45></ICMS></imp><infCTeNorm><infCarga><vCarga>1500.00</vCarga><proPred>Moveis</proPred><infQ><cUnid>03</cUnid><tpMed>CX</tpMed><qCarga>1.0000</qCarga></infQ></infCarga><infDoc><infNFe><chave>43241012345678901234550015565859711838903634</chave></infNFe></infDoc><infModal versaoModal=\"4.00\"><rodo><RNTRC>01234567</RNTRC></rodo></infModal><cobr><fat></fat><dup><nDup>1</nDup><dVenc>2024-07-08</dVenc><vDup>50.00</vDup></dup></cobr></infCTeNorm></infCte><infCTeSupl><qrCodCTe>https://dfe-portal.svrs.rs.gov.br/cte/qrCode?chCTe=43250712345678901234570010000000051057343842&amp;tpAmb=2</qrCodCTe></infCTeSupl></CTe>";
        private const string InvalidXmlWithoutInfCteElement = "<CTe xmlns=\"http://www.portalfiscal.inf.br/cte\"><infCta Id=\"CTe43250712345678901234570010000000051057343842\" versao=\"4.00\"><ide><cUF>43</cUF><cCT>05734384</cCT><CFOP>5353</CFOP><natOp>5353 - PREST SERVICO TRANSP A ESTAB COM (Dentro do Estado)</natOp><mod>57</mod><serie>1</serie><nCT>5</nCT><dhEmi>2025-07-29T20:31:54-03:00</dhEmi><tpImp>1</tpImp><tpEmis>1</tpEmis><cDV>2</cDV><tpAmb>2</tpAmb><tpCTe>0</tpCTe><procEmi>0</procEmi><verProc>5.55</verProc><cMunEnv>4314902</cMunEnv><xMunEnv>Porto Alegre</xMunEnv><UFEnv>RS</UFEnv><modal>01</modal><tpServ>0</tpServ><cMunIni>4314902</cMunIni><xMunIni>Porto Alegre</xMunIni><UFIni>RS</UFIni><cMunFim>4314902</cMunFim><xMunFim>Porto Alegre</xMunFim><UFFim>RS</UFFim><retira>1</retira><indIEToma>2</indIEToma><toma3><toma>3</toma></toma3></ide><emit><CNPJ>12345678901234</CNPJ><IE>0470027053</IE><xNome>SIGE CLOUD SISTEMA DE GESTAO LTDA - ME</xNome><enderEmit><xLgr>Rua Tiradentes</xLgr><nro>195</nro><xBairro>Centro</xBairro><cMun>4314902</cMun><xMun>Porto Alegre</xMun><CEP>95770000</CEP><UF>RS</UF><fone>080000041</fone></enderEmit><CRT>3</CRT></emit><rem><CPF>99999999999</CPF><IE>ISENTO</IE><xNome>TRANSPORTADORA ABC LTDA</xNome><fone>51987654321</fone><enderReme><xLgr>Av. Principal</xLgr><nro>1000</nro><xBairro>Centro Urbano</xBairro><cMun>4314902</cMun><xMun>Porto Alegre</xMun><CEP>95760000</CEP><UF>RS</UF><cPais>1058</cPais><xPais>BRASIL</xPais></enderReme></rem><dest><CPF>99999999999</CPF><IE>ISENTO</IE><xNome>DISTRIBUIDORA XYZ S.A.</xNome><fone>51912345678</fone><enderDest><xLgr>Rua das Flores</xLgr><nro>50</nro><xBairro>Jardim Amarelo</xBairro><cMun>4314902</cMun><xMun>Porto Alegre</xMun><CEP>95760000</CEP><UF>RS</UF><cPais>1058</cPais><xPais>BRASIL</xPais></enderDest></dest><vPrest><vTPrest>50.00</vTPrest><vRec>50.00</vRec><Comp><xNome>Frete</xNome><vComp>50.00</vComp></Comp></vPrest><imp><ICMS><ICMS45><CST>40</CST></ICMS45></ICMS></imp><infCTeNorm><infCarga><vCarga>1500.00</vCarga><proPred>Moveis</proPred><infQ><cUnid>03</cUnid><tpMed>CX</tpMed><qCarga>1.0000</qCarga></infQ></infCarga><infDoc><infNFe><chave>43241012345678901234550015565859711838903634</chave></infNFe></infDoc><infModal versaoModal=\"4.00\"><rodo><RNTRC>01234567</RNTRC></rodo></infModal><cobr><fat></fat><dup><nDup>1</nDup><dVenc>2024-07-08</dVenc><vDup>50.00</vDup></dup></cobr></infCTeNorm></infCta><infCTeSupl><qrCodCTe>https://dfe-portal.svrs.rs.gov.br/cte/qrCode?chCTe=43250712345678901234570010000000051057343842&amp;tpAmb=2</qrCodCTe></infCTeSupl></CTe>";
        private const string InvalidXmlWithoutIdeElement = "<CTe xmlns=\"http://www.portalfiscal.inf.br/cte\"><infCte Id=\"CTe43250712345678901234570010000000051057343842\" versao=\"4.00\"><ida><cUF>43</cUF><cCT>05734384</cCT><CFOP>5353</CFOP><natOp>5353 - PREST SERVICO TRANSP A ESTAB COM (Dentro do Estado)</natOp><mod>57</mod><serie>1</serie><nCT>5</nCT><dhEmi>2025-07-29T20:31:54-03:00</dhEmi><tpImp>1</tpImp><tpEmis>1</tpEmis><cDV>2</cDV><tpAmb>2</tpAmb><tpCTe>0</tpCTe><procEmi>0</procEmi><verProc>5.55</verProc><cMunEnv>4314902</cMunEnv><xMunEnv>Porto Alegre</xMunEnv><UFEnv>RS</UFEnv><modal>01</modal><tpServ>0</tpServ><cMunIni>4314902</cMunIni><xMunIni>Porto Alegre</xMunIni><UFIni>RS</UFIni><cMunFim>4314902</cMunFim><xMunFim>Porto Alegre</xMunFim><UFFim>RS</UFFim><retira>1</retira><indIEToma>2</indIEToma><toma3><toma>3</toma></toma3></ida><emit><CNPJ>12345678901234</CNPJ><IE>0470027053</IE><xNome>SIGE CLOUD SISTEMA DE GESTAO LTDA - ME</xNome><enderEmit><xLgr>Rua Tiradentes</xLgr><nro>195</nro><xBairro>Centro</xBairro><cMun>4314902</cMun><xMun>Porto Alegre</xMun><CEP>95770000</CEP><UF>RS</UF><fone>080000041</fone></enderEmit><CRT>3</CRT></emit><rem><CPF>99999999999</CPF><IE>ISENTO</IE><xNome>TRANSPORTADORA ABC LTDA</xNome><fone>51987654321</fone><enderReme><xLgr>Av. Principal</xLgr><nro>1000</nro><xBairro>Centro Urbano</xBairro><cMun>4314902</cMun><xMun>Porto Alegre</xMun><CEP>95760000</CEP><UF>RS</UF><cPais>1058</cPais><xPais>BRASIL</xPais></enderReme></rem><dest><CPF>99999999999</CPF><IE>ISENTO</IE><xNome>DISTRIBUIDORA XYZ S.A.</xNome><fone>51912345678</fone><enderDest><xLgr>Rua das Flores</xLgr><nro>50</nro><xBairro>Jardim Amarelo</xBairro><cMun>4314902</cMun><xMun>Porto Alegre</xMun><CEP>95760000</CEP><UF>RS</UF><cPais>1058</cPais><xPais>BRASIL</xPais></enderDest></dest><vPrest><vTPrest>50.00</vTPrest><vRec>50.00</vRec><Comp><xNome>Frete</xNome><vComp>50.00</vComp></Comp></vPrest><imp><ICMS><ICMS45><CST>40</CST></ICMS45></ICMS></imp><infCTeNorm><infCarga><vCarga>1500.00</vCarga><proPred>Moveis</proPred><infQ><cUnid>03</cUnid><tpMed>CX</tpMed><qCarga>1.0000</qCarga></infQ></infCarga><infDoc><infNFe><chave>43241012345678901234550015565859711838903634</chave></infNFe></infDoc><infModal versaoModal=\"4.00\"><rodo><RNTRC>01234567</RNTRC></rodo></infModal><cobr><fat></fat><dup><nDup>1</nDup><dVenc>2024-07-08</dVenc><vDup>50.00</vDup></dup></cobr></infCTeNorm></infCte><infCTeSupl><qrCodCTe>https://dfe-portal.svrs.rs.gov.br/cte/qrCode?chCTe=43250712345678901234570010000000051057343842&amp;tpAmb=2</qrCodCTe></infCTeSupl></CTe>";
        private const string InvalidXmlWithModElementIncorrect = "<CTe xmlns=\"http://www.portalfiscal.inf.br/cte\"><infCte Id=\"CTe43250712345678901234570010000000051057343842\" versao=\"4.00\"><ide><cUF>43</cUF><cCT>05734384</cCT><CFOP>5353</CFOP><natOp>5353 - PREST SERVICO TRANSP A ESTAB COM (Dentro do Estado)</natOp><mod>65</mod><serie>1</serie><nCT>5</nCT><dhEmi>2025-07-29T20:31:54-03:00</dhEmi><tpImp>1</tpImp><tpEmis>1</tpEmis><cDV>2</cDV><tpAmb>2</tpAmb><tpCTe>0</tpCTe><procEmi>0</procEmi><verProc>5.55</verProc><cMunEnv>4314902</cMunEnv><xMunEnv>Porto Alegre</xMunEnv><UFEnv>RS</UFEnv><modal>01</modal><tpServ>0</tpServ><cMunIni>4314902</cMunIni><xMunIni>Porto Alegre</xMunIni><UFIni>RS</UFIni><cMunFim>4314902</cMunFim><xMunFim>Porto Alegre</xMunFim><UFFim>RS</UFFim><retira>1</retira><indIEToma>2</indIEToma><toma3><toma>3</toma></toma3></ide><emit><CNPJ>12345678901234</CNPJ><IE>0470027053</IE><xNome>SIGE CLOUD SISTEMA DE GESTAO LTDA - ME</xNome><enderEmit><xLgr>Rua Tiradentes</xLgr><nro>195</nro><xBairro>Centro</xBairro><cMun>4314902</cMun><xMun>Porto Alegre</xMun><CEP>95770000</CEP><UF>RS</UF><fone>080000041</fone></enderEmit><CRT>3</CRT></emit><rem><CPF>99999999999</CPF><IE>ISENTO</IE><xNome>TRANSPORTADORA ABC LTDA</xNome><fone>51987654321</fone><enderReme><xLgr>Av. Principal</xLgr><nro>1000</nro><xBairro>Centro Urbano</xBairro><cMun>4314902</cMun><xMun>Porto Alegre</xMun><CEP>95760000</CEP><UF>RS</UF><cPais>1058</cPais><xPais>BRASIL</xPais></enderReme></rem><dest><CPF>99999999999</CPF><IE>ISENTO</IE><xNome>DISTRIBUIDORA XYZ S.A.</xNome><fone>51912345678</fone><enderDest><xLgr>Rua das Flores</xLgr><nro>50</nro><xBairro>Jardim Amarelo</xBairro><cMun>4314902</cMun><xMun>Porto Alegre</xMun><CEP>95760000</CEP><UF>RS</UF><cPais>1058</cPais><xPais>BRASIL</xPais></enderDest></dest><vPrest><vTPrest>50.00</vTPrest><vRec>50.00</vRec><Comp><xNome>Frete</xNome><vComp>50.00</vComp></Comp></vPrest><imp><ICMS><ICMS45><CST>40</CST></ICMS45></ICMS></imp><infCTeNorm><infCarga><vCarga>1500.00</vCarga><proPred>Moveis</proPred><infQ><cUnid>03</cUnid><tpMed>CX</tpMed><qCarga>1.0000</qCarga></infQ></infCarga><infDoc><infNFe><chave>43241012345678901234550015565859711838903634</chave></infNFe></infDoc><infModal versaoModal=\"4.00\"><rodo><RNTRC>01234567</RNTRC></rodo></infModal><cobr><fat></fat><dup><nDup>1</nDup><dVenc>2024-07-08</dVenc><vDup>50.00</vDup></dup></cobr></infCTeNorm></infCte><infCTeSupl><qrCodCTe>https://dfe-portal.svrs.rs.gov.br/cte/qrCode?chCTe=43250712345678901234570010000000051057343842&amp;tpAmb=2</qrCodCTe></infCTeSupl></CTe>";

        [Fact]
        public void Sign_ValidCTeXmlAndCertificate_ReturnsSignedXml()
        {
            string xmlContent = File.ReadAllText(_ctePath);
            Assert.False(string.IsNullOrWhiteSpace(xmlContent), "O conteúdo do XML de exemplo do CT-e não pode ser vazio.");

            X509Certificate2 certificate = new X509Certificate2(_certificatePath, _certificatePassword, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);
            Assert.NotNull(certificate);

            CTeXmlSigner signer = new CTeXmlSigner();

            string signedXml = signer.Sign(xmlContent, certificate);

            Assert.False(string.IsNullOrWhiteSpace(signedXml));
            Assert.Contains("<Signature", signedXml);
        }

        [Fact]
        public void Sign_InvalidXmlContentWithoutReferenceId_ThrowsInvalidOperationException()
        {
            string invalidXml = InvalidXmlWithoutReferenceId;

            X509Certificate2 certificate = new X509Certificate2(_certificatePath, _certificatePassword, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);
            CTeXmlSigner signer = new CTeXmlSigner();

            var ex = Assert.Throws<MissingReferenceIdException>(() => signer.Sign(invalidXml, certificate));
            Assert.Contains("O atributo 'Id' (referenceId) não foi encontrado ou está vazio no elemento 'cte:infCte'.", ex.Message);
        }

        [Fact]
        public void Sign_InvalidXmlContentWithoutElementInfCte_ThrowsInvalidOperationException()
        {
            string invalidXml = InvalidXmlWithoutInfCteElement;

            X509Certificate2 certificate = new X509Certificate2(_certificatePath, _certificatePassword, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);
            CTeXmlSigner signer = new CTeXmlSigner();

            var ex = Assert.Throws<InvalidXmlFormatException>(() => signer.Sign(invalidXml, certificate));
            Assert.Contains("O XML fornecido não contém a tag raiz esperada para assinatura digital: 'cte:infCte'.", ex.Message);
        }

        [Fact]
        public void Sign_InvalidXmlContentWithoutElementIde_ThrowsInvalidOperationException()
        {
            string invalidXml = InvalidXmlWithoutIdeElement;

            X509Certificate2 certificate = new X509Certificate2(_certificatePath, _certificatePassword, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);
            CTeXmlSigner signer = new CTeXmlSigner();

            var ex = Assert.Throws<MissingXmlElementException>(() => signer.Sign(invalidXml, certificate));
            Assert.Contains("Elemento 'ide' não encontrado no XML dentro de 'infCte'.", ex.Message);
        }

        [Fact]
        public void Sign_InvalidXmlContent_ThrowsArgumentException()
        {
            string invalidXml = "";
            X509Certificate2 certificate = new X509Certificate2(_certificatePath, _certificatePassword, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);
            CTeXmlSigner signer = new CTeXmlSigner();

            var ex = Assert.Throws<InvalidXmlFormatException>(() => signer.Sign(invalidXml, certificate));
            Assert.Contains("O XML fornecido não está no formato esperado ou é nulo/vazio.", ex.Message);
        }

        [Fact]
        public void Sign_NullCertificate_ThrowsArgumentNullException()
        {
            string xmlContent = File.ReadAllText(_ctePath);
            X509Certificate2 nullCertificate = null;
            CTeXmlSigner signer = new CTeXmlSigner();

            var ex = Assert.Throws<InvalidCertificateException>(() => signer.Sign(xmlContent, nullCertificate));
            Assert.Contains("O certificado digital fornecido é inválido ou não possui uma chave privada acessível.", ex.Message);
        }

        [Fact]
        public void Sign_NFeXmlPassedToCTeSigner_ThrowsInvalidOperationException()
        {
            string nfceXmlContent = InvalidXmlWithModElementIncorrect;
            X509Certificate2 certificate = new X509Certificate2(_certificatePath, _certificatePassword, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);
            CTeXmlSigner signer = new CTeXmlSigner();

            var ex = Assert.Throws<UnexpectedDocumentTypeException>(() => signer.Sign(nfceXmlContent, certificate));
            Assert.Contains("O XML fornecido não é do tipo de documento esperado. Esperado modelo: 57, Encontrado modelo: 65.", ex.Message);
        }

        [Fact]
        public void Sign_CertificateWithoutPrivateKey_ThrowsInvalidOperationException()
        {
            string xmlContent = File.ReadAllText(_ctePath);

            X509Certificate2 certificate = new X509Certificate2(_certificateInvalidPath);
            Assert.Null(certificate.GetRSAPrivateKey());

            CTeXmlSigner signer = new CTeXmlSigner();

            var ex = Assert.Throws<InvalidCertificateException>(() => signer.Sign(xmlContent, certificate));
            Assert.Contains("O certificado digital fornecido é inválido ou não possui uma chave privada acessível.", ex.Message);
        }

        [Fact]
        public void Sign_ValidCTeXmlAndCertificate_IsSignatureValidReturnsTrue()
        {
            string xmlContent = File.ReadAllText(_ctePath);
            X509Certificate2 certificate = new X509Certificate2(_certificatePath, _certificatePassword, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);

            CTeXmlSigner signer = new CTeXmlSigner();
            string signedXml = signer.Sign(xmlContent, certificate);

            Assert.False(string.IsNullOrWhiteSpace(signedXml), "O XML assinado não pode ter seu conteúdo vazio");
            Assert.Contains("<Signature", signedXml);

            Assert.True(signer.IsSignatureValid(signedXml), "O XML foi assinado com sucesso!");
        }

        [Fact]
        public void Sign_ValidXmlWithValidCertificate_ReturnsSignedXml()
        {
            string xmlContent = File.ReadAllText(_dfeValidPath);
            CTeXmlSigner signer = new CTeXmlSigner();

            var expected = signer.IsSignatureValid(xmlContent);
            Assert.True(expected, "A assinatura digital do XML assinado deve ser válida.");
        }

        [Fact]
        public void Sign_ValidXmlWithInvalidCertificate_ReturnsSignedXml()
        {
            string xmlContent = File.ReadAllText(_dfeInvalidPath);
            CTeXmlSigner signer = new CTeXmlSigner();

            var expected = signer.IsSignatureValid(xmlContent);
            Assert.False(expected, "A assinatura digital do XML assinado deve ser válida.");
        }

        [Fact]
        public void Sign_NullOrEmptyXmlContent_ThrowsArgumentException()
        {
            string xmlContent = string.Empty;
            CTeXmlSigner signer = new CTeXmlSigner();

            var ex = Assert.Throws<InvalidXmlFormatException>(() => signer.IsSignatureValid(xmlContent));
            Assert.Contains("O XML fornecido não está no formato esperado ou é nulo/vazio.", ex.Message);
        }

        [Fact]
        public void IsSignatureValid_XmlWithoutSignatureElement_ThrowsMissingSignatureElementException()
        {
            string xmlContent = File.ReadAllText(_ctePath);
            CTeXmlSigner signer = new CTeXmlSigner();

            var ex = Assert.Throws<MissingSignatureElementException>(() => signer.IsSignatureValid(xmlContent));
            Assert.Contains("O XML fornecido não contém a tag Signature necessária para a validação da assinatura digital.", ex.Message);
        }
    }
}
