CREATE TABLE [dbo].[Usuario] (
    [IdUsuario]         INT            IDENTITY (1, 1) NOT NULL,
    [IdHomologacionRol] INT            NOT NULL,
    [IdONA]             INT            NULL,
    [Nombre]            NVARCHAR (100) NOT NULL,
    [Apellido]          NVARCHAR (100) DEFAULT ('') NOT NULL,
    [Telefono]          NVARCHAR (20)  DEFAULT ('') NOT NULL,
    [Email]             NVARCHAR (100) NOT NULL,
    [Clave]             NVARCHAR (333) NOT NULL,
    [Estado]            NVARCHAR (1)   DEFAULT ('A') NOT NULL,
    [FechaCreacion]     DATETIME       DEFAULT (getdate()) NOT NULL,
    [FechaModifica]     DATETIME       DEFAULT (getdate()) NULL,
    [IdUserCreacion]    INT            DEFAULT ((0)) NOT NULL,
    [IdUserModifica]    INT            CONSTRAINT [Usuario_IdUserModifica_Default] DEFAULT ((0)) NULL,
    CONSTRAINT [PK_U_IdUsuario] PRIMARY KEY CLUSTERED ([IdUsuario] ASC),
    CONSTRAINT [CK_U_Estado] CHECK ([Estado]='X' OR [Estado]='A'),
    CONSTRAINT [FK_U_IdHomologacionRol] FOREIGN KEY ([IdHomologacionRol]) REFERENCES [dbo].[Homologacion] ([IdHomologacion]),
    CONSTRAINT [FK_U_IdONA] FOREIGN KEY ([IdONA]) REFERENCES [dbo].[ONA] ([IdONA]),
    CONSTRAINT [UK_U_Email] UNIQUE NONCLUSTERED ([Email] ASC)
);

