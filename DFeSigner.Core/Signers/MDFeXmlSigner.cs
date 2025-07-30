using System;
using System.Xml;
using DFeSigner.Core.Exceptions;

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
        /// <exception cref="MissingXmlElementException">Lançada se o elemento 'ide' não for encontrado.</exception>
        /// <exception cref="UnexpectedDocumentTypeException">Lançada se o elemento 'mod' for diferente de 58(MDF-e).</exception>
        /// <exception cref="InvalidXmlFormatException">Lançada se o elemento root para a assinatura 'infMDFe' não for encontrado.</exception>
        /// <exception cref="MissingReferenceIdException">Lançada se o atributo referenceId não for encontrado no elemento 'infMDFe'.</exception>
        protected override string GetReferenceId(XmlDocument document)
        {
            XmlNamespaceManager ns = new(document.NameTable);
            ns.AddNamespace(PrefixNFeNamespace, NFeNamespace);

            XmlElement ideElement = document.GetElementsByTagName("ide")[0] as XmlElement;
            if (ideElement == null)
            {
                throw new MissingXmlElementException("ide", "infMDFe");
            }

            string modeloDocumento = document.GetElementsByTagName("mod")[0].InnerText;
            if (modeloDocumento != "58")
            {
                throw new UnexpectedDocumentTypeException("58", modeloDocumento);
            }

            XmlElement elementToSign = document.GetElementsByTagName("infMDFe")[0] as XmlElement;
            if (elementToSign == null)
            {
                throw new InvalidXmlFormatException($"{PrefixNFeNamespace}:infMDFe");
            }

            string referenceId = elementToSign.Attributes["Id"]?.Value;
            if (string.IsNullOrWhiteSpace(referenceId))
            {
                throw new MissingReferenceIdException($"{PrefixNFeNamespace}:infMDFe");
            }

            return referenceId;
        }
    }
}