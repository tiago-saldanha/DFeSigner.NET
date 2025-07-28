using System;
using System.Security.Cryptography.Xml;
using System.Security.Cryptography.X509Certificates;
using System.Xml;

namespace DFeSigner.Core.Signers
{
    /// <summary>
    /// Classe responsável por gerar a assinatura digital de um XML de NF-e.
    /// Esta é uma implementação inicial e pode ser generalizada posteriormente.
    /// </summary>
    public class NFeXmlSigner
    {
        private static readonly string NFeNamespace = "http://www.portalfiscal.inf.br/nfe";
        private static readonly string PrefixNFeNamespace = "nfe"; // Usado para referências XPath

        /// <summary>
        /// Gera a assinatura digital de um XML de NF-e.
        /// </summary>
        /// <param name="xmlContent">O conteúdo do XML da NF-e a ser assinado.</param>
        /// <param name="certificate">O certificado digital X.509 a ser usado para a assinatura.</param>
        /// <returns>O XML da NF-e com a assinatura digital incluída, ou lança uma exceção em caso de erro.</returns>
        /// <exception cref="ArgumentException">Lançada se o conteúdo XML for nulo ou vazio.</exception>
        /// <exception cref="ArgumentNullException">Lançada se o certificado for nulo.</exception>
        /// <exception cref="InvalidOperationException">Lançada se o elemento a ser assinado não for encontrado ou se o certificado não tiver chave privada.</exception>
        /// <exception cref="ApplicationException">Lançada para erros gerais durante o processo de assinatura.</exception>
        public string Sign(string xmlContent, X509Certificate2 certificate)
        {
            if (string.IsNullOrWhiteSpace(xmlContent))
                throw new ArgumentException("O conteúdo XML não pode ser nulo ou vazio.", nameof(xmlContent));
            if (certificate == null)
                throw new ArgumentNullException(nameof(certificate), "O certificado digital não pode ser nulo.");

            try
            {
                XmlDocument doc = new XmlDocument();
                doc.PreserveWhitespace = true;
                doc.LoadXml(xmlContent);

                XmlNamespaceManager ns = new XmlNamespaceManager(doc.NameTable);
                ns.AddNamespace(PrefixNFeNamespace, NFeNamespace);

                XmlElement elementToSign = doc.SelectSingleNode($"//{PrefixNFeNamespace}:infNFe", ns) as XmlElement;

                if (elementToSign == null)
                {
                    throw new InvalidOperationException($"Elemento '{PrefixNFeNamespace}:infNFe' não encontrado no XML. Verifique se o XML é uma NF-e válida.");
                }

                string referenceId = elementToSign.Attributes["Id"]?.Value;
                if (string.IsNullOrWhiteSpace(referenceId))
                {
                    throw new InvalidOperationException($"Atributo 'Id' não encontrado ou vazio no elemento '{PrefixNFeNamespace}:infNFe'.");
                }

                SignedXml signedXml = new SignedXml(doc);

                signedXml.SigningKey = certificate.GetRSAPrivateKey();
                if (signedXml.SigningKey == null)
                {
                    throw new InvalidOperationException("O certificado não possui uma chave privada RSA acessível ou compatível para assinatura.");
                }

                signedXml.SignedInfo.CanonicalizationMethod = SignedXml.XmlDsigC14NTransformUrl;
                signedXml.SignedInfo.SignatureMethod = SignedXml.XmlDsigRSASHA256Url;

                Reference reference = new Reference($"#{referenceId}");
                reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
                reference.AddTransform(new XmlDsigC14NTransform());
                reference.DigestMethod = SignedXml.XmlDsigSHA256Url;

                signedXml.AddReference(reference);

                KeyInfo keyInfo = new KeyInfo();
                keyInfo.AddClause(new KeyInfoX509Data(certificate));
                signedXml.KeyInfo = keyInfo;

                signedXml.ComputeSignature();
                XmlElement xmlSignature = signedXml.GetXml();

                doc.DocumentElement.AppendChild(xmlSignature);

                return doc.OuterXml;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Erro ao assinar o XML: {ex.Message}", ex);
            }
        }
    }
}