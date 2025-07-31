using System;
using System.Security.Cryptography.Xml;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using DFeSigner.Core.Exceptions;

namespace DFeSigner.Core.Signers
{
    /// <summary>
    /// Classe abstrata base para a assinatura digital de Documentos Fiscais Eletrônicos (DF-e).
    /// Esta classe define o contrato para a assinatura e manipula as operações comuns de criptografia XML.
    /// </summary>
    public abstract class DFeXmlSigner
    {
        /// <summary>
        /// Assina digitalmente um documento XML de DF-e usando um certificado digital.
        /// Este método é o ponto de entrada principal para a assinatura.
        /// </summary>
        /// <param name="xmlContent">O conteúdo do XML a ser assinado.</param>
        /// <param name="certificate">O certificado digital X.509 a ser usado para a assinatura.</param>
        /// <returns>O XML assinado como uma string, ou lança uma exceção em caso de erro.</returns>
        /// <exception cref="InvalidXmlFormatException">Lançada se o conteúdo XML for nulo ou vazio.</exception>
        /// <exception cref="InvalidCertificateException">Lançada se o certificado for nulo ou não possuir uma chave privada acessível.</exception>
        public string Sign(string xmlContent, X509Certificate2 certificate)
        {
            ValidateInput(xmlContent, certificate);

            XmlDocument doc = GetXmlDocument(xmlContent);
            string referenceId = GetReferenceId(doc);
            SignedXml signedXml = GetInitializedSignedXml(doc, certificate);
            signedXml.AddReference(GetReference(referenceId));
            signedXml.KeyInfo = GetKeyInfo(certificate);

            signedXml.ComputeSignature();
            XmlElement xmlSignature = signedXml.GetXml();
            doc.DocumentElement.AppendChild(xmlSignature);

            return doc.OuterXml;
        }

        /// <summary>
        /// Verifica se um documento XML possui uma assinatura digital válida.
        /// Este método procura pelo elemento &lt;Signature&gt; no namespace XML Digital Signature
        /// e valida a integridade da assinatura.
        /// </summary>
        /// <param name="xmlContent">O conteúdo do XML como uma string para ser validado.</param>
        /// <returns>
        /// <c>true</c> se a assinatura digital no XML for válida; caso contrário, <c>false</c>.
        /// </returns>
        /// <exception cref="InvalidXmlFormatException">Lançada quando o <paramref name="xmlContent"/> é nulo ou vazio.</exception>
        /// <exception cref="MissingSignatureElementException">Lançada quando o elemento 'Signature' não é encontrado no XML.</exception>
        public bool IsSignatureValid(string xmlContent)
        {
            XmlDocument doc = GetXmlDocument(xmlContent);
            SignedXml signedXml = new(doc);

            XmlElement signatureElement = doc.GetElementsByTagName("Signature", SignedXml.XmlDsigNamespaceUrl)[0] as XmlElement;
            if (signatureElement == null)
            {
                throw new MissingSignatureElementException();
            }

            signedXml.LoadXml(signatureElement);

            return signedXml.CheckSignature();
        }

        /// <summary>
        /// Método abstrato que deve ser implementado pelas classes concretas
        /// para identificar o elemento XML a ser assinado e seu ID de referência.
        /// </summary>
        /// <param name="document">O objeto XmlDocument contendo o XML a ser processado.</param>
        /// <returns>Uma string contendo o atributo 'Id' do elemento root a ser assinado.</returns>
        protected abstract string GetReferenceId(XmlDocument document);

        /// <summary>
        /// Realiza validações iniciais nos parâmetros de entrada para o processo de assinatura.
        /// Lança exceções se o conteúdo XML ou o certificado forem nulos ou inválidos.
        /// </summary>
        /// <param name="xmlContent">O conteúdo do XML a ser validado.</param>
        /// <param name="certificate">O certificado digital X.509 a ser validado.</param>
        /// <exception cref="InvalidXmlFormatException">Lançada se o conteúdo XML for nulo ou vazio.</exception>
        /// <exception cref="InvalidCertificateException">Lançada se o certificado for nulo ou não possuir uma chave privada acessível.</exception>
        private void ValidateInput(string xmlContent, X509Certificate2 certificate)
        {
            if (string.IsNullOrWhiteSpace(xmlContent))
                throw new InvalidXmlFormatException();
            if (certificate == null)
                throw new InvalidCertificateException();
        }

        /// <summary>
        /// Carrega o conteúdo XML em um objeto XmlDocument e configura para preservar espaços em branco.
        /// </summary>
        /// <param name="xmlContent">O conteúdo XML como string.</param>
        /// <returns>Um objeto XmlDocument carregado e configurado.</returns>
        /// <exception cref="InvalidXmlFormatException">Lançada quando o <paramref name="xmlContent"/> é nulo ou vazio.</exception>
        private XmlDocument GetXmlDocument(string xmlContent)
        {
            if (string.IsNullOrWhiteSpace(xmlContent))
                throw new InvalidXmlFormatException();
            XmlDocument doc = new();
            doc.PreserveWhitespace = true;
            doc.LoadXml(xmlContent);
            return doc;
        }

        /// <summary>
        /// Inicializa e configura o objeto SignedXml com o documento e a chave de assinatura do certificado.
        /// Este método é privado e encapsula a lógica de setup inicial da assinatura.
        /// </summary>
        /// <param name="document">O XmlDocument que será assinado.</param>
        /// <param name="certificate">O certificado X.509 contendo a chave privada.</param>
        /// <returns>Um objeto SignedXml configurado.</returns>
        /// <exception cref="InvalidOperationException">Lançada se o certificado não possuir uma chave privada RSA acessível ou compatível.</exception>
        private SignedXml GetInitializedSignedXml(XmlDocument document, X509Certificate2 certificate)
        {
            SignedXml signedXml = new SignedXml(document);
            signedXml.SigningKey = certificate.GetRSAPrivateKey();

            if (signedXml.SigningKey == null)
            {
                throw new InvalidCertificateException();
            }

            signedXml.SignedInfo.CanonicalizationMethod = SignedXml.XmlDsigC14NTransformUrl;
            signedXml.SignedInfo.SignatureMethod = SignedXml.XmlDsigRSASHA1Url;

            return signedXml;
        }

        /// <summary>
        /// Cria e configura um objeto KeyInfo para a assinatura XML.
        /// Este método encapsula a definição do URI de referência e das transformações padrão.
        /// </summary>
        /// <param name="referenceId">O ID do elemento XML a ser referenciado na assinatura.</param>
        /// <returns>Um objeto Reference configurado com as transformações e o método de digest padrão.</returns>
        private Reference GetReference(string referenceId)
        {
            Reference reference = new($"#{referenceId}");
            reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
            reference.AddTransform(new XmlDsigC14NTransform());
            reference.DigestMethod = SignedXml.XmlDsigSHA1Url;
            return reference;
        }

        /// <summary>
        /// Cria e configura um objeto KeyInfo com as informações do certificado X.509.
        /// Este método encapsula a adição dos dados do certificado à seção KeyInfo da assinatura.
        /// </summary>
        /// <param name="certificate">O certificado digital X.509 a ser incluído no KeyInfo.</param>
        /// <returns>Um objeto KeyInfo contendo os dados do certificado.</returns>
        private KeyInfo GetKeyInfo(X509Certificate2 certificate)
        {
            KeyInfo keyInfo = new();
            keyInfo.AddClause(new KeyInfoX509Data(certificate));
            return keyInfo;
        }
    }
}