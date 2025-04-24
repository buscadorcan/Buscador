CREATE TABLE [dbo].[EventTrackingBK] (
    [IdEventTracking] INT            IDENTITY (1, 1) NOT NULL,
    [TipoUsuario]     NVARCHAR (25)  NOT NULL,
    [NombreUsuario]   NVARCHAR (100) NOT NULL,
    [NombrePagina]    NVARCHAR (100) NOT NULL,
    [NombreControl]   NVARCHAR (100) NOT NULL,
    [NombreAccion]    NVARCHAR (100) NOT NULL,
    [UbicacionJson]   NVARCHAR (MAX) NOT NULL,
    [ParametroJson]   NVARCHAR (MAX) NOT NULL,
    [ErrorTracking]   NVARCHAR (MAX) NOT NULL,
    [FechaCreacion]   DATETIME       NOT NULL
);

