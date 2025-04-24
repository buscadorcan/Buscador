CREATE TABLE [dbo].[EsquemaFullText] (
    [IdEsquemaFullText] INT            IDENTITY (1, 1) NOT NULL,
    [IdEsquemaData]     INT            NOT NULL,
    [IdHomologacion]    INT            NOT NULL,
    [FullTextData]      NVARCHAR (MAX) COLLATE Latin1_General_CI_AI NULL,
    CONSTRAINT [PK_EFT_IdEsquemaFullText] PRIMARY KEY CLUSTERED ([IdEsquemaFullText] ASC)
);

