CREATE TABLE [dbo].[ONAConexion] (
    [IdONA]          INT            NOT NULL,
    [Host]           NVARCHAR (100) NOT NULL,
    [Puerto]         INT            DEFAULT ((0)) NOT NULL,
    [Usuario]        NVARCHAR (100) NOT NULL,
    [Contrasenia]    NVARCHAR (100) NOT NULL,
    [BaseDatos]      NVARCHAR (100) NOT NULL,
    [OrigenDatos]    NVARCHAR (100) NOT NULL,
    [Migrar]         NVARCHAR (1)   DEFAULT ('S') NOT NULL,
    [Estado]         NVARCHAR (1)   DEFAULT ('A') NOT NULL,
    [FechaCreacion]  DATETIME       DEFAULT (getdate()) NOT NULL,
    [FechaModifica]  DATETIME       DEFAULT (getdate()) NULL,
    [IdUserCreacion] INT            DEFAULT ((0)) NOT NULL,
    [IdUserModifica] INT            CONSTRAINT [ONAConexion_IdUserModifica_Default] DEFAULT ((0)) NULL,
    CONSTRAINT [PK_OC_IdONA] PRIMARY KEY CLUSTERED ([IdONA] ASC),
    CONSTRAINT [CK_OC_Estado] CHECK ([Estado]='X' OR [Estado]='A'),
    CONSTRAINT [CK_OC_Migrar] CHECK ([Migrar]='N' OR [Migrar]='S'),
    CONSTRAINT [CK_OC_OrigenDatos] CHECK ([OrigenDatos]='EXCEL' OR [OrigenDatos]='SQLLITE' OR [OrigenDatos]='SQLSERVER' OR [OrigenDatos]='MYSQL'),
    CONSTRAINT [FK_OC_IdONA] FOREIGN KEY ([IdONA]) REFERENCES [dbo].[ONA] ([IdONA])
);

