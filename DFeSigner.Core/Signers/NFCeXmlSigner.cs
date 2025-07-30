using System;
using System.Xml;
using DFeSigner.Core.Exceptions;

namespace DFeSigner.Core.Signers
{
    /// <summary>
    /// Implementação concreta da assinatura digital para NFC-e (Nota Fiscal de Consumidor Eletrônica).
    /// </summary>
    public class NFCeXmlSigner : DFeXmlSigner
    {
        private static readonly string NFCeNamespace = "http://www.portalfiscal.inf.br/nfe";
        private static readonly string PrefixNFCeNamespace = "nfe";

        /// <summary>
        /// Implementação específica para NFC-e para identificar o elemento 'infNFe' a ser assinado.
        /// Embora o nome do elemento seja 'infNFe' (o mesmo da NF-e), este método valida
        /// que o XML é de fato uma NFC-e através do atributo 'mod="65"' no nó 'ide'.
        /// </summary>
        /// <param name="document">O objeto XmlDocument contendo o XML da NFC-e.</param>
        /// <returns>Uma tupla contendo o XmlElement 'infNFe' e o seu atributo 'Id'.</returns>
        /// <exception cref="MissingXmlElementException">Lançada se o elemento 'ide' não for encontrado.</exception>
        /// <exception cref="UnexpectedDocumentTypeException">Lançada se o elemento 'mod' for diferente de 65(NFC-e).</exception>
        /// <exception cref="InvalidXmlFormatException">Lançada se o elemento root para a assinatura 'infNFe' não for encontrado.</exception>
        /// <exception cref="MissingReferenceIdException">Lançada se o atributo referenceId não for encontrado no elemento 'infNFe'.</exception>
        protected override string GetReferenceId(XmlDocument document)
        {
            XmlNamespaceManager ns = new XmlNamespaceManager(document.NameTable);
            ns.AddNamespace(PrefixNFCeNamespace, NFCeNamespace);

            XmlElement ideElement = document.GetElementsByTagName("ide")[0] as XmlElement;
            if (ideElement == null)
            {
                throw new MissingXmlElementException("ide", "infNFe");
            }
            
            string modeloDocumento = document.GetElementsByTagName("mod")[0].InnerText;
            if (modeloDocumento != "65")
            {
                throw new UnexpectedDocumentTypeException("65", modeloDocumento);
            }

            XmlElement elementToSign = document.GetElementsByTagName("infNFe")[0] as XmlElement;
            if (elementToSign == null)
            {
                throw new InvalidXmlFormatException($"{PrefixNFCeNamespace}:infNFe");
            }

            string referenceId = elementToSign.Attributes["Id"]?.Value;
            if (string.IsNullOrWhiteSpace(referenceId))
            {
                throw new MissingReferenceIdException($"{PrefixNFCeNamespace}:infNFe");
            }

            return referenceId;
        }
    }
}