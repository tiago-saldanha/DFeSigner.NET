using System.Xml;
using DFeSigner.Core.Exceptions;

namespace DFeSigner.Core.Signers
{
    /// <summary>
    /// Implementação concreta da assinatura digital para MDF-e.
    /// </summary>
    public class MDFeXmlSigner : DFeXmlSigner
    {
        private const string MDFeNamespace = "http://www.portalfiscal.inf.br/mdfe";
        private const string PrefixMDFeNamespace = "mdfe";
        private const string RootElement = "infMDFe";

        /// <summary>
        /// Implementação específica para MDF-e para identificar o elemento root 'infMDFe' a ser assinado.
        /// </summary>
        /// <param name="document">O objeto XmlDocument contendo o XML da MDF-e.</param>
        /// <returns>Uma string contendo o atributo 'Id' do elemento root 'infMDFe'.</returns>
        /// <exception cref="MissingXmlElementException">Lançada se o elemento 'ide' não for encontrado.</exception>
        /// <exception cref="UnexpectedDocumentTypeException">Lançada se o elemento 'mod' for diferente de 58(MDF-e).</exception>
        /// <exception cref="InvalidXmlFormatException">Lançada se o elemento root para a assinatura 'infMDFe' não for encontrado.</exception>
        /// <exception cref="MissingReferenceIdException">Lançada se o atributo referenceId não for encontrado no elemento root 'infMDFe'.</exception>
        protected override string GetReferenceId(XmlDocument document)
        {
            XmlNamespaceManager ns = new(document.NameTable);
            ns.AddNamespace(PrefixMDFeNamespace, MDFeNamespace);

            XmlElement ideElement = document.SelectSingleNode($"//{PrefixMDFeNamespace}:ide", ns) as XmlElement;
            if (ideElement == null)
            {
                throw new MissingXmlElementException("ide", RootElement);
            }

            string model = document.SelectSingleNode($"//{PrefixMDFeNamespace}:mod", ns)?.InnerText;
            if (model != "58")
            {
                throw new UnexpectedDocumentTypeException("58", model);
            }

            XmlElement elementToSign = document.SelectSingleNode($"//{PrefixMDFeNamespace}:{RootElement}", ns) as XmlElement;
            if (elementToSign == null)
            {
                throw new InvalidXmlFormatException($"{PrefixMDFeNamespace}:{RootElement}");
            }

            string referenceId = elementToSign.Attributes["Id"]?.Value;
            if (string.IsNullOrWhiteSpace(referenceId))
            {
                throw new MissingReferenceIdException($"{PrefixMDFeNamespace}:{RootElement}");
            }

            return referenceId;
        }
    }
}