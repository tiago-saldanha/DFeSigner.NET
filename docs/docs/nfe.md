## Assinatura e Validação de NF-e (Nota Fiscal Eletrônica)

```csharp
using DFeSigner.Core.Signers;
using DFeSigner.Core.Exceptions;
using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;

// --- Exemplo com NF-e ---
public class NFeSignerExample
{
    public static void Run(X509Certificate2 certificate, string xmlNFePath)
    {
        Console.WriteLine("--- Assinando NF-e ---");
        try
        {
            string xmlNFeContent = File.ReadAllText(xmlNFePath);
            NFeXmlSigner nfeSigner = new NFeXmlSigner();
            string signedNFeXml = nfeSigner.Sign(xmlNFeContent, certificate);
            Console.WriteLine("NF-e assinada com sucesso!");
            File.WriteAllText("nfe_assinada.xml", signedNFeXml);

            Console.WriteLine("Validando assinatura da NF-e...");
            bool isNFeSignatureValid = nfeSigner.IsSignatureValid(signedNFeXml);
            Console.WriteLine($"Assinatura da NF-e é válida: {isNFeSignatureValid}");
        }
        catch (DFeSignerException dfeEx)
        {
            Console.WriteLine($"Erro na assinatura/validação da NF-e: {dfeEx.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ocorreu um erro inesperado na NF-e: {ex.Message}");
        }
    }
}
```