
CREATE view [dbo].[vwEsquemaFull] as
SELECT	 ev.IdONA
		,ev.IdEsquema
		,ev.IdEsquemaVista
		,ed.IdEsquemaData
		,ef.IdEsquemaFullText
		,ed.VistaPK
		,ed.VistaFK
		,ef.IdHomologacion
		,ef.FullTextData
FROM	EsquemaFullText	(NOLOCK)ef
JOIN	EsquemaData		(NOLOCK)ed	on ef.IdEsquemaData		= ed.IdEsquemaData
JOIN	EsquemaVista	(NOLOCK)ev	on ed.IdEsquemaVista	= ed.IdEsquemaVista
WHERE	ev.Estado	=	'A'
