--/*----------------------------------------------------------------------------------------\
--|    ©Copyright 2K25					                          BUSCADOR ANDINO		  |
--|-----------------------------------------------------------------------------------------|
--| Este código está protegido por las leyes y tratados internacionales de derechos de autor|
--\-----------------------------------------------------------------------------------------/
--  [App]				: Buscador Andino											
--	- Date          : 2K25.FEB.06	
--	- Author        : patricio.paccha														
--	- Description   : función para mostrar el esquema con sus datos buscados
--\----------------------------------------------------------------------------------------*/
CREATE OR ALTER FUNCTION [dbo].[fnEsquemaDatoBuscado] (@IdEsquemaData INT, @TextoBuscar VARCHAR(200))
----| 2K25.FEB.25 | patricio.paccha | BUSCADOR ANDINO | Versión: 1.0
----| Descripción: Función para mostrar el esquema con sus datos buscados
RETURNS TABLE AS RETURN
	WITH EsquemaOrganiza_	AS
	(	
		SELECT	VistaFK, ONAIdONA, OrgEsquemaAcreditado
		FROM	EsquemaOrganiza	(NOLOCK)  
		WHERE	IdEsquemaData = @IdEsquemaData
	)
	SELECT  DISTINCT 
			 o.IdEsquemaData
			,0		IdEsquema
			,DataEsquemaJson
	FROM	EsquemaOrganiza_	b
	JOIN	EsquemaOrganiza		o(NOLOCK)ON o.VistaFK		= b.VistaFK
	JOIN	EsquemaFullText		e(NOLOCK)ON o.IdEsquemaData = e.IdEsquemaData
	WHERE	o.OrgEsquemaAcreditado = b.OrgEsquemaAcreditado
	AND		o.ONAIdONA		= b.ONAIdONA
	AND		e.FullTextData	LIKE '%'+@TextoBuscar+'%' --> Mejorar para frases
GO

