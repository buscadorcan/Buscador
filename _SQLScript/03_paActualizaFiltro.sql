CREATE OR ALTER procedure [dbo].[paActualizaFiltro] AS
BEGIN
	print '[paActualizaFiltro]: Inicia limpieza en EsquemaFullText'
	DELETE [EsquemaFullText] 
	WHERE	FullTextData IS NULL
	OR		FullTextData IN ('','-',' ')
	OR		ISNUMERIC(FullTextData) = 1;
	print '[paActualizaFiltro]: Finaliza limpieza en EsquemaFullText'

	print '[paActualizaFiltro]: Inicia poblar filtros'
	TRUNCATE TABLE [HomologacionFiltro] ;
	INSERT INTO [HomologacionFiltro]	(
				 IdHomologacionFiltro
				,MostrarWeb
				,CodigoHomologacionFil	)
	SELECT  DISTINCT
			 v.IdHomologacion
			,CASE	WHEN	v.CodigoHomologacion IN ( 'KEY_FIL_NOR' ,'KEY_FIL_ONA')	THEN	UPPER(TRIM(FullTextData))
					ELSE	(SELECT STRING_AGG(UPPER(LEFT(value, 1)) + LOWER(SUBSTRING(value, 2, LEN(value))), ' ') FROM STRING_SPLIT(FullTextData, ' '))
			 END
			,v.CodigoHomologacion   
	FROM	vwFiltro			v
	JOIN	Homologacion		h(NOLOCK)	ON h.IdHomologacionFiltro = v.IdHomologacion
	JOIN	EsquemaFullText		e(NOLOCK)	ON e.IdHomologacion		  = h.IdHomologacion
	WHERE	FullTextData		IS NOT NULL
	order by v.CodigoHomologacion
	print '[paActualizaFiltro]: Finaliza poblar filtros'
	
	print '[paActualizaFiltro]: Inicia poblar [EsquemaOrganiza]'
	DECLARE	 
	 @OrgNombreComercial			INT =  81
	,@OrgRazonSocial				INT =  82
	,@OrgPais						INT =  84
	,@OrgCiudad						INT =  85
	,@OrgDireccion					INT =  86
	,@OrgTelefono					INT =  89
	,@OrgWeb						INT =  87
	,@OrgCorreo						INT =  88
	,@OrgEstadoAcreditado			INT =  90
	,@OrgEsquemaAcreditado			INT =  91
	,@OrgNormaAcreditada			INT =  92
	,@OrgReconocimiento				INT =  93
	,@OrgFechaEfectivaAcreditacion	INT =  94
	,@OrgPeriodoVigencia			INT =  95
	,@OrgFechaActualizacion			INT =  96
	,@OrgCodigoAcreditacion			INT =  83
	,@OrgUrlCertificado				INT =  97
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
				,ONAPais	
				,ONAUrlIcono
				-- ORG
				,ONASiglas						
				,OrgNombreComercial				
				,OrgRazonSocial					
				,OrgPais						
				,OrgCiudad						
				,OrgDireccion					
				,OrgTelefono					
				,OrgWeb							
				,OrgCorreo						
				,OrgEstadoAcreditado			
				,OrgEsquemaAcreditado			
				,OrgNormaAcreditada				
				,OrgReconocimiento				
				,OrgFechaEfectivaAcreditacion	
				,OrgPeriodoVigencia				
				,OrgFechaActualizacion			
				,OrgCodigoAcreditacion			
				,OrgUrlCertificado				
	)	SELECT  DISTINCT	
				-- ESQ-ORG
				 case when e.MostrarWebOrden = 1 then d.VistapK else VistaFK end  PK
				,d.IdEsquemaData     
				,d.IdEsquemaVista     
				,d.VistaFK            
				,d.VistaPK            
				,d.DataEsquemaJson    
				,e.IdEsquema	
				,o.IdONA		ONAIdONA			
				,h.MostrarWeb	ONAPais	
				,o.UrlIcono		ONAUrlIcono
				-- ORG
				,o.Siglas		ONASiglas
				,(	SELECT  Data 
					FROM	OPENJSON(d.dataEsquemaJson) 
					WITH	( IdHomologacion INT,  Data NVARCHAR(100) ) 
					WHERE   IdHomologacion =  @OrgNombreComercial
				 ) 
				 ,(	SELECT  Data 
					FROM	OPENJSON(d.dataEsquemaJson) 
					WITH	( IdHomologacion INT,  Data NVARCHAR(100) ) 
					WHERE   IdHomologacion =  @OrgRazonSocial
				 ) 
				 ,(	SELECT  Data 
					FROM	OPENJSON(d.dataEsquemaJson) 
					WITH	( IdHomologacion INT,  Data NVARCHAR(100) ) 
					WHERE   IdHomologacion =  @OrgPais
				 ) 
				 ,(	SELECT  Data 
					FROM	OPENJSON(d.dataEsquemaJson) 
					WITH	( IdHomologacion INT,  Data NVARCHAR(100) ) 
					WHERE   IdHomologacion =  @OrgCiudad
				 ) 
				 ,(	SELECT  Data 
					FROM	OPENJSON(d.dataEsquemaJson) 
					WITH	( IdHomologacion INT,  Data NVARCHAR(100) ) 
					WHERE   IdHomologacion =  @OrgDireccion
				 ) 
				 ,(	SELECT  Data 
					FROM	OPENJSON(d.dataEsquemaJson) 
					WITH	( IdHomologacion INT,  Data NVARCHAR(100) ) 
					WHERE   IdHomologacion =  @OrgTelefono
				 ) 
				 ,(	SELECT  Data 
					FROM	OPENJSON(d.dataEsquemaJson) 
					WITH	( IdHomologacion INT,  Data NVARCHAR(100) ) 
					WHERE   IdHomologacion =  @OrgWeb
				 ) 
				 ,(	SELECT  Data 
					FROM	OPENJSON(d.dataEsquemaJson) 
					WITH	( IdHomologacion INT,  Data NVARCHAR(100) ) 
					WHERE   IdHomologacion =  @OrgCorreo
				 ) 
				 ,(	SELECT  Data 
					FROM	OPENJSON(d.dataEsquemaJson) 
					WITH	( IdHomologacion INT,  Data NVARCHAR(100) ) 
					WHERE   IdHomologacion =  @OrgEstadoAcreditado
				 ) 
				 ,(	SELECT  Data 
					FROM	OPENJSON(d.dataEsquemaJson) 
					WITH	( IdHomologacion INT,  Data NVARCHAR(100) ) 
					WHERE   IdHomologacion =  @OrgEsquemaAcreditado
				 ) 
				 ,(	SELECT  Data 
					FROM	OPENJSON(d.dataEsquemaJson) 
					WITH	( IdHomologacion INT,  Data NVARCHAR(100) ) 
					WHERE   IdHomologacion =  @OrgNormaAcreditada
				 ) 
				 ,(	SELECT  Data 
					FROM	OPENJSON(d.dataEsquemaJson) 
					WITH	( IdHomologacion INT,  Data NVARCHAR(100) ) 
					WHERE   IdHomologacion =  @OrgReconocimiento
				 ) 
				 ,(	SELECT  Data 
					FROM	OPENJSON(d.dataEsquemaJson) 
					WITH	( IdHomologacion INT,  Data NVARCHAR(100) ) 
					WHERE   IdHomologacion =  @OrgFechaEfectivaAcreditacion
				 ) 
				 ,(	SELECT  Data 
					FROM	OPENJSON(d.dataEsquemaJson) 
					WITH	( IdHomologacion INT,  Data NVARCHAR(100) ) 
					WHERE   IdHomologacion =  @OrgPeriodoVigencia
				 ) 
				 ,(	SELECT  Data 
					FROM	OPENJSON(d.dataEsquemaJson) 
					WITH	( IdHomologacion INT,  Data NVARCHAR(100) ) 
					WHERE   IdHomologacion =  @OrgFechaActualizacion
				 ) 
				 ,(	SELECT  Data 
					FROM	OPENJSON(d.dataEsquemaJson) 
					WITH	( IdHomologacion INT,  Data NVARCHAR(100) ) 
					WHERE   IdHomologacion =  @OrgCodigoAcreditacion
				 ) 
				 ,(	SELECT  Data 
					FROM	OPENJSON(d.dataEsquemaJson) 
					WITH	( IdHomologacion INT,  Data NVARCHAR(100) ) 
					WHERE   IdHomologacion =  @OrgUrlCertificado
				 ) 
	FROM	EsquemaData		d(NOLOCK)	
	JOIN	EsquemaVista	v(NOLOCK)ON	v.IdEsquemaVista= d.IdEsquemaVista
	JOIN	Esquema			e(NOLOCK)ON	e.IdEsquema		= v.IdEsquema
	JOIN	ONA				o(NOLOCK)ON	o.IdONA			= v.IdONA
	JOIN	Homologacion	h(NOLOCK)ON	h.IdHomologacion= o.IdHomologacionPais
	where	e.MostrarWebOrden  = 1;
	print '[paActualizaFiltro]: Finaliza poblar [EsquemaOrganiza]'
END;


