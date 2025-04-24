
create   procedure [dbo].[setDiccionario]
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
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Procedimiento para crear  el diccionario de datos', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'setDiccionario';

