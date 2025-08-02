using System.Xml;
using DFeSigner.Core.Exceptions;

namespace DFeSigner.Core.Signers
{
    /// <summary>
    /// Implementação concreta da assinatura digital para MDF-e.
    /// </summary>
    public class MDFeXmlSigner : DFeXmlSigner
    {
        private const string _mdfeNamespace = "http://www.portalfiscal.inf.br/mdfe";
        private const string _prefix = "mdfe";
        private const string _rootElement = "infMDFe";
        private const string _documentModel = "58";

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
            ns.AddNamespace(_prefix, _mdfeNamespace);

            XmlElement ideElement = document.SelectSingleNode($"//{_prefix}:{IdeTagElement}", ns) as XmlElement;
            if (ideElement == null)
            {
                throw new MissingXmlElementException(IdeTagElement, _rootElement);
            }

            string model = document.SelectSingleNode($"//{_prefix}:{ModTagElement}", ns)?.InnerText;
            if (model != _documentModel)
            {
                throw new UnexpectedDocumentTypeException(_documentModel, model);
            }

            XmlElement elementToSign = document.SelectSingleNode($"//{_prefix}:{_rootElement}", ns) as XmlElement;
            if (elementToSign == null)
            {
                throw new InvalidXmlFormatException($"{_prefix}:{_rootElement}");
            }

            string referenceId = elementToSign.Attributes["Id"]?.Value;
            if (string.IsNullOrWhiteSpace(referenceId))
            {
                throw new MissingReferenceIdException($"{_prefix}:{_rootElement}");
            }

            return referenceId;
        }
    }
}