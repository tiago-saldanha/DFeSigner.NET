using System.Xml;
using DFeSigner.Core.Exceptions;

namespace DFeSigner.Core.Signers
{
    /// <summary>
    /// Implementação concreta da assinatura digital para CT-e.
    /// </summary>
    public class CTeXmlSigner : DFeXmlSigner
    {
        private const string CTeNamespace = "http://www.portalfiscal.inf.br/cte";
        private const string PrefixCTeNamespace = "cte";
        private const string RootElement = "infCte";

        /// <summary>
        /// Implementação específica para CT-e para identificar o elemento root 'infCte' a ser assinado.
        /// </summary>
        /// <param name="document">O objeto XmlDocument contendo o XML da CT-e.</param>
        /// <returns>Uma string contendo o atributo 'Id' do elemento root 'infCte'.</returns>
        /// <exception cref="MissingXmlElementException">Lançada se o elemento 'ide' não for encontrado.</exception>
        /// <exception cref="UnexpectedDocumentTypeException">Lançada se o elemento 'mod' for diferente de 57(CT-e).</exception>
        /// <exception cref="InvalidXmlFormatException">Lançada se o elemento root para a assinatura 'infCte' não for encontrado.</exception>
        /// <exception cref="MissingReferenceIdException">Lançada se o atributo referenceId não for encontrado no elemento root 'infCte'.</exception>
        protected override string GetReferenceId(XmlDocument document)
        {
            XmlNamespaceManager ns = new(document.NameTable);
            ns.AddNamespace(PrefixCTeNamespace, CTeNamespace);

            XmlElement ideElement = document.SelectSingleNode($"//{PrefixCTeNamespace}:ide", ns) as XmlElement;
            if (ideElement == null)
            {
                throw new MissingXmlElementException("ide", RootElement);
            }

            string model = document.SelectSingleNode($"//{PrefixCTeNamespace}:mod", ns)?.InnerText;
            if (model != "57")
            {
                throw new UnexpectedDocumentTypeException("57", model);
            }

            XmlElement elementToSign = document.SelectSingleNode($"//{PrefixCTeNamespace}:{RootElement}", ns) as XmlElement;
            if (elementToSign == null)
            {
                throw new InvalidXmlFormatException($"{PrefixCTeNamespace}:{RootElement}");
            }

            string referenceId = elementToSign.Attributes["Id"]?.Value;
            if (string.IsNullOrWhiteSpace(referenceId))
            {
                throw new MissingReferenceIdException($"{PrefixCTeNamespace}:{RootElement}");
            }

            return referenceId;
        }
    }
}