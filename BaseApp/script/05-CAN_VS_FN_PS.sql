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
	SELECT IdHomologacion ,MostrarWeb ,TooltipWeb ,MostrarWebOrden ,'FiltroPais' NombreFiltro		
	FROM Homologacion (NOLOCK)	WHERE CodigoHomologacion	='KEY_DIM_PAI_FIL'  -- 'PAIS '          ,2    'filtro 1')
	UNION
	SELECT IdHomologacion ,MostrarWeb ,TooltipWeb ,MostrarWebOrden ,'FiltroOna' NombreFiltro		
	FROM Homologacion (NOLOCK) 	WHERE CodigoHomologacion	='KEY_DIM_ORG_FIL'  -- 'ONA'			,3    'filtro 2')
	UNION
	SELECT IdHomologacion ,MostrarWeb ,TooltipWeb ,MostrarWebOrden ,'FiltroEsquema' NombreFiltro	
	FROM Homologacion (NOLOCK) 	WHERE CodigoHomologacion	='KEY_DIM_ESQ_FIL'  -- 'ESQUEMA'		,4    'filtro 3')
	UNION
	SELECT IdHomologacion ,MostrarWeb ,TooltipWeb ,MostrarWebOrden ,'FiltroNorma' NombreFiltro		
	FROM Homologacion (NOLOCK) 	WHERE CodigoHomologacion	='KEY_DIM_NOR_FIL'  -- 'NORMA'          ,5    'filtro 4')
	UNION
	SELECT IdHomologacion ,MostrarWeb ,TooltipWeb ,MostrarWebOrden ,'FiltroEstado' NombreFiltro		
	FROM Homologacion (NOLOCK)	WHERE CodigoHomologacion	='KEY_DIM_EST_FIL'  -- 'ESTADO'         ,6    'filtro 5')
	UNION
	SELECT IdHomologacion ,MostrarWeb ,TooltipWeb ,MostrarWebOrden ,'FiltroRecomocimiento' NombreFiltro 
	FROM Homologacion (NOLOCK) 	WHERE CodigoHomologacion	='KEY_DIM_REC_FIL'  -- 'RECOMOCIMIENTO' ,7    'filtro 6')
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
				FROM	Homologacion		(NOLOCK)
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
	SELECT	DISTINCT upper(FullTextData) 'MostrarWeb'
	FROM	CanFullText  (NOLOCK)			---> and Estado = 'A'
	WHERE	CASE 
			WHEN @IdHomologacionGrupo = 2 THEN CASE WHEN IdHomologacion = 114 THEN 1 ELSE 0 END	-->	PAIS		   ('OrgPais': ecuador, peru ,....)
			WHEN @IdHomologacionGrupo = 3 THEN CASE WHEN IdHomologacion = 104 THEN 1 ELSE 0 END	-->	ONA			   ('OnaAbreviacion': SAE,..)
			WHEN @IdHomologacionGrupo = 4 THEN CASE WHEN IdHomologacion = 121 THEN 1 ELSE 0 END	-->	ESQUEMA		   ('OrgEsquemaAcreditado': CALIBRACIÓN, CLÍNICOS,..)
			WHEN @IdHomologacionGrupo = 5 THEN CASE WHEN IdHomologacion = 122 THEN 1 ELSE 0 END	-->	NORMA		   ('OrgNormaAcreditada': NTE INEN-ISO/IEC17043: ,..)
			WHEN @IdHomologacionGrupo = 6 THEN CASE WHEN IdHomologacion = 120 THEN 1 ELSE 0 END	-->	ESTADO		   ('OrgEstadoAcreditado')
			WHEN @IdHomologacionGrupo = 7 THEN CASE WHEN IdHomologacion = 123 THEN 1 ELSE 0 END	-->	RECOMOCIMIENTO ('OrgReconocimiento': INTERNACIONAL, NACIONAL)
			END = 1;
GO
--> select * from fnFiltroDetalle (4)  

CREATE OR ALTER FUNCTION fnHomologacionEsquemaCampo ( @IdHomologacionEsquema INT )  
--RETURNS TABLE AS
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
														FROM	HomologacionEsquema		(NOLOCK)
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
			,VistaNombre
			,(select dbo.fnHomologacionEsquemaCampo(IdHomologacionEsquema)) EsquemaJson
	FROM	HomologacionEsquema		WITH (NOLOCK)
	WHERE	IdHomologacionEsquema = @IdHomologacionEsquema	
	AND		Estado = 'A'
GO

--select * from fnHomologacionEsquemaTodo ( 368)

CREATE OR ALTER FUNCTION fnHomologacionEsquemaTodo ( @IdEnte VARCHAR(16))  
RETURNS TABLE AS
RETURN
	SELECT	 DISTINCT
			 he.IdHomologacionEsquema	
			,he.MostrarWebOrden	
			,he.MostrarWeb	
			,he.TooltipWeb	
	FROM	HomologacionEsquema		he (NOLOCK)
	JOIN	candataset	dl (NOLOCK)	ON he.IdHomologacionEsquema = dl.IdHomologacionEsquema
	WHERE	he.MostrarWebOrden > 1	
	AND		he.Estado = 'A'
	AND		dl.IdEnte = @IdEnte
GO

--select * from fnHomologacionEsquemaDato ( 5, '368' ) (@IdHomologacionEsquema , @IdEnte )

CREATE OR ALTER FUNCTION fnHomologacionEsquemaDato (  @IdHomologacionEsquema INT, @IdEnte VARCHAR(16) )  
RETURNS TABLE AS
RETURN
	SELECT	 Idcandataset
			,IdHomologacionEsquema
			,DataEsquemaJson
	FROM	candataset	(NOLOCK)	
	WHERE	IdHomologacionEsquema	= @IdHomologacionEsquema
	AND		IdEnte			= @IdEnte
	 
GO
------------------------ fnPredictWords
CREATE OR ALTER FUNCTION fn_PredictWords (@Prefix NVARCHAR(100))
RETURNS @TopWords TABLE (Word NVARCHAR(max))
AS
BEGIN
    INSERT INTO @TopWords (Word)
    SELECT DISTINCT TOP 10 FullTextData
    FROM   CanFullText  (NOLOCK)
    WHERE  FullTextData LIKE '%' + @Prefix + '%'
    RETURN
END
GO
 
CREATE OR ALTER FUNCTION fnDropSpacesTabs (@input NVARCHAR(MAX))	
RETURNS NVARCHAR(MAX)
AS
BEGIN
    DECLARE @result NVARCHAR(MAX);
    
    -- Reemplazar tabulaciones, saltos de línea y retornos de carro por espacios
    SET @result = REPLACE(REPLACE(REPLACE(@input, CHAR(13), ' '), CHAR(10), ' '), CHAR(9), ' ');
    
    -- Eliminar espacios duplicados
    WHILE CHARINDEX('  ', @result) > 0
    BEGIN
        SET @result = REPLACE(@result, '  ', ' ');
    END
    
    -- Retornar el resultado
    RETURN LTRIM(RTRIM(@result));
END;
GO

CREATE OR ALTER PROCEDURE psBuscarPalabra ( @paramJSON NVARCHAR(max) = NULL , @PageNumber INT = 1, @RowsPerPage INT = 20, @RowsTotal INT = 0 OUTPUT) AS
BEGIN 
--N'{	"ExactaBuscar"			:false
--	,"TextoBuscar"			:"leche"
--	,"FiltroPais"			:[]
--	,"FiltroOna"			:[]
--	,"FiltroEsquema"		:[]
--	,"FiltroNorma"			:[]
--	,"FiltroEstado"			:[]
--	,"FiltroRecomocimiento"	:[]
--}',1,10;
	BEGIN TRY	
		SELECT  @RowsTotal				= 0;
		DECLARE @FiltroPais				NVARCHAR(400)	 
		DECLARE @FiltroOna				NVARCHAR(400)	 
		DECLARE @FiltroEsquema			NVARCHAR(400)	 
		DECLARE @FiltroNorma			NVARCHAR(400)	 
		DECLARE @FiltroEstado			NVARCHAR(400)	 
		DECLARE @FiltroRecomocimiento	NVARCHAR(400)	 
		DECLARE @EnteBuscado 			TABLE (IdEnte VARCHAR(16) , IdVista VARCHAR(16) , TipoData VARCHAR(40), IdHomologacion INT, Texto NVARCHAR(4000))
		DECLARE @FiltroBusqueda			TABLE (IdHomologacion INT , Texto NVARCHAR(100) COLLATE Latin1_General_CI_AI)
		DECLARE @IdHomologacionEsquema	INTEGER			= (SELECT TOP 1 IdHomologacionEsquema from HomologacionEsquema (NOLOCK) order by MostrarWebOrden)
		DECLARE @TextoBuscar			NVARCHAR(200)	= lower(trim(JSON_VALUE(@paramJSON,'$.TextoBuscar')))
		DECLARE @ExactaBuscar			BIT				= JSON_VALUE(@paramJSON, '$.ExactaBuscar')
	END TRY
	BEGIN CATCH
		DECLARE  @ErrorNumber	INT = ERROR_NUMBER()
				,@ErrorSeverity INT = ERROR_SEVERITY()
				,@ErrorState	INT = ERROR_STATE()
				,@ErrorLine		INT = ERROR_LINE()
				,@ErrorProcedure NVARCHAR(128)  = ERROR_PROCEDURE()
				,@ErrorMessage	 NVARCHAR(2000) = ERROR_MESSAGE()

		DECLARE @FullErrorMessage NVARCHAR(3000)='ERROR AL BUSCAR EN: ' + ISNULL(@ErrorProcedure, 'psBuscarPalabra') + CHAR(13) + CHAR(10) 
		SELECT  @FullErrorMessage +='* Error Number: '  + CAST(@ErrorNumber AS NVARCHAR(10))	+ CHAR(13) + CHAR(10) +
									'* Severity: '		+ CAST(@ErrorSeverity AS NVARCHAR(10))	+ CHAR(13) + CHAR(10) +
									'* State: '			+ CAST(@ErrorState AS NVARCHAR(10))		+ CHAR(13) + CHAR(10) +
									'* Line: '			+ CAST(@ErrorLine AS NVARCHAR(10))		+ CHAR(13) + CHAR(10) +
									'* Message: '		+ @ErrorMessage;
		PRINT @FullErrorMessage;
	END CATCH
	SElECT @TextoBuscar = dbo.fnDropSpacesTabs(isnull(@TextoBuscar,'')), @ExactaBuscar = isnull(@ExactaBuscar,0)
	
	INSERT  INTO @FiltroBusqueda	
	SELECT	DISTINCT 114, value		FROM OPENJSON(JSON_QUERY(@paramJSON, '$.FiltroPais'))			-->	PAIS		   : IdHomologacion = 114
	UNION
	SELECT	DISTINCT 104, value		FROM OPENJSON(JSON_QUERY(@paramJSON, '$.FiltroOna'))			-->	ONA			   : IdHomologacion = 104
	UNION
	SELECT	DISTINCT 121, value		FROM OPENJSON(JSON_QUERY(@paramJSON, '$.FiltroEsquema'))		-->	ESQUEMA		   : IdHomologacion = 121
	UNION
	SELECT	DISTINCT 122, value		FROM OPENJSON(JSON_QUERY(@paramJSON, '$.FiltroNorma'))			-->	NORMA		   : IdHomologacion = 122
	UNION
	SELECT	DISTINCT 120, value		FROM OPENJSON(JSON_QUERY(@paramJSON, '$.FiltroEstado'))			-->	ESTADO		   : IdHomologacion = 120
	UNION
	SELECT	DISTINCT 123, value		FROM OPENJSON(JSON_QUERY(@paramJSON, '$.FiltroRecomocimiento'))	-->	RECOMOCIMIENTO : IdHomologacion = 123	

	--> Busqueda Exacta:	"word_phase", ""
    IF  @ExactaBuscar = 1
		INSERT	INTO @EnteBuscado (IdEnte, IdVista, TipoData)
		SELECT	DISTINCT o.IdEnte  , o.IdVista , '1.BuscaExacta'		--,FullTextData
		FROM	CanFullText o  (NOLOCK)
		WHERE	o.IdEnte is not null
		AND		o.FullTextData = @TextoBuscar
		AND		(	EXISTS 
					(	SELECT	1 
						FROM	@FiltroBusqueda fb 
						WHERE	fb.IdHomologacion = o.IdHomologacion 
						AND		fb.Texto		  = o.FullTextData
					)
					OR NOT EXISTS (SELECT 1 FROM @FiltroBusqueda)
				)

	--> Busqueda NoExacta: 	"word_phase", ""   ( sinonimos + stopWord + Rank)
	ELSE
	IF  @TextoBuscar = '' 
		INSERT	INTO @EnteBuscado (IdEnte, IdVista, TipoData)
		SELECT  DISTINCT o.IdEnte  , o.IdVista , '0.BuscaVacia'		--,FullTextData
		FROM	CanFullText o  (NOLOCK) 
		WHERE	(	EXISTS 
					(	SELECT	1 
						FROM	@FiltroBusqueda fb 
						WHERE	fb.IdHomologacion = o.IdHomologacion 
						AND		fb.Texto		  = o.FullTextData
					)
					OR NOT EXISTS (SELECT 1 FROM @FiltroBusqueda)
				)
	ELSE
	begin
		--> THESAURUS
		--DECLARE @TextoBuscarSinonimo NVARCHAR(200) = ' FORMSOF(THESAURUS, "' + @TextoBuscar +'" )' 
		--INSERT	INTO @EnteBuscado (IdEnte, IdVista, TipoData, IdHomologacion , Texto)
		--SELECT  DISTINCT o.IdEnte  , o.IdVista , '0.BuscaThesa'			,o.[IdHomologacion] ,o.[FullTextData]
		--FROM	CanFullText o  (NOLOCK)
		--WHERE	CONTAINS(FullTextData,  @TextoBuscarSinonimo )
		--AND		(	EXISTS 
		--			(	SELECT	1 
		--				FROM	@FiltroBusqueda fb 
		--				WHERE	fb.IdHomologacion = o.IdHomologacion 
		--				AND		fb.Texto		  = o.FullTextData
		--			)
		--			OR NOT EXISTS (SELECT 1 FROM @FiltroBusqueda)
		--		)
		--> INFLECTIONAL
		DECLARE @TextoBuscarInfle NVARCHAR(200) = ' FORMSOF(INFLECTIONAL, "' + @TextoBuscar +'" )'
		INSERT	INTO @EnteBuscado (IdEnte, IdVista, TipoData	,IdHomologacion , Texto)
		SELECT  DISTINCT o.IdEnte  , o.IdVista , '0.BuscaInflec'		,o.[IdHomologacion] ,o.[FullTextData]
		FROM	CanFullText o  (NOLOCK)
		WHERE	CONTAINS(FullTextData,  @TextoBuscarInfle )
		AND		(	EXISTS 
					(	SELECT	1 
						FROM	@FiltroBusqueda fb 
						WHERE	fb.IdHomologacion = o.IdHomologacion 
						AND		fb.Texto		  = o.FullTextData
					)
					OR NOT EXISTS (SELECT 1 FROM @FiltroBusqueda)
				)

		--> RANK 
		INSERT	INTO @EnteBuscado (IdEnte, IdVista, TipoData ,IdHomologacion , Texto)
		SELECT	DISTINCT o.IdEnte  , o.IdVista , '0.BuscaRank'			,o.[IdHomologacion] ,o.[FullTextData]  --, OFT.RANK  
		FROM	CanFullText o  (NOLOCK)
		JOIN	FREETEXTTABLE(CanFullText, FullTextData,  @TextoBuscar ) as OFT		--,LANGUAGE N'English', 2) AS OFT  
		ON		o.IdCanFullText		= OFT.[KEY]									--AND o.IdEnte			= OFT.IdEnte 	 
		WHERE	(	EXISTS 
					(	SELECT	1 
						FROM	@FiltroBusqueda fb 
						WHERE	fb.IdHomologacion = o.IdHomologacion 
						AND		fb.Texto		  = o.FullTextData
					)
					OR NOT EXISTS (SELECT 1 FROM @FiltroBusqueda)
				)
		--ORDER BY RANK DESC; 
	end;

	--SELECT * FROM @EnteBuscado;
	-- Eliminar Duplicados  
	WITH tbEnteBuscado AS 
	(	 SELECT ROW_NUMBER() OVER (PARTITION BY IdEnte, IdVista ORDER BY IdEnte) AS IdRow
		 FROM @EnteBuscado
	)	 DELETE FROM tbEnteBuscado
		 WHERE IdRow > 1
	--SELECT * FROM @EnteBuscado

	IF  (@PageNumber = 1)
		SELECT	@RowsTotal	 = COUNT(*) 
		FROM	@EnteBuscado e
		JOIN	CanDataSet	 c  ON  c.IdEnte = e.IdEnte
		WHERE	IdHomologacionEsquema = @IdHomologacionEsquema

	SELECT  DISTINCT				 --@RowsTotal RowsTotal, @RowsPerPage RowsPage, e.TipoData,
			 c.IdEnte 
			,c.IdVista 
			,c.IdHomologacionEsquema
			,c.DataEsquemaJson
	FROM	@EnteBuscado e
	JOIN	CanDataSet	 c  ON  c.IdEnte = e.IdEnte
	WHERE	IdHomologacionEsquema = @IdHomologacionEsquema
	ORDER BY c.IdEnte  
	OFFSET (@PageNumber - 1) * @RowsPerPage ROWS
	FETCH NEXT @RowsPerPage ROWS ONLY;
END;
GO	

--> modificar : IdEnte
--> ExactaBuscar 
--  true  = Buscar exacta 
--  false = Buscar vacio, sinonimo  + rank

exec psBuscarPalabra		 
N'{	 "ExactaBuscar"			:false
	,"TextoBuscar"			:"leche arsenico chocolate"
	,"FiltroPais"			:[]
	,"FiltroOna"			:[]
	,"FiltroEsquema"		:[]
	,"FiltroNorma"			:[]
	,"FiltroEstado"			:[]
	,"FiltroRecomocimiento"	:[]
}',1,20;

 


--	SELECT	DISTINCT *
--	FROM	CanFullText
--	--WHERE  FullTextData like 'laboratorio de calificación de leche cruda' 
--	WHERE	CONTAINS(FullTextData,  '"cocoa"' )
	
--	SELECT	DISTINCT *
--	FROM	CanFullText
--	--WHERE  FullTextData like 'laboratorio de calificación de leche cruda' 
--	WHERE	CONTAINS(FullTextData,  '"chocolate"' )
	
--SELECT *
--FROM CanFullText
--WHERE CONTAINS(FullTextData, 'FORMSOF(THESAURUS, "cocoa")');
 

 -- EXEC sys.sp_fulltext_load_thesaurus_file 3082, @loadOnlyIfNotLoaded = 1;
 --	SELECT	 *
 -- FROM	CanFullText
	
 

--EXEC dbo.setDiccionario	'dbo.vwFiltro					', NULL ,'vista para los filtros de la pagina principal'
--EXEC dbo.setDiccionario	'dbo.vwDimension				', NULL ,'vista para las dimensiones o campos homologados'
--EXEC dbo.setDiccionario	'dbo.vwGrilla					', NULL ,'vita de esquema principal de la busqueda'
--EXEC dbo.setDiccionario	'dbo.fnFiltroDetalle			', NULL ,'funcion para traer los detalles de cada filtro'	
--EXEC dbo.setDiccionario	'dbo.fnHomologacionEsquemaCampo	', NULL ,'funcion para obtener los esquema y sus campos'				
--EXEC dbo.setDiccionario	'dbo.fnHomologacionEsquema		', NULL ,'funcion para obtener un esquema'			
--EXEC dbo.setDiccionario	'dbo.fnHomologacionEsquemaTodo	', NULL ,'funcion para obtener todos los esquema'				
--EXEC dbo.setDiccionario	'dbo.fnHomologacionEsquemaDato	', NULL ,'funcion para obtener un esquema y sus datos'				
--EXEC dbo.setDiccionario	'dbo.psBuscarPalabra			', NULL ,'procedimiento almacenado para la busqueda principal'				
--go

--EXEC DBO.Bitacora ' CREATE OR ALTER 
--					,dbo.vwFiltro					
--					,dbo.vwDimension				
--					,dbo.vwGrilla					
--					,dbo.fnFiltroDetalle			
--					,dbo.fnHomologacionEsquemaCampo	
--					,dbo.fnHomologacionEsquema		
--					,dbo.fnHomologacionEsquemaTodo	
--					,dbo.fnHomologacionEsquemaDato
--					,dbo.psBuscarPalabra	'
--GO

