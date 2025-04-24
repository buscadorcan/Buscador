CREATE TABLE [dbo].[Esquema] (
    [IdEsquema]       INT            IDENTITY (1, 1) NOT NULL,
    [MostrarWebOrden] INT            DEFAULT ((1)) NOT NULL,
    [MostrarWeb]      NVARCHAR (200) DEFAULT ('GRILLA') NOT NULL,
    [TooltipWeb]      NVARCHAR (200) DEFAULT ('') NOT NULL,
    [EsquemaVista]    NVARCHAR (100) DEFAULT ('') NOT NULL,
    [EsquemaJson]     NVARCHAR (MAX) DEFAULT ('{}') NOT NULL,
    [Estado]          NVARCHAR (1)   DEFAULT ('A') NOT NULL,
    [FechaCreacion]   DATETIME       DEFAULT (getdate()) NOT NULL,
    [FechaModifica]   DATETIME       DEFAULT (getdate()) NULL,
    [IdUserCreacion]  INT            DEFAULT ((0)) NOT NULL,
    [IdUserModifica]  INT            CONSTRAINT [Esquema_IdUserModifica_Default] DEFAULT ((0)) NULL,
    CONSTRAINT [PK_E_IdEsquema] PRIMARY KEY CLUSTERED ([IdEsquema] ASC),
    CONSTRAINT [CK_E_EsquemaJson] CHECK (isjson([EsquemaJson])=(1)),
    CONSTRAINT [CK_E_Estado] CHECK ([Estado]='X' OR [Estado]='A')
);


GO
CREATE NONCLUSTERED INDEX [IX_Esquema]
    ON [dbo].[Esquema]([MostrarWeb] ASC)
    INCLUDE([Estado], [EsquemaVista]);

