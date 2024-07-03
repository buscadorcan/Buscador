/*----------------------------------------------------------------------------------------\
|    ©Copyright 2K24												BUSCADOR ANDINO		  |
|-----------------------------------------------------------------------------------------|
| Este código está protegido por las leyes y tratados internacionales de derechos de autor|
\-----------------------------------------------------------------------------------------/
  [App]            : Buscador Andino											
	- Date         : 2K24.JUN.25	
	- Author       : patricio.paccha														
	- Version	   : 1.0										
	- Description  : Crea el diccionario de datos para el buscador andino
\----------------------------------------------------------------------------------------*/
USE CAN_DB
GO

create or alter FUNCTION dbo.fn_Split
/* dbo.fn_Split
/-----------------------------------------------------------------------------------------\
|    ©Copyright 2K24												BUSCADOR ANDINO		  |
|-----------------------------------------------------------------------------------------|
| Este código está protegido por las leyes y tratados internacionales de derechos de autor|
\-----------------------------------------------------------------------------------------/
  [App]            : Diccionario de datos											
	- Date         : 2K24.JUN.20													
	- Author       : patricio.paccha														
	- Description  : Divide una cadena de caracteres dado un caracter
\----------------------------------------------------------------------------------------*/
(
    @String NVARCHAR(MAX),
    @Delimiter NCHAR(1)
)
RETURNS @OutputTable TABLE 
(
	IdItem	INT primary key identity(1,1),
    Item	NVARCHAR(500) NOT NULL
)
AS
BEGIN
    DECLARE @StartIndex INT, @EndIndex INT

    SET @StartIndex = 1
    IF SUBSTRING(@String, LEN(@String), 1) <> @Delimiter
        SET @String = @String + @Delimiter

    WHILE CHARINDEX(@Delimiter, @String, @StartIndex) > 0
    BEGIN
        SET @EndIndex	= CHARINDEX(@Delimiter, @String, @StartIndex)
        INSERT INTO @OutputTable(Item)
        SELECT	RTRIM(LTRIM(SUBSTRING(@String, @StartIndex, @EndIndex - @StartIndex))) 
        SET @StartIndex = @EndIndex + 1
    END
    RETURN
END;
GO

create or alter procedure dbo.setDiccionario
/* dbo.setDiccionario
/-----------------------------------------------------------------------------------------\
|    ©Copyright 2K24												BUSCADOR ANDINO		  |
|-----------------------------------------------------------------------------------------|
| Este código está protegido por las leyes y tratados internacionales de derechos de autor|
\-----------------------------------------------------------------------------------------/
  [App]            : Diccionario de datos											
	- Date         : 2K24.JUN.20													
	- Author       : patricio.paccha														
	- Description  : Documenta las entidades de una base de datos
\----------------------------------------------------------------------------------------*/
(
    @schemaEntity_i	nvarchar(150) = null,
    @column_i		nvarchar(150) = null,
    @coment_i		nvarchar(550) = null
)
as
BEGIN
	if	@schemaEntity_i	is null or @coment_i is NULL
    BEGIN
		PRINT 'Ejemplo:	'
		PRINT 'exec [setDiccionario]   Schema.Entity    ,null		,Coment'
		PRINT 'exec [setDiccionario]   Schema.Entity    ,Column		,Coment'
		RETURN 0;
	END

	declare  @schema_i	nvarchar(150),
			 @entity_i	nvarchar(150)

	if not exists( select  1  from fn_Split(@schemaEntity_i , '.') where idItem = 2 )
	begin
		print '(Error) El parametro debe tener un formato schema.entity, Ejemplo: dbo.tabla'
		RETURN 0;
	end 
	
	select  @schema_i = item  from fn_Split(@schemaEntity_i , '.') where idItem = 1
	select  @entity_i = item  from fn_Split(@schemaEntity_i , '.') where idItem = 2
	select  @coment_i = REPLACE(rtrim(ltrim(@coment_i)), char(9), '')
	select  @column_i = REPLACE(rtrim(ltrim(@column_i)), char(9), '')

	if	@schema_i	= '' or
		@entity_i	= '' or
		@coment_i	= '' 
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
			
		print '(Ok) [setDiccionario] ' + @entity_type +' : '+ @schema_i + '.' + @entity_i + '.' + isnull(@column_i,'')
	end try
	begin catch
		if  @@TRANCOUNT > 0	
			rollback
		print '(Error) [setDiccionario] ' + @entity_type +' : '+ @schema_i + '.' + @entity_i + '.' + isnull(@column_i,'')
		print cast(ERROR_NUMBER() as varchar) + ', ' + ERROR_MESSAGE();
	end catch
end;
GO

create or alter procedure dbo.getDiccionario
/*  dbo.getDiccionario
/-----------------------------------------------------------------------------------------\
|    ©Copyright 2K24												BUSCADOR ANDINO		  |
|-----------------------------------------------------------------------------------------|
| Este código está protegido por las leyes y tratados internacionales de derechos de autor|
\-----------------------------------------------------------------------------------------/
  [App]            : Diccionario de datos											
	- Date         : 2K24.JUN.20													
	- Author       : patricio.paccha														
	- Description  : Obtiene la documentación de las entidades de una base de datos
\----------------------------------------------------------------------------------------*/
(
	@entities NVARCHAR(max) = NULL
)	as
begin
	PRINT 'Ejemplo:	'
	PRINT 'exec [setDiccionario]   "Schema.Entity" '
	PRINT 'exec [setDiccionario]   "Schema.Entity1, Schema.Entity2, ..." '
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
	SELECT RTRIM(LTRIM(SUBSTRING(Item, 0, CHARINDEX('.', Item))))				AS Schema_,
		   RTRIM(LTRIM(SUBSTRING(Item, CHARINDEX('.', Item) + 1, LEN(Item))))	AS Entity_
	FROM fn_Split(@entities, ',');

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

	select * from @entity_

	select	TABLE_CATALOG+'.'+TABLE_SCHEMA +'.'+TABLE_NAME		ENTIDAD
			,Entity_type_										TIPO_ENTIDAD
			,''													TIPO_DATO	
			,''													CARACTER_LONGITUD
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
end
Go