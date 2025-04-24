
CREATE   VIEW [dbo].[vwHomologacion] AS
	SELECT	 hi.IdHomologacion
			,hi.IdHomologacionGrupo
			,hi.Indexar
			,hi.MostrarWeb
			,hi.TooltipWeb
			,hi.MostrarWebOrden
			,hi.MascaraDato
			,hi.SiNoHayDato
			,hi.NombreHomologado
			,hi.CodigoHomologacion
			,hp.CodigoHomologacion	AS CodigoHomologacionKEY
			,hi.NombreHomologado + ' / ' + hi.MostrarWeb AS CustomMostrarWeb
	FROM    Homologacion (NOLOCK) hi
	JOIN    Homologacion (NOLOCK) hp on hi.IdHomologacionGrupo = hp.IdHomologacion
	WHERE	hp.Estado =  'A'
	AND		hi.Estado =  'A' 
