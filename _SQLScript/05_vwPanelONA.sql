CREATE OR ALTER VIEW [dbo].[vwPanelONA] AS
SELECT DISTINCT	 o.Siglas				Sigla 
				,h.MostrarWeb			Pais 
				,isnull(o.UrlIcono,'')	Icono 
				,count(e.IdEsquemaData) NroOrg
FROM  EsquemaOrganiza	e(NOLOCK) 
RIGHT JOIN ONA			o(NOLOCK) ON e.ONAIdONA		 = o.IdONA
INNER JOIN Homologacion	h(NOLOCK) ON h.IdHomologacion=o.IdHomologacionPais
GROUP BY o.Siglas, h.MostrarWeb, isnull(o.UrlIcono,'')
GO