/*----------------------------------------------------------------------------------------\
|    �Copyright 2K25					                          BUSCADOR ANDINO		  |
|-----------------------------------------------------------------------------------------|
| Este c�digo est� protegido por las leyes y tratados internacionales de derechos de autor|
\-----------------------------------------------------------------------------------------/
  [App]				: Buscador Andino											
	- Date          : 2K25.FEB.06	
	- Author        : patricio.paccha														
	- Description   : funci�n para mostrar el esquema con sus datos buscados
\----------------------------------------------------------------------------------------*/
CREATE OR ALTER FUNCTION [dbo].[fnEsquemaDatoBuscado] (@IdEsquemaData INT,  @VistaPK VARCHAR(200),  @TextoBuscar VARCHAR(200))
--| 2K25.FEB.25 | patricio.paccha | BUSCADOR ANDINO | Versi�n: 1.0
--| Descripci�n: Funci�n para mostrar el esquema con sus datos buscados
RETURNS TABLE AS RETURN
	WITH EsquemaVista_	AS
	(	
		SELECT  DISTINCT IdEsquema
		FROM	EsquemaVista	v(NOLOCK)  
		JOIN	EsquemaData		d(NOLOCK) ON d.IdEsquemaVista   = v.IdEsquemaVista
		WHERE	d.IdEsquemaData = @IdEsquemaData 
		AND		v.Estado		= 'A'
	)
	SELECT  DISTINCT 
			 IdEsquemaData
			,IdEsquema
			,DataEsquemaJson
	FROM	EsquemaVista	v(NOLOCK)  
	JOIN	EsquemaData		d(NOLOCK) ON d.IdEsquemaVista   = v.IdEsquemaVista
	AND		v.Estado		= 'A'
	AND		v.IdEsquema		= (SELECT IdEsquema FROM EsquemaVista_ )
	AND		VistaFK			= @VistaPK
	AND		d.DataEsquemaJson LIKE '%'+@TextoBuscar+'%' --> Mejorar para frases
GO