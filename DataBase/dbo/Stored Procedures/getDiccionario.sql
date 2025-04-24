
create   procedure [dbo].[getDiccionario]
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

GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Procedimiento para obtener el diccionario de datos', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'getDiccionario';

