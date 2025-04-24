

--- 15 ---
CREATE VIEW [dbo].[vw_ActualizacionONA] AS

SELECT 
    FORMAT(FechaCreacion, 'yyyy/MM/dd') AS Fecha, 
    Siglas AS ONA, 
    COUNT(*) AS Actualizaciones 
FROM 
    LogMigracion a
JOIN 
    ONA b ON a.IdONA = b.IdONA
GROUP BY 
    FORMAT(FechaCreacion, 'yyyy/MM/dd'), Siglas;
