
CREATE    VIEW [dbo].[vwFiltroDetalle] AS 
SELECT	IdHF IdHomologacion, MostrarWeb, CodigoHomologacionFil CodigoHomologacion
FROM	[HomologacionFiltro] (NOLOCK) 
