CREATE PROCEDURE [dbo].[GetTiempoEnSession]
AS
WITH Agrupado AS (
    SELECT 
        JSON_VALUE(UbicacionJson, '$.IpAddress') AS IpDirec,
        us.Nombre CodigoHomologacionRol,
        CAST(et.FechaCreacion AS DATE) AS Fecha, -- Agrupa por día
        MIN(et.FechaCreacion) AS FechaInicio,
        MAX(et.FechaCreacion) AS FechaFin
    FROM dbo.EventTracking et JOIN	Usuario us ON et.IdUsuario = us.IdUsuario
    GROUP BY 
        JSON_VALUE(UbicacionJson, '$.IpAddress'), 
        us.Nombre, 
        CAST(et.FechaCreacion AS DATE)
)
SELECT 
    IpDirec,
	NULL AS Latitud,
	NULL AS Longitud,
    CodigoHomologacionRol,
    Fecha,
    FechaInicio,
    FechaFin,
    DATEDIFF(MINUTE, FechaInicio, FechaFin) AS TiempoDeConeccionEnMin
FROM Agrupado
WHERE DATEDIFF(MINUTE, FechaInicio, FechaFin) > 0
ORDER BY Fecha, 
         IpDirec,
		 CodigoHomologacionRol;



