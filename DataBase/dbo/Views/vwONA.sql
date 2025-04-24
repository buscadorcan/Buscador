
 CREATE   VIEW [dbo].[vwONA] AS
	SELECT	 IdONA
			,H.MostrarWeb Pais
			,RazonSocial
			,Siglas
			,Ciudad
			,Correo
			,Direccion
			,PaginaWeb
			,Telefono
			,UrlIcono
			,UrlLogo
	FROM	ONA			 O 
	JOIN	Homologacion H ON O.IdHomologacionPais = H.IdHomologacion
	WHERE	H.Estado	 ='A' and o.estado = 'A'
