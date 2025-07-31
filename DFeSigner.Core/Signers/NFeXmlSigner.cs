using System.Xml;
using DFeSigner.Core.Exceptions;

namespace DFeSigner.Core.Signers
{
    /// <summary>
    /// Implementação concreta da assinatura digital para NF-e.
    /// </summary>
    public class NFeXmlSigner : DFeXmlSigner
    {
        private const string NFeNamespace = "http://www.portalfiscal.inf.br/nfe";
        private const string PrefixNFeNamespace = "nfe";
        private const string RootElement = "infNFe";

        /// <summary>
        /// Implementação específica para NF-e para identificar o elemento root 'infNFe' a ser assinado.
        /// </summary>
        /// <param name="document">O objeto XmlDocument contendo o XML da NF-e.</param>
        /// <returns>Uma string contendo o atributo 'Id' do elemento root 'infNFe'.</returns>
        /// <exception cref="MissingXmlElementException">Lançada se o elemento 'ide' não for encontrado.</exception>
        /// <exception cref="UnexpectedDocumentTypeException">Lançada se o elemento 'mod' for diferente de 55(NF-e).</exception>
        /// <exception cref="InvalidXmlFormatException">Lançada se o elemento root para a assinatura 'infNFe' não for encontrado.</exception>
        /// <exception cref="MissingReferenceIdException">Lançada se o atributo referenceId não for encontrado no elemento root 'infNFe'.</exception>
        /// 
        protected override string GetReferenceId(XmlDocument document)
        {
            XmlNamespaceManager ns = new(document.NameTable);
            ns.AddNamespace(PrefixNFeNamespace, NFeNamespace);

            XmlElement ideElement = document.SelectSingleNode($"//{PrefixNFeNamespace}:ide", ns) as XmlElement;
            if (ideElement == null)
            {
                throw new MissingXmlElementException("ide", RootElement);
            }

            string model = document.SelectSingleNode($"//{PrefixNFeNamespace}:mod", ns)?.InnerText;
            if (model != "55")
            {
                throw new UnexpectedDocumentTypeException("55", model);
            }

            XmlElement elementToSign = document.SelectSingleNode($"//{PrefixNFeNamespace}:{RootElement}", ns) as XmlElement;
            if (elementToSign == null)
            {
                throw new InvalidXmlFormatException($"{PrefixNFeNamespace}:{RootElement}");
            }

            string referenceId = elementToSign.Attributes["Id"]?.Value;
            if (string.IsNullOrWhiteSpace(referenceId))
            {
                throw new MissingReferenceIdException($"{PrefixNFeNamespace}:{RootElement}");
            }

            return referenceId;
        }   
    }
}