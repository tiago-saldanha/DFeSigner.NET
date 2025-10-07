using System.Xml;
using DFeSigner.Core.Exceptions;

namespace DFeSigner.Core.Signers
{
    /// <summary>
    /// Implementação concreta da assinatura digital para NF-e.
    /// </summary>
    public class EventoNFeXmlSigner : DFeXmlSigner
    {
        private const string _nfeNamespace = "http://www.portalfiscal.inf.br/nfe";
        private const string _prefix = "nfe";
        private const string _rootElement = "infEvento";

        /// <summary>
        /// Implementação específica para Evento NFe para identificar o elemento 'infEvento' a ser assinado.
        /// </summary>
        /// <param name="document">O objeto XmlDocument contendo o XML do Evento.</param>
        /// <returns>Uma string contendo o atributo 'Id' do elemento 'infEvento'.</returns>
        /// <exception cref="InvalidXmlFormatException">Lançada se o elemento root para a assinatura 'infEvento' não for encontrado.</exception>
        /// <exception cref="MissingReferenceIdException">Lançada se o atributo referenceId não for encontrado no elemento root 'infEvento'.</exception>
        protected override string GetReferenceId(XmlDocument document)
        {
            XmlNamespaceManager ns = new(document.NameTable);
            ns.AddNamespace(_prefix, _nfeNamespace);

            XmlElement elementToSign = document.SelectSingleNode($"//*[local-name()='{_rootElement}']", ns) as XmlElement;

            if (elementToSign == null)
            {
                elementToSign = document.SelectSingleNode($"//{_prefix}:{_rootElement}", ns) as XmlElement;
            }

            if (elementToSign == null)
            {
                throw new InvalidXmlFormatException($"{_rootElement} (namespace: {_nfeNamespace})");
            }

            string referenceId = elementToSign.Attributes["Id"]?.Value;
            if (string.IsNullOrWhiteSpace(referenceId))
            {
                throw new MissingReferenceIdException(_rootElement);
            }

            return referenceId;
        }
    }
}