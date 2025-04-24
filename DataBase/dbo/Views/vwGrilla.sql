CREATE VIEW [dbo].[vwGrilla] AS 
	SELECT	 H.IdHomologacion
			,H.MostrarWeb
			,H.TooltipWeb
			,H.MostrarWebOrden
			,H.InfoExtraJson
	FROM	Homologacion	H	WITH (NOLOCK)
	JOIN	(	SELECT DISTINCT IdHomologacion IdHomologacion
				FROM  OPENJSON((	SELECT  TOP 1 EsquemaJson
									FROM	Esquema (NOLOCK)
									WHERE	Estado = 'A'
									ORDER BY MostrarWebOrden
				))
				WITH (IdHomologacion INT '$.IdHomologacion')
			)	HE	ON HE.IdHomologacion = H.IdHomologacion
	WHERE   H.Mostrar ='S'
