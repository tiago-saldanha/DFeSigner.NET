### Carregar o Certificado Digital

Primeiro, carregue seu certificado digital X.509. Você pode usar um arquivo PFX (A1) ou um certificado instalado no repositório do Windows (A3).

```csharp
using System.Security.Cryptography.X509Certificates;
using System.IO;

public class CertificateLoader
{
    public static X509Certificate2 LoadCertificate(string certificatePath, string certificatePassword)
    {
        // Para certificados PFX (A1)
        X509Certificate2 certificate = new X509Certificate2(
            certificatePath,
            certificatePassword,
            X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet
        );
        Console.WriteLine("Certificado carregado com sucesso.");
        return certificate;
    }

    // Exemplo para carregar certificado A3 por nome do assunto (subject name)
    public static X509Certificate2 GetCertificateBySubjectName(string subjectName)
    {
        X509Store store = new X509Store(StoreLocation.CurrentUser); // Ou CurrentUser, LocalMachine
        try
        {
            store.Open(OpenFlags.ReadOnly);
            foreach (X509Certificate2 cert in store.Certificates)
            {
                if (cert.Subject.Contains(subjectName))
                {
                    if (cert.HasPrivateKey)
                    {
                        Console.WriteLine($"Certificado '{subjectName}' encontrado e carregado.");
                        return cert;
                    }
                }
            }
        }
        finally
        {
            store.Close();
        }
        throw new Exception($"Certificado com assunto '{subjectName}' não encontrado ou sem chave privada.");
    }
}
```