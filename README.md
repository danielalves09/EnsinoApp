# 📚 EnsinoApp

[![.NET](https://img.shields.io/badge/.NET-9.0-blueviolet)](https://dotnet.microsoft.com/)
[![ASP.NET Core MVC](https://img.shields.io/badge/ASP.NET_Core-MVC-blue)](https://learn.microsoft.com/aspnet/core/mvc)
[![SQL Server](https://img.shields.io/badge/SQL_Server-2019+-red)](https://www.microsoft.com/sql-server)
[![Docker](https://img.shields.io/badge/Docker-Ready-blue)](https://www.docker.com/)
[![License](https://img.shields.io/badge/License-MIT-green)](LICENSE)
[![Status](https://img.shields.io/badge/Status-Em_Desenvolvimento-yellow)]()

Sistema web de gestão acadêmica e administrativa dos cursos de casais da **CCVideira**, desenvolvido em **ASP.NET Core 9 MVC**. Permite gerenciar campus, turmas, matrículas, relatórios semanais de líderes e emissão de certificados com validação pública por QR Code.

---

## 📋 Sumário

- [Funcionalidades](#-funcionalidades)
- [Tecnologias](#-tecnologias)
- [Arquitetura](#-arquitetura)
- [Estrutura do Projeto](#-estrutura-do-projeto)
- [Pré-requisitos](#-pré-requisitos)
- [Configuração e Execução](#-configuração-e-execução)
- [Docker](#-docker)
- [Variáveis de Ambiente](#-variáveis-de-ambiente)
- [Banco de Dados](#-banco-de-dados)
- [Perfis de Acesso](#-perfis-de-acesso)
- [Fluxo Principal](#-fluxo-principal)
- [Certificados](#-certificados)
- [Autor](#-autor)

---

## ✨ Funcionalidades

### Área Pública (sem login)

- Landing page com apresentação dos cursos disponíveis
- Formulário de inscrição online para casais
- Validação pública de certificados via código ou QR Code

### Gestão Acadêmica (Admin / Pastor / Coordenador)

- **Campus** — cadastro e gerenciamento de unidades físicas
- **Supervisões** — agrupamento de líderes por campus
- **Cursos** — criação de cursos com lições associadas, dashboard por curso
- **Turmas** — abertura de turmas por curso/campus/líder, controle de status
- **Matrículas** — processamento de inscrições online e matrícula manual de casais
- **Casais** — cadastro completo com dados de contato e endereço
- **Líderes (Usuários)** — cadastro de usuários com roles e foto de perfil
- **Dashboard principal** — KPIs, gráficos de inscrições/matrículas/casais por campus

### Área do Líder

- Visão das turmas sob sua responsabilidade
- Lançamento de relatório semanal de presença por casal
- Conclusão de curso de matrícula individual
- Geração e download em lote de certificados PDF (com QR Code)
- Visualização de certificados emitidos

### Conta do Usuário

- Atualização de nome e e-mail
- Alteração de senha
- Upload e crop de foto de perfil

---

## 🧱 Tecnologias

| Camada             | Tecnologia                                            |
| ------------------ | ----------------------------------------------------- |
| Framework          | ASP.NET Core 9 MVC (.NET 9)                           |
| ORM                | Entity Framework Core 9                               |
| Autenticação       | ASP.NET Identity com roles (`IdentityRole<int>`)      |
| Banco de dados     | SQL Server 2019+                                      |
| Geração de PDF     | DinkToPdf (wkhtmltopdf)                               |
| Geração de QR Code | QRCoder                                               |
| Logs               | Serilog (console + arquivo diário rotacionado)        |
| Frontend           | AdminLTE 3 + Bootstrap 4 + jQuery                     |
| Notificações UI    | Toastr.js                                             |
| Gráficos           | Chart.js                                              |
| Crop de imagem     | Cropper.js                                            |
| Infraestrutura     | Docker + Docker Compose                               |
| Proteção de dados  | ASP.NET Data Protection (chaves persistidas em disco) |

---

## 🏛 Arquitetura

O projeto segue o padrão **Repository + Service Layer** com injeção de dependência via `AddScoped`.

```
Controller → Service → Repository → DbContext (EF Core)
```

- **Controller** — recebe a requisição, delega ao Service, devolve View ou JSON
- **Service** — contém a lógica de negócio; não acessa DbContext diretamente
- **Repository** — único ponto de acesso ao banco; usa `AsNoTracking()` em consultas de leitura
- **ViewModel** — objetos dedicados para cada View, sem expor entidades diretamente
- **NotificationService** — centraliza mensagens de feedback ao usuário via TempData (Toastr)
- **ExceptionMiddleware** — captura exceções não tratadas, loga via Serilog e retorna resposta padronizada

### Decisões de design relevantes

- `DbContext` é `Scoped` (uma instância por request). Não use `Task.WhenAll` com múltiplas queries no mesmo contexto — use `await` sequencial.
- Consultas de leitura sempre usam `.AsNoTracking()` para evitar overhead de change tracking.
- Métodos de contagem e consulta assíncrona usam `CountAsync()` / `FirstOrDefaultAsync()` para não bloquear threads do servidor.
- A geração de PDF com DinkToPdf usa `SynchronizedConverter` registrado como `Singleton` (thread-safe requerido pela biblioteca nativa).

---

## 📁 Estrutura do Projeto

```
EnsinoApp/
├── Config/                     # Configurações auxiliares (CustomAssemblyLoadContext)
├── Controllers/                # Controllers MVC
│   ├── AuthController.cs       # Login / Logout
│   ├── HomeController.cs       # Dashboard principal (Admin/Pastor/Coordenador)
│   ├── LiderController.cs      # Área do líder (turmas, relatórios, certificados)
│   ├── MatriculaController.cs  # Processamento de inscrições e matrícula
│   ├── CursoController.cs
│   ├── TurmaController.cs
│   ├── CampusController.cs
│   ├── SupervisaoController.cs
│   ├── LicaoController.cs
│   ├── InscricaoController.cs  # Pública — formulário de inscrição
│   ├── CertificadoController.cs# Pública — validação de certificado
│   ├── LandingController.cs    # Página inicial pública
│   └── UsuariosController.cs
│
├── Data/
│   ├── EnsinoAppContext.cs      # DbContext + IdentityDbContext
│   ├── Configurations/          # Fluent API por entidade (IEntityTypeConfiguration)
│   └── Migrations/
│
├── Models/
│   ├── Entities/                # Entidades do domínio
│   └── Enums/                   # StatusTurma, StatusMatricula, StatusPresenca, StatusCasal
│
├── ViewModels/                  # Um ViewModel por tela, nunca expõe entidades
├── Views/                       # Razor Views (.cshtml)
├── ViewComponents/              # UserMenuViewComponent
│
├── Repositories/                # Acesso a dados por entidade
│   ├── ICrudRepository.cs       # Interface genérica (FindAll, FindById, Create, Update, Delete)
│   ├── Campus/
│   ├── Casal/
│   ├── Cursos/
│   ├── Inscricao/
│   ├── Licao/
│   ├── Matricula/
│   ├── RelatorioSemanal/
│   ├── Supervisao/
│   ├── Turmas/
│   └── Usuarios/
│
├── Services/                    # Lógica de negócio por domínio
│   ├── Campus/
│   ├── Casal/
│   ├── Certificado/             # CertificadoService, PdfService, RazorViewToStringRenderer
│   ├── Cursos/
│   ├── Inscricao/
│   ├── Licao/
│   ├── Lider/
│   ├── Matricula/
│   ├── Notifications/           # INotificationService (Toastr via TempData)
│   ├── Supervisao/
│   ├── Turmas/
│   ├── Usuarios/
│   └── Util/                    # IUtilService (formatação de nomes)
│
├── Middlewares/
│   └── ExceptionMiddleware.cs   # Captura global de exceções + log
│
├── wwwroot/
│   ├── css/                     # Estilos (AdminLTE + ensino-app.css)
│   ├── images/
│   ├── uploads/perfis/          # Fotos de perfil dos usuários
│   └── Certificados/            # PDFs gerados
│
├── lib/                         # DLLs nativas do DinkToPdf (Windows/Linux/macOS)
├── Program.cs
├── appsettings.json
├── Dockerfile
└── docker-compose.yml
```

---

## ✅ Pré-requisitos

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [SQL Server 2019+](https://www.microsoft.com/sql-server) ou SQL Server via Docker
- [Docker](https://www.docker.com/) (opcional, para rodar tudo em container)

---

## ⚙️ Configuração e Execução

### 1. Clonar o repositório

```bash
git clone https://github.com/danielalves09/EnsinoApp.git
cd EnsinoApp
```

### 2. Configurar a connection string

Edite `appsettings.Development.json` (ou use variável de ambiente em produção):

```json
{
  "ConnectionStrings": {
    "EnsinoAppConnection": "Server=localhost;Database=EnsinoApp;User Id=sa;Password=SuaSenha;TrustServerCertificate=True;"
  },
  "AppSettings": {
    "CertificadoBaseUrl": "https://seudominio.com/certificado/validar/",
    "CertificadosFolder": "Certificados"
  }
}
```

### 3. Aplicar as migrations

```bash
dotnet ef database update
```

### 4. Executar

```bash
dotnet run
```

Acesse: `http://localhost:5108`

---

## 🐳 Docker

A aplicação está preparada para rodar em container. O Kestrel escuta na porta `80` e a connection string deve ser passada via variável de ambiente.

### Subir com Docker Compose

```bash
docker compose up -d
```

Acesse: `http://localhost:8080`

### Variáveis de ambiente para Docker

```yaml
environment:
  ASPNETCORE_ENVIRONMENT: Production
  ConnectionStrings__EnsinoAppConnection: "Server=db;Database=EnsinoApp;User Id=sa;Password=SuaSenha;TrustServerCertificate=True;"
  AppSettings__CertificadoBaseUrl: "https://seudominio.com/certificado/validar/"
  AppSettings__CertificadosFolder: "Certificados"
```

---

## 🔧 Variáveis de Ambiente

| Variável                                 | Descrição                              | Exemplo                                 |
| ---------------------------------------- | -------------------------------------- | --------------------------------------- |
| `ConnectionStrings__EnsinoAppConnection` | Connection string do SQL Server        | `Server=...`                            |
| `AppSettings__CertificadoBaseUrl`        | URL base para o QR Code do certificado | `https://site.com/certificado/validar/` |
| `AppSettings__CertificadosFolder`        | Pasta dentro de `wwwroot` para os PDFs | `Certificados`                          |
| `ASPNETCORE_ENVIRONMENT`                 | Ambiente de execução                   | `Development` / `Production`            |

---

## 🗄 Banco de Dados

### Entidades principais

| Entidade           | Descrição                                                                                                            |
| ------------------ | -------------------------------------------------------------------------------------------------------------------- |
| `Usuario`          | Extends `IdentityUser<int>`. Representa líderes e administradores. Possui campus, supervisão, foto de perfil e role. |
| `Campus`           | Unidade física da CCVideira com endereço completo.                                                                   |
| `Supervisao`       | Agrupamento de líderes dentro de um campus.                                                                          |
| `Curso`            | Curso acadêmico (ex: Casados Para Sempre, ONE, Pais Para Toda a Vida). Contém lições.                                |
| `Licao`            | Lição semanal de um curso (título, número, descrição).                                                               |
| `Turma`            | Instância de um curso com líder, datas e status.                                                                     |
| `Casal`            | Casal matriculado. Dados de contato, endereço e status.                                                              |
| `Matricula`        | Casal inscrito em uma turma. Contém status, data de conclusão, caminho do certificado e código de validação.         |
| `RelatorioSemanal` | Presença e observação de um casal em uma lição específica.                                                           |
| `InscricaoOnline`  | Inscrição pública feita pelo formulário. Processada pelo admin para gerar casal + matrícula.                         |

### Índices relevantes

- `Casal.EmailConjuge1` e `EmailConjuge2` — únicos
- `InscricaoOnline.EmailMarido` e `EmailEsposa` — únicos
- `Matricula(IdCasal, IdTurma)` — único (impede matrícula dupla)
- `Matricula.CodigoValidacao` — único com filtro `IS NOT NULL` (busca pública de certificado)

### Migrations

```bash
# Criar nova migration
dotnet ef migrations add NomeDaMigration

# Aplicar ao banco
dotnet ef database update

# Reverter última migration
dotnet ef database update PenultimaMigration
```

---

## 🔐 Perfis de Acesso

| Role          | Acesso                                                   |
| ------------- | -------------------------------------------------------- |
| `Admin`       | Acesso total ao sistema                                  |
| `Pastor`      | Mesmo acesso do Admin                                    |
| `Coordenador` | Gestão de cursos, turmas, matrículas, campus, usuários   |
| `Lider`       | Área restrita: turmas próprias, relatórios, certificados |
| _(sem login)_ | Landing page, inscrição online, validação de certificado |

### Configurações de senha (Identity)

- Mínimo 6 caracteres
- Requer dígito, letra maiúscula e minúscula
- Lockout de 5 minutos após tentativas inválidas

---

## 🔄 Fluxo Principal

```
1. Casal acessa a landing page
2. Preenche o formulário de Inscrição Online
3. Admin processa a inscrição: cria Casal + Matrícula em uma Turma
4. Líder lança Relatório Semanal de presença a cada lição
5. Quando o casal conclui todas as lições, o Líder marca a matrícula como Concluída
6. Líder gera os Certificados em lote (PDF + QR Code)
7. Casal pode validar o certificado publicamente via URL ou formulário
```

---

## 📄 Certificados

Os certificados são gerados em PDF via **DinkToPdf** (wkhtmltopdf). O fluxo é:

1. Uma Razor View (`CertificadoTemplate.cshtml`) é renderizada para HTML em memória via `RazorViewToStringRenderer`
2. O HTML é convertido para PDF pelo `SynchronizedConverter` (DinkToPdf)
3. O PDF é salvo em `wwwroot/Certificados/` e disponibilizado para download em lote via ZIP
4. Um código de validação único (10 caracteres alfanuméricos) é gerado e salvo na matrícula
5. Um QR Code aponta para `{CertificadoBaseUrl}{CodigoValidacao}`
6. Qualquer pessoa pode validar o certificado em `/certificado/validar/{codigo}`

> **Atenção:** A coluna `CodigoValidacao` deve ter índice único no banco para que a busca pública seja eficiente. Veja a migration correspondente.

---

## 🧾 Logs

Logs estruturados com **Serilog**. Em produção, apenas erros são registrados.

```
Logs/
└── erros-YYYYMMDD.log   # Rotação diária, retém 30 arquivos
```

Para alterar o nível mínimo, edite `Program.cs`:

```csharp
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Error()   // Altere para .Information() para logs mais detalhados
    ...
```

---

## 🖼 Upload de Imagens

Fotos de perfil são salvas em `wwwroot/uploads/perfis/` com o nome `usuario_{id}.{ext}`. O crop é feito no front-end via **Cropper.js** antes do upload.

---

## 👨‍💻 Autor

**Daniel Alves Moreira**  
Desenvolvedor .NET  
Projeto desenvolvido para a **CCVideira**

---

## 📌 Status

🚧 Em desenvolvimento contínuo

### Melhorias planejadas

- [ ] Paginação server-side em todos os listagens (atualmente feita em memória)
- [ ] Converter métodos de contagem síncronos para `async` (`ContarTotal`, `ContarAtivas`)
- [ ] Remover acesso direto ao `DbContext` no `InscricaoOnlineService`
- [ ] Corrigir N+1 queries em `CasalService.ObterResumoCasais`
- [ ] Adicionar índice em `Matricula.CodigoValidacao`
- [ ] Testes unitários nas camadas de Service e Repository
