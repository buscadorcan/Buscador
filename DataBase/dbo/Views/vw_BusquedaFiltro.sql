CREATE VIEW [dbo].[vw_BusquedaFiltro]  AS
SELECT  'ONA'                 AS 'FiltroPor'
        ,10                   AS 'Busqueda'
UNION
SELECT  'Pais'
        ,20
UNION
SELECT  'Calificación'
        ,50

