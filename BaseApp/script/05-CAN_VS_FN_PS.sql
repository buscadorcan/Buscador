/*----------------------------------------------------------------------------------------\
|    ©Copyright 2K24												BUSCADOR ANDINO		  |
|-----------------------------------------------------------------------------------------|
| Este código está protegido por las leyes y tratados internacionales de derechos de autor|
\-----------------------------------------------------------------------------------------/
  [App]            : Buscador Andino											
	- Date         : 2K24.JUN.25	
	- Author       : patricio.paccha														
	- Version	   : 1.0										
	- Description  : vistas para la busqueda de organizaciones certificadas
\----------------------------------------------------------------------------------------*/

USE CAN_DB;
GO

EXEC DBO.Bitacora '@script','05-CAN_VS_FN_PS.sql'
GO

CREATE OR ALTER VIEW vwFiltro AS 
	SELECT	 IdHomologacion
			,MostrarWeb		
			,TooltipWeb	
			,MostrarWebOrden
	FROM	Homologacion		WITH(NOLOCK)
	WHERE	CodigoHomologacion	IN
	( -- codigo 		EtiquetaColumna
	 'KEY_DIM_PAI_FIL'  -- 'Pais origen'               ,1    'filtro 1')
	,'KEY_DIM_ORG_FIL'  -- 'Organismo de Acreditación' ,2    'filtro 2')
	,'KEY_DIM_ESQ_FIL'  -- 'Esquena de Acreditación'   ,3    'filtro 3')
	,'KEY_DIM_NOR_FIL'  -- 'Norma Acreditada'          ,4    'filtro 4')
	,'KEY_DIM_EST_FIL'  -- 'Estado'                    ,5    'filtro 5')
	,'KEY_DIM_REC_FIL'  -- 'Reconocimiento'            ,6    'filtro 6')
	)
GO

CREATE OR ALTER VIEW vwDimension AS 
	SELECT   H.IdHomologacion
			,H.NombreHomologado
			,H.MostrarWeb
			,H.TooltipWeb
			,H.MostrarWebOrden
			,H.MascaraDato
			,H.SiNoHayDato
			,H.NombreHomologado + ' / ' + H.MostrarWeb AS CustomMostrarWeb
	FROM    Homologacion H		WITH (NOLOCK)
	JOIN	(	SELECT DISTINCT IdHomologacion
				FROM	Homologacion		WITH (NOLOCK)
				WHERE	CodigoHomologacion  NOT IN ('KEY_DIM_PAI_FIL', 'KEY_DIM_ORG_FIL')
			)   HG	ON H.IdHomologacionGrupo = HG.IdHomologacion
GO

CREATE OR ALTER VIEW vwGrilla AS 
	SELECT	 H.IdHomologacion
			,H.MostrarWeb
			,H.TooltipWeb
			,H.MostrarWebOrden
	FROM	Homologacion	H	WITH (NOLOCK)
	JOIN	(	SELECT DISTINCT IdHomologacion
				FROM  OPENJSON((	SELECT TOP 1 EsquemaJson
									FROM HomologacionEsquema WITH (NOLOCK)
									ORDER BY MostrarWebOrden
				))
				WITH (IdHomologacion INT '$.IdHomologacion')
			)	HE	ON HE.IdHomologacion = H.IdHomologacion
GO

CREATE OR ALTER FUNCTION fnFiltroDetalle (	@IdHomologacionGrupo	INT )  
RETURNS TABLE AS
RETURN
	SELECT	IdHomologacion,     MostrarWeb,     TooltipWeb,	MostrarWebOrden
	FROM	dbo.Homologacion    WITH (NOLOCK)
	WHERE	IdHomologacionGrupo = @IdHomologacionGrupo and Estado = 'A'
GO

CREATE OR ALTER FUNCTION fnHomologacionEsquemaCampo ( @IdHomologacionEsquema INT )  
--RETURNS TABLE AS
--RETURN
RETURNS NVARCHAR(MAX) AS
BEGIN	
	DECLARE @json NVARCHAR(MAX) ='{}';
	SELECT	@json = (	SELECT	 H.IdHomologacion
								,H.MostrarWeb
								,H.TooltipWeb
								,H.MostrarWebOrden
								,H.NombreHomologado
						FROM	Homologacion	H	WITH (NOLOCK)
						JOIN	(	SELECT DISTINCT IdHomologacion
									FROM  OPENJSON((	SELECT	EsquemaJson
														FROM	HomologacionEsquema		WITH (NOLOCK)
														WHERE	IdHomologacionEsquema = @IdHomologacionEsquema
												  ))
									WITH (IdHomologacion INT '$.IdHomologacion')
								)	HE	ON HE.IdHomologacion = H.IdHomologacion
						FOR JSON AUTO 
					);
	RETURN @json;
END;
GO

CREATE OR ALTER FUNCTION fnHomologacionEsquema ( @IdHomologacionEsquema	INT )  
RETURNS TABLE AS
RETURN
	SELECT	 IdHomologacionEsquema	
			,MostrarWebOrden	
			,MostrarWeb	
			,TooltipWeb	
			,(select dbo.fnHomologacionEsquemaCampo(IdHomologacionEsquema)) EsquemaJson
	FROM	HomologacionEsquema		WITH (NOLOCK)
	WHERE	IdHomologacionEsquema = @IdHomologacionEsquema	
	AND		Estado = 'A'
GO

CREATE OR ALTER FUNCTION fnHomologacionEsquemaTodo ( )  
RETURNS TABLE AS
RETURN
	SELECT	 IdHomologacionEsquema	
			,MostrarWebOrden	
			,MostrarWeb	
			,TooltipWeb	
	FROM	HomologacionEsquema		WITH (NOLOCK)
	WHERE	MostrarWebOrden > 1	
	AND		Estado = 'A'
GO

CREATE OR ALTER FUNCTION fnHomologacionEsquemaDato (  @IdHomologacionEsquema INT, @IdDataLakeOrganizacion INT )  
RETURNS TABLE AS
RETURN
	SELECT	 IdDataLakeOrganizacion	
			,IdHomologacionEsquema
			,DataEsquemaJson
	FROM	DataLakeOrganizacion	 WITH (NOLOCK)	
	WHERE	IdDataLakeOrganizacion = @IdDataLakeOrganizacion
	AND		IdHomologacionEsquema  = @IdHomologacionEsquema
	AND		Estado				   = 'A'
GO

CREATE OR ALTER PROCEDURE psBuscarPalabra ( @paramJSON NVARCHAR(max) = NULL , @PageNumber INT = 1, @RowsPerPage INT = 20, @RowsTotal INT = 0 OUTPUT) AS
BEGIN
	IF  (@paramJSON is null)			
		RETURN '{}';
	--	DECLARE @paramJSON NVARCHAR(max) = N'{ "TextoBuscar": "or", "IdHomologacionFiltro":["41","42","44"] }'
	DECLARE @BuscarPalabra			NVARCHAR(200)	= ltrim(rtrim(JSON_VALUE(@paramJSON,'$.TextoBuscar')))
    DECLARE @HomologacionFiltro		TABLE (IdHomologacion INT)
    DECLARE @DataLakeOrgBusqueda	TABLE (IdDataLakeOrganizacion INT)
	 
	IF  (LTRIM(RTRIM(@BuscarPalabra)) = '')	
		RETURN '{}';

	INSERT	INTO @HomologacionFiltro
	SELECT	DISTINCT value  
	FROM	OPENJSON(JSON_QUERY(@paramJSON, '$.IdHomologacionFiltro'))
	
	-->	buscando la palabra
	INSERT	INTO @DataLakeOrgBusqueda (IdDataLakeOrganizacion)
	SELECT	DISTINCT IdDataLakeOrganizacion
	FROM	OrganizacionFullText
	WHERE	FullTextOrganizacion LIKE '%' + @BuscarPalabra + '%'
	AND (	IdHomologacion IN (SELECT IdHomologacion FROM @HomologacionFiltro)
			OR NOT EXISTS (SELECT 1 FROM @HomologacionFiltro)
		)
	--ORDER BY IdDataLakeOrganizacion  
	--OFFSET (@PageNumber - 1) * @RowsPerPage ROWS
	--FETCH NEXT @RowsPerPage ROWS ONLY;

	IF  (@PageNumber = 1)
		SELECT @RowsTotal = COUNT(*) FROM @DataLakeOrgBusqueda

	SELECT  O.IdDataLakeOrganizacion,
			O.IdHomologacionEsquema,
			O.DataEsquemaJson
	FROM DataLakeOrganizacion O WITH (NOLOCK)
	JOIN @DataLakeOrgBusqueda B ON B.IdDataLakeOrganizacion = O.IdDataLakeOrganizacion
	WHERE O.Estado = 'A'
	ORDER BY O.IdDataLakeOrganizacion  
	OFFSET (@PageNumber - 1) * @RowsPerPage ROWS
	FETCH NEXT @RowsPerPage ROWS ONLY;
END;
GO

-- exec psBuscarPalabra N'{ "TextoBuscar": "5",
-- 						 "IdHomologacionFiltro":["41","42","44"]
-- 						}'


EXEC dbo.setDiccionario	'dbo.vwFiltro					', NULL ,'vista para los filtros de la pagina principal'
EXEC dbo.setDiccionario	'dbo.vwDimension				', NULL ,'vista para las dimensiones o campos homologados'
EXEC dbo.setDiccionario	'dbo.vwGrilla					', NULL ,'vita de esquema principal de la busqueda'
EXEC dbo.setDiccionario	'dbo.fnFiltroDetalle			', NULL ,'funcion para traer los detalles de cada filtro'	
EXEC dbo.setDiccionario	'dbo.fnHomologacionEsquemaCampo	', NULL ,'funcion para obtener los esquema y sus campos'				
EXEC dbo.setDiccionario	'dbo.fnHomologacionEsquema		', NULL ,'funcion para obtener un esquema'			
EXEC dbo.setDiccionario	'dbo.fnHomologacionEsquemaTodo	', NULL ,'funcion para obtener todos los esquema'				
EXEC dbo.setDiccionario	'dbo.fnHomologacionEsquemaDato	', NULL ,'funcion para obtener un esquema y sus datos'				
EXEC dbo.setDiccionario	'dbo.psBuscarPalabra			', NULL ,'procedimiento almacenado para la busqueda principal'				
GO

EXEC DBO.Bitacora ' CREATE OR ALTER 
					,dbo.vwFiltro					
					,dbo.vwDimension				
					,dbo.vwGrilla					
					,dbo.fnFiltroDetalle			
					,dbo.fnHomologacionEsquemaCampo	
					,dbo.fnHomologacionEsquema		
					,dbo.fnHomologacionEsquemaTodo	
					,dbo.fnHomologacionEsquemaDato
					,dbo.psBuscarPalabra	'
GO

