

--- 2 ---
CREATE   VIEW [dbo].[vw_AcreditacionEsquema] AS

    SELECT 
       data as Esquema
		, count (data) as Organizacion
    FROM 
        Esquemadata
    CROSS APPLY OPENJSON(DataEsquemaJson) 
    WITH (
        IdHomologacion INT '$.IdHomologacion',
        Data NVARCHAR(MAX) '$.Data'
    )
    WHERE IdHomologacion = 91
	group by data
