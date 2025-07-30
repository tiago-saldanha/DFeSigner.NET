using System;
using System.Xml;

namespace DFeSigner.Core.Signers
{
    /// <summary>
    /// Implementação concreta da assinatura digital para MDF-e.
    /// </summary>
    public class MDFeXmlSigner : DFeXmlSigner
    {
        private static readonly string NFeNamespace = "http://www.portalfiscal.inf.br/mdfe";
        private static readonly string PrefixNFeNamespace = "mdfe";

        /// <summary>
        /// Implementação específica para MDF-e para identificar o elemento 'infMDFe' a ser assinado.
        /// </summary>
        /// <param name="document">O objeto XmlDocument contendo o XML da MDF-e.</param>
        /// <returns>Uma tupla contendo o XmlElement 'infMDFe' e o seu atributo 'Id'.</returns>
        /// <exception cref="InvalidOperationException">Lançada se o elemento 'infMDFe' ou seu 'Id' não for encontrado,
        /// ou se o XML não for identificado como um MDF-e (modelo 58).</exception>
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
            if (modeloDocumento != "58")
            {
                throw new InvalidOperationException($"O XML fornecido não é um MDF-e (modelo 58). Modelo encontrado: {modeloDocumento}.");
            }

            XmlElement elementToSign = document.GetElementsByTagName("infMDFe")[0] as XmlElement;
            if (elementToSign == null)
            {
                throw new InvalidOperationException($"Elemento '{PrefixNFeNamespace}:infMDFe' não encontrado no XML. Verifique se o XML é um MDF-e válida.");
            }

            string referenceId = elementToSign.Attributes["Id"]?.Value;
            if (string.IsNullOrWhiteSpace(referenceId))
            {
                throw new InvalidOperationException($"Atributo 'Id' não encontrado ou vazio no elemento '{PrefixNFeNamespace}:infMDFe'.");
            }

            return referenceId;
        }
    }
}