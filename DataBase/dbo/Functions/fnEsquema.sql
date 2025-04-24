
/*----------------------------------------------------------------------------------------\
|    ©Copyright 2K25					                          BUSCADOR ANDINO		  |
|-----------------------------------------------------------------------------------------|
| Este código está protegido por las leyes y tratados internacionales de derechos de autor|
\-----------------------------------------------------------------------------------------/
  [App]				: Buscador Andino											
	- Date          : 2K25.FEB.05	
	- Author        : patricio.paccha														
	- Version	    : 1.0										
	- Description   : función para obtener el esquema con sus campos de homologación
\----------------------------------------------------------------------------------------*/
CREATE     FUNCTION [dbo].[fnEsquema] ( @IdEsquema	INT )  
--| 2K25.FEB.05 | patricio.paccha | BUSCADOR ANDINO | Versión: 1.0 
--| Descripción: Función para obtener el esquema con sus campos de homologación
RETURNS TABLE AS
RETURN
	SELECT   IdEsquema
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
											WHERE	IdEsquema = @IdEsquema
										))	WITH	(IdHomologacion INT '$.IdHomologacion')
				) HE	ON HE.IdHomologacion = H.IdHomologacion
				WHERE	H.Estado = 'A'
				and  h.Mostrar = 'S'
				FOR JSON AUTO
			)	EsquemaJson
	FROM	Esquema (NOLOCK)
	WHERE	IdEsquema = @IdEsquema
	AND		Estado = 'A';
