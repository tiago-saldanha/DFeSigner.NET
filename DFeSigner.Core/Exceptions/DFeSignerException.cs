using System;

namespace DFeSigner.Core.Exceptions
{
    /// <summary>
    /// Exceção base para erros relacionados à assinatura de documentos fiscais eletrônicos.
    /// </summary>
    public class DFeSignerException : Exception
    {
        public DFeSignerException(string message) : base(message) { }
    }

    /// <summary>
    /// Exceção para quando um certificado digital inválido ou sem chave privada é utilizado.
    /// </summary>
    public class InvalidCertificateException : DFeSignerException
    {
        public InvalidCertificateException() : base("O certificado digital fornecido é inválido ou não possui uma chave privada acessível.") { }
    }

    /// <summary>
    /// Exceção para quando o XML fornecido não possui o elemento raiz esperado para a assinatura digital.
    /// </summary>
    public class InvalidXmlFormatException : DFeSignerException
    {
        /// <summary>
        /// Obtém o nome do elemento XML raiz que era esperado para a assinatura.
        /// </summary>
        public string ExpectedRootTag { get; }

        public InvalidXmlFormatException()
            : base("O XML fornecido não está no formato esperado ou é nulo/vazio.")
        {
        }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="InvalidXmlFormatException"/>
        /// com uma mensagem de erro especificada e o nome da tag raiz esperada.
        /// </summary>
        /// <param name="expectedRootTag">O nome da tag raiz esperada para a assinatura digital (ex: "infNFe", "infCTe", "infMDFe").</param>
        public InvalidXmlFormatException(string expectedRootTag)
            : base($"O XML fornecido não contém a tag raiz esperada para assinatura digital: '{expectedRootTag}'.")
        {
            ExpectedRootTag = expectedRootTag;
        }
    }

    /// <summary>
    /// Exceção para quando o XML fornecido não contém o atributo 'Id' (referenceId)
    /// necessário para a assinatura digital no elemento raiz do documento fiscal eletrônico.
    /// </summary>
    public class MissingReferenceIdException : DFeSignerException
    {
        /// <summary>
        /// Obtém o nome do elemento XML onde o atributo 'Id' era esperado (ex: "infNFe", "infCTe", "infMDFe").
        /// </summary>
        public string ElementName { get; }

        /// <summary>
        /// Obtém o nome do atributo de referência que estava faltando (geralmente "Id").
        /// </summary>
        public string AttributeName { get; } = "Id";

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="MissingReferenceIdException"/>
        /// com o nome do elemento XML onde o atributo 'Id' era esperado.
        /// </summary>
        /// <param name="elementName">O nome do elemento XML (ex: "infNFe", "infCTe", "infMDFe").</param>
        public MissingReferenceIdException(string elementName)
            : base($"O atributo 'Id' (referenceId) não foi encontrado ou está vazio no elemento '{elementName}'.")
        {
            ElementName = elementName;
        }
    }

    /// <summary>
    /// Exceção para quando o XML é válido, mas não é do tipo de documento fiscal esperado (ex: NF-e passada para assinador de MDF-e).
    /// </summary>
    public class UnexpectedDocumentTypeException : DFeSignerException
    {
        public string ExcepctedModel { get; }
        public string FoundModel { get; }

        public UnexpectedDocumentTypeException(string expectedModel, string foundModel)
            : base($"O XML fornecido não é do tipo de documento esperado. Esperado modelo: {expectedModel}, Encontrado modelo: {foundModel}.")
        {
            ExcepctedModel = expectedModel;
            FoundModel = foundModel;
        }
    }

    /// <summary>
    /// Exceção para quando um elemento XML crítico para a assinatura não é encontrado.
    /// </summary>
    public class MissingXmlElementException : DFeSignerException
    {
        public string ElementName { get; }
        public string ParentElement { get; }

        public MissingXmlElementException(string elementName, string parentElement)
            : base($"Elemento '{elementName}' não encontrado no XML{(parentElement != null ? $" dentro de '{parentElement}'" : "")}.")
        {
            ElementName = elementName;
            ParentElement = parentElement;
        }
    }

    /// <summary>
    /// Exceção para quando um XML não contém a tag Signature.
    /// </summary>
    public class MissingSignatureElementException : DFeSignerException
    {
        public MissingSignatureElementException()
            : base("O XML fornecido não contém a tag Signature necessária para a validação da assinatura digital.") { }
    }
}
