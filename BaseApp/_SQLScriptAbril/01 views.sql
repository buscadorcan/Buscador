CREATE VIEW vw_EventTrackingSession AS
WITH UltimaConexion AS (
    SELECT 
        CodigoHomologacionRol,
        NombreControl,
		FechaCreacion,
        FechaCreacion AS FechaInicio,
        JSON_VALUE(UbicacionJson, '$.IpAddress') AS IpDirec,
        MAX(FechaCreacion) OVER (PARTITION BY CodigoHomologacionRol) AS FechaFin
    FROM dbo.EventTracking
)
SELECT 
    CodigoHomologacionRol, 
    NombreControl, 
	FechaCreacion,
    FechaInicio, 
    FechaFin, 
    IpDirec, 
    DATEDIFF(SECOND, FechaInicio, FechaFin) AS TiempoEnSegundos
FROM UltimaConexion;


CREATE VIEW vw_EventTrackingPorDia AS
SELECT
    CAST(FechaCreacion AS DATE) AS Fecha,
    CodigoHomologacionRol,
    COUNT(*) AS TotalEventos,
    COUNT(DISTINCT IdUsuario) AS totalUsuario
FROM dbo.EventTracking
GROUP BY
    CAST(FechaCreacion AS DATE),
    CodigoHomologacionRol;

CREATE VIEW vw_EventTrackingPorDiaDetalle AS
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

