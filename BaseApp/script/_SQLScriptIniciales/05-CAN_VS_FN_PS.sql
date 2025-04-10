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
--| vwFiltro
CREATE OR ALTER   VIEW [dbo].[vwFiltro] AS 
SELECT 	IdHomologacion ,MostrarWeb ,TooltipWeb ,MostrarWebOrden 
FROM	Homologacion (NOLOCK) 	
WHERE	CodigoHomologacion IN
		('KEY_DIM_PAI_FIL'  -- 'PAIS '           'filtro 1')
		,'KEY_DIM_ORG_FIL'  -- 'ONA'			 'filtro 2')
		,'KEY_DIM_ESQ_FIL'  -- 'ESQUEMA'		 'filtro 3')
		,'KEY_DIM_NOR_FIL'  -- 'NORMA'           'filtro 4')
		,'KEY_DIM_EST_FIL'  -- 'ESTADO'          'filtro 5')
		,'KEY_DIM_REC_FIL'  -- 'RECOMOCIMIENTO'  'filtro 6')
		)
GO

--| vwDimension
CREATE OR ALTER   VIEW [dbo].[vwDimension] AS 
	SELECT   IdHomologacion
			,NombreHomologado
			,MostrarWeb
			,TooltipWeb
			,MostrarWebOrden
			,MascaraDato
			,SiNoHayDato
			,NombreHomologado + ' / ' + MostrarWeb AS CustomMostrarWeb
	FROM    Homologacion (NOLOCK)
	WHERE	IdHomologacionGrupo   
	IN		(1	--'KEY_DIM_ESQUEMA'
			)
GO

--| vwGrilla
CREATE OR ALTER   VIEW [dbo].[vwGrilla] AS 
	SELECT	 H.IdHomologacion
			,H.MostrarWeb
			,H.TooltipWeb
			,H.MostrarWebOrden
	FROM	Homologacion	H	WITH (NOLOCK)
	JOIN	(	SELECT DISTINCT IdHomologacion
				FROM  OPENJSON((	SELECT  TOP 1 EsquemaJson
									FROM	Esquema (NOLOCK)
									ORDER BY MostrarWebOrden
				))
				WITH (IdHomologacion INT '$.IdHomologacion')
			)	HE	ON HE.IdHomologacion = H.IdHomologacion
GO
--| vwRol
CREATE OR ALTER   VIEW [dbo].[vwRol] AS 
	SELECT	 h1.IdHomologacion	IdHomologacionRol
			,h1.MostrarWeb		Rol
	FROM	Homologacion h1
	JOIN	Homologacion h2 ON h1.IdHomologacionGrupo = h2.IdHomologacion
	WHERE	h2.CodigoHomologacion = 'KEY_ROL'
GO
--| vwEndPoint
CREATE OR ALTER   VIEW [dbo].[vwEndPoint] AS 
	SELECT	 h1.IdHomologacion	IdHomologacionEndPoint
			,h1.MostrarWeb		EndPointNombre
			,h1.TooltipWeb		EndPointUrl
	FROM	Homologacion h1
	JOIN	Homologacion h2 ON h1.IdHomologacionGrupo = h2.IdHomologacion
	WHERE	h2.CodigoHomologacion = 'KEY_END_POINT'
GO

--| fnFiltroDetalle
CREATE OR ALTER FUNCTION fnFiltroDetalle (	@IdHomologacionGrupo	INT )  
RETURNS TABLE AS
RETURN
	SELECT	DISTINCT upper(FullTextData) 'MostrarWeb'
	FROM	[EsquemaFullText]   (NOLOCK) 
	WHERE	CASE 
			WHEN @IdHomologacionGrupo = 2 THEN CASE WHEN IdHomologacion = 81 THEN 1 ELSE 0 END	-->	PAIS		   ('OrgPais': ecuador, peru ,....)
			WHEN @IdHomologacionGrupo = 3 THEN CASE WHEN IdHomologacion = 71 THEN 1 ELSE 0 END	-->	ONA			   ('OnaAbreviacion': SAE,..)
			WHEN @IdHomologacionGrupo = 4 THEN CASE WHEN IdHomologacion = 88 THEN 1 ELSE 0 END	-->	ESQUEMA		   ('OrgEsquemaAcreditado': CALIBRACIÓN, CLÍNICOS,..)
			WHEN @IdHomologacionGrupo = 5 THEN CASE WHEN IdHomologacion = 89 THEN 1 ELSE 0 END	-->	NORMA		   ('OrgNormaAcreditada': NTE INEN-ISO/IEC17043: ,..)
			WHEN @IdHomologacionGrupo = 6 THEN CASE WHEN IdHomologacion = 87 THEN 1 ELSE 0 END	-->	ESTADO		   ('OrgEstadoAcreditado')
			WHEN @IdHomologacionGrupo = 7 THEN CASE WHEN IdHomologacion = 90 THEN 1 ELSE 0 END	-->	RECOMOCIMIENTO ('OrgReconocimiento': INTERNACIONAL, NACIONAL)
			--|select * from Homologacion where NombreHomologado = 'OrgReconocimiento'
			END = 1;
GO
--| fnEsquemaCampo
CREATE OR ALTER   FUNCTION [dbo].[fnEsquemaCampo] ( @IdEsquema INT )  
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
														FROM	Esquema		(NOLOCK)
														WHERE	IdEsquema = @IdEsquema
												  ))
									WITH (IdHomologacion INT '$.IdHomologacion')
								)	HE	ON HE.IdHomologacion = H.IdHomologacion
						FOR JSON AUTO 
					);
	RETURN @json;
END;
GO
--| fnEsquema
CREATE OR ALTER   FUNCTION [dbo].[fnEsquema] ( @IdEsquema	INT )  
RETURNS TABLE AS
RETURN
	SELECT	 IdEsquema	
			,MostrarWebOrden	
			,MostrarWeb	
			,TooltipWeb
			,EsquemaVista
			,(select dbo.fnEsquemaCampo(IdEsquema)) EsquemaJson
	FROM	Esquema		WITH (NOLOCK)
	WHERE	IdEsquema	= @IdEsquema	
	AND		Estado		= 'A'
	--select * from fnEsquema ( 1)
GO

--| fnEsquemaTodo
CREATE OR ALTER   FUNCTION [dbo].[fnEsquemaTodo] ( @VistaPK VARCHAR(16))  
RETURNS TABLE AS
RETURN
	SELECT	 DISTINCT
			 e.IdEsquema	
			,e.MostrarWebOrden	
			,e.MostrarWeb	
			,e.TooltipWeb	
	FROM	[Esquema]				e (NOLOCK)
	JOIN	[EsquemaVista]			v (NOLOCK)	ON e.IdEsquema		= v.IdEsquema
	JOIN	[EsquemaData]			d (NOLOCK)	ON v.IdEsquemaVista = d.IdEsquemaVista
	WHERE	e.MostrarWebOrden > 1	
	AND		e.Estado   = 'A'
	AND		d.VistaPK  = @VistaPK
GO

--| fnEsquemaDato
CREATE OR ALTER   FUNCTION [dbo].[fnEsquemaDato] (  @IdEsquema INT, @VistaPK VARCHAR(16) )  
RETURNS TABLE AS
RETURN
	SELECT	 IdEsquemaData
			,IdEsquema
			,DataEsquemaJson
	FROM	[EsquemaData] (NOLOCK)  ed 
	JOIN	EsquemaVista  (NOLOCK)	ev on ed.IdEsquemaVista = ev.IdEsquemaVista
	WHERE	IdEsquema	= @IdEsquema
	AND		VistaPK		= @VistaPK
GO	 

--| fnPredictWord
CREATE OR ALTER FUNCTION [dbo].[fnPredictWord] (@Prefix NVARCHAR(100))
RETURNS @TopWords TABLE (Word NVARCHAR(max))
AS
BEGIN
    INSERT INTO @TopWords (Word)
    SELECT DISTINCT TOP 10 FullTextData
    FROM   [EsquemaFullText]  (NOLOCK)
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

CREATE OR ALTER   PROCEDURE [dbo].[paBuscarPalabra] ( @paramJSON NVARCHAR(max) = NULL , @PageNumber INT = 1, @RowsPerPage INT = 20, @RowsTotal INT = 0 OUTPUT) AS
BEGIN 
--N'{	"ExactaBuscar"		:false
--	,"TextoBuscar"			:"leche"
--	,"FiltroPais"			:["ecuador", "peru"]
--	,"FiltroOna"			:[]
--	,"FiltroEsquema"		:[]
--	,"FiltroNorma"			:[]
--	,"FiltroEstado"			:["Acreditado"]
--	,"FiltroRecomocimiento"	:["nacional"]
--}',1,10;
	BEGIN TRY	
		SELECT  @RowsTotal				= 0;
		DECLARE @FiltroPais				NVARCHAR(400)	 
		DECLARE @FiltroOna				NVARCHAR(400)	 
		DECLARE @FiltroEsquema			NVARCHAR(400)	 
		DECLARE @FiltroNorma			NVARCHAR(400)	 
		DECLARE @FiltroEstado			NVARCHAR(400)	 
		DECLARE @FiltroRecomocimiento	NVARCHAR(400)	 
		DECLARE @EnteBuscadoFiltrado	TABLE (IdEsquemaData VARCHAR(16) , IdHomologacion INT, CumpleFiltro INT default (0))
		DECLARE @EnteBuscado 			TABLE (IdEsquemaData VARCHAR(16) , IdVista VARCHAR(16) , TipoData VARCHAR(40), IdHomologacion INT, Texto NVARCHAR(4000))
		DECLARE @FiltroBusqueda			TABLE (IdHomologacion INT , Texto NVARCHAR(100) COLLATE Latin1_General_CI_AI)
		DECLARE @IdEsquema	INTEGER			= (SELECT TOP 1 IdEsquema from Esquema (NOLOCK) order by MostrarWebOrden)
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
	--| fnFiltroDetalle
	INSERT  INTO @FiltroBusqueda	
	SELECT	DISTINCT 81, value		FROM OPENJSON(JSON_QUERY(@paramJSON, '$.FiltroPais'))			-->	PAIS		   : IdHomologacion = 114
	UNION
	SELECT	DISTINCT 71, value		FROM OPENJSON(JSON_QUERY(@paramJSON, '$.FiltroOna'))			-->	ONA			   : IdHomologacion = 104
	UNION
	SELECT	DISTINCT 88, value		FROM OPENJSON(JSON_QUERY(@paramJSON, '$.FiltroEsquema'))		-->	ESQUEMA		   : IdHomologacion = 121
	UNION
	SELECT	DISTINCT 89, value		FROM OPENJSON(JSON_QUERY(@paramJSON, '$.FiltroNorma'))			-->	NORMA		   : IdHomologacion = 122
	UNION
	SELECT	DISTINCT 87, value		FROM OPENJSON(JSON_QUERY(@paramJSON, '$.FiltroEstado'))			-->	ESTADO		   : IdHomologacion = 120
	UNION
	SELECT	DISTINCT 90, value		FROM OPENJSON(JSON_QUERY(@paramJSON, '$.FiltroRecomocimiento'))	-->	RECOMOCIMIENTO : IdHomologacion = 123	

	--> Busqueda Exacta:	"word_phase", ""
    IF  @ExactaBuscar = 1
		INSERT	INTO @EnteBuscado (IdEsquemaData, TipoData)
		SELECT	DISTINCT o.IdEsquemaData  ,  '1.BuscaExacta'		--,o.IdVista ,FullTextData
		FROM	EsquemaFullText o  (NOLOCK)
		WHERE	o.IdEsquemaData is not null
		AND		o.FullTextData = @TextoBuscar
	--> Busqueda NoExacta: 	"word_phase", ""   ( sinonimos + stopWord + Rank)
	ELSE
	IF  @TextoBuscar = '' 
		INSERT	INTO @EnteBuscado (IdEsquemaData, TipoData)
		SELECT  DISTINCT o.IdEsquemaData  ,  '0.BuscaVacia'		--,o.IdVista ,FullTextData
		FROM	EsquemaFullText o  (NOLOCK) 
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
		--> INFLECTIONAL + THESAURUS
		DECLARE @TextoBuscarInfle NVARCHAR(200) = ' FORMSOF(INFLECTIONAL, "' + @TextoBuscar +'" )'
		INSERT	INTO @EnteBuscado (IdEsquemaData, TipoData	,IdHomologacion , Texto)
		SELECT  DISTINCT o.IdEsquemaData  , '0.BuscaInflec'		,o.[IdHomologacion] ,o.[FullTextData]  -- o.IdVista ,
		FROM	EsquemaFullText o  (NOLOCK)
		WHERE	CONTAINS(FullTextData,  @TextoBuscarInfle )

		--> RANK 
		INSERT	INTO @EnteBuscado (IdEsquemaData, TipoData ,IdHomologacion , Texto)
		SELECT	DISTINCT o.IdEsquemaData  , '0.BuscaRank'			,o.[IdHomologacion] ,o.[FullTextData]  --, o.IdVista , OFT.RANK  
		FROM	EsquemaFullText o  (NOLOCK)
		JOIN	FREETEXTTABLE(EsquemaFullText, FullTextData,  @TextoBuscar ) as OFT		--,LANGUAGE N'English', 2) AS OFT  
		ON		o.IdEsquemaFullText		= OFT.[KEY]										--AND o.IdEsquemaData			= OFT.IdEsquemaData 	 
		--ORDER BY RANK DESC; 
	end;
	 
	 

	IF	EXISTS (select 1 from @FiltroBusqueda)
		WITH tbfiltro AS 
		(	select  distinct IdEsquemaData,	 f.IdHomologacion ,  
					CASE WHEN	EXISTS 
					(	SELECT	1 
						FROM	EsquemaFullText (NOLOCK) c 
						WHERE	c.IdEsquemaData	= e.IdEsquemaData
						AND		c.IdHomologacion= f.IdHomologacion
						AND		c.FullTextData IN (SELECT lower(trim(ff.Texto)) FROM @FiltroBusqueda ff WHERE ff.IdHomologacion = c.IdHomologacion)
					) 
					THEN 1 
					ELSE 0 	END AS CumpleFiltro
			from	@EnteBuscado e
			cross	join ( select distinct IdHomologacion from @FiltroBusqueda ) f
		)	insert  into @EnteBuscadoFiltrado
			select  *
			from	tbfiltro
	ELSE
		INSERT  INTO @EnteBuscadoFiltrado
		SELECT	DISTINCT IdEsquemaData, 0 IdHomologacion, 1 CumpleFiltro
		FROM	@EnteBuscado
	;
	WITH tbEnteBuscado AS 
	(	 SELECT ROW_NUMBER() OVER (PARTITION BY IdEsquemaData,CumpleFiltro ORDER BY IdEsquemaData,CumpleFiltro) AS IdRow, *
		 FROM	@EnteBuscadoFiltrado
	)    DELETE FROM tbEnteBuscado
		 WHERE IdRow > 1

	--select * from @EnteBuscadoFiltrado

	DELETE FROM @EnteBuscadoFiltrado
	WHERE IdEsquemaData IN (
		SELECT DISTINCT IdEsquemaData
		FROM @EnteBuscadoFiltrado
		WHERE cumpleFiltro = 0
	); 

	IF  (@PageNumber = 1)
		SELECT	@RowsTotal	 = COUNT(*) 
		FROM	@EnteBuscadoFiltrado e
		JOIN	EsquemaData	 c  ON  c.IdEsquemaData = e.IdEsquemaData
		--WHERE	IdEsquema = @IdEsquema

	SELECT  DISTINCT				 --@RowsTotal RowsTotal, @RowsPerPage RowsPage, e.TipoData,
			 c.IdEsquemaData 
			,c.VistaPK 
			,v.IdEsquema
			,c.DataEsquemaJson
	FROM	@EnteBuscadoFiltrado e
	JOIN	EsquemaData	 c  ON  c.IdEsquemaData  = e.IdEsquemaData
	JOIN	EsquemaVista v  ON  v.IdEsquemaVista = c.IdEsquemaVista
	--WHERE	IdEsquema = @IdEsquema
	ORDER BY c.IdEsquemaData  
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
	,"TextoBuscar"			:""
	,"FiltroPais"			:["españa", "peru","ecuador","Argentina"]
	,"FiltroOna"			:[]
	,"FiltroEsquema"		:["inspeccion"]
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

