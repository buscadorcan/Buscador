CREATE TABLE [dbo].[HomologacionFiltro] (
    [IdHF]                  INT           IDENTITY (1, 1) NOT NULL,
    [IdHomologacionFiltro]  INT           NOT NULL,
    [MostrarWeb]            NVARCHAR (90) NOT NULL,
    [CodigoHomologacionFil] NVARCHAR (30) NOT NULL,
    [FechaCreacion]         DATETIME      DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [PK_HF_IdHF] PRIMARY KEY CLUSTERED ([IdHF] ASC)
);

