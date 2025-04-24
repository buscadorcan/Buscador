CREATE VIEW [dbo].[vwHomologacionGrupo] AS
SELECT	 IdHomologacion
		,IdHomologacionGrupo
		,MostrarWeb
		,TooltipWeb
		,MostrarWebOrden
		,CodigoHomologacion
		,Estado
FROM	Homologacion (NOLOCK)
WHERE	IdHomologacionGrupo IS NULL
AND		Estado = 'A'

