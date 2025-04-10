USE [CAN]
GO
/****** Object:  StoredProcedure [dbo].[paActualizaFiltro]    Script Date: 21/3/2025 23:17:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER   procedure [dbo].[paActualizaFiltro] AS
BEGIN

-- Actualizar Esquemadata con los valores de la consulta
--select * from  EsquemaData where IdEsquemaVista in( 1,11) --78 padre -- 79 hijo principal
--select * from  EsquemaData where IdEsquemaVista in( 2,12) --79 padre -- 109 hijo principal
--select * from  EsquemaData where IdEsquemaVista in( 3,13) --79 padre -- 99 hijo principal
--select * from  EsquemaData where IdEsquemaVista in( 4,14) --79 padre -- 67 hijo principal
--select * from  EsquemaData where IdEsquemaVista in( 5,15) --79 padre -- 57 hijo principal

UPDATE dbo.EsquemaData
SET DataEsquemaJson = replace(DataEsquemaJson,':null',':""');

UPDATE dbo.EsquemaFullText
SET FullTextData = dbo.fnDropSpacesTabs(FullTextData);
 
----
UPDATE e
SET 
    e.VistaFK = c.Padre,
    e.VistaPK = c.Hija
FROM Esquemadata e (nolock)
JOIN (
    -- Consulta combinada para obtener los valores de hija y padre
    SELECT 
        hija.IdEsquemaData,
        padre AS Padre,
        hija AS Hija
    FROM 
        (SELECT 
            idesquemadata AS IdEsquemaData,
            Data AS Hija
         FROM Esquemadata (nolock)
         CROSS APPLY OPENJSON(DataEsquemaJson) 
         WITH (
             IdHomologacion INT '$.IdHomologacion',
             Data NVARCHAR(MAX) '$.Data'
         )
         WHERE IdHomologacion IN (79)   and IdEsquemaVista in (  1,6,11,17)) hija
    INNER JOIN 
        (SELECT 
            idesquemadata AS IdEsquemaData,
            Data AS Padre
         FROM Esquemadata (nolock)
         CROSS APPLY OPENJSON(DataEsquemaJson) 
         WITH (
             IdHomologacion INT '$.IdHomologacion',
             Data NVARCHAR(MAX) '$.Data'
         )
         WHERE IdHomologacion IN (82)  and  IdEsquemaVista in( 1,6,11,17) ) padre
    ON padre.IdEsquemaData = hija.IdEsquemaData
) c
ON e.IdEsquemaData = c.IdEsquemaData;

----
UPDATE e
SET 
    e.VistaFK = c.Padre,
    e.VistaPK = c.Hija
FROM Esquemadata e (nolock)
JOIN (
    -- Consulta combinada para obtener los valores de hija y padre
    SELECT 
        hija.IdEsquemaData,
        padre AS Padre,
        hija AS Hija
    FROM 
        (SELECT 
            idesquemadata AS IdEsquemaData,
            Data AS Hija
         FROM Esquemadata (nolock)
         CROSS APPLY OPENJSON(DataEsquemaJson) 
         WITH (
             IdHomologacion INT '$.IdHomologacion',
             Data NVARCHAR(MAX) '$.Data'
         )
         WHERE IdHomologacion IN (109)   and IdEsquemaVista in (  2,7,12,18)) hija
    INNER JOIN 
        (SELECT 
            idesquemadata AS IdEsquemaData,
            Data AS Padre
         FROM Esquemadata (nolock)
         CROSS APPLY OPENJSON(DataEsquemaJson) 
         WITH (
             IdHomologacion INT '$.IdHomologacion',
             Data NVARCHAR(MAX) '$.Data'
         )
         WHERE IdHomologacion IN (79)  and  IdEsquemaVista in( 2,7,12,18) ) padre
    ON padre.IdEsquemaData = hija.IdEsquemaData
) c
ON e.IdEsquemaData = c.IdEsquemaData;

-- 
UPDATE e
SET 
    e.VistaFK = c.Padre,
    e.VistaPK = c.Hija
FROM Esquemadata e (nolock)
JOIN (
    -- Consulta combinada para obtener los valores de hija y padre
    SELECT 
        hija.IdEsquemaData,
        padre AS Padre,
        hija AS Hija
    FROM 
        (SELECT 
            idesquemadata AS IdEsquemaData,
            Data AS Hija
         FROM Esquemadata (nolock)
         CROSS APPLY OPENJSON(DataEsquemaJson) 
         WITH (
             IdHomologacion INT '$.IdHomologacion',
             Data NVARCHAR(MAX) '$.Data'
         )
         WHERE IdHomologacion IN (99)   and IdEsquemaVista in (  3,8,13,19)) hija
    INNER JOIN 
        (SELECT 
            idesquemadata AS IdEsquemaData,
            Data AS Padre
         FROM Esquemadata (nolock)
         CROSS APPLY OPENJSON(DataEsquemaJson) 
         WITH (
             IdHomologacion INT '$.IdHomologacion',
             Data NVARCHAR(MAX) '$.Data'
         )
         WHERE IdHomologacion IN (79)  and  IdEsquemaVista in( 3,8,13,19) ) padre
    ON padre.IdEsquemaData = hija.IdEsquemaData
) c
ON e.IdEsquemaData = c.IdEsquemaData;

----
UPDATE e
SET 
    e.VistaFK = c.Padre,
    e.VistaPK = c.Hija
FROM Esquemadata e (nolock)
JOIN (
    -- Consulta combinada para obtener los valores de hija y padre
    SELECT 
        hija.IdEsquemaData,
        padre AS Padre,
        hija AS Hija
    FROM 
        (SELECT 
            idesquemadata AS IdEsquemaData,
            Data AS Hija
         FROM Esquemadata (nolock)
         CROSS APPLY OPENJSON(DataEsquemaJson) 
         WITH (
             IdHomologacion INT '$.IdHomologacion',
             Data NVARCHAR(MAX) '$.Data'
         )
         WHERE IdHomologacion IN (67)   and IdEsquemaVista in (  4,9,14,20)) hija
    INNER JOIN 
        (SELECT 
            idesquemadata AS IdEsquemaData,
            Data AS Padre
         FROM Esquemadata (nolock)
         CROSS APPLY OPENJSON(DataEsquemaJson) 
         WITH (
             IdHomologacion INT '$.IdHomologacion',
             Data NVARCHAR(MAX) '$.Data'
         )
         WHERE IdHomologacion IN (79)  and  IdEsquemaVista in( 4,9,14,20) ) padre
    ON padre.IdEsquemaData = hija.IdEsquemaData
) c
ON e.IdEsquemaData = c.IdEsquemaData;

-- 

UPDATE e
SET 
    e.VistaFK = c.Padre,
    e.VistaPK = c.Hija
FROM Esquemadata e (nolock)
JOIN (
    -- Consulta combinada para obtener los valores de hija y padre
    SELECT 
        hija.IdEsquemaData,
        padre AS Padre,
        hija AS Hija
    FROM 
        (SELECT 
            idesquemadata AS IdEsquemaData,
            Data AS Hija
         FROM Esquemadata (nolock)
         CROSS APPLY OPENJSON(DataEsquemaJson) 
         WITH (
             IdHomologacion INT '$.IdHomologacion',
             Data NVARCHAR(MAX) '$.Data'
         )
         WHERE IdHomologacion IN (57)   and IdEsquemaVista in ( 5,10,15,21)) hija
    INNER JOIN 
        (SELECT 
            idesquemadata AS IdEsquemaData,
            Data AS Padre
         FROM Esquemadata (nolock)
         CROSS APPLY OPENJSON(DataEsquemaJson) 
         WITH (
             IdHomologacion INT '$.IdHomologacion',
             Data NVARCHAR(MAX) '$.Data'
         )
         WHERE IdHomologacion IN (79)  and  IdEsquemaVista in( 5,10,15,21) ) padre
    ON padre.IdEsquemaData = hija.IdEsquemaData
) c
ON e.IdEsquemaData = c.IdEsquemaData;

--- los nuevos
UPDATE e
SET 
    e.VistaFK = c.Padre,
    e.VistaPK = c.Hija
FROM Esquemadata e (nolock)
JOIN (
    (SELECT 
            idesquemadata AS IdEsquemaData,
            Data AS padre, 
			'' as hija
         FROM Esquemadata (nolock)
         CROSS APPLY OPENJSON(DataEsquemaJson) 
         WITH (
             IdHomologacion INT '$.IdHomologacion',
             Data NVARCHAR(MAX) '$.Data'
         )
         WHERE IdHomologacion IN (79)   and IdEsquemaVista in ( select idesquemavista from esquemadata 
		where VistaFK ='' and IdEsquemaVista in (  select distinct idesquemavista from esquemavista v, esquema q
		 where v.IdEsquema= q.IdEsquema and v.IdEsquema <>1 and q.Estado='A')))) c
ON e.IdEsquemaData = c.IdEsquemaData;


	print '[paActualizaFiltro]: Inicia limpieza en EsquemaFullText'
	DELETE [EsquemaFullText] 
	WHERE	FullTextData IS NULL
	OR		FullTextData IN ('','-',' ')
	--OR		ISNUMERIC(FullTextData) = 1;
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


	print 'ALTER INDEX ALL ON esquemadata, esquemafulltext, esquemaorganiza '
	ALTER INDEX ALL ON esquemadata		REBUILD;
	ALTER INDEX ALL ON esquemafulltext	REBUILD;
	ALTER INDEX ALL ON esquemaorganiza 	REBUILD;

END;
