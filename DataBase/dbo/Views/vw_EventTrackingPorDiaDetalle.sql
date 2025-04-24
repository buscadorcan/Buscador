CREATE VIEW vw_EventTrackingPorDiaDetalle AS
SELECT
    CAST(FechaCreacion AS DATE) AS Fecha,
    IdUsuario,
    CodigoHomologacionRol,
    NombreControl,
    COUNT(*) AS totalUsuario
FROM dbo.EventTracking
GROUP BY
    CAST(FechaCreacion AS DATE),
    IdUsuario,
    CodigoHomologacionRol,
    NombreControl;