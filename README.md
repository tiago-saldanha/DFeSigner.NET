# DFeSigner.NET

![.NET](https://img.shields.io/badge/.NET-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)
![xUnit](https://img.shields.io/badge/xUnit-802B7D?style=for-the-badge&logo=xunit&logoColor=white)
![GitHub](https://img.shields.io/badge/GitHub-100000?style=for-the-badge&logo=github&logoColor=white)
![License](https://img.shields.io/badge/License-MIT-green.svg)

## 📖 Sobre o Projeto

`DFeSigner.NET` é uma biblioteca em C# (.NET) projetada para simplificar o processo de assinatura digital de Documentos Fiscais Eletrônicos (DF-e) no padrão brasileiro. Utilizando certificados digitais X.509 (A1 ou A3), a biblioteca oferece uma solução flexível e robusta para integrar a funcionalidade de assinatura em suas aplicações, com foco inicial em NF-e (Nota Fiscal Eletrônica) e NFC-e (Nota Fiscal de Consumidor Eletrônica).

O projeto adota uma arquitetura modular com uma classe base abstrata, permitindo a fácil extensão para outros tipos de DF-e no futuro, como CT-e (Conhecimento de Transporte Eletrônico) e MDF-e (Manifesto Eletrônico de Documentos Fiscais). Acompanha um projeto de testes unitários abrangente para garantir a confiabilidade e a validação das funcionalidades de assinatura.

## ✨ Funcionalidades

* **Assinatura de NF-e:** Assina digitalmente documentos XML de Nota Fiscal Eletrônica (modelo 55).
* **Assinatura de NFC-e:** Assina digitalmente documentos XML de Nota Fiscal de Consumidor Eletrônica (modelo 65).
* **Classe Abstrata `DFeXmlSigner`:** Fornece uma base comum para a lógica de assinatura XML, promovendo reuso de código e facilitando a adição de novos tipos de DF-e.
* **Validação de Tipo de Documento:** As implementações concretas (NF-e, NFC-e) validam o modelo do documento no XML para garantir que o tipo correto de assinador está sendo usado.
* **Tratamento de Erros Robusto:** Lançamento de exceções claras para cenários como XML inválido, certificado nulo ou certificado sem chave privada.
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
    cd DFeSigner.NET
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

public class ExemploAssinatura
{
    public static void Main(string[] args)
    {
        try
        {
            // 1. Carregar o XML da NF-e (substitua pelo seu XML real)
            string xmlNFeContent = File.ReadAllText("caminho/para/sua/nfe.xml");

            // 2. Carregar o Certificado Digital (exemplo de certificado PFX com senha)
            // Certifique-se de que o certificado.pfx está acessível e a senha está correta.
            // Para certificados A3 (instalados no Windows), você pode usar o GetCertificateBySubjectName().
            string certificatePath = "caminho/para/seu/certificado.pfx";
            string certificatePassword = "sua_senha_do_certificado"; 
            
            X509Certificate2 certificate = new X509Certificate2(
                certificatePath, 
                certificatePassword, 
                X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet
            );

            // 3. Instanciar o assinador específico (NFeXmlSigner para NF-e)
            NFeXmlSigner nfeSigner = new NFeXmlSigner();

            // 4. Assinar o XML
            string signedNFeXml = nfeSigner.Sign(xmlNFeContent, certificate);

            Console.WriteLine("NF-e assinada com sucesso!");
            // Opcional: Salvar o XML assinado
            File.WriteAllText("nfe_assinada.xml", signedNFeXml);

            // --- Exemplo com NFC-e ---
            string xmlNFCeContent = File.ReadAllText("caminho/para/sua/nfce.xml");
            NFCeXmlSigner nfceSigner = new NFCeXmlSigner();
            string signedNFCeXml = nfceSigner.Sign(xmlNFCeContent, certificate);
            Console.WriteLine("NFC-e assinada com sucesso!");
            File.WriteAllText("nfce_assinada.xml", signedNFCeXml);

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ocorreu um erro: {ex.Message}");
            Console.WriteLine(ex.InnerException?.Message); // Detalhes da exceção interna
        }
    }
}
