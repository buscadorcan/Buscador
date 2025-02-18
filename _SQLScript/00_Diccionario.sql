
/* 
 * Copyright © SIDESOFT | BuscadorAndino | 2025.feb.18
 * Cleaning: limpieza de objetos de base de datos
 */
 
drop table if exists ds;
drop table if exists [BK_Usuario];
drop table if exists EsquemaFullText_BK;
drop table if exists LogScript;
drop procedure if exists bitacora;
drop procedure if exists [setDiccionario];
drop procedure if exists [getDiccionario];
GO

CREATE OR ALTER PROCEDURE [dbo].[paGetDiccionario] ( @Entities NVARCHAR(max) = NULL )AS
/* Copyright © SIDESOFT | BuscadorAndino | patricio.paccha | 2025.feb.18 
** paGetDiccionario: Obtiene la documentación de las entidades de una base de datos
*/
BEGIN
	PRINT 'Ejemplo:	'
	PRINT 'exec [paGetDiccionario] ''Schema.Entity'' '
	PRINT 'exec [paGetDiccionario] ''Schema.Entity1, Schema.Entity2, ...'' '

	SET @entities = ISNULL( @entities, '')				-- Eliminar NULL
	SET @entities = REPLACE(@entities, CHAR(10), '');	-- Eliminar LF
	SET @entities = REPLACE(@entities, CHAR(13), '');	-- Eliminar CR
	SET @entities = REPLACE(@entities, CHAR(9), '');	-- Eliminar Tab
	SET @entities = REPLACE(@entities, ' ', '');		-- Eliminar Espacios
	if	@entities	= ''
		RETURN 0;
	
	DECLARE @entity_ TABLE ( Id_			int primary key identity(1,1)
							,Schema_		nvarchar(150)	default('') NOT NULL
							,Entity_		nvarchar(150)	default('') NOT NULL
							,Entity_type_	nvarchar(150)	default('') NOT NULL
							,Valid_			bit				default(0) NOT NULL)
	
	INSERT INTO @entity_ (Schema_, Entity_)
	SELECT	RTRIM(LTRIM(SUBSTRING(value, 0, CHARINDEX('.', value))))				AS Schema_,
			RTRIM(LTRIM(SUBSTRING(value, CHARINDEX('.', value) + 1, LEN(value))))	AS Entity_
	FROM	STRING_SPLIT(@entities, ',');

	UPDATE e
	SET Valid_ = 1,
		Entity_type_ = CASE 
			WHEN o.type = 'U' THEN 'TABLE'
			WHEN o.type = 'V' THEN 'VIEW'
			WHEN o.type = 'P' THEN 'PROCEDURE'
			WHEN o.type IN ('FN', 'IF', 'TF') THEN 'FUNCTION'
			ELSE ''
		END
	FROM @entity_ e
	JOIN sys.objects o (NOLOCK) 
		ON o.name = e.Entity_
		AND schema_name(o.schema_id) = e.Schema_;

	--select * from @entity_
	select	TABLE_CATALOG+'.'+TABLE_SCHEMA +'.'+TABLE_NAME		OBJETO_NOMBRE
			,Entity_type_										OBJETO_TIPO
			,''													DATO_TIPO	
			,''													DATO_LONGITUD
			,(SELECT value from fn_listextendedproperty(N'MS_Description', N'SCHEMA', Schema_, Entity_type_, Entity_, null, null ) ) DESCRIPCION
	from	INFORMATION_SCHEMA.TABLES t with (nolock)
	join	@entity_ e on 	t.table_name = e.Entity_
	where	e.Valid_ = 1
	union
	Select	TABLE_CATALOG+'.'+TABLE_SCHEMA +'.'+TABLE_NAME+'.'+COLUMN_NAME	 
			,'COLUMN'	 
			,DATA_TYPE	
			,CHARACTER_MAXIMUM_LENGTH
			,(SELECT value from fn_listextendedproperty(N'MS_Description', N'SCHEMA', Schema_, Entity_type_, Entity_, N'COLUMN', c.column_name) )   
	from	INFORMATION_SCHEMA.COLUMNS c with (nolock)
	join	@entity_ e on 	c.table_name = e.Entity_
	where	e.Valid_ = 1
	union
	select	r.ROUTINE_CATALOG + '.' + r.ROUTINE_SCHEMA + '.' + r.ROUTINE_NAME  
			,Entity_type_	 
			,''    
			,''    
			,ep.value    
	from	INFORMATION_SCHEMA.ROUTINES r with (nolock)
	join	@entity_ e ON r.ROUTINE_NAME = e.Entity_ AND r.ROUTINE_SCHEMA = e.Schema_
	LEFT JOIN sys.extended_properties ep ON ep.major_id = OBJECT_ID(r.ROUTINE_SCHEMA + '.' + r.ROUTINE_NAME)
			AND ep.name = 'MS_Description'
	WHERE	e.Valid_ = 1 AND r.ROUTINE_TYPE = Entity_type_;
END;

GO

CREATE OR ALTER PROCEDURE [dbo].[paSetDiccionario]
(	@schemaEntity_i	nvarchar(150) = null,
	@column_i		nvarchar(150) = null,
	@coment_i		nvarchar(550) = null 
)AS
/* Copyright © SIDESOFT | BuscadorAndino | patricio.paccha | 2025.feb.18 
** paSetDiccionario:	Documenta las entidades de una base de datos
*/
BEGIN
	IF	isnull(@schemaEntity_i,'')	='' 
	OR	isnull(@coment_i,'')		=''
	OR	(select count(*) from STRING_SPLIT(@schemaEntity_i , '.')) <> 2 
    BEGIN
		PRINT 'Ejemplo:	'
		PRINT 'exec [paSetDiccionario]   Schema.Entity    ,null		,Coment'
		PRINT 'exec [paSetDiccionario]   Schema.Entity    ,Column	,Coment'
		RETURN 0;
	END

	declare  @schema_i	nvarchar(150),
			 @entity_i	nvarchar(150)
	
	;WITH SchemaEntity AS 
	(	SELECT value AS Item, ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS idItem
	    FROM STRING_SPLIT(@schemaEntity_i, '.')
	)	
	SELECT	 @schema_i = (select Item  from SchemaEntity where idItem = 1 )
			,@entity_i = (select Item  from SchemaEntity where idItem = 2 )
			,@coment_i = REPLACE(rtrim(ltrim(@coment_i)), char(9), '')
			,@column_i = REPLACE(rtrim(ltrim(@column_i)), char(9), '')

	IF	@schema_i	= ''  
	OR	@entity_i	= '' 
	OR	@coment_i	= '' 
		RETURN 0;
	 
	declare @entity_type nvarchar(50) = ''
	SET @entity_type = (SELECT TOP (1) case when type	= 'U' then 'TABLE'
											when type	= 'V' then 'VIEW'
											when type	= 'P' then 'PROCEDURE'
											when type	in ('FN', 'IF', 'TF') then 'FUNCTION'
										else ''
										END
						FROM	sys.objects with (nolock)
						WHERE	schema_name(schema_id) = @schema_i and name = @entity_i  )
 
	if @entity_type IS NULL OR @entity_type =''  
	begin
		print '(Error) No existe : ' + @schema_i + '.' + @entity_i
		RETURN 0;
	end

	begin try
		if @entity_type in ('TABLE', 'VIEW', 'PROCEDURE', 'FUNCTION') and  @column_i is null	
			if exists(select value from fn_listextendedproperty(N'MS_Description', N'SCHEMA', @schema_i, @entity_type, @entity_i, null, null)) 
					EXECUTE sp_updateextendedproperty	N'MS_Description', @coment_i, N'SCHEMA', @schema_i, @entity_type, @entity_i
			else	EXECUTE sp_addextendedproperty		N'MS_Description', @coment_i, N'SCHEMA', @schema_i, @entity_type, @entity_i
		else
			if exists(select value from fn_listextendedproperty(N'MS_Description', N'SCHEMA', @schema_i, @entity_type, @entity_i, N'COLUMN', @column_i)) 
					EXECUTE sp_updateextendedproperty	N'MS_Description', @coment_i, N'SCHEMA', @schema_i, @entity_type, @entity_i, N'COLUMN', @column_i
			else	EXECUTE sp_addextendedproperty		N'MS_Description', @coment_i, N'SCHEMA', @schema_i, @entity_type, @entity_i, N'COLUMN', @column_i
			
		print '(Ok) [paSetDiccionario] ' + @entity_type +' : '+ @schema_i + '.' + @entity_i + '.' + isnull(@column_i,'')
	end try
	begin catch
		if  @@TRANCOUNT > 0	
			rollback
		print '(Error) [paSetDiccionario] ' + @entity_type +' : '+ @schema_i + '.' + @entity_i + '.' + isnull(@column_i,'')
		print cast(ERROR_NUMBER() as varchar) + ', ' + ERROR_MESSAGE();
	end catch
end;
