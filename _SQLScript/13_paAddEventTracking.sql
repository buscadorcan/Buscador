
--CREATE OR ALTER		 PROCEDURE [paAddEventTracking]
--     @TipoUsuario	  NVARCHAR(25)   
--    ,@NombreUsuario   NVARCHAR(100)  
--    ,@NombrePagina    NVARCHAR(100)  
--    ,@NombreControl   NVARCHAR(100)  
--    ,@NombreAccion    NVARCHAR(100)  
--    ,@UbicacionJson   NVARCHAR(MAX)
--    ,@ParametroJson   NVARCHAR(MAX) = NULL
--AS
----| Descipcion: Agrega un registro de seguimiento de eventos del usuario
--BEGIN
--	DECLARE @ErrorTracking NVARCHAR(max) =''
--    IF	ISJSON(@ParametroJson) <> 1
--		SELECT @ErrorTracking  = @ParametroJson, @ParametroJson = '{0}'

--	INSERT INTO [EventTracking] (TipoUsuario, NombreUsuario, NombrePagina, NombreControl, NombreAccion, UbicacionJson, ParametroJson, ErrorTracking)
--    VALUES (@TipoUsuario, @NombreUsuario, @NombrePagina, @NombreControl, @NombreAccion, @UbicacionJson, @ParametroJson, @ErrorTracking);
--END;
--GO


