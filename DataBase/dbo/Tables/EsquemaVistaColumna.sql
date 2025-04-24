CREATE TABLE [dbo].[EsquemaVistaColumna] (
    [IdEsquemaVistaColumna] INT            IDENTITY (1, 1) NOT NULL,
    [IdEsquemaVista]        INT            NOT NULL,
    [ColumnaEsquemaIdH]     INT            NOT NULL,
    [ColumnaEsquema]        NVARCHAR (100) NOT NULL,
    [ColumnaVista]          NVARCHAR (100) NOT NULL,
    [ColumnaVistaPK]        BIT            DEFAULT ((0)) NOT NULL,
    [Estado]                NVARCHAR (1)   DEFAULT ('A') NOT NULL,
    [FechaCreacion]         DATETIME       DEFAULT (getdate()) NOT NULL,
    [FechaModifica]         DATETIME       DEFAULT (getdate()) NULL,
    [IdUserCreacion]        INT            DEFAULT ((0)) NOT NULL,
    [IdUserModifica]        INT            CONSTRAINT [EsquemaVistaColumna_IdUserModifica_Default] DEFAULT ((0)) NULL,
    CONSTRAINT [PK_EVC_IdEsquemaVistaColumna] PRIMARY KEY CLUSTERED ([IdEsquemaVistaColumna] ASC),
    CONSTRAINT [CK_EVC_Estado] CHECK ([Estado]='X' OR [Estado]='A'),
    CONSTRAINT [FK_EVC_IdEsquemaVista] FOREIGN KEY ([IdEsquemaVista]) REFERENCES [dbo].[EsquemaVista] ([IdEsquemaVista])
);

