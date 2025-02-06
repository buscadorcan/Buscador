/*----------------------------------------------------------------------------------------\
|    �Copyright 2K25					                          BUSCADOR ANDINO		  |
|-----------------------------------------------------------------------------------------|
| Este c�digo est� protegido por las leyes y tratados internacionales de derechos de autor|
\-----------------------------------------------------------------------------------------/
  [App]				: Buscador Andino											
	- Date          : 2K25.FEB.05	
	- Author        : patricio.paccha														
	- Version	    : 1.0										
	- Description   : funci�n para mostrar el esquema con sus datos buscados
\----------------------------------------------------------------------------------------*/

CREATE OR ALTER FUNCTION [dbo].[fnEsquemaDatoBuscado] (@IdONA INT,  @IdEsquema INT, @PK VARCHAR(200),  @TextoBuscar VARCHAR(200))
--| 2K25.FEB.25 | patricio.paccha | BUSCADOR ANDINO | Versi�n: 1.0
--| Descripci�n: Funci�n para mostrar el esquema con sus datos buscados
RETURNS TABLE AS RETURN
	SELECT  DISTINCT 
			 d.IdEsquemaData
			,v.IdEsquema
			,d.DataEsquemaJson
	FROM	EsquemaVista	v(NOLOCK)  
	JOIN	EsquemaData		d(NOLOCK) ON d.IdEsquemaVista   = v.IdEsquemaVista
	AND		v.Estado		= 'A'
	AND		v.IdONA			= @IdONA
	AND		v.IdEsquema		= @IdEsquema
	AND		VistaFK			= @PK
	AND		d.DataEsquemaJson LIKE '%'+@TextoBuscar+'%' --> Mejorar para frases
GO