using System.Security.Cryptography.X509Certificates;
using DFeSigner.Core.Exceptions;
using DFeSigner.Core.Signers;

namespace DFeSigner.Tests
{
    public class NFeXmlSignerTests
    {
        private readonly string _nfePath = Path.Combine(AppContext.BaseDirectory, "Xml", "nfe.xml");
        private readonly string _nfcePath = Path.Combine(AppContext.BaseDirectory, "Xml", "nfce.xml");
        private readonly string _certificatePath = Path.Combine(AppContext.BaseDirectory, "Certificates", "certificate.pfx");
        private readonly string _certificateInvalidPath = Path.Combine(AppContext.BaseDirectory, "Certificates", "certificate.cer");
        private readonly string _certificatePassword = "123";

        private const string InvalidXmlWithoutReferenceId = "<NFe xmlns=\"http://www.portalfiscal.inf.br/nfe\"><infNFe versao=\"4.00\"><ide><cUF>43</cUF><cNF>49920547</cNF><natOp>5102 VENDA DE MERCADORIA ADQUIRIDA OU RECEBIDA DE TERCE</natOp><mod>55</mod><serie>18</serie><nNF>3131349</nNF><dhEmi>2025-07-25T09:54:00-03:00</dhEmi><dhSaiEnt>2025-07-25T09:54:00-03:00</dhSaiEnt><tpNF>1</tpNF><idDest>1</idDest><cMunFG>4308102</cMunFG><tpImp>1</tpImp><tpEmis>1</tpEmis><cDV>7</cDV><tpAmb>2</tpAmb><finNFe>1</finNFe><indFinal>0</indFinal><indPres>1</indPres><procEmi>0</procEmi><verProc>5.55</verProc></ide><emit><CNPJ>99999999000199</CNPJ><xNome>EMPRESA DEMONSTRATIVA LTDA</xNome><xFant>DEMO GESTAO</xFant><enderEmit><xLgr>Avenida Principal</xLgr><nro>1000</nro><xCpl>Sala 101</xCpl><xBairro>Centro</xBairro><cMun>4308102</cMun><xMun>Cidade Fictícia</xMun><UF>RS</UF><CEP>99999000</CEP><cPais>1058</cPais><xPais>Brasil</xPais></enderEmit><IE>999999999</IE><CRT>3</CRT></emit><dest><CPF>99999999999</CPF><xNome>CLIENTE TESTE HOMOLOGACAO</xNome><enderDest><xLgr>Rua Secundária</xLgr><nro>456</nro><xCpl>Apartamento 2B</xCpl><xBairro>Bairro Teste</xBairro><cMun>4319505</cMun><xMun>Outra Cidade</xMun><UF>RS</UF><CEP>99999111</CEP><cPais>1058</cPais><xPais>Brasil</xPais><fone>51988887777</fone></enderDest><indIEDest>2</indIEDest><email>teste@exemplo.com.br</email></dest><autXML><CNPJ>99999999000199</CNPJ></autXML><det nItem=\"1\"><prod><cProd>16052</cProd><cEAN>SEM GTIN</cEAN><xProd>NOTA FISCAL EMITIDA EM AMBIENTE DE HOMOLOGACAO - SEM VALOR FISCAL</xProd><NCM>84733041</NCM><CFOP>5102</CFOP><uCom>UN</uCom><qCom>1.0000</qCom><vUnCom>3231.8200000000</vUnCom><vProd>3231.82</vProd><cEANTrib>SEM GTIN</cEANTrib><uTrib>UN</uTrib><qTrib>1.0000</qTrib><vUnTrib>3231.8200000000</vUnTrib><indTot>1</indTot><xPed>5223948</xPed><nItemPed>1</nItemPed></prod><imposto><ICMS><ICMS40><orig>0</orig><CST>40</CST></ICMS40></ICMS><PIS><PISNT><CST>07</CST></PISNT></PIS><COFINS><COFINSNT><CST>07</CST></COFINSNT></COFINS></imposto></det><total><ICMSTot><vBC>0.00</vBC><vICMS>0.00</vICMS><vICMSDeson>0.00</vICMSDeson><vFCP>0.00</vFCP><vBCST>0.00</vBCST><vST>0.00</vST><vFCPST>0.00</vFCPST><vFCPSTRet>0.00</vFCPSTRet><qBCMono>0.00</qBCMono><vICMSMono>0.00</vICMSMono><qBCMonoReten>0.00</qBCMonoReten><vICMSMonoReten>0.00</vICMSMonoReten><qBCMonoRet>0.00</qBCMonoRet><vICMSMonoRet>0.00</vICMSMonoRet><vProd>3231.82</vProd><vFrete>0.00</vFrete><vSeg>0.00</vSeg><vDesc>0.00</vDesc><vII>0.00</vII><vIPI>0.00</vIPI><vIPIDevol>0.00</vIPIDevol><vPIS>0.00</vPIS><vCOFINS>0.00</vCOFINS><vOutro>0.00</vOutro><vNF>3231.82</vNF></ICMSTot></total><transp><modFrete>9</modFrete></transp><pag><detPag><tPag>01</tPag><vPag>3231.82</vPag></detPag></pag><infAdic><infCpl>Documento emitido por EPP OU ME optante pelo Simples Nacional. Nao gera credito fiscal de IPI..||Pedidos N: 5223948||Val Aprox Tributos R$ 569,12 (17,61%) Federal e R$ 549,41 (17,00%) Estadual - Fonte: IBPT</infCpl></infAdic><infRespTec><CNPJ>99999999000199</CNPJ><xContato>CONTATO TESTE</xContato><email>contato@exemplo.com.br</email><fone>51999999999</fone></infRespTec></infNFe></NFe>";
        private const string InvalidXmlWithoutInfNFeElement = "<NFe xmlns=\"http://www.portalfiscal.inf.br/nfe\"><infNFa Id=\"NFe43250799999999000199550180031313491499205477\" versao=\"4.00\"><ide><cUF>43</cUF><cNF>49920547</cNF><natOp>5102 VENDA DE MERCADORIA ADQUIRIDA OU RECEBIDA DE TERCE</natOp><mod>55</mod><serie>18</serie><nNF>3131349</nNF><dhEmi>2025-07-25T09:54:00-03:00</dhEmi><dhSaiEnt>2025-07-25T09:54:00-03:00</dhSaiEnt><tpNF>1</tpNF><idDest>1</idDest><cMunFG>4308102</cMunFG><tpImp>1</tpImp><tpEmis>1</tpEmis><cDV>7</cDV><tpAmb>2</tpAmb><finNFe>1</finNFe><indFinal>0</indFinal><indPres>1</indPres><procEmi>0</procEmi><verProc>5.55</verProc></ide><emit><CNPJ>99999999000199</CNPJ><xNome>EMPRESA DEMONSTRATIVA LTDA</xNome><xFant>DEMO GESTAO</xFant><enderEmit><xLgr>Avenida Principal</xLgr><nro>1000</nro><xCpl>Sala 101</xCpl><xBairro>Centro</xBairro><cMun>4308102</cMun><xMun>Cidade Fictícia</xMun><UF>RS</UF><CEP>99999000</CEP><cPais>1058</cPais><xPais>Brasil</xPais></enderEmit><IE>999999999</IE><CRT>3</CRT></emit><dest><CPF>99999999999</CPF><xNome>CLIENTE TESTE HOMOLOGACAO</xNome><enderDest><xLgr>Rua Secundária</xLgr><nro>456</nro><xCpl>Apartamento 2B</xCpl><xBairro>Bairro Teste</xBairro><cMun>4319505</cMun><xMun>Outra Cidade</xMun><UF>RS</UF><CEP>99999111</CEP><cPais>1058</cPais><xPais>Brasil</xPais><fone>51988887777</fone></enderDest><indIEDest>2</indIEDest><email>teste@exemplo.com.br</email></dest><autXML><CNPJ>99999999000199</CNPJ></autXML><det nItem=\"1\"><prod><cProd>16052</cProd><cEAN>SEM GTIN</cEAN><xProd>NOTA FISCAL EMITIDA EM AMBIENTE DE HOMOLOGACAO - SEM VALOR FISCAL</xProd><NCM>84733041</NCM><CFOP>5102</CFOP><uCom>UN</uCom><qCom>1.0000</qCom><vUnCom>3231.8200000000</vUnCom><vProd>3231.82</vProd><cEANTrib>SEM GTIN</cEANTrib><uTrib>UN</uTrib><qTrib>1.0000</qTrib><vUnTrib>3231.8200000000</vUnTrib><indTot>1</indTot><xPed>5223948</xPed><nItemPed>1</nItemPed></prod><imposto><ICMS><ICMS40><orig>0</orig><CST>40</CST></ICMS40></ICMS><PIS><PISNT><CST>07</CST></PISNT></PIS><COFINS><COFINSNT><CST>07</CST></COFINSNT></COFINS></imposto></det><total><ICMSTot><vBC>0.00</vBC><vICMS>0.00</vICMS><vICMSDeson>0.00</vICMSDeson><vFCP>0.00</vFCP><vBCST>0.00</vBCST><vST>0.00</vST><vFCPST>0.00</vFCPST><vFCPSTRet>0.00</vFCPSTRet><qBCMono>0.00</qBCMono><vICMSMono>0.00</vICMSMono><qBCMonoReten>0.00</qBCMonoReten><vICMSMonoReten>0.00</vICMSMonoReten><qBCMonoRet>0.00</qBCMonoRet><vICMSMonoRet>0.00</vICMSMonoRet><vProd>3231.82</vProd><vFrete>0.00</vFrete><vSeg>0.00</vSeg><vDesc>0.00</vDesc><vII>0.00</vII><vIPI>0.00</vIPI><vIPIDevol>0.00</vIPIDevol><vPIS>0.00</vPIS><vCOFINS>0.00</vCOFINS><vOutro>0.00</vOutro><vNF>3231.82</vNF></ICMSTot></total><transp><modFrete>9</modFrete></transp><pag><detPag><tPag>01</tPag><vPag>3231.82</vPag></detPag></pag><infAdic><infCpl>Documento emitido por EPP OU ME optante pelo Simples Nacional. Nao gera credito fiscal de IPI..||Pedidos N: 5223948||Val Aprox Tributos R$ 569,12 (17,61%) Federal e R$ 549,41 (17,00%) Estadual - Fonte: IBPT</infCpl></infAdic><infRespTec><CNPJ>99999999000199</CNPJ><xContato>CONTATO TESTE</xContato><email>contato@exemplo.com.br</email><fone>51999999999</fone></infRespTec></infNFa></NFe>";
        private const string InvalidXmlWithoutIdeElement = "<NFe xmlns=\"http://www.portalfiscal.inf.br/nfe\"><infNFe Id=\"NFe43250799999999000199550180031313491499205477\" versao=\"4.00\"><ida><cUF>43</cUF><cNF>49920547</cNF><natOp>5102 VENDA DE MERCADORIA ADQUIRIDA OU RECEBIDA DE TERCE</natOp><mod>55</mod><serie>18</serie><nNF>3131349</nNF><dhEmi>2025-07-25T09:54:00-03:00</dhEmi><dhSaiEnt>2025-07-25T09:54:00-03:00</dhSaiEnt><tpNF>1</tpNF><idDest>1</idDest><cMunFG>4308102</cMunFG><tpImp>1</tpImp><tpEmis>1</tpEmis><cDV>7</cDV><tpAmb>2</tpAmb><finNFe>1</finNFe><indFinal>0</indFinal><indPres>1</indPres><procEmi>0</procEmi><verProc>5.55</verProc></ida><emit><CNPJ>99999999000199</CNPJ><xNome>EMPRESA DEMONSTRATIVA LTDA</xNome><xFant>DEMO GESTAO</xFant><enderEmit><xLgr>Avenida Principal</xLgr><nro>1000</nro><xCpl>Sala 101</xCpl><xBairro>Centro</xBairro><cMun>4308102</cMun><xMun>Cidade Fictícia</xMun><UF>RS</UF><CEP>99999000</CEP><cPais>1058</cPais><xPais>Brasil</xPais></enderEmit><IE>999999999</IE><CRT>3</CRT></emit><dest><CPF>99999999999</CPF><xNome>CLIENTE TESTE HOMOLOGACAO</xNome><enderDest><xLgr>Rua Secundária</xLgr><nro>456</nro><xCpl>Apartamento 2B</xCpl><xBairro>Bairro Teste</xBairro><cMun>4319505</cMun><xMun>Outra Cidade</xMun><UF>RS</UF><CEP>99999111</CEP><cPais>1058</cPais><xPais>Brasil</xPais><fone>51988887777</fone></enderDest><indIEDest>2</indIEDest><email>teste@exemplo.com.br</email></dest><autXML><CNPJ>99999999000199</CNPJ></autXML><det nItem=\"1\"><prod><cProd>16052</cProd><cEAN>SEM GTIN</cEAN><xProd>NOTA FISCAL EMITIDA EM AMBIENTE DE HOMOLOGACAO - SEM VALOR FISCAL</xProd><NCM>84733041</NCM><CFOP>5102</CFOP><uCom>UN</uCom><qCom>1.0000</qCom><vUnCom>3231.8200000000</vUnCom><vProd>3231.82</vProd><cEANTrib>SEM GTIN</cEANTrib><uTrib>UN</uTrib><qTrib>1.0000</qTrib><vUnTrib>3231.8200000000</vUnTrib><indTot>1</indTot><xPed>5223948</xPed><nItemPed>1</nItemPed></prod><imposto><ICMS><ICMS40><orig>0</orig><CST>40</CST></ICMS40></ICMS><PIS><PISNT><CST>07</CST></PISNT></PIS><COFINS><COFINSNT><CST>07</CST></COFINSNT></COFINS></imposto></det><total><ICMSTot><vBC>0.00</vBC><vICMS>0.00</vICMS><vICMSDeson>0.00</vICMSDeson><vFCP>0.00</vFCP><vBCST>0.00</vBCST><vST>0.00</vST><vFCPST>0.00</vFCPST><vFCPSTRet>0.00</vFCPSTRet><qBCMono>0.00</qBCMono><vICMSMono>0.00</vICMSMono><qBCMonoReten>0.00</qBCMonoReten><vICMSMonoReten>0.00</vICMSMonoReten><qBCMonoRet>0.00</qBCMonoRet><vICMSMonoRet>0.00</vICMSMonoRet><vProd>3231.82</vProd><vFrete>0.00</vFrete><vSeg>0.00</vSeg><vDesc>0.00</vDesc><vII>0.00</vII><vIPI>0.00</vIPI><vIPIDevol>0.00</vIPIDevol><vPIS>0.00</vPIS><vCOFINS>0.00</vCOFINS><vOutro>0.00</vOutro><vNF>3231.82</vNF></ICMSTot></total><transp><modFrete>9</modFrete></transp><pag><detPag><tPag>01</tPag><vPag>3231.82</vPag></detPag></pag><infAdic><infCpl>Documento emitido por EPP OU ME optante pelo Simples Nacional. Nao gera credito fiscal de IPI..||Pedidos N: 5223948||Val Aprox Tributos R$ 569,12 (17,61%) Federal e R$ 549,41 (17,00%) Estadual - Fonte: IBPT</infCpl></infAdic><infRespTec><CNPJ>99999999000199</CNPJ><xContato>CONTATO TESTE</xContato><email>contato@exemplo.com.br</email><fone>51999999999</fone></infRespTec></infNFe></NFe>";

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
        public void Sign_InvalidXmlContentWithoutReferenceId_ThrowsInvalidOperationException()
        {
            string invalidXml = InvalidXmlWithoutReferenceId;

            X509Certificate2 certificate = new X509Certificate2(_certificatePath, _certificatePassword, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);
            NFeXmlSigner signer = new NFeXmlSigner();

            var ex = Assert.Throws<MissingReferenceIdException>(() => signer.Sign(invalidXml, certificate));
            Assert.Contains("O atributo 'Id' (referenceId) não foi encontrado ou está vazio no elemento 'nfe:infNFe'.", ex.Message);
        }

        [Fact]
        public void Sign_InvalidXmlContentWithoutElementInfNFe_ThrowsInvalidOperationException()
        {
            string invalidXml = InvalidXmlWithoutInfNFeElement;
            
            X509Certificate2 certificate = new X509Certificate2(_certificatePath, _certificatePassword, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);
            NFeXmlSigner signer = new NFeXmlSigner();

            var ex = Assert.Throws<InvalidXmlFormatException>(() => signer.Sign(invalidXml, certificate));
            Assert.Contains($"O XML fornecido não contém a tag raiz esperada para assinatura digital: 'nfe:infNFe'.", ex.Message);
        }

        [Fact]
        public void Sign_InvalidXmlContentWithoutElementIde_ThrowsInvalidOperationException()
        {
            string invalidXml = InvalidXmlWithoutIdeElement;

            X509Certificate2 certificate = new X509Certificate2(_certificatePath, _certificatePassword, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);
            NFeXmlSigner signer = new NFeXmlSigner();

            var ex = Assert.Throws<MissingXmlElementException>(() => signer.Sign(invalidXml, certificate));
            Assert.Contains("Elemento 'ide' não encontrado no XML dentro de 'infNFe'.", ex.Message);
        }

        [Fact]
        public void Sign_InvalidXmlContent_ThrowsArgumentException()
        {
            string invalidXml = "";
            X509Certificate2 certificate = new X509Certificate2(_certificatePath, _certificatePassword, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);
            NFeXmlSigner signer = new NFeXmlSigner();

            var ex = Assert.Throws<InvalidXmlFormatException>(() => signer.Sign(invalidXml, certificate));
            Assert.Contains("O XML fornecido não está no formato esperado ou é nulo/vazio.", ex.Message);
        }

        [Fact]
        public void Sign_NullCertificate_ThrowsArgumentNullException()
        {
            string xmlContent = File.ReadAllText(_nfePath);
            X509Certificate2 nullCertificate = null;
            NFeXmlSigner signer = new NFeXmlSigner();

            var ex = Assert.Throws<InvalidCertificateException>(() => signer.Sign(xmlContent, nullCertificate));
            Assert.Contains("O certificado digital fornecido é inválido ou não possui uma chave privada acessível.", ex.Message);
        }

        [Fact]
        public void Sign_NFCeXmlPassedToNFeSigner_ThrowsInvalidOperationException()
        {
            string nfceXmlContent = File.ReadAllText(_nfcePath);
            X509Certificate2 certificate = new X509Certificate2(_certificatePath, _certificatePassword, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);
            NFeXmlSigner signer = new NFeXmlSigner();

            var ex = Assert.Throws<UnexpectedDocumentTypeException>(() => signer.Sign(nfceXmlContent, certificate));
            Assert.Contains("O XML fornecido não é do tipo de documento esperado. Esperado modelo: 55, Encontrado modelo: 65.", ex.Message);
        }

        [Fact]
        public void Sign_CertificateWithoutPrivateKey_ThrowsInvalidOperationException()
        {
            string xmlContent = File.ReadAllText(_nfePath);

            X509Certificate2 certificate = new X509Certificate2(_certificateInvalidPath);
            Assert.Null(certificate.GetRSAPrivateKey());

            NFeXmlSigner signer = new NFeXmlSigner();

            var ex = Assert.Throws<InvalidCertificateException>(() => signer.Sign(xmlContent, certificate));
            Assert.Contains("O certificado digital fornecido é inválido ou não possui uma chave privada acessível.", ex.Message);
        }
    }
}