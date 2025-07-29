using System.Security.Cryptography.X509Certificates;
using DFeSigner.Core.Signers;

namespace DFeSigner.Tests
{
    public class NFCeXmlSignerTests
    {
        private readonly string _nfcePath = Path.Combine(AppContext.BaseDirectory, "nfce.xml");
        private readonly string _certificatePath = Path.Combine(AppContext.BaseDirectory, "certificate.pfx");
        private readonly string _certificateInvalidPath = Path.Combine(AppContext.BaseDirectory, "certificate.cer");
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
        public void Sign_InvalidXmlContentWithoutReferenceId_ThrowsInvalidOperationException()
        {
            string invalidXml = "<NFe xmlns=\"http://www.portalfiscal.inf.br/nfe\"><infNFe versao=\"4.00\"><ide><cUF>43</cUF><cNF>49920547</cNF><natOp>VENDA DE MERCADORIA CONSUMIDOR FINAL</natOp><mod>65</mod><serie>1</serie><nNF>3131349</nNF><dhEmi>2025-07-25T09:54:00-03:00</dhEmi><dhSaiEnt>2025-07-25T09:54:00-03:00</dhSaiEnt><tpNF>1</tpNF><idDest>1</idDest><cMunFG>4308102</cMunFG><tpImp>4</tpImp><tpEmis>1</tpEmis><cDV>7</cDV><tpAmb>2</tpAmb><finNFe>1</finNFe><indFinal>1</indFinal><indPres>1</indPres><procEmi>0</procEmi><verProc>5.55</verProc></ide><emit><CNPJ>99999999000199</CNPJ><xNome>EMPRESA DEMONSTRATIVA LTDA</xNome><xFant>DEMO GESTAO</xFant><enderEmit><xLgr>Avenida Principal</xLgr><nro>1000</nro><xCpl>Sala 101</xCpl><xBairro>Centro</xBairro><cMun>4308102</cMun><xMun>Cidade Fictícia</xMun><UF>RS</UF><CEP>99999000</CEP><cPais>1058</cPais><xPais>Brasil</xPais></enderEmit><IE>999999999</IE><CRT>3</CRT></emit><dest><CPF>99999999999</CPF><xNome>CONSUMIDOR</xNome><indIEDest>9</indIEDest></dest><det nItem=\"1\"><prod><cProd>16052</cProd><cEAN>SEM GTIN</cEAN><xProd>NOTA FISCAL EMITIDA EM AMBIENTE DE HOMOLOGACAO - SEM VALOR FISCAL</xProd><NCM>84733041</NCM><CFOP>5102</CFOP><uCom>UN</uCom><qCom>1.0000</qCom><vUnCom>3231.8200000000</vUnCom><vProd>3231.82</vProd><cEANTrib>SEM GTIN</cEANTrib><uTrib>UN</uTrib><qTrib>1.0000</qTrib><vUnTrib>3231.8200000000</vUnTrib><indTot>1</indTot><xPed>5223948</xPed><nItemPed>1</nItemPed></prod><imposto><ICMS><ICMS40><orig>0</orig><CST>40</CST></ICMS40></ICMS><PIS><PISNT><CST>07</CST></PISNT></PIS><COFINS><COFINSNT><CST>07</CST></COFINSNT></COFINS></imposto></det><total><ICMSTot><vBC>0.00</vBC><vICMS>0.00</vICMS><vICMSDeson>0.00</vICMSDeson><vFCP>0.00</vFCP><vBCST>0.00</vBCST><vST>0.00</vST><vFCPST>0.00</vFCPST><vFCPSTRet>0.00</vFCPSTRet><qBCMono>0.00</qBCMono><vICMSMono>0.00</vICMSMono><qBCMonoReten>0.00</qBCMonoReten><vICMSMonoReten>0.00</vICMSMonoReten><qBCMonoRet>0.00</qBCMonoRet><vICMSMonoRet>0.00</vICMSMonoRet><vProd>3231.82</vProd><vFrete>0.00</vFrete><vSeg>0.00</vSeg><vDesc>0.00</vDesc><vII>0.00</vII><vIPI>0.00</vIPI><vIPIDevol>0.00</vIPIDevol><vPIS>0.00</vPIS><vCOFINS>0.00</vCOFINS><vOutro>0.00</vOutro><vNF>3231.82</vNF></ICMSTot></total><transp><modFrete>9</modFrete></transp><pag><detPag><tPag>01</tPag><vPag>3231.82</vPag></detPag></pag><infAdic><infCpl>Documento emitido por EPP OU ME optante pelo Simples Nacional. Nao gera credito fiscal de IPI..||Pedidos N: 5223948||Val Aprox Tributos R$ 569,12 (17,61%) Federal e R$ 549,41 (17,00%) Estadual - Fonte: IBPT</infCpl></infAdic></infNFe></NFe>";

            X509Certificate2 certificate = new X509Certificate2(_certificatePath, _certificatePassword, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);
            NFCeXmlSigner signer = new NFCeXmlSigner();

            Assert.Throws<InvalidOperationException>(() => signer.Sign(invalidXml, certificate));
        }

        [Fact]
        public void Sign_InvalidXmlContentWithoutElementInfNFe_ThrowsInvalidOperationException()
        {
            string invalidXml = "<NFe xmlns=\"http://www.portalfiscal.inf.br/nfe\"><infNFa Id=\"NFe43250799999999000199650180031313491499205477\" versao=\"4.00\"><ide><cUF>43</cUF><cNF>49920547</cNF><natOp>VENDA DE MERCADORIA CONSUMIDOR FINAL</natOp><mod>65</mod><serie>1</serie><nNF>3131349</nNF><dhEmi>2025-07-25T09:54:00-03:00</dhEmi><dhSaiEnt>2025-07-25T09:54:00-03:00</dhSaiEnt><tpNF>1</tpNF><idDest>1</idDest><cMunFG>4308102</cMunFG><tpImp>4</tpImp><tpEmis>1</tpEmis><cDV>7</cDV><tpAmb>2</tpAmb><finNFe>1</finNFe><indFinal>1</indFinal><indPres>1</indPres><procEmi>0</procEmi><verProc>5.55</verProc></ide><emit><CNPJ>99999999000199</CNPJ><xNome>EMPRESA DEMONSTRATIVA LTDA</xNome><xFant>DEMO GESTAO</xFant><enderEmit><xLgr>Avenida Principal</xLgr><nro>1000</nro><xCpl>Sala 101</xCpl><xBairro>Centro</xBairro><cMun>4308102</cMun><xMun>Cidade Fictícia</xMun><UF>RS</UF><CEP>99999000</CEP><cPais>1058</cPais><xPais>Brasil</xPais></enderEmit><IE>999999999</IE><CRT>3</CRT></emit><dest><CPF>99999999999</CPF><xNome>CONSUMIDOR</xNome><indIEDest>9</indIEDest></dest><det nItem=\"1\"><prod><cProd>16052</cProd><cEAN>SEM GTIN</cEAN><xProd>NOTA FISCAL EMITIDA EM AMBIENTE DE HOMOLOGACAO - SEM VALOR FISCAL</xProd><NCM>84733041</NCM><CFOP>5102</CFOP><uCom>UN</uCom><qCom>1.0000</qCom><vUnCom>3231.8200000000</vUnCom><vProd>3231.82</vProd><cEANTrib>SEM GTIN</cEANTrib><uTrib>UN</uTrib><qTrib>1.0000</qTrib><vUnTrib>3231.8200000000</vUnTrib><indTot>1</indTot><xPed>5223948</xPed><nItemPed>1</nItemPed></prod><imposto><ICMS><ICMS40><orig>0</orig><CST>40</CST></ICMS40></ICMS><PIS><PISNT><CST>07</CST></PISNT></PIS><COFINS><COFINSNT><CST>07</CST></COFINSNT></COFINS></imposto></det><total><ICMSTot><vBC>0.00</vBC><vICMS>0.00</vICMS><vICMSDeson>0.00</vICMSDeson><vFCP>0.00</vFCP><vBCST>0.00</vBCST><vST>0.00</vST><vFCPST>0.00</vFCPST><vFCPSTRet>0.00</vFCPSTRet><qBCMono>0.00</qBCMono><vICMSMono>0.00</vICMSMono><qBCMonoReten>0.00</qBCMonoReten><vICMSMonoReten>0.00</vICMSMonoReten><qBCMonoRet>0.00</qBCMonoRet><vICMSMonoRet>0.00</vICMSMonoRet><vProd>3231.82</vProd><vFrete>0.00</vFrete><vSeg>0.00</vSeg><vDesc>0.00</vDesc><vII>0.00</vII><vIPI>0.00</vIPI><vIPIDevol>0.00</vIPIDevol><vPIS>0.00</vPIS><vCOFINS>0.00</vCOFINS><vOutro>0.00</vOutro><vNF>3231.82</vNF></ICMSTot></total><transp><modFrete>9</modFrete></transp><pag><detPag><tPag>01</tPag><vPag>3231.82</vPag></detPag></pag><infAdic><infCpl>Documento emitido por EPP OU ME optante pelo Simples Nacional. Nao gera credito fiscal de IPI..||Pedidos N: 5223948||Val Aprox Tributos R$ 569,12 (17,61%) Federal e R$ 549,41 (17,00%) Estadual - Fonte: IBPT</infCpl></infAdic></infNFa></NFe>";

            X509Certificate2 certificate = new X509Certificate2(_certificatePath, _certificatePassword, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);
            NFCeXmlSigner signer = new NFCeXmlSigner();

            Assert.Throws<InvalidOperationException>(() => signer.Sign(invalidXml, certificate));
        }

        [Fact]
        public void Sign_InvalidXmlContentWithoutElementIde_ThrowsInvalidOperationException()
        {
            string invalidXml = "<NFe xmlns=\"http://www.portalfiscal.inf.br/nfe\"><infNFe Id=\"NFe43250799999999000199650180031313491499205477\" versao=\"4.00\"><ida><cUF>43</cUF><cNF>49920547</cNF><natOp>VENDA DE MERCADORIA CONSUMIDOR FINAL</natOp><mod>65</mod><serie>1</serie><nNF>3131349</nNF><dhEmi>2025-07-25T09:54:00-03:00</dhEmi><dhSaiEnt>2025-07-25T09:54:00-03:00</dhSaiEnt><tpNF>1</tpNF><idDest>1</idDest><cMunFG>4308102</cMunFG><tpImp>4</tpImp><tpEmis>1</tpEmis><cDV>7</cDV><tpAmb>2</tpAmb><finNFe>1</finNFe><indFinal>1</indFinal><indPres>1</indPres><procEmi>0</procEmi><verProc>5.55</verProc></ida><emit><CNPJ>99999999000199</CNPJ><xNome>EMPRESA DEMONSTRATIVA LTDA</xNome><xFant>DEMO GESTAO</xFant><enderEmit><xLgr>Avenida Principal</xLgr><nro>1000</nro><xCpl>Sala 101</xCpl><xBairro>Centro</xBairro><cMun>4308102</cMun><xMun>Cidade Fictícia</xMun><UF>RS</UF><CEP>99999000</CEP><cPais>1058</cPais><xPais>Brasil</xPais></enderEmit><IE>999999999</IE><CRT>3</CRT></emit><dest><CPF>99999999999</CPF><xNome>CONSUMIDOR</xNome><indIEDest>9</indIEDest></dest><det nItem=\"1\"><prod><cProd>16052</cProd><cEAN>SEM GTIN</cEAN><xProd>NOTA FISCAL EMITIDA EM AMBIENTE DE HOMOLOGACAO - SEM VALOR FISCAL</xProd><NCM>84733041</NCM><CFOP>5102</CFOP><uCom>UN</uCom><qCom>1.0000</qCom><vUnCom>3231.8200000000</vUnCom><vProd>3231.82</vProd><cEANTrib>SEM GTIN</cEANTrib><uTrib>UN</uTrib><qTrib>1.0000</qTrib><vUnTrib>3231.8200000000</vUnTrib><indTot>1</indTot><xPed>5223948</xPed><nItemPed>1</nItemPed></prod><imposto><ICMS><ICMS40><orig>0</orig><CST>40</CST></ICMS40></ICMS><PIS><PISNT><CST>07</CST></PISNT></PIS><COFINS><COFINSNT><CST>07</CST></COFINSNT></COFINS></imposto></det><total><ICMSTot><vBC>0.00</vBC><vICMS>0.00</vICMS><vICMSDeson>0.00</vICMSDeson><vFCP>0.00</vFCP><vBCST>0.00</vBCST><vST>0.00</vST><vFCPST>0.00</vFCPST><vFCPSTRet>0.00</vFCPSTRet><qBCMono>0.00</qBCMono><vICMSMono>0.00</vICMSMono><qBCMonoReten>0.00</qBCMonoReten><vICMSMonoReten>0.00</vICMSMonoReten><qBCMonoRet>0.00</qBCMonoRet><vICMSMonoRet>0.00</vICMSMonoRet><vProd>3231.82</vProd><vFrete>0.00</vFrete><vSeg>0.00</vSeg><vDesc>0.00</vDesc><vII>0.00</vII><vIPI>0.00</vIPI><vIPIDevol>0.00</vIPIDevol><vPIS>0.00</vPIS><vCOFINS>0.00</vCOFINS><vOutro>0.00</vOutro><vNF>3231.82</vNF></ICMSTot></total><transp><modFrete>9</modFrete></transp><pag><detPag><tPag>01</tPag><vPag>3231.82</vPag></detPag></pag><infAdic><infCpl>Documento emitido por EPP OU ME optante pelo Simples Nacional. Nao gera credito fiscal de IPI..||Pedidos N: 5223948||Val Aprox Tributos R$ 569,12 (17,61%) Federal e R$ 549,41 (17,00%) Estadual - Fonte: IBPT</infCpl></infAdic></infNFe></NFe>";

            X509Certificate2 certificate = new X509Certificate2(_certificatePath, _certificatePassword, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);
            NFCeXmlSigner signer = new NFCeXmlSigner();

            Assert.Throws<InvalidOperationException>(() => signer.Sign(invalidXml, certificate));
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

        [Fact]
        public void Sign_CertificateWithoutPrivateKey_ThrowsInvalidOperationException()
        {
            string xmlContent = File.ReadAllText(_nfcePath);

            X509Certificate2 certificate = new X509Certificate2(_certificateInvalidPath);
            Assert.Null(certificate.GetRSAPrivateKey());

            NFCeXmlSigner signer = new NFCeXmlSigner();

            var ex = Assert.Throws<InvalidOperationException>(() => signer.Sign(xmlContent, certificate));
            Assert.Contains("O certificado não possui uma chave privada RSA acessível ou compatível para assinatura.", ex.Message);
        }
    }
}