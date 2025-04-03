-- =============================================
-- Author:		<Author,,jhonatan>
-- Create date: <Create Date,02-04-2025,>
-- Description:	<Description, consulta los filtro mas usado,>
-- =============================================
CREATE PROCEDURE dbo.GetFiltroMasUsado

AS

 WITH Filtros AS (
    SELECT 
        e.CodigoHomologacionRol,
        JSON_VALUE(e.UbicacionJson, '$.IpAddress') AS IpAddress,
        'FiltroPais' AS FiltroTipo, 
        paises.[value] AS FiltroValor 
    FROM dbo.EventTracking e
    OUTER APPLY OPENJSON(ParametroJson, '$.FiltroPais') AS paises
    UNION ALL
    SELECT 
        e.CodigoHomologacionRol,
        JSON_VALUE(e.UbicacionJson, '$.IpAddress'),
        'FiltroOna', 
        onas.[value] 
    FROM dbo.EventTracking e
    OUTER APPLY OPENJSON(ParametroJson, '$.FiltroOna') AS onas
    UNION ALL
    SELECT 
        e.CodigoHomologacionRol,
        JSON_VALUE(e.UbicacionJson, '$.IpAddress'),
        'FiltroNorma', 
        normas.[value] 
    FROM dbo.EventTracking e
    OUTER APPLY OPENJSON(ParametroJson, '$.FiltroNorma') AS normas
    UNION ALL
    SELECT 
        e.CodigoHomologacionRol,
        JSON_VALUE(e.UbicacionJson, '$.IpAddress'),
        'FiltroEsquema', 
        esquemas.[value] 
    FROM dbo.EventTracking e
    OUTER APPLY OPENJSON(ParametroJson, '$.FiltroEsquema') AS esquemas
    UNION ALL
    SELECT 
        e.CodigoHomologacionRol,
        JSON_VALUE(e.UbicacionJson, '$.IpAddress'),
        'FiltroEstado', 
        estados.[value] 
    FROM dbo.EventTracking e
    OUTER APPLY OPENJSON(ParametroJson, '$.FiltroEstado') AS estados
    UNION ALL
    SELECT 
        e.CodigoHomologacionRol,
        JSON_VALUE(e.UbicacionJson, '$.IpAddress'),
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
GROUP BY CodigoHomologacionRol, IpAddress, FiltroTipo, FiltroValor
ORDER BY ROW_NUMBER() OVER (PARTITION BY FiltroTipo ORDER BY COUNT(*) DESC);


