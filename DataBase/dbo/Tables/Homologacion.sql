CREATE TABLE [dbo].[Homologacion] (
    [IdHomologacion]       INT            IDENTITY (1, 1) NOT NULL,
    [IdHomologacionGrupo]  INT            DEFAULT (NULL) NULL,
    [Mostrar]              CHAR (1)       DEFAULT ('S') NOT NULL,
    [Indexar]              NVARCHAR (1)   DEFAULT ('N') NOT NULL,
    [MostrarWebOrden]      INT            DEFAULT ((0)) NULL,
    [MostrarWeb]           NVARCHAR (90)  NOT NULL,
    [TooltipWeb]           NVARCHAR (200) DEFAULT ('') NOT NULL,
    [MascaraDato]          NVARCHAR (10)  DEFAULT ('TEXTO') NOT NULL,
    [SiNoHayDato]          NVARCHAR (10)  DEFAULT ('') NOT NULL,
    [InfoExtraJson]        NVARCHAR (MAX) DEFAULT ('{}') NOT NULL,
    [CodigoHomologacion]   NVARCHAR (30)  DEFAULT ('') NULL,
    [NombreHomologado]     NVARCHAR (90)  DEFAULT ('') NOT NULL,
    [Estado]               NVARCHAR (1)   DEFAULT ('A') NOT NULL,
    [FechaCreacion]        DATETIME       DEFAULT (getdate()) NOT NULL,
    [FechaModifica]        DATETIME       DEFAULT (getdate()) NULL,
    [IdUserCreacion]       INT            DEFAULT ((0)) NOT NULL,
    [IdUserModifica]       INT            CONSTRAINT [DF_IdUserModifica_Default] DEFAULT ((0)) NULL,
    [IdHomologacionFiltro] INT            NULL,
    CONSTRAINT [PK_H_IdHomologacion] PRIMARY KEY CLUSTERED ([IdHomologacion] ASC),
    CONSTRAINT [CK_H_Estado] CHECK ([Estado]='X' OR [Estado]='A'),
    CONSTRAINT [CK_H_InfoExtraJson] CHECK (isjson([InfoExtraJson])=(1)),
    CONSTRAINT [CK_H_MascaraDato] CHECK ([MascaraDato]='FORMULA' OR [MascaraDato]='ICO' OR [MascaraDato]='NUMERICO' OR [MascaraDato]='FECHA' OR [MascaraDato]='TEXTO'),
    CONSTRAINT [CK_H_Mostrar] CHECK ([Mostrar]='N' OR [Mostrar]='S'),
    CONSTRAINT [FK_Homologacion_Homologacion] FOREIGN KEY ([IdHomologacionGrupo]) REFERENCES [dbo].[Homologacion] ([IdHomologacion])
);


GO
CREATE NONCLUSTERED INDEX [IX_Homologacion]
    ON [dbo].[Homologacion]([IdHomologacionGrupo] ASC)
    INCLUDE([Estado], [Mostrar], [Indexar], [MostrarWeb]);

