using System;
using System.Xml;
using DFeSigner.Core.Exceptions;

namespace DFeSigner.Core.Signers
{
    /// <summary>
    /// Implementação concreta da assinatura digital para NF-e.
    /// </summary>
    public class NFeXmlSigner : DFeXmlSigner
    {
        private static readonly string NFeNamespace = "http://www.portalfiscal.inf.br/nfe";
        private static readonly string PrefixNFeNamespace = "nfe";

        /// <summary>
        /// Implementação específica para NF-e para identificar o elemento 'infNFe' a ser assinado.
        /// </summary>
        /// <param name="document">O objeto XmlDocument contendo o XML da NF-e.</param>
        /// <returns>Uma tupla contendo o XmlElement 'infNFe' e o seu atributo 'Id'.</returns>
        /// <exception cref="MissingXmlElementException">Lançada se o elemento 'ide' não for encontrado.</exception>
        /// <exception cref="UnexpectedDocumentTypeException">Lançada se o elemento 'mod' for diferente de 55(NF-e).</exception>
        /// <exception cref="InvalidXmlFormatException">Lançada se o elemento root para a assinatura 'infNFe' não for encontrado.</exception>
        /// <exception cref="MissingReferenceIdException">Lançada se o atributo referenceId não for encontrado no elemento 'infNFe'.</exception>
        /// 
        protected override string GetReferenceId(XmlDocument document)
        {
            XmlNamespaceManager ns = new(document.NameTable);
            ns.AddNamespace(PrefixNFeNamespace, NFeNamespace);

            XmlElement ideElement = document.GetElementsByTagName("ide")[0] as XmlElement;
            if (ideElement == null)
            {
                throw new MissingXmlElementException("ide", "infNFe");
            }

            string modeloDocumento = document.GetElementsByTagName("mod")[0].InnerText;
            if (modeloDocumento != "55")
            {
                throw new UnexpectedDocumentTypeException("55", modeloDocumento);
            }

            XmlElement elementToSign = document.GetElementsByTagName("infNFe")[0] as XmlElement;
            if (elementToSign == null)
            {
                throw new InvalidXmlFormatException($"{PrefixNFeNamespace}:infNFe");
            }

            string referenceId = elementToSign.Attributes["Id"]?.Value;
            if (string.IsNullOrWhiteSpace(referenceId))
            {
                throw new MissingReferenceIdException($"{PrefixNFeNamespace}:infNFe");
            }

            return referenceId;
        }   
    }
}