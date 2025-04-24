CREATE TABLE [dbo].[EsquemaData] (
    [IdEsquemaData]   INT            IDENTITY (1, 1) NOT NULL,
    [IdEsquemaVista]  INT            NOT NULL,
    [VistaFK]         VARCHAR (200)  NULL,
    [VistaPK]         VARCHAR (40)   NULL,
    [DataEsquemaJson] NVARCHAR (MAX) NOT NULL,
    [DataFecha]       DATETIME       DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [PK_ED_IdEsquemaData] PRIMARY KEY CLUSTERED ([IdEsquemaData] ASC),
    CONSTRAINT [FK_ED_IdEsquemaVista] FOREIGN KEY ([IdEsquemaVista]) REFERENCES [dbo].[EsquemaVista] ([IdEsquemaVista])
);


GO
CREATE NONCLUSTERED INDEX [IX_EsquemaData]
    ON [dbo].[EsquemaData]([IdEsquemaVista] ASC)
    INCLUDE([VistaFK], [VistaPK]);

