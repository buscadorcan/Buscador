


CREATE  VIEW [dbo].[vwRol] AS 
	SELECT	 h1.IdHomologacion		IdHomologacionRol
			,h1.MostrarWeb			Rol
			,h1.CodigoHomologacion	CodigoHomologacion
	FROM	Homologacion h1			(NOLOCK)
	JOIN	Homologacion h2			(NOLOCK)	ON h1.IdHomologacionGrupo = h2.IdHomologacion
	WHERE	h2.CodigoHomologacion = 'KEY_ROL'
	AND		h1.Estado = 'A'
	AND		h2.Estado = 'A'
