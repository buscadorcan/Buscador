CREATE TABLE [dbo].[EventTracking] (
    [IdEventTracking]        INT            IDENTITY (1, 1) NOT NULL,
    [IdUsuario]              INT            DEFAULT ((0)) NOT NULL,
    [CodigoHomologacionRol]  NVARCHAR (50)  DEFAULT ('') NOT NULL,
    [CodigoHomologacionMenu] NVARCHAR (50)  DEFAULT ('') NOT NULL,
    [NombreControl]          NVARCHAR (100) DEFAULT ('') NOT NULL,
    [NombreAccion]           NVARCHAR (100) DEFAULT ('') NOT NULL,
    [UbicacionJson]          NVARCHAR (MAX) DEFAULT ('{}') NOT NULL,
    [ParametroJson]          NVARCHAR (MAX) DEFAULT ('{}') NOT NULL,
    [ErrorTracking]          NVARCHAR (MAX) DEFAULT ('') NOT NULL,
    [FechaCreacion]          DATETIME       DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [PK_ET_IdEventTracking] PRIMARY KEY CLUSTERED ([IdEventTracking] ASC),
    CONSTRAINT [CK_ET_ParametroJson] CHECK (isjson([ParametroJson])=(1))
);

