CREATE OR ALTER procedure [dbo].[paActualizaFiltro] AS
BEGIN
	--> actualiza filtros
	TRUNCATE TABLE [HomologacionFiltro];
	INSERT INTO [HomologacionFiltro]	(
				 IdHomologacionFiltro
				,MostrarWeb
				,CodigoHomologacionFil	)
	SELECT  DISTINCT v.IdHomologacion, UPPER(trim(FullTextData)),  V.CodigoHomologacion
	FROM	vwFiltro			v
	JOIN	Homologacion		h(NOLOCK)	ON h.IdHomologacionFiltro = v.IdHomologacion
	JOIN	EsquemaFullText		e(NOLOCK)	ON e.IdHomologacion		  = h.IdHomologacion
	WHERE	FullTextData		IS NOT NULL
	order by v.CodigoHomologacion
	
	--> actualiza organizacion
	DECLARE	 @IdHomologacionRazonSocial INT = 0
			,@IdHomologacionNombre		INT = 82
			,@IdHomologacionPaisORG		INT = 0
	TRUNCATE TABLE [EsquemaOrganiza];
	INSERT INTO [EsquemaOrganiza]	(
				 PK					
				,IdEsquemaData		
				,IdEsquemaVista     
				,VistaFK            
				,VistaPK            
				,DataEsquemaJson    
				,IdEsquema			
				,ONAIdONA				
				,ONASiglas			
				,ONAPais			
				,ORGPais			
				,ORGRazonSocial		
				,ORGNombre			)
	SELECT  DISTINCT	
				 case when e.MostrarWebOrden = 1 then d.VistapK else VistaFK end
				,d.IdEsquemaData     
				,d.IdEsquemaVista     
				,d.VistaFK            
				,d.VistaPK            
				,d.DataEsquemaJson    
				,e.IdEsquema	
				,o.IdONA		ONAIdONA			
				,o.Siglas		ONASiglas
				,h.MostrarWeb	ONAPais			
				,(	SELECT  Data 
					FROM	OPENJSON(d.dataEsquemaJson) 
					WITH	( IdHomologacion INT,  Data NVARCHAR(100) ) 
					WHERE   IdHomologacion = @IdHomologacionPaisORG
				 ) AS 		ORGPais			
				,(	SELECT  Data 
					FROM	OPENJSON(d.dataEsquemaJson) 
					WITH	( IdHomologacion INT,  Data NVARCHAR(100) ) 
					WHERE   IdHomologacion = @IdHomologacionRazonSocial
				 ) AS 		ORGRazonSocial
				,(	SELECT  Data 
					FROM	OPENJSON(d.dataEsquemaJson) 
					WITH	( IdHomologacion INT,  Data NVARCHAR(100) ) 
					WHERE   IdHomologacion = @IdHomologacionNombre
				 ) AS 		ORGNombre
	FROM	EsquemaData		d(NOLOCK)	
	JOIN	EsquemaVista	v(NOLOCK)ON	v.IdEsquemaVista= d.IdEsquemaVista
	JOIN	Esquema			e(NOLOCK)ON	e.IdEsquema		= v.IdEsquema
	JOIN	ONA				o(NOLOCK)ON	o.IdONA			= v.IdONA
	JOIN	Homologacion	h(NOLOCK)ON	h.IdHomologacion= o.IdHomologacionPais
	where	e.MostrarWebOrden  = 1;
END;
