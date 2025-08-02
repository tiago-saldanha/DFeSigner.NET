## Assinatura e Validação de CT-e (Conhecimento de Transporte Eletrônico)

```csharp
// --- Exemplo com CT-e ---
public class CTeSignerExample
{
    public static void Run(X509Certificate2 certificate, string xmlCTePath)
    {
        Console.WriteLine("--- Assinando CT-e ---");
        try
        {
            string xmlCTeContent = File.ReadAllText(xmlCTePath);
            CTeXmlSigner cteSigner = new CTeXmlSigner();
            string signedCTeXml = cteSigner.Sign(xmlCTeContent, certificate);
            Console.WriteLine("CT-e assinada com sucesso!");
            File.WriteAllText("cte_assinada.xml", signedCTeXml);

            Console.WriteLine("Validando assinatura do CT-e...");
            bool isCTeSignatureValid = cteSigner.IsSignatureValid(signedCTeXml);
            Console.WriteLine($"Assinatura do CT-e é válida: {isCTeSignatureValid}");
        }
        catch (DFeSignerException dfeEx)
        {
            Console.WriteLine($"Erro na assinatura/validação do CT-e: {dfeEx.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ocorreu um erro inesperado no CT-e: {ex.Message}");
        }
    }
}
```