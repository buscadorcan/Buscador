CREATE   FUNCTION [dbo].[fnEsquemaTodo] (@VistaFK VARCHAR(200), @IdONA INT)
RETURNS TABLE AS RETURN
	SELECT  DISTINCT 
			 e.IdEsquema
			,e.MostrarWebOrden
			,e.MostrarWeb
			,e.TooltipWeb
			,e.EsquemaVista
			,e.EsquemaJson
			,e.Estado
	FROM	Esquema			e(NOLOCK)
	JOIN	EsquemaVista	v(NOLOCK) ON e.IdEsquema	   = v.IdEsquema 
	JOIN	EsquemaData		d(NOLOCK) ON d.IdEsquemaVista   = v.IdEsquemaVista
	WHERE	d.VistaFK	  in (	SELECT  VistaPK
								FROM	EsquemaVista	v(NOLOCK)  
								JOIN	EsquemaData		d(NOLOCK) ON d.IdEsquemaVista   = v.IdEsquemaVista
								WHERE	VistaFK			= @VistaFK
								AND		v.IdONA			= @IdONA
							)
	AND		e.MostrarWebOrden > 1
	AND		v.IdONA	 = @IdONA
	AND		e.Estado = 'A'
	AND		v.Estado = 'A';
