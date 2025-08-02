# DFeSigner.NET

![.NET](https://img.shields.io/badge/.NET-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)
![xUnit](https://img.shields.io/badge/xUnit-802B7D?style=for-the-badge&logo=xunit&logoColor=white)
![GitHub](https://img.shields.io/badge/GitHub-100000?style=for-the-badge&logo=github&logoColor=white)
![License](https://img.shields.io/badge/License-MIT-green.svg)

---

## 📖 Sobre o Projeto

`DFeSigner.NET` é uma biblioteca em C# (.NET) projetada para simplificar o processo de assinatura digital de Documentos Fiscais Eletrônicos (DF-e) no padrão brasileiro. Utilizando certificados digitais X.509 (A1 ou A3), a biblioteca oferece uma solução flexível e robusta para integrar a funcionalidade de assinatura em suas aplicações, com foco inicial em NF-e (Nota Fiscal Eletrônica), NFC-e (Nota Fiscal de Consumidor Eletrônica), CT-e (Conhecimento de Transporte Eletrônico) e MDF-e (Manifesto Eletrônico de Documentos Fiscais).

O projeto adota uma arquitetura modular permitindo a fácil extensão para outros tipos de DF-e no futuro. Acompanha um projeto de testes unitários abrangente para garantir a confiabilidade e a validação das funcionalidades de assinatura.

---

## ✨ Funcionalidades Principais

* **Assinatura de DF-e:** Suporte robusto para assinatura digital de diversos tipos de Documentos Fiscais Eletrônicos.
* **Validação de Assinatura Digital:** Verificação da integridade e autenticidade de documentos XML assinados.
* **Validação de Tipo de Documento:** Garante que o assinador correto está sendo utilizado para cada modelo de DF-e (NF-e, NFC-e, CT-e, MDF-e).
* **Tratamento de Erros Robusto:** Exceções personalizadas para cenários comuns como XML inválido, certificado ausente ou incorreto, e estrutura XML inesperada.
* **Testes Unitários Abrangentes:** Projeto xUnit dedicado para garantir a confiabilidade e exemplificar o uso.
* **Suporte a Certificados X.509:** Compatibilidade com certificados PFX (A1) e certificados instalados no repositório do Windows (A3).

---

## 🚀 Tecnologias Utilizadas

* **C# (.NET):** Linguagem de programação e plataforma principal.
* **XML (System.Xml):** Para manipulação e leitura de documentos XML.
* **Criptografia XML (System.Security.Cryptography.Xml):** Componente do .NET para operações de assinatura XML digital.
* **Certificados X.509 (System.Security.Cryptography.X509Certificates):** Para gerenciamento e uso de certificados digitais.
* **xUnit:** Framework de testes unitários para .NET.

---

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

---

## 💡 Como Usar: Assinatura e Validação de DF-e

A biblioteca é projetada para ser fácil de usar. Você precisa de um conteúdo XML (string) e um certificado digital X.509. Primeiramente veja como configurar o seu **[Certificado Digital](/docs/certificado)**.

Com seu certificado carregado, você pode prosseguir para assinar e validar os diversos tipos de Documentos Fiscais Eletrônicos. Clique no tipo de documento desejado para ver um exemplo de código completo e detalhes específicos:

**[NF-e (Nota Fiscal Eletrônica)](/docs/nfe)**

**[NFC-e (Nota Fiscal do Consumidor Eletrônica)](/docs/nfce)**

**[CT-e (Conhecimento de Transporte Eletrônico)](/docs/cte)**

**[MDF-e (Manifesto de Documentos Fiscais Eletrônica)](/docs/mdfe)**

Você também pode ver a nossa seção sobre **[tratamento de erros](/docs/tratamento-de-erros)**.