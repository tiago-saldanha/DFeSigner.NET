using System;
using System.Xml;

namespace DFeSigner.Core.Signers
{
    /// <summary>
    /// Implementação concreta da assinatura digital para NFC-e (Nota Fiscal de Consumidor Eletrônica).
    /// </summary>
    public class NFCeXmlSigner : DFeXmlSigner
    {
        private static readonly string NFCeNamespace = "http://www.portalfiscal.inf.br/nfe";
        private static readonly string PrefixNFCeNamespace = "nfe"; // Usado para referências XPath

        /// <summary>
        /// Implementação específica para NFC-e para identificar o elemento 'infNFe' a ser assinado.
        /// Embora o nome do elemento seja 'infNFe' (o mesmo da NF-e), este método valida
        /// que o XML é de fato uma NFC-e através do atributo 'mod="65"' no nó 'ide'.
        /// </summary>
        /// <param name="document">O objeto XmlDocument contendo o XML da NFC-e.</param>
        /// <returns>Uma tupla contendo o XmlElement 'infNFe' e o seu atributo 'Id'.</returns>
        /// <exception cref="InvalidOperationException">Lançada se o elemento 'infNFe' ou seu 'Id' não for encontrado,
        /// ou se o XML não for identificado como uma NFC-e (modelo 65).</exception>
        protected override string GetReferenceId(XmlDocument document)
        {
            XmlNamespaceManager ns = new XmlNamespaceManager(document.NameTable);
            ns.AddNamespace(PrefixNFCeNamespace, NFCeNamespace);

            XmlElement ideElement = document.GetElementsByTagName("ide")[0] as XmlElement;
            if (ideElement == null)
            {
                throw new InvalidOperationException("Elemento 'ide' não encontrado no XML. O XML pode não ser um documento fiscal válido.");
            }
            
            string modeloDocumento = document.GetElementsByTagName("mod")[0].InnerText;
            if (modeloDocumento != "65")
            {
                throw new InvalidOperationException($"O XML fornecido não é uma NFC-e (modelo 65). Modelo encontrado: {modeloDocumento}.");
            }

            XmlElement elementToSign = document.GetElementsByTagName("infNFe")[0] as XmlElement;
            if (elementToSign == null)
            {
                throw new InvalidOperationException($"Elemento '{PrefixNFCeNamespace}:infNFe' não encontrado no XML da NFC-e. Verifique a estrutura do XML.");
            }

            string referenceId = elementToSign.Attributes["Id"]?.Value;
            if (string.IsNullOrWhiteSpace(referenceId))
            {
                throw new InvalidOperationException($"Atributo 'Id' não encontrado ou vazio no elemento '{PrefixNFCeNamespace}:infNFe'.");
            }

            return referenceId;
        }
    }
}