CREATE PROCEDURE [dbo].[GetFiltroMasUsado]
AS
 WITH Filtros AS (
    SELECT 
        e.CodigoHomologacionRol,
        JSON_VALUE(e.UbicacionJson, '$.IpAddress') AS IpAddress,
		e.FechaCreacion Fecha,
        'FiltroPais' AS FiltroTipo, 
        paises.[value] AS FiltroValor 
    FROM dbo.EventTracking e
    OUTER APPLY OPENJSON(ParametroJson, '$.FiltroPais') AS paises
    UNION ALL
    SELECT 
        e.CodigoHomologacionRol,
        JSON_VALUE(e.UbicacionJson, '$.IpAddress'),
		e.FechaCreacion Fecha,
        'FiltroOna', 
        onas.[value] 
    FROM dbo.EventTracking e
    OUTER APPLY OPENJSON(ParametroJson, '$.FiltroOna') AS onas
    UNION ALL
    SELECT 
        e.CodigoHomologacionRol,
        JSON_VALUE(e.UbicacionJson, '$.IpAddress'),
		e.FechaCreacion Fecha,
        'FiltroNorma', 
        normas.[value] 
    FROM dbo.EventTracking e
    OUTER APPLY OPENJSON(ParametroJson, '$.FiltroNorma') AS normas
    UNION ALL
    SELECT 
        e.CodigoHomologacionRol,
        JSON_VALUE(e.UbicacionJson, '$.IpAddress'),
		e.FechaCreacion Fecha,
        'FiltroEsquema', 
        esquemas.[value] 
    FROM dbo.EventTracking e
    OUTER APPLY OPENJSON(ParametroJson, '$.FiltroEsquema') AS esquemas
    UNION ALL
    SELECT 
        e.CodigoHomologacionRol,
        JSON_VALUE(e.UbicacionJson, '$.IpAddress'),
		e.FechaCreacion Fecha,
        'FiltroEstado', 
        estados.[value] 
    FROM dbo.EventTracking e
    OUTER APPLY OPENJSON(ParametroJson, '$.FiltroEstado') AS estados
    UNION ALL
    SELECT 
        e.CodigoHomologacionRol,
        JSON_VALUE(e.UbicacionJson, '$.IpAddress'),
		e.FechaCreacion Fecha,
        'FiltroRecomocimiento', 
        reconocimientos.[value] 
    FROM dbo.EventTracking e
    OUTER APPLY OPENJSON(ParametroJson, '$.FiltroRecomocimiento') AS reconocimientos
)
SELECT TOP 1 WITH TIES 
    CodigoHomologacionRol, 
    IpAddress, 
    FiltroTipo, 
    FiltroValor, 
    COUNT(*) AS Uso
FROM Filtros
WHERE FiltroValor IS NOT NULL
GROUP BY CodigoHomologacionRol, IpAddress,FiltroTipo, FiltroValor
ORDER BY ROW_NUMBER() OVER (PARTITION BY FiltroTipo ORDER BY COUNT(*) DESC);


