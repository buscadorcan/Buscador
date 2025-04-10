create or ALTER VIEW vw_EventTrackingSession AS
SELECT CodigoHomologacionRol, 
NombreControl, 
FechaCreacion, 
JSON_VALUE(UbicacionJson, '$.IpAddress') AS IpDirec, 
MIN(FechaCreacion) OVER (PARTITION BY FechaCreacion, CodigoHomologacionRol, NombreControl) AS FechaInicio, 
                  MAX(FechaCreacion) OVER (PARTITION BY FechaCreacion, CodigoHomologacionRol, NombreControl) AS FechaFin, 
                  DATEDIFF(SECOND, MIN(FechaCreacion) OVER (PARTITION BY FechaCreacion, CodigoHomologacionRol, 
                  NombreControl), MAX(FechaCreacion) OVER (PARTITION BY FechaCreacion, CodigoHomologacionRol, NombreControl)) AS TiempoEnSegundos
FROM     dbo.EventTracking;
go



create or ALTER VIEW vw_EventTrackingPorDia AS
SELECT
    CAST(FechaCreacion AS DATE) AS Fecha,
    CodigoHomologacionRol,
    COUNT(*) AS TotalEventos,
    COUNT(DISTINCT IdUsuario) AS totalUsuario
FROM dbo.EventTracking
GROUP BY
    CAST(FechaCreacion AS DATE),
    CodigoHomologacionRol;
go


create or ALTER VIEW vw_EventTrackingPorDiaDetalle AS
SELECT
    CAST(FechaCreacion AS DATE) AS Fecha,
    IdUsuario,
    CodigoHomologacionRol,
    NombreControl,
    COUNT(*) AS totalUsuario
FROM dbo.EventTracking
GROUP BY
    CAST(FechaCreacion AS DATE),
    IdUsuario,
    CodigoHomologacionRol,
    NombreControl;
go
