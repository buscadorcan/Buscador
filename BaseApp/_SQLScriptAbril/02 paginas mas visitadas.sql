-- =============================================
-- Author:		<Author,,JHONATAN>
-- Create date: <Create 02-04-2025,,>
-- Description:	<consulta las paginas mas visitadas,,>
-- =============================================
CREATE PROCEDURE dbo.getPaginasMasVisitada

AS

	  WITH UsoPorIP AS (
		SELECT 
			CodigoHomologacionRol, 
			CodigoHomologacionMenu,
			JSON_VALUE(UbicacionJson, '$.IpAddress') AS IpAddress,
			COUNT(*) AS uso,
			ROW_NUMBER() OVER (PARTITION BY JSON_VALUE(UbicacionJson, '$.IpAddress') ORDER BY COUNT(*) DESC) AS rn
		FROM dbo.EventTracking 
		WHERE UbicacionJson IS NOT NULL 
			  AND JSON_VALUE(UbicacionJson, '$.IpAddress') <> '::1'
		GROUP BY CodigoHomologacionRol, CodigoHomologacionMenu, JSON_VALUE(UbicacionJson, '$.IpAddress')
	)
	SELECT CodigoHomologacionRol, 
	CodigoHomologacionMenu, 
	IpAddress,	
	NULL AS Latitud,
	NULL AS Longitud, 
	uso
	FROM UsoPorIP
	WHERE rn = 1
	ORDER BY uso DESC;
