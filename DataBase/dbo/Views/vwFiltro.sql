CREATE  VIEW [dbo].[vwFiltro] AS 
SELECT 	IdHomologacion ,MostrarWeb ,TooltipWeb ,MostrarWebOrden, CodigoHomologacion
FROM	Homologacion (NOLOCK) 	
WHERE	Estado  =	 'A'
AND		CodigoHomologacion IN
		('KEY_FIL_PAI'  -- 'PAIS '           'filtro 1')
		,'KEY_FIL_ONA'  -- 'ONA'			 'filtro 2')
		,'KEY_FIL_ESQ'  -- 'ESQUEMA'		 'filtro 3')
		,'KEY_FIL_NOR'  -- 'NORMA'           'filtro 4')
		,'KEY_FIL_EST'  -- 'ESTADO'          'filtro 5')
		,'KEY_FIL_REC'  -- 'RECOMOCIMIENTO'  'filtro 6')
		)
