IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260327133949_CriarTabelas'
)
BEGIN
    CREATE TABLE [AspNetRoles] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(256) NULL,
        [NormalizedName] nvarchar(256) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260327133949_CriarTabelas'
)
BEGIN
    CREATE TABLE [Campuses] (
        [Id] int NOT NULL IDENTITY,
        [Nome] nvarchar(max) NOT NULL,
        [Telefone] nvarchar(max) NOT NULL,
        [Rua] nvarchar(max) NOT NULL,
        [Numero] nvarchar(max) NOT NULL,
        [Complemento] nvarchar(max) NULL,
        [Bairro] nvarchar(max) NOT NULL,
        [Cidade] nvarchar(max) NOT NULL,
        [Estado] nvarchar(max) NOT NULL,
        [Cep] nvarchar(max) NULL,
        CONSTRAINT [PK_Campuses] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260327133949_CriarTabelas'
)
BEGIN
    CREATE TABLE [AspNetRoleClaims] (
        [Id] int NOT NULL IDENTITY,
        [RoleId] int NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260327133949_CriarTabelas'
)
BEGIN
    CREATE TABLE [Casais] (
        [Id] int NOT NULL IDENTITY,
        [IdCampus] int NOT NULL,
        [NomeConjuge1] nvarchar(max) NOT NULL,
        [NomeConjuge2] nvarchar(max) NOT NULL,
        [TelefoneConjuge1] nvarchar(max) NOT NULL,
        [TelefoneConjuge2] nvarchar(max) NOT NULL,
        [EmailConjuge1] nvarchar(450) NOT NULL,
        [EmailConjuge2] nvarchar(450) NOT NULL,
        [Status] int NOT NULL DEFAULT 0,
        [Rua] nvarchar(max) NOT NULL,
        [Numero] nvarchar(max) NOT NULL,
        [Complemento] nvarchar(max) NULL,
        [Bairro] nvarchar(max) NOT NULL,
        [Cidade] nvarchar(max) NOT NULL,
        [Estado] nvarchar(max) NOT NULL,
        [Cep] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_Casais] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Casais_Campuses_IdCampus] FOREIGN KEY ([IdCampus]) REFERENCES [Campuses] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260327133949_CriarTabelas'
)
BEGIN
    CREATE TABLE [Cursos] (
        [Id] int NOT NULL IDENTITY,
        [IdCampus] int NOT NULL,
        [Nome] nvarchar(max) NOT NULL,
        [Descricao] nvarchar(max) NOT NULL,
        [Ativo] bit NOT NULL DEFAULT CAST(1 AS bit),
        CONSTRAINT [PK_Cursos] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Cursos_Campuses_IdCampus] FOREIGN KEY ([IdCampus]) REFERENCES [Campuses] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260327133949_CriarTabelas'
)
BEGIN
    CREATE TABLE [Supervisoes] (
        [Id] int NOT NULL IDENTITY,
        [Nome] nvarchar(max) NOT NULL,
        [IdCampus] int NOT NULL,
        CONSTRAINT [PK_Supervisoes] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Supervisoes_Campuses_IdCampus] FOREIGN KEY ([IdCampus]) REFERENCES [Campuses] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260327133949_CriarTabelas'
)
BEGIN
    CREATE TABLE [InscricoesOnline] (
        [Id] int NOT NULL IDENTITY,
        [NomeMarido] nvarchar(max) NOT NULL,
        [NomeEsposa] nvarchar(max) NOT NULL,
        [TelefoneMarido] nvarchar(20) NOT NULL,
        [TelefoneEsposa] nvarchar(20) NOT NULL,
        [EmailMarido] nvarchar(450) NOT NULL,
        [EmailEsposa] nvarchar(450) NOT NULL,
        [Rua] nvarchar(max) NULL,
        [Numero] nvarchar(max) NULL,
        [Complemento] nvarchar(max) NULL,
        [Bairro] nvarchar(max) NULL,
        [Cidade] nvarchar(max) NULL,
        [Estado] nvarchar(max) NULL,
        [Cep] nvarchar(max) NULL,
        [IdCampus] int NOT NULL,
        [IdCurso] int NOT NULL,
        [ParticipaGC] bit NOT NULL,
        [NomeGC] nvarchar(100) NULL,
        [DataInscricao] datetime2 NOT NULL,
        [Processada] bit NOT NULL,
        CONSTRAINT [PK_InscricoesOnline] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_InscricoesOnline_Campuses_IdCampus] FOREIGN KEY ([IdCampus]) REFERENCES [Campuses] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_InscricoesOnline_Cursos_IdCurso] FOREIGN KEY ([IdCurso]) REFERENCES [Cursos] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260327133949_CriarTabelas'
)
BEGIN
    CREATE TABLE [Licoes] (
        [Id] int NOT NULL IDENTITY,
        [IdCurso] int NOT NULL,
        [NumeroSemana] int NOT NULL,
        [Titulo] nvarchar(max) NOT NULL,
        [Descricao] nvarchar(max) NOT NULL,
        [Ativa] bit NOT NULL DEFAULT CAST(1 AS bit),
        CONSTRAINT [PK_Licoes] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Licoes_Cursos_IdCurso] FOREIGN KEY ([IdCurso]) REFERENCES [Cursos] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260327133949_CriarTabelas'
)
BEGIN
    CREATE TABLE [AspNetUsers] (
        [Id] int NOT NULL IDENTITY,
        [IdCampus] int NOT NULL,
        [NomeMarido] nvarchar(max) NOT NULL,
        [NomeEsposa] nvarchar(max) NOT NULL,
        [IdSupervisao] int NULL,
        [Ativo] bit NOT NULL DEFAULT CAST(1 AS bit),
        [DataCriacao] datetime2 NOT NULL,
        [FotoPerfil] nvarchar(max) NULL,
        [UserName] nvarchar(256) NULL,
        [NormalizedUserName] nvarchar(256) NULL,
        [Email] nvarchar(256) NULL,
        [NormalizedEmail] nvarchar(256) NULL,
        [EmailConfirmed] bit NOT NULL,
        [PasswordHash] nvarchar(max) NULL,
        [SecurityStamp] nvarchar(max) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        [PhoneNumber] nvarchar(max) NULL,
        [PhoneNumberConfirmed] bit NOT NULL,
        [TwoFactorEnabled] bit NOT NULL,
        [LockoutEnd] datetimeoffset NULL,
        [LockoutEnabled] bit NOT NULL,
        [AccessFailedCount] int NOT NULL,
        CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetUsers_Campuses_IdCampus] FOREIGN KEY ([IdCampus]) REFERENCES [Campuses] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_AspNetUsers_Supervisoes_IdSupervisao] FOREIGN KEY ([IdSupervisao]) REFERENCES [Supervisoes] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260327133949_CriarTabelas'
)
BEGIN
    CREATE TABLE [AspNetUserClaims] (
        [Id] int NOT NULL IDENTITY,
        [UserId] int NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260327133949_CriarTabelas'
)
BEGIN
    CREATE TABLE [AspNetUserLogins] (
        [LoginProvider] nvarchar(450) NOT NULL,
        [ProviderKey] nvarchar(450) NOT NULL,
        [ProviderDisplayName] nvarchar(max) NULL,
        [UserId] int NOT NULL,
        CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
        CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260327133949_CriarTabelas'
)
BEGIN
    CREATE TABLE [AspNetUserRoles] (
        [UserId] int NOT NULL,
        [RoleId] int NOT NULL,
        CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
        CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260327133949_CriarTabelas'
)
BEGIN
    CREATE TABLE [AspNetUserTokens] (
        [UserId] int NOT NULL,
        [LoginProvider] nvarchar(450) NOT NULL,
        [Name] nvarchar(450) NOT NULL,
        [Value] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
        CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260327133949_CriarTabelas'
)
BEGIN
    CREATE TABLE [Turmas] (
        [Id] int NOT NULL IDENTITY,
        [IdCurso] int NOT NULL,
        [IdCampus] int NOT NULL,
        [IdLider] int NOT NULL,
        [DataInicio] datetime2 NOT NULL,
        [DataFim] datetime2 NOT NULL,
        [Status] int NOT NULL DEFAULT 0,
        [DiaSemana] int NOT NULL,
        CONSTRAINT [PK_Turmas] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Turmas_AspNetUsers_IdLider] FOREIGN KEY ([IdLider]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Turmas_Campuses_IdCampus] FOREIGN KEY ([IdCampus]) REFERENCES [Campuses] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Turmas_Cursos_IdCurso] FOREIGN KEY ([IdCurso]) REFERENCES [Cursos] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260327133949_CriarTabelas'
)
BEGIN
    CREATE TABLE [AgendaLicoes] (
        [Id] int NOT NULL IDENTITY,
        [IdTurma] int NOT NULL,
        [IdLicao] int NOT NULL,
        [DataAula] datetime2 NOT NULL,
        [DiaSemana] int NOT NULL,
        [Local] nvarchar(300) NULL,
        [Observacoes] nvarchar(1000) NULL,
        [LembreteEnviado] bit NOT NULL DEFAULT CAST(0 AS bit),
        CONSTRAINT [PK_AgendaLicoes] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AgendaLicoes_Licoes_IdLicao] FOREIGN KEY ([IdLicao]) REFERENCES [Licoes] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_AgendaLicoes_Turmas_IdTurma] FOREIGN KEY ([IdTurma]) REFERENCES [Turmas] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260327133949_CriarTabelas'
)
BEGIN
    CREATE TABLE [Matriculas] (
        [Id] int NOT NULL IDENTITY,
        [IdCasal] int NOT NULL,
        [IdTurma] int NOT NULL,
        [DataMatricula] datetime2 NOT NULL,
        [DataConclusao] datetime2 NULL,
        [CertificadoEmitido] bit NOT NULL,
        [CaminhoCertificado] nvarchar(max) NULL,
        [CodigoValidacao] nvarchar(max) NULL,
        [NomeGC] nvarchar(100) NULL,
        [Status] int NOT NULL,
        CONSTRAINT [PK_Matriculas] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Matriculas_Casais_IdCasal] FOREIGN KEY ([IdCasal]) REFERENCES [Casais] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_Matriculas_Turmas_IdTurma] FOREIGN KEY ([IdTurma]) REFERENCES [Turmas] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260327133949_CriarTabelas'
)
BEGIN
    CREATE TABLE [RelatoriosSemanais] (
        [Id] int NOT NULL IDENTITY,
        [IdMatricula] int NOT NULL,
        [IdLicao] int NOT NULL,
        [Observacoes] nvarchar(max) NOT NULL,
        [Presenca] int NOT NULL,
        [IdUsuario] int NOT NULL,
        [DataRegistro] datetime2 NOT NULL,
        [DataLicao] datetime2 NOT NULL,
        CONSTRAINT [PK_RelatoriosSemanais] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_RelatoriosSemanais_AspNetUsers_IdUsuario] FOREIGN KEY ([IdUsuario]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_RelatoriosSemanais_Licoes_IdLicao] FOREIGN KEY ([IdLicao]) REFERENCES [Licoes] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_RelatoriosSemanais_Matriculas_IdMatricula] FOREIGN KEY ([IdMatricula]) REFERENCES [Matriculas] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260327133949_CriarTabelas'
)
BEGIN
    CREATE INDEX [IX_AgendaLicoes_DataAula_LembreteEnviado] ON [AgendaLicoes] ([DataAula], [LembreteEnviado]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260327133949_CriarTabelas'
)
BEGIN
    CREATE INDEX [IX_AgendaLicoes_IdLicao] ON [AgendaLicoes] ([IdLicao]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260327133949_CriarTabelas'
)
BEGIN
    CREATE UNIQUE INDEX [IX_AgendaLicoes_IdTurma_IdLicao] ON [AgendaLicoes] ([IdTurma], [IdLicao]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260327133949_CriarTabelas'
)
BEGIN
    CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260327133949_CriarTabelas'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260327133949_CriarTabelas'
)
BEGIN
    CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260327133949_CriarTabelas'
)
BEGIN
    CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260327133949_CriarTabelas'
)
BEGIN
    CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260327133949_CriarTabelas'
)
BEGIN
    CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260327133949_CriarTabelas'
)
BEGIN
    CREATE INDEX [IX_AspNetUsers_IdCampus] ON [AspNetUsers] ([IdCampus]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260327133949_CriarTabelas'
)
BEGIN
    CREATE INDEX [IX_AspNetUsers_IdSupervisao] ON [AspNetUsers] ([IdSupervisao]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260327133949_CriarTabelas'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260327133949_CriarTabelas'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Casais_EmailConjuge1] ON [Casais] ([EmailConjuge1]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260327133949_CriarTabelas'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Casais_EmailConjuge2] ON [Casais] ([EmailConjuge2]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260327133949_CriarTabelas'
)
BEGIN
    CREATE INDEX [IX_Casais_IdCampus] ON [Casais] ([IdCampus]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260327133949_CriarTabelas'
)
BEGIN
    CREATE INDEX [IX_Cursos_IdCampus] ON [Cursos] ([IdCampus]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260327133949_CriarTabelas'
)
BEGIN
    CREATE UNIQUE INDEX [IX_InscricoesOnline_EmailEsposa] ON [InscricoesOnline] ([EmailEsposa]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260327133949_CriarTabelas'
)
BEGIN
    CREATE UNIQUE INDEX [IX_InscricoesOnline_EmailMarido] ON [InscricoesOnline] ([EmailMarido]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260327133949_CriarTabelas'
)
BEGIN
    CREATE INDEX [IX_InscricoesOnline_IdCampus] ON [InscricoesOnline] ([IdCampus]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260327133949_CriarTabelas'
)
BEGIN
    CREATE INDEX [IX_InscricoesOnline_IdCurso] ON [InscricoesOnline] ([IdCurso]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260327133949_CriarTabelas'
)
BEGIN
    CREATE INDEX [IX_Licoes_IdCurso] ON [Licoes] ([IdCurso]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260327133949_CriarTabelas'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Matriculas_IdCasal_IdTurma] ON [Matriculas] ([IdCasal], [IdTurma]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260327133949_CriarTabelas'
)
BEGIN
    CREATE INDEX [IX_Matriculas_IdTurma] ON [Matriculas] ([IdTurma]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260327133949_CriarTabelas'
)
BEGIN
    CREATE INDEX [IX_RelatoriosSemanais_IdLicao] ON [RelatoriosSemanais] ([IdLicao]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260327133949_CriarTabelas'
)
BEGIN
    CREATE INDEX [IX_RelatoriosSemanais_IdMatricula] ON [RelatoriosSemanais] ([IdMatricula]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260327133949_CriarTabelas'
)
BEGIN
    CREATE INDEX [IX_RelatoriosSemanais_IdUsuario] ON [RelatoriosSemanais] ([IdUsuario]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260327133949_CriarTabelas'
)
BEGIN
    CREATE INDEX [IX_Supervisoes_IdCampus] ON [Supervisoes] ([IdCampus]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260327133949_CriarTabelas'
)
BEGIN
    CREATE INDEX [IX_Turmas_IdCampus] ON [Turmas] ([IdCampus]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260327133949_CriarTabelas'
)
BEGIN
    CREATE INDEX [IX_Turmas_IdCurso] ON [Turmas] ([IdCurso]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260327133949_CriarTabelas'
)
BEGIN
    CREATE INDEX [IX_Turmas_IdLider] ON [Turmas] ([IdLider]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260327133949_CriarTabelas'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260327133949_CriarTabelas', N'9.0.0');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260408224500_PeriodoInscricao'
)
BEGIN
    CREATE TABLE [PeriodosInscricao] (
        [Id] int NOT NULL IDENTITY,
        [IdCurso] int NOT NULL,
        [IdCampus] int NOT NULL,
        [DataAbertura] datetime2 NOT NULL,
        [DataEncerramento] datetime2 NOT NULL,
        [VagasTotal] int NOT NULL,
        [VagasOcupadas] int NOT NULL DEFAULT 0,
        [Ativo] bit NOT NULL DEFAULT CAST(0 AS bit),
        CONSTRAINT [PK_PeriodosInscricao] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_PeriodosInscricao_Campuses_IdCampus] FOREIGN KEY ([IdCampus]) REFERENCES [Campuses] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_PeriodosInscricao_Cursos_IdCurso] FOREIGN KEY ([IdCurso]) REFERENCES [Cursos] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260408224500_PeriodoInscricao'
)
BEGIN
    CREATE INDEX [IX_PeriodosInscricao_IdCampus] ON [PeriodosInscricao] ([IdCampus]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260408224500_PeriodoInscricao'
)
BEGIN
    CREATE INDEX [IX_PeriodosInscricao_IdCurso] ON [PeriodosInscricao] ([IdCurso]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260408224500_PeriodoInscricao'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260408224500_PeriodoInscricao', N'9.0.0');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260514025716_LayoutCertificado'
)
BEGIN
    ALTER TABLE [Cursos] ADD [IdLayoutCertificado] int NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260514025716_LayoutCertificado'
)
BEGIN
    CREATE TABLE [LayoutsCertificado] (
        [Id] int NOT NULL IDENTITY,
        [Nome] nvarchar(200) NOT NULL,
        [Descricao] nvarchar(500) NULL,
        [TemplateHtml] nvarchar(max) NOT NULL,
        [ImagemFundoUrl] nvarchar(500) NULL,
        [Orientacao] nvarchar(20) NOT NULL DEFAULT N'Landscape',
        [Ativo] bit NOT NULL DEFAULT CAST(1 AS bit),
        [DataCriacao] datetime2 NOT NULL,
        CONSTRAINT [PK_LayoutsCertificado] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260514025716_LayoutCertificado'
)
BEGIN
    CREATE INDEX [IX_Cursos_IdLayoutCertificado] ON [Cursos] ([IdLayoutCertificado]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260514025716_LayoutCertificado'
)
BEGIN
    ALTER TABLE [Cursos] ADD CONSTRAINT [FK_Cursos_LayoutsCertificado_IdLayoutCertificado] FOREIGN KEY ([IdLayoutCertificado]) REFERENCES [LayoutsCertificado] ([Id]) ON DELETE SET NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260514025716_LayoutCertificado'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260514025716_LayoutCertificado', N'9.0.0');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260516190813_CursoLayout'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260516190813_CursoLayout', N'9.0.0');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260523153122_CamposAdicionaisInscricao'
)
BEGIN
    ALTER TABLE [InscricoesOnline] ADD [Convidado] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260523153122_CamposAdicionaisInscricao'
)
BEGIN
    ALTER TABLE [InscricoesOnline] ADD [DataNascimentoEsposa] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260523153122_CamposAdicionaisInscricao'
)
BEGIN
    ALTER TABLE [InscricoesOnline] ADD [DataNascimentoMarido] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260523153122_CamposAdicionaisInscricao'
)
BEGIN
    ALTER TABLE [InscricoesOnline] ADD [NomeCasalConvidador] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260523153122_CamposAdicionaisInscricao'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260523153122_CamposAdicionaisInscricao', N'9.0.0');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260608010503_diaSemana'
)
BEGIN
    ALTER TABLE [PeriodosInscricao] ADD [DiasDisponiveis] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260608010503_diaSemana'
)
BEGIN
    ALTER TABLE [InscricoesOnline] ADD [DiaPreferencial] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260608010503_diaSemana'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260608010503_diaSemana', N'9.0.0');
END;

COMMIT;
GO

