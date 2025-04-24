CREATE TABLE [dbo].[LogMigracionDetalle] (
    [IdLogMigracionDetalle] INT            IDENTITY (1, 1) NOT NULL,
    [IdLogMigracion]        INT            NULL,
    [NroMigracion]          INT            NULL,
    [IdEsquemaVista]        INT            NULL,
    [ColumnaEsquemaIdH]     INT            NULL,
    [ColumnaEsquema]        NVARCHAR (100) NULL,
    [ColumnaVista]          NVARCHAR (100) NULL,
    [ColumnaVistaPK]        BIT            DEFAULT ((0)) NULL,
    [Fecha]                 DATETIME       DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [PK_LMD_IdLogMigracionDetalle] PRIMARY KEY CLUSTERED ([IdLogMigracionDetalle] ASC)
);

