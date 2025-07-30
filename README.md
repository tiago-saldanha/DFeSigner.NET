# DFeSigner.NET

![.NET](https://img.shields.io/badge/.NET-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)
![xUnit](https://img.shields.io/badge/xUnit-802B7D?style=for-the-badge&logo=xunit&logoColor=white)
![GitHub](https://img.shields.io/badge/GitHub-100000?style=for-the-badge&logo=github&logoColor=white)
![License](https://img.shields.io/badge/License-MIT-green.svg)

## 📖 Sobre o Projeto

`DFeSigner.NET` é uma biblioteca em C# (.NET) projetada para simplificar o processo de assinatura digital de Documentos Fiscais Eletrônicos (DF-e) no padrão brasileiro. Utilizando certificados digitais X.509 (A1 ou A3), a biblioteca oferece uma solução flexível e robusta para integrar a funcionalidade de assinatura em suas aplicações, com foco inicial em NF-e (Nota Fiscal Eletrônica), NFC-e (Nota Fiscal de Consumidor Eletrônica), CT-e (Conhecimento de Transporte Eletrônico) e MDF-e (Manifesto Eletrônico de Documentos Fiscais).

O projeto adota uma arquitetura modular permitindo a fácil extensão para outros tipos de DF-e no futuro. Acompanha um projeto de testes unitários abrangente para garantir a confiabilidade e a validação das funcionalidades de assinatura.

## ✨ Funcionalidades

* **Assinatura de NF-e:** AAssina digitalmente documentos XML de Nota Fiscal Eletrônica (modelo 55).
* **Assinatura de NFC-e:** Assina digitalmente documentos XML de Nota Fiscal de Consumidor Eletrônica (modelo 65).
* **Assinatura de CT-e:** Assina digitalmente documentos XML de Conhecimento de Transporte Eletrônico (modelo 57).
* **Assinatura de MDF-e:** Assina digitalmente documentos XML de Manifesto Eletrônico de Documentos Fiscais (modelo 58).

* **Validação de Tipo de Documento:** As implementações concretas (NF-e, NFC-e, CT-e, MDF-e) validam o modelo do documento no XML para garantir que o tipo correto de assinador está sendo usado.
* **Tratamento de Erros Robusto:** Lançamento de exceções personalizadas e claras para cenários como XML inválido, certificado nulo, certificado sem chave privada, XML sem a tag raiz esperada ou sem o referenceId.
* **Testes Unitários:** Projeto de testes dedicado (xUnit) com exemplos de uso e cenários de sucesso e falha.
* **Suporte a Certificados X.509:** Compatível com certificados PFX (A1) carregados de arquivo ou certificados instalados no repositório do Windows (A3).

## 🚀 Tecnologias Utilizadas

* **C# (.NET):** Linguagem de programação e plataforma principal.
* **XML (System.Xml):** Para manipulação e leitura de documentos XML.
* **Criptografia XML (System.Security.Cryptography.Xml):** Componente do .NET para operações de assinatura XML digital.
* **Certificados X.509 (System.Security.Cryptography.X509Certificates):** Para gerenciamento e uso de certificados digitais.
* **xUnit:** Framework de testes unitários para .NET.

## 🛠️ Instalação e Configuração

Para configurar e executar o projeto, siga os passos abaixo:

1.  **Clone o Repositório:**
    ```bash
    git clone https://github.com/tiago-saldanha/DFeSigner.NET.git
    cd DFeSigner
    ```

2.  **Abra no Visual Studio:**
    * Abra o arquivo de solução `DFeSigner.sln` no Visual Studio (2019 ou superior).

3.  **Restaure as Dependências:**
    * O Visual Studio deve restaurar automaticamente os pacotes NuGet. Caso contrário, clique com o botão direito na solução e selecione `Restore NuGet Packages`.

4.  **Construa a Solução:**
    * Clique em `Build` > `Build Solution` (ou `Ctrl+Shift+B`) para compilar os projetos `DFeSigner.Core` e `DFeSigner.Tests`.

## 💡 Como Usar

A biblioteca é projetada para ser fácil de usar. Você precisa de um conteúdo XML (string) e um certificado digital X.509.

### **Exemplo de Uso (Assinando uma NF-e):**

```csharp
using DFeSigner.Core.Signers;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using System;
using DFeSigner.Core.Exceptions;

public class ExemploAssinatura
{
    public static void Main(string[] args)
    {
        try
        {
            string xmlNFePath = "caminho/para/sua/nfe.xml";
            string xmlNFCePath = "caminho/para/sua/nfce.xml";
            string xmlCTePath = "caminho/para/sua/cte.xml";
            string xmlMDFePath = "caminho/para/sua/mdfe.xml";
            string certificatePath = "caminho/para/seu/certificado.pfx";
            string certificatePassword = "sua_senha_do_certificado"; 
            
            // 1. Carregar o Certificado Digital (exemplo de certificado PFX com senha)
            // Para certificados A3 (instalados no Windows), você pode usar o GetCertificateBySubjectName().
            X509Certificate2 certificate = new X509Certificate2(
                certificatePath, 
                certificatePassword, 
                X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet
            );
            Console.WriteLine("Certificado carregado com sucesso.");

            // --- Exemplo com NF-e ---
            Console.WriteLine("\nAssinando NF-e...");
            string xmlNFeContent = File.ReadAllText(xmlNFePath);
            NFeXmlSigner nfeSigner = new NFeXmlSigner();
            string signedNFeXml = nfeSigner.Sign(xmlNFeContent, certificate);
            Console.WriteLine("NF-e assinada com sucesso!");
            File.WriteAllText("nfe_assinada.xml", signedNFeXml);

            // --- Exemplo com NFC-e ---
            Console.WriteLine("\nAssinando NFC-e...");
            string xmlNFCeContent = File.ReadAllText(xmlNFCePath);
            NFCeXmlSigner nfceSigner = new NFCeXmlSigner();
            string signedNFCeXml = nfceSigner.Sign(xmlNFCeContent, certificate);
            Console.WriteLine("NFC-e assinada com sucesso!");
            File.WriteAllText("nfce_assinada.xml", signedNFCeXml);

            // --- Exemplo com CT-e ---
            Console.WriteLine("\nAssinando CT-e...");
            string xmlCTeContent = File.ReadAllText(xmlCTePath);
            CTeXmlSigner cteSigner = new CTeXmlSigner();
            string signedCTeXml = cteSigner.Sign(xmlCTeContent, certificate);
            Console.WriteLine("CT-e assinada com sucesso!");
            File.WriteAllText("cte_assinada.xml", signedCTeXml);

            // --- Exemplo com MDF-e ---
            Console.WriteLine("\nAssinando MDF-e...");
            string xmlMDFeContent = File.ReadAllText(xmlMDFePath);
            MDFeXmlSigner mdfeSigner = new MDFeXmlSigner();
            string signedMDFeXml = mdfeSigner.Sign(xmlMDFeContent, certificate);
            Console.WriteLine("MDF-e assinada com sucesso!");
            File.WriteAllText("mdfe_assinada.xml", signedMDFeXml);

        }
        catch (DFeSignerException dfeEx)
        {
            Console.WriteLine($"Erro na assinatura do DF-e: {dfeEx.Message}");
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
            else if (dfeEx is UnexpectedDocumentTypeException unexpectedTypeEx)
            {
                Console.WriteLine($"Tipo de documento inesperado. Esperado: {unexpectedTypeEx.ExpectedModel}, Encontrado: {unexpectedTypeEx.FoundModel}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ocorreu um erro inesperado: {ex.Message}");
        }
    }
}
```

## 📄 Licença
Este projeto está licenciado sob a Licença MIT. Veja o arquivo [LICENSE](https://github.com/tiago-saldanha/DFeSigner.NET/blob/master/LICENSE) para mais detalhes.

## Copyright
© 2025 Tiago Ávila Saldanha
