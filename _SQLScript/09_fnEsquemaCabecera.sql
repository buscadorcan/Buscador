
create or alter    FUNCTION [dbo].[fnEsquemaCabecera] ( 
    @IdEsquemadata INT
)
--| 2K25.FEB.25 | patricio.paccha | BUSCADOR ANDINO | Versión: 1.0
--| Descripción: Función para mostrar el esquema con sus datos buscados
RETURNS TABLE 
AS 
RETURN 
(
	with Esquema_ as
	(
		SELECT	distinct ev.idesquema 
		FROM	EsquemaOrganiza	(NOLOCK)  eo 
		join esquemadata (NOLOCK) ed  on  ed.IdEsquemaData = eo.IdEsquemaData
		join EsquemaVista ev on ev.IdEsquemaVista = ed.IdEsquemaVista
		WHERE	eo.IdEsquemaData = @IdEsquemadata
	)
	SELECT		e.IdEsquema
				,MostrarWebOrden
				,MostrarWeb
				,TooltipWeb
				,EsquemaVista
				,(	SELECT	 H.IdHomologacion
							,H.MostrarWeb
							,H.TooltipWeb
							,H.MostrarWebOrden
							,H.NombreHomologado
					FROM	Homologacion H  (NOLOCK)
					JOIN (	SELECT	DISTINCT IdHomologacion IdHomologacion
							FROM	OPENJSON((	SELECT	EsquemaJson
												FROM	Esquema   (NOLOCK)
												WHERE	IdEsquema = ee.IdEsquema
											))	WITH	(IdHomologacion INT '$.IdHomologacion')
					) HE	ON HE.IdHomologacion = H.IdHomologacion
					WHERE	H.Estado = 'A'
					FOR JSON AUTO
				)	EsquemaJson
		FROM	Esquema e (NOLOCK)
		join Esquema_ ee on ee.IdEsquema= e.IdEsquema
		AND		Estado = 'A'
);
go
