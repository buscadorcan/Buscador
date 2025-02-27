
-- CREATE OR ALTER		 PROCEDURE [paAddEventTracking]
--    @IdUsuario        INT = 0  
--    ,@CodigoHomologacionRol	  NVARCHAR(25)  =''  
--    ,@CodigoHomologacionMenu   NVARCHAR(100)  
--    ,@NombreControl   NVARCHAR(100)  
--    ,@NombreAccion    NVARCHAR(100)  
--    ,@UbicacionJson   NVARCHAR(MAX)
--    ,@ParametroJson   NVARCHAR(MAX) = NULL
-- AS
-- --| Descipcion: Agrega un registro de seguimiento de eventos del usuario
-- BEGIN
-- 	DECLARE @ErrorTracking NVARCHAR(max) =''
--    IF	ISJSON(@ParametroJson) <> 1
-- 		SELECT @ErrorTracking  = @ParametroJson, @ParametroJson = '{0}'

-- 	INSERT INTO [EventTracking] (CodigoHomologacionRol, CodigoHomologacionMenu, IdUsuario,  NombreControl, NombreAccion, UbicacionJson, ParametroJson, ErrorTracking)
--    VALUES (@CodigoHomologacionRol, @CodigoHomologacionMenu, @IdUsuario,  @NombreControl, @NombreAccion, @UbicacionJson, @ParametroJson, @ErrorTracking);
-- END;
-- GO 


