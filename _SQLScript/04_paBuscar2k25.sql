/*----------------------------------------------------------------------------------------\
|    ©Copyright 2K25					                          BUSCADOR ANDINO		  |
|-----------------------------------------------------------------------------------------|
| Este código está protegido por las leyes y tratados internacionales de derechos de autor|
\-----------------------------------------------------------------------------------------/
  [App]				: Buscador Andino											
	- Date          : 2K24.FEB.25	
	- Author        : patricio.paccha														
	- Version	    : 1.0										 
	- Description   : Función de busqueda (exacta, RANK +  INFLECTIONAL + THESAURUS - stopWord  ) + filtros
\----------------------------------------------------------------------------------------*/

CREATE OR ALTER PROCEDURE [dbo].[paBuscar2k25] (	
--| 2K25.FEB.05 | patricio.paccha | BUSCADOR ANDINO | Versión: 1.0 
--| Descripción:  Función de busqueda (exacta, RANK +  INFLECTIONAL + THESAURUS - stopWord  ) + filtros
--DECLARE  
 @paramJSON			NVARCHAR(MAX) = '{}' 
,@PageNumber		INT = 1 
,@RowsPerPage		INT = 10 
,@RowsTotal		INT = 0				 OUTPUT 
,@vwPanelONAjson	NVARCHAR(MAX) = '{}' OUTPUT
) AS
BEGIN 
--DECLARE @paramJSON			NVARCHAR(MAX) = 
--N'{"ExactaBuscar"			:false
--	,"TextoBuscar"			:"agua"
--	,"FiltroPais"			:[]
--	,"FiltroOna"			:[]
--	,"FiltroEsquema"		:[]
--	,"FiltroNorma"			:[]
--	,"FiltroEstado"			:[]
--	,"FiltroRecomocimiento"	:[]
--}'
--,@PageNumber	INT = 1 
--,@RowsPerPage	INT = 1000 
--,@RowsTotal		INT = 0				  
--,@vwPanelONAjson	NVARCHAR(MAX) = '{}';

	IF  ISJSON(@paramJSON) = 1		
	BEGIN TRY	
		SELECT  @RowsTotal			=0;
		DECLARE @IdHomologacion		INT
		DECLARE @organizacion		TABLE	(IdEsquemaData  INT, PK NVARCHAR(400))
		DECLARE @IdEsquema			INTEGER=(SELECT TOP 1 IdEsquema FROM Esquema(NOLOCK) WHERE MostrarWebOrden = 1)
		DECLARE @EnteBuscado 		TABLE	(IdEsquemaData  INT, TipoBusqueda   VARCHAR(40), IdHomologacion INT,Texto NVARCHAR(4000) COLLATE Latin1_General_CI_AI, Ranking INT)   
		DECLARE @FiltrarPor			TABLE	(IdHomologacion INT default (0), CodigoHomologacion NVARCHAR(20),	Texto NVARCHAR(4000) COLLATE Latin1_General_CI_AI)
		DECLARE @EnteBuscadoUnico	TABLE	(RankTotal  INT, PK NVARCHAR(400),	  Texto NVARCHAR(4000) COLLATE Latin1_General_CI_AI)    
		DECLARE @TextoBuscar		NVARCHAR(200)	= lower(trim(isnull(JSON_VALUE(@paramJSON,'$.TextoBuscar'),'')))
		DECLARE @ExactaBuscar		BIT				= isnull(JSON_VALUE(@paramJSON, '$.ExactaBuscar'),0)
	END TRY
	BEGIN CATCH
		DECLARE  @ErrorNumber		INT = ERROR_NUMBER()
				,@ErrorLine			INT = ERROR_LINE()
		DECLARE  @FullErrorMessage	NVARCHAR(3000)	=	'> paBuscar2K25' + CHAR(13) + CHAR(10) +
														'* Error: '  + CAST(@ErrorNumber AS NVARCHAR(10)) + CHAR(13) + CHAR(10) +
														'* Line : '	 + CAST(@ErrorLine AS NVARCHAR(10))	 + CHAR(13) + CHAR(10) +
														'* Message: '+ ERROR_MESSAGE();
		THROW 50000, @FullErrorMessage, 1;
	END CATCH

	SElECT @TextoBuscar = dbo.fnDropSpacesTabs(isnull(@TextoBuscar,'')),  @ExactaBuscar = isnull(@ExactaBuscar,0)
	
	;WITH getFiltroParam_ AS
	(	SELECT	DISTINCT 0 IdHomologacion,'KEY_FIL_PAI' CodigoHomologacion, value  Texto  FROM OPENJSON(JSON_QUERY(@paramJSON, '$.FiltroPais'))		
		UNION	SELECT	DISTINCT 0 ,'KEY_FIL_ONA', value FROM OPENJSON(JSON_QUERY(@paramJSON, '$.FiltroOna'))			
		UNION	SELECT	DISTINCT 0 ,'KEY_FIL_ESQ', value FROM OPENJSON(JSON_QUERY(@paramJSON, '$.FiltroEsquema'))		
		UNION	SELECT	DISTINCT 0 ,'KEY_FIL_NOR', value FROM OPENJSON(JSON_QUERY(@paramJSON, '$.FiltroNorma'))		
		UNION	SELECT	DISTINCT 0 ,'KEY_FIL_EST', value FROM OPENJSON(JSON_QUERY(@paramJSON, '$.FiltroEstado'))		
		UNION	SELECT	DISTINCT 0 ,'KEY_FIL_REC', value FROM OPENJSON(JSON_QUERY(@paramJSON, '$.FiltroRecomocimiento'))
	)	INSERT  INTO	@FiltrarPor ( IdHomologacion , CodigoHomologacion, Texto )
		SELECT	h.IdHomologacion, f.CodigoHomologacion, f.Texto
		FROM	getFiltroParam_   f
		JOIN	(	SELECT  DISTINCT	hf.CodigoHomologacionFil, h.IdHomologacion
					FROM	HomologacionFiltro	hf(NOLOCK)
					JOIN	Homologacion		h (NOLOCK)	ON hf.IdHomologacionFiltro = h.IdHomologacionFiltro
				) h	ON h.CodigoHomologacionFil= f.CodigoHomologacion

	INSERT	INTO	 @organizacion
	SELECT	DISTINCT IdEsquemaData, PK 
	FROM	EsquemaOrganiza(NOLOCK)

	IF EXISTS(SELECT 1 FROM @FiltrarPor WHERE CodigoHomologacion='KEY_FIL_ESQ') 
	BEGIN
		SET		@IdHomologacion  = (SELECT TOP 1 IdHomologacion FROM @FiltrarPor WHERE CodigoHomologacion='KEY_FIL_ESQ')
		;WITH T AS 
		(	SELECT	DISTINCT a.IdEsquemaData
			FROM	@organizacion	 o
			JOIN	EsquemaFullText	 a  ON o.IdEsquemaData = a.IdEsquemaData
			WHERE	a.FullTextData   IN (SELECT texto from @FiltrarPor WHERE IdHomologacion=@IdHomologacion)
			AND		a.IdHomologacion = @IdHomologacion
		)	DELETE FROM @organizacion	WHERE IdEsquemaData NOT IN (SELECT  IdEsquemaData FROM T);
	END
	IF EXISTS(SELECT 1 FROM @FiltrarPor WHERE CodigoHomologacion='KEY_FIL_EST') 
	BEGIN
		SET		@IdHomologacion  = (SELECT TOP 1 IdHomologacion FROM @FiltrarPor WHERE CodigoHomologacion='KEY_FIL_EST')
		;WITH T AS 
		(	SELECT	DISTINCT a.IdEsquemaData
			FROM	@organizacion	 o
			JOIN	EsquemaFullText	 a  ON o.IdEsquemaData = a.IdEsquemaData
			WHERE	a.FullTextData   IN (SELECT texto from @FiltrarPor WHERE IdHomologacion=@IdHomologacion)
			AND		a.IdHomologacion = @IdHomologacion
		)	DELETE FROM @organizacion	WHERE IdEsquemaData NOT IN (SELECT  IdEsquemaData FROM T);
	END
	IF EXISTS(SELECT 1 FROM @FiltrarPor WHERE CodigoHomologacion='KEY_FIL_NOR') 
	BEGIN
		SET		@IdHomologacion  = (SELECT TOP 1 IdHomologacion FROM @FiltrarPor WHERE CodigoHomologacion='KEY_FIL_NOR')
		;WITH T AS 
		(	SELECT	DISTINCT a.IdEsquemaData
			FROM	@organizacion	 o
			JOIN	EsquemaFullText	 a  ON o.IdEsquemaData = a.IdEsquemaData
			WHERE	a.FullTextData   IN (SELECT texto from @FiltrarPor WHERE IdHomologacion=@IdHomologacion)
			AND		a.IdHomologacion = @IdHomologacion
		)	DELETE FROM @organizacion	WHERE IdEsquemaData NOT IN (SELECT  IdEsquemaData FROM T);
	END
	IF EXISTS(SELECT 1 FROM @FiltrarPor WHERE CodigoHomologacion='KEY_FIL_ONA') 
	BEGIN
		SET		@IdHomologacion  = (SELECT TOP 1 IdHomologacion FROM @FiltrarPor WHERE CodigoHomologacion='KEY_FIL_ONA')
		;WITH T AS 
		(	SELECT	DISTINCT a.IdEsquemaData
			FROM	@organizacion	 o
			JOIN	EsquemaFullText	 a  ON o.IdEsquemaData = a.IdEsquemaData
			WHERE	a.FullTextData   IN (SELECT texto from @FiltrarPor WHERE IdHomologacion=@IdHomologacion)
			AND		a.IdHomologacion = @IdHomologacion
		)	DELETE FROM @organizacion	WHERE IdEsquemaData NOT IN (SELECT  IdEsquemaData FROM T);
	END
	IF EXISTS(SELECT 1 FROM @FiltrarPor WHERE CodigoHomologacion='KEY_FIL_PAI') 
	BEGIN
		SET		@IdHomologacion  = (SELECT TOP 1 IdHomologacion FROM @FiltrarPor WHERE CodigoHomologacion='KEY_FIL_PAI')
		;WITH T AS 
		(	SELECT	DISTINCT a.IdEsquemaData
			FROM	@organizacion	 o
			JOIN	EsquemaFullText	 a  ON o.IdEsquemaData = a.IdEsquemaData
			WHERE	a.FullTextData   IN (SELECT texto from @FiltrarPor WHERE IdHomologacion=@IdHomologacion)
			AND		a.IdHomologacion = @IdHomologacion
		)	DELETE FROM @organizacion	WHERE IdEsquemaData NOT IN (SELECT  IdEsquemaData FROM T);
	END
	IF EXISTS(SELECT 1 FROM @FiltrarPor WHERE CodigoHomologacion='KEY_FIL_REC') 
	BEGIN
		SET		@IdHomologacion  = (SELECT TOP 1 IdHomologacion FROM @FiltrarPor WHERE CodigoHomologacion='KEY_FIL_REC')
		;WITH T AS 
		(	SELECT	DISTINCT a.IdEsquemaData
			FROM	@organizacion	 o
			JOIN	EsquemaFullText	 a  ON o.IdEsquemaData = a.IdEsquemaData
			WHERE	a.FullTextData   IN (SELECT texto from @FiltrarPor WHERE IdHomologacion=@IdHomologacion)
			AND		a.IdHomologacion =  @IdHomologacion
		)	DELETE FROM @organizacion	WHERE IdEsquemaData NOT IN (SELECT  IdEsquemaData FROM T);
	END

	IF  @TextoBuscar <> ''	
	BEGIN	 
		INSERT	INTO @EnteBuscado (IdEsquemaData, TipoBusqueda, IdHomologacion , Texto, Ranking)
		SELECT	DISTINCT IdEsquemaData  ,  'Exacta'	, IdHomologacion , FullTextData, 1000
		FROM	EsquemaFullText  (NOLOCK)
		WHERE	IdEsquemaData is not null
		AND		FullTextData  =  @TextoBuscar
		IF  @ExactaBuscar <> 1
		BEGIN	
			--> INFLECTIONAL + THESAURUS - stopWord 
			DECLARE @TextoBuscarInfle NVARCHAR(200) = ' FORMSOF(INFLECTIONAL, "' + @TextoBuscar +'" )'
			INSERT	INTO @EnteBuscado (IdEsquemaData, TipoBusqueda	,IdHomologacion , Texto , Ranking)
			SELECT  DISTINCT o.IdEsquemaData  , 'Inflec'		,o.[IdHomologacion] ,o.[FullTextData]  , 500 
			FROM	EsquemaFullText  (NOLOCK) o 
			WHERE	CONTAINS(FullTextData,  @TextoBuscarInfle )
			--> RANK + THESAURUS
			INSERT	INTO @EnteBuscado (IdEsquemaData, TipoBusqueda ,IdHomologacion , Texto , Ranking)
			SELECT	DISTINCT o.IdEsquemaData  , 'RankThesa'		,o.[IdHomologacion] ,o.[FullTextData]  , ftt.RANK  
			FROM	EsquemaFullText  (NOLOCK) o 
			JOIN	FREETEXTTABLE(EsquemaFullText, FullTextData, @TextoBuscar) as ftt		--,LANGUAGE N'English', 2) AS ftt  
			ON		o.IdEsquemaFullText		= ftt.[KEY]			--AND o.IdEsquemaData	= ftt.IdEsquemaData 	 
			--ORDER BY RANK DESC; 
		END
	END
	--

	;WITH EnteBuscadoRank AS
	(
		SELECT IdEsquemaData, sum(Ranking) RankTotal
		FROM @EnteBuscado
		GROUP BY IdEsquemaData
	)
	INSERT  INTO @EnteBuscadoUnico 
	SELECT	SUM (RankTotal) RankTotal, pk , '' -- TEXTO
	FROM (
			SELECT  DISTINCT RankTotal, o.PK   			-- PADRES-ORG   o.IdEsquemaData
			FROM	@organizacion	o
			JOIN	EnteBuscadoRank	e	ON  e.IdEsquemaData = o.IdEsquemaData
			UNION
			SELECT  DISTINCT RankTotal, d.VistaFK	-- HIJOS   d.IdEsquemaData 
			FROM	EnteBuscadoRank	e	
			JOIN	EsquemaData		d(NOLOCK) ON  e.IdEsquemaData  = d.IdEsquemaData
			JOIN	@organizacion	o ON o.PK = d.VistaFK 
	)  T	GROUP BY PK
	 

	IF  (@PageNumber = 1)
		WITH vwPanelONA AS
		(	
			SELECT DISTINCT	 n.Siglas				Sigla 
							,h.MostrarWeb			Pais 
							,isnull(n.UrlIcono,'')	Icono 
							,count(e.PK)			NroOrg
			FROM		@EnteBuscadoUnico	e 	
			INNER JOIN	EsquemaOrganiza		o(NOLOCK)ON	o.PK				= e.PK
			RIGHT JOIN  ONA					n(NOLOCK)ON o.ONAIdONA		= n.IdONA
			INNER JOIN  Homologacion		h(NOLOCK)ON h.IdHomologacion= n.IdHomologacionPais
			GROUP BY n.Siglas, h.MostrarWeb, isnull(n.UrlIcono,'')
		)	SELECT   @RowsTotal		= ( SELECT SUM(NroOrg) FROM vwPanelONA )
					,@vwPanelONAjson= ( SELECT Sigla, Pais, Icono, NroOrg FROM vwPanelONA FOR JSON AUTO ); 

	--> returnGrilla
	SELECT  DISTINCT
			 o.ONAIdONA				IdONA	
			,o.ONASiglas			Siglas	 
			,@TextoBuscar			Texto	 
			,o.VistaPK 
			,o.VistaFK 
			,o.IdEsquema
			,o.IdEsquemaVista
			,o.IdEsquemaData 
			,o.DataEsquemaJson	
			,e.RankTotal
	FROM	@EnteBuscadoUnico	e
	JOIN	EsquemaOrganiza		o(NOLOCK)ON  o.PK  = e.PK
	ORDER BY e.RankTotal 
	OFFSET (@PageNumber - 1) * @RowsPerPage ROWS
	FETCH NEXT @RowsPerPage ROWS ONLY;
END