using System;
using System.Xml;

namespace DFeSigner.Core.Signers
{
    /// <summary>
    /// Implementação concreta da assinatura digital para CT-e.
    /// </summary>
    public class CTeXmlSigner : DFeXmlSigner
    {
        private static readonly string CTeNamespace = "http://www.portalfiscal.inf.br/cte";
        private static readonly string PrefixCTeNamespace = "cte";

        /// <summary>
        /// Implementação específica para CT-e para identificar o elemento 'infCte' a ser assinado.
        /// </summary>
        /// <param name="document">O objeto XmlDocument contendo o XML da CT-e.</param>
        /// <returns>Uma tupla contendo o XmlElement 'infCte' e o seu atributo 'Id'.</returns>
        /// <exception cref="InvalidOperationException">Lançada se o elemento 'infCte' ou seu 'Id' não for encontrado,
        /// ou se o XML não for identificado como uma CT-e (modelo 57).</exception>
        protected override string GetReferenceId(XmlDocument document)
        {
            XmlNamespaceManager ns = new(document.NameTable);
            ns.AddNamespace(PrefixCTeNamespace, CTeNamespace);

            XmlElement ideElement = document.GetElementsByTagName("ide")[0] as XmlElement;
            if (ideElement == null)
            {
                throw new InvalidOperationException("Elemento 'ide' não encontrado no XML. O XML pode não ser um documento fiscal válido.");
            }

            string modeloDocumento = document.GetElementsByTagName("mod")[0].InnerText;
            if (modeloDocumento != "57")
            {
                throw new InvalidOperationException($"O XML fornecido não é um CT-e (modelo 57). Modelo encontrado: {modeloDocumento}.");
            }

            XmlElement elementToSign = document.GetElementsByTagName("infCte")[0] as XmlElement;
            if (elementToSign == null)
            {
                throw new InvalidOperationException($"Elemento '{PrefixCTeNamespace}:infCte' não encontrado no XML. Verifique se o XML é uma NF-e válida.");
            }

            string referenceId = elementToSign.Attributes["Id"]?.Value;
            if (string.IsNullOrWhiteSpace(referenceId))
            {
                throw new InvalidOperationException($"Atributo 'Id' não encontrado ou vazio no elemento '{PrefixCTeNamespace}:infCte'.");
            }

            return referenceId;
        }
    }
}