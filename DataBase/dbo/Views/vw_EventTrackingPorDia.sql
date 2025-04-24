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