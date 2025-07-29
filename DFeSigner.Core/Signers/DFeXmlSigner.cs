using System;
using System.Security.Cryptography.Xml;
using System.Security.Cryptography.X509Certificates;
using System.Xml;

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
        /// <exception cref="ArgumentException">Lançada se o conteúdo XML for nulo ou vazio.</exception>
        /// <exception cref="ArgumentNullException">Lançada se o certificado for nulo.</exception>
        /// <exception cref="InvalidOperationException">Lançada se o elemento a ser assinado não for encontrado ou o certificado não tiver chave privada.</exception>
        /// <exception cref="ApplicationException">Lançada para erros gerais durante o processo de assinatura.</exception>
        public string Sign(string xmlContent, X509Certificate2 certificate)
        {
            if (string.IsNullOrWhiteSpace(xmlContent))
                throw new ArgumentException("O conteúdo XML não pode ser nulo ou vazio.", nameof(xmlContent));
            if (certificate == null)
                throw new ArgumentNullException(nameof(certificate), "O certificado digital não pode ser nulo.");

            XmlDocument doc = new();
            doc.PreserveWhitespace = true;
            doc.LoadXml(xmlContent);

            // Chama o método abstrato para obter o elemento a ser assinado e seu ID
            (XmlElement elementToSign, string referenceId) = GetElementToSign(doc);

            if (elementToSign == null)
            {
                throw new InvalidOperationException("Não foi possível encontrar o elemento XML para assinar. Verifique a implementação de GetElementToSign.");
            }

            SignedXml signedXml = new(doc)
            {
                SigningKey = certificate.GetRSAPrivateKey()
            };
            if (signedXml.SigningKey == null)
            {
                throw new InvalidOperationException("O certificado não possui uma chave privada RSA acessível ou compatível para assinatura.");
            }

            signedXml.SignedInfo.CanonicalizationMethod = SignedXml.XmlDsigC14NTransformUrl;
            signedXml.SignedInfo.SignatureMethod = SignedXml.XmlDsigRSASHA256Url;

            Reference reference = new($"#{referenceId}");
            reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
            reference.AddTransform(new XmlDsigC14NTransform());
            reference.DigestMethod = SignedXml.XmlDsigSHA256Url;

            signedXml.AddReference(reference);

            KeyInfo keyInfo = new();
            keyInfo.AddClause(new KeyInfoX509Data(certificate));
            signedXml.KeyInfo = keyInfo;

            signedXml.ComputeSignature();

            XmlElement xmlSignature = signedXml.GetXml();

            doc.DocumentElement.AppendChild(xmlSignature);

            return doc.OuterXml;
        }

        /// <summary>
        /// Método abstrato que deve ser implementado pelas classes concretas
        /// para identificar o elemento XML a ser assinado e seu ID de referência.
        /// </summary>
        /// <param name="document">O objeto XmlDocument contendo o XML a ser processado.</param>
        /// <returns>Uma tupla contendo o XmlElement a ser assinado e o seu atributo 'Id'.</returns>
        protected abstract (XmlElement Element, string ReferenceId) GetElementToSign(XmlDocument document);
    }
}