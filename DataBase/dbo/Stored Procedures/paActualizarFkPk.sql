
CREATE    procedure [dbo].[paActualizarFkPk] AS
BEGIN
	-- Actualizar Esquemadata con los valores de la consulta
--select * from  EsquemaData where IdEsquemaVista in( 1,11) --78 padre -- 79 hijo principal

--select * from  EsquemaData where IdEsquemaVista in( 2,12) --79 padre -- 109 hijo principal
--select * from  EsquemaData where IdEsquemaVista in( 3,13) --79 padre -- 99 hijo principal
--select * from  EsquemaData where IdEsquemaVista in( 4,14) --79 padre -- 67 hijo principal
--select * from  EsquemaData where IdEsquemaVista in( 5,15) --79 padre -- 57 hijo principal


 
----
UPDATE e
SET 
    e.VistaFK = c.Padre,
    e.VistaPK = c.Hija
FROM Esquemadata e
JOIN (
    -- Consulta combinada para obtener los valores de hija y padre
    SELECT 
        hija.IdEsquemaData,
        padre AS Padre,
        hija AS Hija
    FROM 
        (SELECT 
            idesquemadata AS IdEsquemaData,
            Data AS Hija
         FROM Esquemadata
         CROSS APPLY OPENJSON(DataEsquemaJson) 
         WITH (
             IdHomologacion INT '$.IdHomologacion',
             Data NVARCHAR(MAX) '$.Data'
         )
         WHERE IdHomologacion IN (79)   and IdEsquemaVista in (  1,6,11,16)) hija
    INNER JOIN 
        (SELECT 
            idesquemadata AS IdEsquemaData,
            Data AS Padre
         FROM Esquemadata
         CROSS APPLY OPENJSON(DataEsquemaJson) 
         WITH (
             IdHomologacion INT '$.IdHomologacion',
             Data NVARCHAR(MAX) '$.Data'
         )
         WHERE IdHomologacion IN (82)  and  IdEsquemaVista in( 1,6,11,16) ) padre
    ON padre.IdEsquemaData = hija.IdEsquemaData
) c
ON e.IdEsquemaData = c.IdEsquemaData;


-- 

----
UPDATE e
SET 
    e.VistaFK = c.Padre,
    e.VistaPK = c.Hija
FROM Esquemadata e
JOIN (
    -- Consulta combinada para obtener los valores de hija y padre
    SELECT 
        hija.IdEsquemaData,
        padre AS Padre,
        hija AS Hija
    FROM 
        (SELECT 
            idesquemadata AS IdEsquemaData,
            Data AS Hija
         FROM Esquemadata
         CROSS APPLY OPENJSON(DataEsquemaJson) 
         WITH (
             IdHomologacion INT '$.IdHomologacion',
             Data NVARCHAR(MAX) '$.Data'
         )
         WHERE IdHomologacion IN (109)   and IdEsquemaVista in (  2,7,12,17)) hija
    INNER JOIN 
        (SELECT 
            idesquemadata AS IdEsquemaData,
            Data AS Padre
         FROM Esquemadata
         CROSS APPLY OPENJSON(DataEsquemaJson) 
         WITH (
             IdHomologacion INT '$.IdHomologacion',
             Data NVARCHAR(MAX) '$.Data'
         )
         WHERE IdHomologacion IN (79)  and  IdEsquemaVista in( 2,7,12,17) ) padre
    ON padre.IdEsquemaData = hija.IdEsquemaData
) c
ON e.IdEsquemaData = c.IdEsquemaData;


-- 

----
UPDATE e
SET 
    e.VistaFK = c.Padre,
    e.VistaPK = c.Hija
FROM Esquemadata e
JOIN (
    -- Consulta combinada para obtener los valores de hija y padre
    SELECT 
        hija.IdEsquemaData,
        padre AS Padre,
        hija AS Hija
    FROM 
        (SELECT 
            idesquemadata AS IdEsquemaData,
            Data AS Hija
         FROM Esquemadata
         CROSS APPLY OPENJSON(DataEsquemaJson) 
         WITH (
             IdHomologacion INT '$.IdHomologacion',
             Data NVARCHAR(MAX) '$.Data'
         )
         WHERE IdHomologacion IN (99)   and IdEsquemaVista in (  3,8,13,18)) hija
    INNER JOIN 
        (SELECT 
            idesquemadata AS IdEsquemaData,
            Data AS Padre
         FROM Esquemadata
         CROSS APPLY OPENJSON(DataEsquemaJson) 
         WITH (
             IdHomologacion INT '$.IdHomologacion',
             Data NVARCHAR(MAX) '$.Data'
         )
         WHERE IdHomologacion IN (79)  and  IdEsquemaVista in( 3,8,13,18) ) padre
    ON padre.IdEsquemaData = hija.IdEsquemaData
) c
ON e.IdEsquemaData = c.IdEsquemaData;


-- 

----
UPDATE e
SET 
    e.VistaFK = c.Padre,
    e.VistaPK = c.Hija
FROM Esquemadata e
JOIN (
    -- Consulta combinada para obtener los valores de hija y padre
    SELECT 
        hija.IdEsquemaData,
        padre AS Padre,
        hija AS Hija
    FROM 
        (SELECT 
            idesquemadata AS IdEsquemaData,
            Data AS Hija
         FROM Esquemadata
         CROSS APPLY OPENJSON(DataEsquemaJson) 
         WITH (
             IdHomologacion INT '$.IdHomologacion',
             Data NVARCHAR(MAX) '$.Data'
         )
         WHERE IdHomologacion IN (67)   and IdEsquemaVista in (  4,9,14,19)) hija
    INNER JOIN 
        (SELECT 
            idesquemadata AS IdEsquemaData,
            Data AS Padre
         FROM Esquemadata
         CROSS APPLY OPENJSON(DataEsquemaJson) 
         WITH (
             IdHomologacion INT '$.IdHomologacion',
             Data NVARCHAR(MAX) '$.Data'
         )
         WHERE IdHomologacion IN (79)  and  IdEsquemaVista in( 4,9,14,19) ) padre
    ON padre.IdEsquemaData = hija.IdEsquemaData
) c
ON e.IdEsquemaData = c.IdEsquemaData;


-- 

UPDATE e
SET 
    e.VistaFK = c.Padre,
    e.VistaPK = c.Hija
FROM Esquemadata e
JOIN (
    -- Consulta combinada para obtener los valores de hija y padre
    SELECT 
        hija.IdEsquemaData,
        padre AS Padre,
        hija AS Hija
    FROM 
        (SELECT 
            idesquemadata AS IdEsquemaData,
            Data AS Hija
         FROM Esquemadata
         CROSS APPLY OPENJSON(DataEsquemaJson) 
         WITH (
             IdHomologacion INT '$.IdHomologacion',
             Data NVARCHAR(MAX) '$.Data'
         )
         WHERE IdHomologacion IN (57)   and IdEsquemaVista in ( 5,10,15,20)) hija
    INNER JOIN 
        (SELECT 
            idesquemadata AS IdEsquemaData,
            Data AS Padre
         FROM Esquemadata
         CROSS APPLY OPENJSON(DataEsquemaJson) 
         WITH (
             IdHomologacion INT '$.IdHomologacion',
             Data NVARCHAR(MAX) '$.Data'
         )
         WHERE IdHomologacion IN (79)  and  IdEsquemaVista in( 5,10,15,20) ) padre
    ON padre.IdEsquemaData = hija.IdEsquemaData
) c
ON e.IdEsquemaData = c.IdEsquemaData;


END;
