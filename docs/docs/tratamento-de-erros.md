## Tratamento de Erros

A biblioteca implementa um tratamento de erros robusto, lançando exceções personalizadas para diferentes cenários. É recomendável capturar a `DFeSignerException` e suas subclasses para um controle mais granular.

```csharp
// Exemplo de uso em um método Main para demonstrar a estrutura completa com tratamento de erros
public class ExemploAssinaturaCompleto
{
    public static void Main(string[] args)
    {
        // Caminhos de exemplo (ajuste para seus arquivos reais)
        string xmlNFePath = "caminho/para/sua/nfe.xml";
        string xmlNFCePath = "caminho/para/sua/nfce.xml";
        string xmlCTePath = "caminho/para/sua/cte.xml";
        string xmlMDFePath = "caminho/para/sua/mdfe.xml";
        string certificatePath = "caminho/para/seu/certificado.pfx";
        string certificatePassword = "sua_senha_do_certificado";

        X509Certificate2 certificate = null;
        try
        {
            certificate = CertificateLoader.LoadCertificate(certificatePath, certificatePassword);

            NFeSignerExample.Run(certificate, xmlNFePath);
            NFCeSignerExample.Run(certificate, xmlNFCePath);
            CTeSignerExample.Run(certificate, xmlCTePath);
            MDFeSignerExample.Run(certificate, xmlMDFePath);
        }
        catch (DFeSignerException dfeEx)
        {
            Console.WriteLine($"Erro na assinatura/validação do DF-e: {dfeEx.Message}");
            if (dfeEx.InnerException != null)
            {
                Console.WriteLine($"Detalhes internos: {dfeEx.InnerException.Message}");
            }
            if (dfeEx is InvalidCertificateException)
            {
                Console.WriteLine("Verifique o certificado digital (caminho, senha ou chave privada).");
            }
            else if (dfeEx is InvalidXmlFormatException xmlFormatEx)
            {
                Console.WriteLine($"O XML não está formatado corretamente. Tag raiz esperada: {xmlFormatEx.ExpectedRootTag}");
            }
            else if (dfeEx is MissingReferenceIdException missingIdEx)
            {
                Console.WriteLine($"O XML não possui o referenceId necessário no elemento '{missingIdEx.ElementName}'.");
            }
            else if (dfeEx is MissingSignatureElementException)
            {
                Console.WriteLine("O XML não contém o elemento de assinatura digital (<Signature>).");
            }
            else if (dfeEx is UnexpectedDocumentTypeException unexpectedTypeEx)
            {
                Console.WriteLine($"Tipo de documento inesperado. Esperado: {unexpectedTypeEx.ExpectedModel}, Encontrado: {unexpectedTypeEx.FoundModel}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ocorreu um erro inesperado: {ex.Message}");
            Console.WriteLine(ex.ToString());
        }
    }
}
```