/*----------------------------------------------------------------------------------------\
|    �Copyright 2K25					                          BUSCADOR ANDINO		  |
|-----------------------------------------------------------------------------------------|
| Este c�digo est� protegido por las leyes y tratados internacionales de derechos de autor|
\-----------------------------------------------------------------------------------------/
  [App]				: Buscador Andino											
	- Date          : 2K25.FEB.25	
	- Author        : patricio.paccha														
	- Version	    : 1.0										
	- Description   : funci�n para obtener el esquema con sus campos de homologaci�n
\----------------------------------------------------------------------------------------*/
CREATE OR ALTER   FUNCTION [dbo].[fnEsquema] ( @IdEsquema	INT )  
--| 2K25.FEB.05 | patricio.paccha | BUSCADOR ANDINO | Versi�n: 1.0 
--| Descripci�n: Funci�n para obtener el esquema con sus campos de homologaci�n
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
				WHERE	H.Mostrar = 'S'
				AND		H.Estado = 'A'
				FOR JSON AUTO
			)	EsquemaJson
	FROM	Esquema (NOLOCK)
	WHERE	IdEsquema = @IdEsquema
	AND		Estado = 'A';
GO