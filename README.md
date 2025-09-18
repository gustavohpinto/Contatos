# Contatos

# Projeto Contatos (.NET 8 + Clean Architecture)

Este é um projeto em .NET 8 utilizando Clean Architecture, Entity Framework Core, xUnit e Swagger. O objetivo é gerenciar contatos com operações de CRUD, ativação e desativação, aplicando boas práticas de separação em camadas.

Tecnologias utilizadas: .NET 8, ASP.NET Core Web API, Entity Framework Core (SQL Server LocalDB), xUnit, Swagger (OpenAPI), Injeção de Dependência (IoC).

Estrutura da solução: Contatos.Api (camada de apresentação com Controllers, Program.cs e Swagger), Contatos.Application (regras de negócio, DTOs, Services), Contatos.Domain (entidades, interfaces, exceções, abstrações), Contatos.Infrastructure (persistência com EF Core, Repositórios, IoC, Time/Clock), Contatos.Tests (testes automatizados com xUnit).

Pré-requisitos: SDK .NET 8 instalado, Visual Studio 2022 (ou VS Code), SQL Server (pode usar LocalDB que já vem com o Visual Studio).

No arquivo Contatos.Api/appsettings.json existe a configuração de banco de dados. Por padrão está assim:

{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=ContactsDb;Trusted_Connection=True;TrustServerCertificate=True"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}

Se desejar usar outro SQL Server, altere a string de conexão. 

Para criar o banco rode os seguintes comandos no Package Manager Console:

Add-Migration Initial -StartupProject Contatos.Api -Project Contatos.Infrastructure
Update-Database -StartupProject Contatos.Api

Isso cria o banco ContactsDb e a tabela Contacts. Para executar a aplicação use:

dotnet run --project Contatos.Api

Ou rode pelo Visual Studio o projeto Contatos.Api. A API ficará disponível em https://localhost:44375/swagger.

Principais endpoints: POST /api/Contact (criar contato), GET /api/Contact/{id} (buscar contato ativo por ID), PUT /api/Contact/{id} (atualizar contato), DELETE /api/Contact/{id} (desativar contato), PUT /api/Contact/{id}/activate (reativar contato). Apenas contatos ativos aparecem em listagens. O campo Sex aceita M ou Male e F ou Female.

Exemplo de criação de contato:

POST /api/Contact
{
  "name": "John Doe",
  "birthDate": "1995-05-10",
  "sex": "M"
}

Resposta:
{
  "id": "b81d6b77-97a4-4e4d-bf82-4ad52cfcf2d4",
  "name": "John Doe",
  "birthDate": "1995-05-10",
  "sex": "Male",
  "age": 29
}

Os testes ficam no projeto Contatos.Tests usando xUnit. Para rodar use:

dotnet test

Isso executa todos os testes unitários configurados.

Projeto desenvolvido como estudo de Clean Architecture com .NET 8, aplicando boas práticas de separação em camadas, repositórios e testes automatizados.
