CREATE VIEW [dbo].[vwDimension] AS 
	SELECT   hi.IdHomologacion
			,hi.NombreHomologado
			,hi.MostrarWeb
			,hi.TooltipWeb
			,hi.MostrarWebOrden
			,hi.MascaraDato
			,hi.SiNoHayDato
			,hi.NombreHomologado + ' / ' + hi.MostrarWeb AS CustomMostrarWeb
	FROM    Homologacion (NOLOCK) hi
	JOIN    Homologacion (NOLOCK) hp on hi.IdHomologacionGrupo = hp.IdHomologacion
	WHERE	hp.CodigoHomologacion =  'KEY_DIM_ESQUEMA'
	AND		hp.Estado =  'A'
	AND		hi.Estado =  'A'
