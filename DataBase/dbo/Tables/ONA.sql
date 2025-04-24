CREATE TABLE [dbo].[ONA] (
    [IdONA]              INT            IDENTITY (1, 1) NOT NULL,
    [IdHomologacionPais] INT            NULL,
    [RazonSocial]        NVARCHAR (300) NOT NULL,
    [Siglas]             NVARCHAR (50)  NOT NULL,
    [Ciudad]             NVARCHAR (50)  NOT NULL,
    [Correo]             NVARCHAR (300) NULL,
    [Direccion]          NVARCHAR (300) NULL,
    [PaginaWeb]          NVARCHAR (300) NULL,
    [Telefono]           NVARCHAR (20)  NULL,
    [UrlIcono]           NVARCHAR (300) NULL,
    [UrlLogo]            NVARCHAR (300) NULL,
    [InfoExtraJson]      NVARCHAR (MAX) DEFAULT ('{}') NOT NULL,
    [Estado]             NVARCHAR (1)   DEFAULT ('A') NOT NULL,
    [FechaCreacion]      DATETIME       DEFAULT (getdate()) NOT NULL,
    [FechaModifica]      DATETIME       DEFAULT (getdate()) NULL,
    [IdUserCreacion]     INT            DEFAULT ((0)) NOT NULL,
    [IdUserModifica]     INT            CONSTRAINT [ONA_IdUserModifica_Default] DEFAULT ((0)) NULL,
    CONSTRAINT [PK_O_IdONA] PRIMARY KEY CLUSTERED ([IdONA] ASC),
    CONSTRAINT [CK_O_Estado] CHECK ([Estado]='X' OR [Estado]='A'),
    CONSTRAINT [CK_O_InfoExtraJson] CHECK (isjson([InfoExtraJson])=(1))
);


GO
CREATE NONCLUSTERED INDEX [IX_ONA]
    ON [dbo].[ONA]([IdHomologacionPais] ASC)
    INCLUDE([Siglas]);

