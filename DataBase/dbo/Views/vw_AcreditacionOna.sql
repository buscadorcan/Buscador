






--- 1 ---
CREATE     VIEW [dbo].[vw_AcreditacionOna] AS
WITH OEC_CTE AS (
    SELECT 
        idesquemadata AS IdEsquemaData
		,IdEsquemaVista
		,Data as empresa
    FROM 
        Esquemadata
    CROSS APPLY OPENJSON(DataEsquemaJson) 
    WITH (
        IdHomologacion INT '$.IdHomologacion',
        Data NVARCHAR(MAX) '$.Data'
    )
    WHERE IdHomologacion = 91
    
) 
select 
h.MostrarWeb Pais,

o.Siglas  as ONA, 
count(empresa) Organizacion
from OEC_CTE j 
join EsquemaVista v ON j.IdEsquemaVista= v.IdEsquemaVista
right join ONA o ON v.IdONA = o.IdONA
join Homologacion h ON o.IdHomologacionPais = h.IdHomologacion
and o.estado = 'A'
group by o.Siglas , h.MostrarWeb,o.UrlIcono
having count(empresa)>0
