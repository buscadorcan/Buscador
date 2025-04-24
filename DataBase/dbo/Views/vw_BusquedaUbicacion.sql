
--- 14 ---
CREATE VIEW [dbo].[vw_BusquedaUbicacion]   AS
SELECT  'Ecuador'                 AS 'Pais'
        ,'Quito'                  AS 'Ciudad'
        ,10                       AS 'Busqueda'
UNION
SELECT  'Perú'
        ,'Lima'
        ,50
UNION
SELECT  'Ecuador'
        ,'Guayaquil'
        ,60
