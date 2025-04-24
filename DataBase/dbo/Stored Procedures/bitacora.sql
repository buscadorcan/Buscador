
create   procedure [dbo].[bitacora]  
/* dbo.bitacora
/-----------------------------------------------------------------------------------------\
|    ©Copyright 2K24												BUSCADOR ANDINO		  |
|-----------------------------------------------------------------------------------------|
| Este código está protegido por las leyes y tratados internacionales de derechos de autor|
\-----------------------------------------------------------------------------------------/
  [App]            : Bitacora de auditoria de scripts									
	- Date         : 2K24.JUN.20													    
	- Author       : patricio.paccha													
	- Description  : Usar solo para la ejecucuión de los script sql 
\----------------------------------------------------------------------------------------*/
(  @Text nvarchar(900)= null,	
   @msg	 nvarchar(900)= null
)  AS
BEGIN
    IF OBJECT_ID ('dbo.LogScript', 'U') IS NULL 
	BEGIN
		PRINT 'No existe la tabla dbo.LogScript para el registro de la bitácora de scrips'
		RETURN 0;
	END
    IF @Text is null and  @msg is null
    BEGIN
        select * from dbo.LogScript (nolock) 
        where IdLogTrace = (select TOP (1) IdLogTrace from dbo.LogScript (nolock) where StateLog = '@script' ORDER BY IdLogScript DESC)
		AND	  StateLog  <> '@script'
        print 'Ejemplo:'
        print 'exec dbo.bitacora "@script", "nombre_del_script.sql"			-- Obligatorio al inicio del script'
        print 'exec dbo.bitacora "MENSAJE" (poner luego de la acción ya realizada-finalizada) '
        print 'exec dbo.bitacora "@error" , "Message del error"				-- en el caso de errores'
        print 'exec dbo.bitacora											-- colocar al final para ver la pila de ejecucion'
        RETURN 0;
    END
    ----------------------------------------------------------------------------------------
    DECLARE  @NameScript NVARCHAR(500)	 =''
            ,@StrTime    NVARCHAR(10)    ='00:00:00'
            ,@TimeLog    datetime        = GETDATE()
            ,@OkError    NVARCHAR(20)    ='OK'
			,@IdLogTrace BIGINT			 = 0

    If @Text= '@error'   SELECT  @OkError ='ERROR',  @Text = @msg
    
	SELECT TOP (1)   @NameScript= TextLog
					,@IdLogTrace= IdLogTrace 
					,@TimeLog	= (SELECT MAX(TimeLog) FROM dbo.LogScript (NOLOCK))
	FROM dbo.LogScript (NOLOCK)	WHERE StateLog = '@script' ORDER BY IdLogScript 
	 
    if @Text = '@script' -- add new exec
    BEGIN
		IF @IdLogTrace = 0
        		INSERT into dbo.LogScript (StateLog ,TextLog, IdLogTrace) values (@Text,@msg,@IdLogTrace + 1)
        ELSE	UPDATE	dbo.LogScript 
				SET		 TextLog	= @msg 
						,TimeLog	= GETDATE()
						,IdLogTrace	= @IdLogTrace + 1
				WHERE	StateLog	= @Text
		SELECT  @NameScript	= @msg  , @Text	= 'Iniciando...', @IdLogTrace	= @IdLogTrace + 1 ,@TimeLog = GETDATE()
    END
    set @strTime = substring(convert( nvarchar(20), getdate()-isnull(@TimeLog,getdate()), 20),12,8) 
    INSERT INTO dbo.LogScript(StateLog,TextLog,TimeRun,NameScript, IdLogTrace) 
    SELECT  @OkError, @Text, @strTime, @NameScript , @IdLogTrace
END
