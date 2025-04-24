 
CREATE     FUNCTION [dbo].[fnEsquemaDato] (  @IdEsquema INT, @VistaFK VARCHAR(200), @IdONA INT)
RETURNS TABLE AS RETURN
	SELECT  DISTINCT 
			 d.IdEsquemaData
			,v.IdEsquema
			,d.DataEsquemaJson
	FROM	EsquemaVista	v(NOLOCK)  
	JOIN	EsquemaData		d(NOLOCK) ON d.IdEsquemaVista   = v.IdEsquemaVista
	WHERE	d.VistaFK	  in(	SELECT  VistaPK
								FROM	EsquemaVista	v(NOLOCK)  
								JOIN	EsquemaData		d(NOLOCK) ON d.IdEsquemaVista   = v.IdEsquemaVista
								WHERE	VistaFK			= @VistaFK
								AND		v.IdONA			= @IdONA
							)
	AND		v.IdEsquema		= @IdEsquema
	AND		v.IdONA			= @IdONA;
