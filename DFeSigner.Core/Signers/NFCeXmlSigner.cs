using System.Xml;
using DFeSigner.Core.Exceptions;

namespace DFeSigner.Core.Signers
{
    /// <summary>
    /// Implementação concreta da assinatura digital para NFC-e (Nota Fiscal de Consumidor Eletrônica).
    /// </summary>
    public class NFCeXmlSigner : DFeXmlSigner
    {
        private const string _nfceNamespace = "http://www.portalfiscal.inf.br/nfe";
        private const string _prefix = "nfe";
        private const string _rootElement = "infNFe";
        private const string _documentModel = "65";

        /// <summary>
        /// Implementação específica para NFC-e para identificar o elemento root 'infNFe' a ser assinado.
        /// Embora o nome do elemento seja 'infNFe' (o mesmo da NF-e), este método valida
        /// que o XML é de fato uma NFC-e através do atributo 'mod="65"' no node 'ide'.
        /// </summary>
        /// <param name="document">O objeto XmlDocument contendo o XML da NFC-e.</param>
        /// <returns>Uma string contendo o atributo 'Id' do elemento root 'infNFe'.</returns>
        /// <exception cref="MissingXmlElementException">Lançada se o elemento 'ide' não for encontrado.</exception>
        /// <exception cref="UnexpectedDocumentTypeException">Lançada se o elemento 'mod' for diferente de 65(NFC-e).</exception>
        /// <exception cref="InvalidXmlFormatException">Lançada se o elemento root para a assinatura 'infNFe' não for encontrado.</exception>
        /// <exception cref="MissingReferenceIdException">Lançada se o atributo referenceId não for encontrado no elemento root 'infNFe'.</exception>
        protected override string GetReferenceId(XmlDocument document)
        {
            XmlNamespaceManager ns = new XmlNamespaceManager(document.NameTable);
            ns.AddNamespace(_prefix, _nfceNamespace);

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