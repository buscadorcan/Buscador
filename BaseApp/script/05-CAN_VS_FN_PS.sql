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
	SELECT	DISTINCT upper(FullTextOrganizacion) 'MostrarWeb'
	FROM	OrganizacionFullText  (NOLOCK)			---> and Estado = 'A'
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

CREATE OR ALTER FUNCTION fnHomologacionEsquemaTodo ( @IdOrganizacion VARCHAR(16))  
RETURNS TABLE AS
RETURN
	SELECT	 DISTINCT
			 he.IdHomologacionEsquema	
			,he.MostrarWebOrden	
			,he.MostrarWeb	
			,he.TooltipWeb	
	FROM	HomologacionEsquema		he (NOLOCK)
	JOIN	DataLakeOrganizacion	dl (NOLOCK)	ON he.IdHomologacionEsquema = dl.IdHomologacionEsquema
	WHERE	he.MostrarWebOrden > 1	
	AND		he.Estado = 'A'
	AND		dl.IdOrganizacion = @IdOrganizacion
GO

--select * from fnHomologacionEsquemaDato ( 5, '368' ) (@IdHomologacionEsquema , @IdOrganizacion )

CREATE OR ALTER FUNCTION fnHomologacionEsquemaDato (  @IdHomologacionEsquema INT, @IdOrganizacion VARCHAR(16) )  
RETURNS TABLE AS
RETURN
	SELECT	 IdDataLakeOrganizacion	
			,IdHomologacionEsquema
			,DataEsquemaJson
	FROM	DataLakeOrganizacion	(NOLOCK)	
	WHERE	IdHomologacionEsquema	= @IdHomologacionEsquema
	AND		IdOrganizacion			= @IdOrganizacion
	AND		Estado					= 'A'
GO

--CREATE OR ALTER FUNCTION dbo.fn_SplitWords (@Text NVARCHAR(MAX))
--RETURNS @Words TABLE (Word NVARCHAR(100))
--AS
--BEGIN
--    DECLARE @XML XML

--    -- Reemplaza caracteres especiales
--    SET @Text = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(@Text, '&', '&amp;'), '<', '&lt;'), '>', '&gt;'), '''', '&apos;'), '"', '&quot;')

--    -- Formatea el texto para XML
--    SET @XML = CAST('<root><word>' + REPLACE(REPLACE(@Text, ' ', '</word><word>'), ',', '</word><word>') + '</word></root>' AS XML)

--    INSERT INTO @Words (Word)
--    SELECT LTRIM(RTRIM(T.c.value('.', 'NVARCHAR(100)')))
--    FROM @XML.nodes('/root/word') T(c)
--    WHERE LTRIM(RTRIM(T.c.value('.', 'NVARCHAR(100)'))) <> ''

--    RETURN
--END
--GO

CREATE OR ALTER FUNCTION fn_PredictWords (@Prefix NVARCHAR(100))
RETURNS @TopWords TABLE (Word NVARCHAR(max))
AS
BEGIN
    INSERT INTO @TopWords (Word)
    SELECT DISTINCT TOP 10 FullTextOrganizacion
    FROM OrganizacionFullText WITH (NOLOCK)
    WHERE FullTextOrganizacion LIKE '%' + @Prefix + '%'
    RETURN
END
GO

--CREATE OR ALTER FUNCTION fn_DropSpacesTabs (@input NVARCHAR(MAX))	
--RETURNS NVARCHAR(MAX)
--AS
--BEGIN
--    DECLARE @pos INT;
--    DECLARE @length INT;
--    DECLARE @result NVARCHAR(MAX);
    
--	SET @result = REPLACE(@result, CHAR(9), ' ');  -- Tab
--    SET @result = REPLACE(@result, CHAR(10), ' '); -- Line Feed
--    SET @result = REPLACE(@result, CHAR(13), ' '); -- Carriage Return

--    SET @result = @input;
--    SET @pos = 1;
--    SET @length = LEN(@result);
    
--    WHILE @pos < @length
--    BEGIN
--        IF SUBSTRING(@result, @pos, 1) IN (' ', CHAR(9)) AND SUBSTRING(@result, @pos + 1, 1) IN (' ', CHAR(9))
--        BEGIN
--            SET @result = STUFF(@result, @pos + 1, 1, '');
--            SET @length = LEN(@result);
--        END
--        ELSE
--        BEGIN
--            SET @pos = @pos + 1;
--        END
--    END
    
--    RETURN @result;
--END;

CREATE OR ALTER FUNCTION fn_DropSpacesTabs (@input NVARCHAR(MAX))	
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
--exec psBuscarPalabra  	 
--	N'{	"ExactaBuscar"			: 1
--	,"TextoBuscar"			:"salmonella"
--	,"FiltroPais"			:["ecuador"]
--	,"FiltroOna"			:[]
--	,"FiltroEsquema"		:[]
--	,"FiltroNorma"			:[]
--	,"FiltroEstado"			:["acreditado"]
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
		DECLARE @Organizacion			TABLE (IdOrganizacion NVARCHAR(16) , IdVista NVARCHAR(16) )
		DECLARE @FiltroBusqueda			TABLE (IdHomologacion INT , Texto NVARCHAR(100))
		DECLARE @IdHomologacionEsquema	INTEGER			= (SELECT TOP 1 IdHomologacionEsquema from HomologacionEsquema (NOLOCK) order by MostrarWebOrden)
		DECLARE @TextoBuscar			NVARCHAR(200)	= lower(LTRIM(RTRIM(JSON_VALUE(@paramJSON,'$.TextoBuscar'))))
		DECLARE @ExactaBuscar			bit			= JSON_VALUE(@paramJSON, '$.ExactaBuscar')
	END TRY
	BEGIN CATCH
		  SELECT ERROR_NUMBER() ,ERROR_SEVERITY() ,ERROR_STATE() ,ERROR_PROCEDURE() ,ERROR_LINE() ,ERROR_MESSAGE()		 
	END CATCH;

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

	--> Busqueda Exacta: "word" , "phase"	
    IF  @ExactaBuscar = 1  AND (@TextoBuscar IS NOT NULL OR @TextoBuscar <> '')
		INSERT	INTO @Organizacion (IdOrganizacion, IdVista)
		SELECT DISTINCT o.IdOrganizacion  , o.IdVista 
		FROM	OrganizacionFullText o  (NOLOCK)
		JOIN	(	select  distinct IdOrganizacion  
					from	OrganizacionFullText  (NOLOCK)
					where	FullTextOrganizacion = @TextoBuscar
				)	b  on b.IdOrganizacion	= o.IdOrganizacion 	 
		WHERE	(	EXISTS 
					(	SELECT	1 
						FROM	@FiltroBusqueda fb 
						WHERE	fb.IdHomologacion = o.IdHomologacion 
						AND		fb.Texto		  = o.FullTextOrganizacion
					)
					OR NOT EXISTS (SELECT 1 FROM @FiltroBusqueda)
				)

	--> Palabra		%	"",  "word" , "phase"   ( sinonimos + Rank)	
	-->	Frase		%	"",  "word" , "phase"   ( sinonimos + stopWord + Rank)
    --> IF  @ModoBuscar = 2
	ELSE
	begin
		--DECLARE @TextoBuscar NVARCHAR(200) = 'leche	  nutri'
		SELECT	@TextoBuscar = dbo.fn_DropSpacesTabs(isnull(@TextoBuscar,''))
		--select * from fn_Split(@TextoBuscar,' ')
		DECLARE @TextoBuscarSinonimo NVARCHAR(200) = 'FORMSOF(THESAURUS, "' + @TextoBuscar +'")'
		--WHERE CONTAINS(CatName , 'FORMSOF (THESAURUS, Jon)')  INFLECTIONAL
		INSERT	INTO @Organizacion (IdOrganizacion, IdVista)
		SELECT DISTINCT o.IdOrganizacion  , o.IdVista
		FROM	OrganizacionFullText o  (NOLOCK)
		JOIN	(	select  distinct IdOrganizacion  
					from	OrganizacionFullText  (NOLOCK)
					where	CONTAINS(FullTextOrganizacion,  @TextoBuscarSinonimo )
				)	b  on b.IdOrganizacion	= o.IdOrganizacion 	 
		WHERE	(	EXISTS 
					(	SELECT	1 
						FROM	@FiltroBusqueda fb 
						WHERE	fb.IdHomologacion = o.IdHomologacion 
						AND		fb.Texto		  = o.FullTextOrganizacion
					)
					OR NOT EXISTS (SELECT 1 FROM @FiltroBusqueda)
				)

		INSERT	INTO @Organizacion (IdOrganizacion, IdVista)
		SELECT	o.IdOrganizacion  , o.IdVista  --, OFT.RANK  
		FROM	OrganizacionFullText o  (NOLOCK)
		JOIN	FREETEXTTABLE(OrganizacionFullText, FullTextOrganizacion,  @TextoBuscar ) as OFT
		--JOIN	FREETEXTTABLE(OrganizacionFullText.IdOrganizacion, FullTextOrganizacion,  @TextoBuscar ) as OFT
			--,LANGUAGE N'English', 2) AS OFT  
		ON	o.IdOrganizacionFullText	= OFT.[KEY]  
		--AND o.IdOrganizacion			= OFT.IdOrganizacion 	 
		WHERE	(	EXISTS 
					(	SELECT	1 
						FROM	@FiltroBusqueda fb 
						WHERE	fb.IdHomologacion = o.IdHomologacion 
						AND		fb.Texto		  = o.FullTextOrganizacion
					)
					OR NOT EXISTS (SELECT 1 FROM @FiltroBusqueda)
				)
		ORDER BY RANK DESC; 
	end
	
	----> Buscar por palabras
 --   IF  @ModoBuscar = 3
	--begin  
	--	SELECT	@TextoBuscar = '"*' + REPLACE(dbo.fn_DropSpacesTabs(@TextoBuscar), ' ', '%') +'*"'
		
	--	INSERT	INTO @Organizacion (IdOrganizacion, IdVista)
	--	SELECT DISTINCT o.IdOrganizacion  , o.IdVista
	--	FROM	OrganizacionFullText o  (NOLOCK)
	--	JOIN	(	select  distinct IdOrganizacion  
	--				from	OrganizacionFullText  (NOLOCK)
	--				where	(@TextoBuscar IS NULL OR @TextoBuscar = '')
	--				or		CONTAINS(FullTextOrganizacion,  @TextoBuscar )
	--			)	b  on b.IdOrganizacion	= o.IdOrganizacion 	 
	--	WHERE	(	EXISTS 
	--				(	SELECT	1 
	--					FROM	@FiltroBusqueda fb 
	--					WHERE	fb.IdHomologacion = o.IdHomologacion 
	--					AND		fb.Texto		  = o.FullTextOrganizacion
	--				)
	--				OR NOT EXISTS (SELECT 1 FROM @FiltroBusqueda)
	--			)
	--end

	----> Buscar con sinonimos
 --   IF  @ModoBuscar = 4
	--begin  
	--	SELECT	@TextoBuscar = 'FORMSOF(THESAURUS, "' + dbo.fn_DropSpacesTabs(@TextoBuscar)+'")'
	--	--WHERE CONTAINS(CatName , 'FORMSOF (THESAURUS, Jon)')  INFLECTIONAL
		
	--	INSERT	INTO @Organizacion (IdOrganizacion, IdVista)
	--	SELECT DISTINCT o.IdOrganizacion  , o.IdVista
	--	FROM	OrganizacionFullText o  (NOLOCK)
	--	JOIN	(	select  distinct IdOrganizacion  
	--				from	OrganizacionFullText  (NOLOCK)
	--				where	(@TextoBuscar IS NULL OR @TextoBuscar = '')
	--				or		CONTAINS(FullTextOrganizacion,  @TextoBuscar )
	--			)	b  on b.IdOrganizacion	= o.IdOrganizacion 	 
	--	WHERE	(	EXISTS 
	--				(	SELECT	1 
	--					FROM	@FiltroBusqueda fb 
	--					WHERE	fb.IdHomologacion = o.IdHomologacion 
	--					AND		fb.Texto		  = o.FullTextOrganizacion
	--				)
	--				OR NOT EXISTS (SELECT 1 FROM @FiltroBusqueda)
	--			)
	--end

	----> Buscar con vectorizacion
 --   IF  @ModoBuscar = 5
	--begin  
	--	--SELECT	@TextoBuscar = 'FORMSOF(THESAURUS, "' + dbo.fn_DropSpacesTabs(@TextoBuscar)+'")'
	--	--INSERT	INTO @Organizacion (IdOrganizacion)
	--	--SELECT o.IdOrganizacion --, OFT.RANK  
	--	--FROM OrganizacionFullText AS o   
	--	--INNER JOIN FREETEXTTABLE(OrganizacionFullText, FullTextOrganizacion,  'chocolate' ) as OFT
	--	--	--,LANGUAGE N'English', 2) AS OFT  
	--	--ON o.IdOrganizacionFullText = OFT.[KEY]  
	--	--ORDER BY RANK DESC;  
	--	--SELECT	@TextoBuscar = isnull(@TextoBuscar,'')
	--	INSERT	INTO @Organizacion (IdOrganizacion, IdVista)
	--	SELECT	o.IdOrganizacion  , o.IdVista  --, OFT.RANK  
	--	FROM	OrganizacionFullText o  (NOLOCK)
	--	JOIN	FREETEXTTABLE(OrganizacionFullText, FullTextOrganizacion,  @TextoBuscar ) as OFT
	--	--JOIN	FREETEXTTABLE(OrganizacionFullText.IdOrganizacion, FullTextOrganizacion,  @TextoBuscar ) as OFT
	--		--,LANGUAGE N'English', 2) AS OFT  
	--	ON	o.IdOrganizacionFullText	= OFT.[KEY]  
	--	--AND o.IdOrganizacion			= OFT.IdOrganizacion 	 
	--	--WHERE	(	EXISTS 
	--	--			(	SELECT	1 
	--	--				FROM	@FiltroBusqueda fb 
	--	--				WHERE	fb.IdHomologacion = o.IdHomologacion 
	--	--				AND		fb.Texto		  = o.FullTextOrganizacion
	--	--			)
	--	--			OR NOT EXISTS (SELECT 1 FROM @FiltroBusqueda)
	--	--		)
	--	ORDER BY RANK DESC;  
	--end

	IF  (@PageNumber = 1)
		SELECT  @RowsTotal = COUNT(*) --FROM @DataLakeOrgBusqueda
		FROM	DataLakeOrganizacion O (NOLOCK)
		JOIN	@Organizacion		 B ON B.IdOrganizacion = O.IdOrganizacion
		WHERE	O.Estado = 'A'
		AND		IdHomologacionEsquema = @IdHomologacionEsquema

	SELECT  DISTINCT
			 O.IdOrganizacion 
			,O.IdVista 
			,O.IdHomologacionEsquema
			,O.DataEsquemaJson
	FROM	DataLakeOrganizacion O WITH (NOLOCK)
	JOIN	@Organizacion B ON B.IdOrganizacion = O.IdOrganizacion
	WHERE	O.Estado = 'A'
	AND		B.IdVista = O.IdVista
	AND		IdHomologacionEsquema = @IdHomologacionEsquema
	ORDER BY O.IdOrganizacion  
	OFFSET (@PageNumber - 1) * @RowsPerPage ROWS
	FETCH NEXT @RowsPerPage ROWS ONLY;
END;
GO

--ModoBuscar 
--0 = Buscar exacta 
--1 = Buscar palabra

--2 = Buscar frase
--3 = Buscar por palabras

--4 = Buscar con sinonimos    -->  leche (existe BD)    ----  lacteo (existir el sinonimo BD)
--5 = Buscar vectorizacion

-- exec psBuscarPalabra N'{		 
--		 "ModoBuscar"			:3
--		,"TextoBuscar"			:"salmonella"
--		,"FiltroPais"			:["ecuador"]
--		,"FiltroOna"			:[]
--		,"FiltroEsquema"		:[]
--		,"FiltroNorma"			:[]
--		,"FiltroEstado"			:["acreditado"]
--		,"FiltroRecomocimiento"	:["nacional"]
--}',1,10;


--UPDATE Conexion set Migrar ='S'
--GO
--UPDATE Homologacion
--set NombreHomologado ='OrgEstadoAcreditado'
--where IdHomologacion = 120
--go


--	SELECT	DISTINCT *
--	FROM	OrganizacionFullText
--	--WHERE  FullTextOrganizacion like 'laboratorio de calificación de leche cruda' 
--	WHERE	CONTAINS(FullTextOrganizacion,  '"cocoa"' )
	
--	SELECT	DISTINCT *
--	FROM	OrganizacionFullText
--	--WHERE  FullTextOrganizacion like 'laboratorio de calificación de leche cruda' 
--	WHERE	CONTAINS(FullTextOrganizacion,  '"chocolate"' )
	
--SELECT *
--FROM OrganizacionFullText
--WHERE CONTAINS(FullTextOrganizacion, 'FORMSOF(THESAURUS, "cocoa")');
 

 -- EXEC sys.sp_fulltext_load_thesaurus_file 3082, @loadOnlyIfNotLoaded = 1;
 --	SELECT	 *
	--FROM	OrganizacionFullText
	
 
	--SELECT o.*, OFT.RANK  
	--FROM OrganizacionFullText AS o   
	--INNER JOIN FREETEXTTABLE(OrganizacionFullText, FullTextOrganizacion,  'ecuador' ) as OFT
	--	--,LANGUAGE N'English', 2) AS KEY_TBL  
	--ON o.IdOrganizacionFullText = OFT.[KEY]  
	--ORDER BY RANK DESC;  
	--GO  




EXEC dbo.setDiccionario	'dbo.vwFiltro					', NULL ,'vista para los filtros de la pagina principal'
EXEC dbo.setDiccionario	'dbo.vwDimension				', NULL ,'vista para las dimensiones o campos homologados'
EXEC dbo.setDiccionario	'dbo.vwGrilla					', NULL ,'vita de esquema principal de la busqueda'
EXEC dbo.setDiccionario	'dbo.fnFiltroDetalle			', NULL ,'funcion para traer los detalles de cada filtro'	
EXEC dbo.setDiccionario	'dbo.fnHomologacionEsquemaCampo	', NULL ,'funcion para obtener los esquema y sus campos'				
EXEC dbo.setDiccionario	'dbo.fnHomologacionEsquema		', NULL ,'funcion para obtener un esquema'			
EXEC dbo.setDiccionario	'dbo.fnHomologacionEsquemaTodo	', NULL ,'funcion para obtener todos los esquema'				
EXEC dbo.setDiccionario	'dbo.fnHomologacionEsquemaDato	', NULL ,'funcion para obtener un esquema y sus datos'				
EXEC dbo.setDiccionario	'dbo.psBuscarPalabra			', NULL ,'procedimiento almacenado para la busqueda principal'				
go

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

