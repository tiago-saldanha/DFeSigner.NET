using System;
using System.Xml;

namespace DFeSigner.Core.Signers
{
    /// <summary>
    /// Implementação concreta da assinatura digital para NF-e.
    /// </summary>
    public class NFeXmlSigner : DFeXmlSigner
    {
        // Namespace específico da NF-e
        private static readonly string NFeNamespace = "http://www.portalfiscal.inf.br/nfe";
        private static readonly string PrefixNFeNamespace = "nfe";

        /// <summary>
        /// Implementação específica para NF-e para identificar o elemento 'infNFe' a ser assinado.
        /// </summary>
        /// <param name="document">O objeto XmlDocument contendo o XML da NF-e.</param>
        /// <returns>Uma tupla contendo o XmlElement 'infNFe' e o seu atributo 'Id'.</returns>
        /// <exception cref="InvalidOperationException">Lançada se o elemento ou o ID não for encontrado.</exception>
        protected override (XmlElement Element, string ReferenceId) GetElementToSign(XmlDocument document)
        {
            XmlNamespaceManager ns = new(document.NameTable);
            ns.AddNamespace(PrefixNFeNamespace, NFeNamespace);

            XmlElement elementToSign = document.SelectSingleNode($"//{PrefixNFeNamespace}:infNFe", ns) as XmlElement;

            if (elementToSign == null)
            {
                throw new InvalidOperationException($"Elemento '{PrefixNFeNamespace}:infNFe' não encontrado no XML. Verifique se o XML é uma NF-e válida.");
            }

            string referenceId = elementToSign.Attributes["Id"]?.Value;
            if (string.IsNullOrWhiteSpace(referenceId))
            {
                throw new InvalidOperationException($"Atributo 'Id' não encontrado ou vazio no elemento '{PrefixNFeNamespace}:infNFe'.");
            }

            return (elementToSign, referenceId);
        }
    }
}