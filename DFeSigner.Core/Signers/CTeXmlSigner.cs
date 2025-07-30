using System.Xml;
using DFeSigner.Core.Exceptions;

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
        /// <exception cref="MissingXmlElementException">Lançada se o elemento 'ide' não for encontrado.</exception>
        /// <exception cref="UnexpectedDocumentTypeException">Lançada se o elemento 'mod' for diferente de 57(CT-e).</exception>
        /// <exception cref="InvalidXmlFormatException">Lançada se o elemento root para a assinatura 'infCte' não for encontrado.</exception>
        /// <exception cref="MissingReferenceIdException">Lançada se o atributo referenceId não for encontrado no elemento 'infCte'.</exception>
        protected override string GetReferenceId(XmlDocument document)
        {
            XmlNamespaceManager ns = new(document.NameTable);
            ns.AddNamespace(PrefixCTeNamespace, CTeNamespace);

            XmlElement ideElement = document.GetElementsByTagName("ide")[0] as XmlElement;
            if (ideElement == null)
            {
                throw new MissingXmlElementException("ide", "infCte");
            }

            string modeloDocumento = document.GetElementsByTagName("mod")[0].InnerText;
            if (modeloDocumento != "57")
            {
                throw new UnexpectedDocumentTypeException("57", modeloDocumento);
            }

            XmlElement elementToSign = document.GetElementsByTagName("infCte")[0] as XmlElement;
            if (elementToSign == null)
            {
                throw new InvalidXmlFormatException($"{PrefixCTeNamespace}:infCte");
            }

            string referenceId = elementToSign.Attributes["Id"]?.Value;
            if (string.IsNullOrWhiteSpace(referenceId))
            {
                throw new MissingReferenceIdException($"{PrefixCTeNamespace}:infCte");
            }

            return referenceId;
        }
    }
}