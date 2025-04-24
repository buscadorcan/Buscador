CREATE    VIEW [dbo].[vwMenu] AS 
	SELECT	 --hm.IdHomologacion		IdHomologacionMenu
		 	 hm.MostrarWebOrden		MostrarWebOrden
			,hm.MostrarWeb			MostrarWeb
			,hm.TooltipWeb			TooltipWeb
			,ISNULL(JSON_VALUE(hm.InfoExtraJson, '$.icono')		,'user')	Icono
			,ISNULL(JSON_VALUE(hm.InfoExtraJson, '$.referencia'),'/')		href
			,hm.CodigoHomologacion	CodigoHomologacion
            ,hr.CodigoHomologacion	CodigoHomologacionRol
	FROM    MenuRol      mr         (NOLOCK) 
    JOIN    Homologacion hm			(NOLOCK)ON hm.IdHomologacion = mr.IdHMenu
	JOIN	Homologacion hr			(NOLOCK)ON hr.IdHomologacion = mr.IdHRol
	WHERE	mr.Estado = 'A'
	AND		hm.Estado = 'A'
	AND		hr.Estado = 'A'
	AND		hm.CodigoHomologacion is not null;
