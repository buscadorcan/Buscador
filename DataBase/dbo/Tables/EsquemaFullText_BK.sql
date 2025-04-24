CREATE TABLE [dbo].[EsquemaFullText_BK] (
    [IdEsquemaFullText] INT            IDENTITY (1, 1) NOT NULL,
    [IdEsquemaData]     INT            NOT NULL,
    [IdHomologacion]    INT            NOT NULL,
    [FullTextData]      NVARCHAR (MAX) NULL
);

