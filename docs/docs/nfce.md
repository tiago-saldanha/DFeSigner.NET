## Assinatura e Validação de NFC-e (Nota Fiscal de Consumidor Eletrônica)

```csharp
// --- Exemplo com NFC-e ---
public class NFCeSignerExample
{
    public static void Run(X509Certificate2 certificate, string xmlNFCePath)
    {
        Console.WriteLine("--- Assinando NFC-e ---");
        try
        {
            string xmlNFCeContent = File.ReadAllText(xmlNFCePath);
            NFCeXmlSigner nfceSigner = new NFCeXmlSigner();
            string signedNFCeXml = nfceSigner.Sign(xmlNFCeContent, certificate);
            Console.WriteLine("NFC-e assinada com sucesso!");
            File.WriteAllText("nfce_assinada.xml", signedNFCeXml);

            Console.WriteLine("Validando assinatura da NFC-e...");
            bool isNFCeSignatureValid = nfceSigner.IsSignatureValid(signedNFCeXml);
            Console.WriteLine($"Assinatura da NFC-e é válida: {isNFCeSignatureValid}");
        }
        catch (DFeSignerException dfeEx)
        {
            Console.WriteLine($"Erro na assinatura/validação da NFC-e: {dfeEx.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ocorreu um erro inesperado na NFC-e: {ex.Message}");
        }
    }
}
```