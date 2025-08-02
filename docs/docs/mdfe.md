## Assinatura e Validação de MDF-e (Manifesto Eletrônico de Documentos Fiscais)

```csharp
// --- Exemplo com MDF-e ---
public class MDFeSignerExample
{
    public static void Run(X509Certificate2 certificate, string xmlMDFePath)
    {
        Console.WriteLine("--- Assinando MDF-e ---");
        try
        {
            string xmlMDFeContent = File.ReadAllText(xmlMDFePath);
            MDFeXmlSigner mdfeSigner = new MDFeXmlSigner();
            string signedMDFeXml = mdfeSigner.Sign(xmlMDFeContent, certificate);
            Console.WriteLine("MDF-e assinada com sucesso!");
            File.WriteAllText("mdfe_assinada.xml", signedMDFeXml);

            Console.WriteLine("Validando assinatura do MDF-e...");
            bool isMDFeSignatureValid = mdfeSigner.IsSignatureValid(signedMDFeXml);
            Console.WriteLine($"Assinatura do MDF-e é válida: {isMDFeSignatureValid}");
        }
        catch (DFeSignerException dfeEx)
        {
            Console.WriteLine($"Erro na assinatura/validação do MDF-e: {dfeEx.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ocorreu um erro inesperado no MDF-e: {ex.Message}");
        }
    }
}
```