using System;
using System.Xml;

namespace DFeSigner.Core.Signers
{
    /// <summary>
    /// Implementação concreta da assinatura digital para NF-e.
    /// </summary>
    public class NFeXmlSigner : DFeXmlSigner
    {
        private static readonly string NFeNamespace = "http://www.portalfiscal.inf.br/nfe";
        private static readonly string PrefixNFeNamespace = "nfe";

        /// <summary>
        /// Implementação específica para NF-e para identificar o elemento 'infNFe' a ser assinado.
        /// </summary>
        /// <param name="document">O objeto XmlDocument contendo o XML da NF-e.</param>
        /// <returns>Uma tupla contendo o XmlElement 'infNFe' e o seu atributo 'Id'.</returns>
        /// <exception cref="InvalidOperationException">Lançada se o elemento 'infNFe' ou seu 'Id' não for encontrado,
        /// ou se o XML não for identificado como uma NF-e (modelo 55).</exception>
        protected override string GetReferenceId(XmlDocument document)
        {
            XmlNamespaceManager ns = new(document.NameTable);
            ns.AddNamespace(PrefixNFeNamespace, NFeNamespace);

            XmlElement ideElement = document.GetElementsByTagName("ide")[0] as XmlElement;
            if (ideElement == null)
            {
                throw new InvalidOperationException("Elemento 'ide' não encontrado no XML. O XML pode não ser um documento fiscal válido.");
            }

            string modeloDocumento = document.GetElementsByTagName("mod")[0].InnerText;
            if (modeloDocumento != "55")
            {
                throw new InvalidOperationException($"O XML fornecido não é uma NF-e (modelo 55). Modelo encontrado: {modeloDocumento}.");
            }

            XmlElement elementToSign = document.GetElementsByTagName("infNFe")[0] as XmlElement;
            if (elementToSign == null)
            {
                throw new InvalidOperationException($"Elemento '{PrefixNFeNamespace}:infNFe' não encontrado no XML. Verifique se o XML é uma NF-e válida.");
            }

            string referenceId = elementToSign.Attributes["Id"]?.Value;
            if (string.IsNullOrWhiteSpace(referenceId))
            {
                throw new InvalidOperationException($"Atributo 'Id' não encontrado ou vazio no elemento '{PrefixNFeNamespace}:infNFe'.");
            }

            return referenceId;
        }   
    }
}