# 📚 EnsinoApp

![.NET](https://img.shields.io/badge/.NET-8%2B-blueviolet)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET-Core-blue)
![Docker](https://img.shields.io/badge/Docker-Ready-blue)
![SQL Server](https://img.shields.io/badge/SQL%20Server-2019+-red)
![License](https://img.shields.io/badge/License-MIT-green)
![Status](https://img.shields.io/badge/Status-Em%20Desenvolvimento-yellow)

## 📌 Descrição do Repositório

**EnsinoApp** é um sistema web desenvolvido em **ASP.NET Core MVC** para gerenciamento acadêmico e administrativo de cursos, turmas, matrículas, líderes e emissão/validação de certificados, com autenticação baseada em **ASP.NET Identity**, layout administrativo com **AdminLTE** e infraestrutura preparada para **Docker**.

---

## 🚀 Funcionalidades

### 👤 Autenticação e Usuários
- Login e logout
- Controle de acesso por **roles**
- Área do usuário:
  - Atualização de dados pessoais
  - Alteração de senha
  - Upload e crop de foto de perfil
- Nome e foto do usuário exibidos no navbar em todas as páginas

### 🎓 Gestão Acadêmica
- Cursos
- Turmas
- Campus
- Supervisões
- Líderes
- Casais
- Matrículas
- Inscrições online

### 📄 Certificados
- Geração de certificados em **PDF**
- QR Code para validação
- Download em lote (ZIP)
- Validação pública via formulário ou URL

### 📊 Relatórios
- Relatórios semanais
- Controle de certificados emitidos

---

## 🖥️ Interface
- **AdminLTE**
- Layout responsivo
- Sidebar adaptável para mobile
- Notificações com **Toastr**

---

## 🧱 Tecnologias Utilizadas
- ASP.NET Core MVC
- Entity Framework Core
- ASP.NET Identity
- SQL Server
- Docker & Docker Compose
- DinkToPdf (wkhtmltopdf)
- Serilog
- jQuery / Bootstrap
- Chart.js

---

## 🐳 Docker

### Estrutura do Projeto

```
/raiz
 ├── EnsinoApp.sln
 ├── EnsinoApp.csproj
 ├── Dockerfile
 ├── docker-compose.yml
 ├── lib/                # DLLs nativas do DinkToPdf
 ├── wwwroot/
 ├── Controllers/
 ├── Services/
 ├── Repositories/
 ├── Data/
 └── Views/
```

### Subir a aplicação

```bash
docker compose up -d
```

Acesse:

```
http://localhost:8080
```

---

## ⚙️ Configuração de Ambiente

Ambientes suportados:
- `Development`
- `Production`

Variável:

```bash
ASPNETCORE_ENVIRONMENT=Development
```

---

## 🧾 Logs
- Logs estruturados com **Serilog**
- Arquivos diários em `/Logs`

---

## 🖼️ Upload de Imagens
- Upload com extensão automática
- Crop no front-end
- Armazenamento em `wwwroot/uploads/usuarios`

---

## 🔐 Segurança
- ASP.NET Identity
- Controle de acesso por roles
- Proteção CSRF
- Validação pública de certificados

---

## 👨‍💻 Autor

**Daniel Alves**  
Desenvolvedor .NET  
Projeto desenvolvido para a **CCVideira**

---

## 📌 Status do Projeto
🚧 Em evolução contínua
