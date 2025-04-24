CREATE TABLE [dbo].[EsquemaVista] (
    [IdEsquemaVista] INT            IDENTITY (1, 1) NOT NULL,
    [IdONA]          INT            NOT NULL,
    [IdEsquema]      INT            NOT NULL,
    [VistaOrigen]    NVARCHAR (100) NOT NULL,
    [Estado]         NVARCHAR (1)   DEFAULT ('A') NOT NULL,
    [FechaCreacion]  DATETIME       DEFAULT (getdate()) NOT NULL,
    [FechaModifica]  DATETIME       DEFAULT (getdate()) NULL,
    [IdUserCreacion] INT            DEFAULT ((0)) NOT NULL,
    [IdUserModifica] INT            CONSTRAINT [EsquemaVista_IdUserModifica_Default] DEFAULT ((0)) NULL,
    CONSTRAINT [PK_EV_IdEsquemaVista] PRIMARY KEY CLUSTERED ([IdEsquemaVista] ASC),
    CONSTRAINT [CK_EV_Estado] CHECK ([Estado]='X' OR [Estado]='A'),
    CONSTRAINT [FK_EV_IdEsquema] FOREIGN KEY ([IdEsquema]) REFERENCES [dbo].[Esquema] ([IdEsquema]),
    CONSTRAINT [FK_EV_IdONA] FOREIGN KEY ([IdONA]) REFERENCES [dbo].[ONA] ([IdONA])
);


GO
CREATE NONCLUSTERED INDEX [IX_EsquemaVista]
    ON [dbo].[EsquemaVista]([IdEsquema] ASC)
    INCLUDE([IdONA], [VistaOrigen], [Estado]);

