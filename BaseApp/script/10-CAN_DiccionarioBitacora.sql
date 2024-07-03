/*----------------------------------------------------------------------------------------\
|    ©Copyright 2K24												BUSCADOR ANDINO		  |
|-----------------------------------------------------------------------------------------|
| Este código está protegido por las leyes y tratados internacionales de derechos de autor|
\-----------------------------------------------------------------------------------------/
  [App]            : MANUAL DE BITACORA DE SCRIPTS & DICCIONARIO DE DATOS											
	- Date         : 2K24.JUN.25	
	- Author       : patricio.paccha														
	- Version	   : 1.0										
	- Description  : Sugerencia para usar la bitacora de scripts y diccionario de datos 
	- Sugerencias:
		+ Todas las entidades deben ser documentadas (tablas, vistas, funciones y columnas)
		+ Todo script debe ser numerados, estandarizados: EPN-DGIP-00_APP_NombreScript.sql
		+ Todo script debe registrase en la bitácora (EXEC DBO.Bitacora '@script','00-CAN_ABC.sql)
		+ POner los mensajes en la bitácora luego de ejecutarlas, ejemplo:
		
		SELECT * FROM dbo.LogScript where idLogtrace = 1
		EXEC DBO.Bitacora 'buscando las acciones ejecutadas en el primer script ejecutado' 
\----------------------------------------------------------------------------------------*/

USE CAN_DB
GO

EXEC DBO.Bitacora '@script','02-CAN_DiccionarioBitacora.sql'


-- DOCUMENTACION 
----------------------------------------------------------------------------------------------
--	 Función
EXEC DBO.setDiccionario	'dbo.fn_Split'		,null	,'Función que divide cadena según un caracter'
--	 Procedimiento
EXEC DBO.setDiccionario	'dbo.setDiccionario' ,null	,'Procedimiento para crear  el diccionario de datos'
EXEC DBO.setDiccionario	'dbo.getDiccionario' ,null	,'Procedimiento para obtener el diccionario de datos'
--	 Tabla o Vistas
EXEC DBO.setDiccionario	'dbo.LogScript'	,NULL			,'Tabla para el registros de la bitacora de scripts'
--	 Columna
EXEC DBO.setDiccionario	'dbo.LogScript'	,'IdLogScript	','PK'
EXEC DBO.setDiccionario	'dbo.LogScript'	,'StateLog		','Indica si hay esta correcto o error'
EXEC DBO.setDiccionario	'dbo.LogScript'	,'TextLog		','mensaje de bitacora'
EXEC DBO.setDiccionario	'dbo.LogScript'	,'TimeRun		','tiempo de ejecucion del script'
EXEC DBO.setDiccionario	'dbo.LogScript'	,'NameScript	','nombre del script'
EXEC DBO.setDiccionario	'dbo.LogScript'	,'TimeLog		','fecha y tiempo en el que se ejecuta el script'
EXEC DBO.setDiccionario	'dbo.LogScript'	,'HostName		','Equipo donde se ejecuta el script'
EXEC DBO.setDiccionario	'dbo.LogScript'	,'LoggedInUser	','Usuario con el que corre el script'
EXEC DBO.setDiccionario	'dbo.LogScript'	,'IdLogTrace	','Indicador de numero de script ejecutados'
EXEC dbo.setDiccionario	'dbo.DataLake'	,'DataSistemaOrigen'	,'Columna de una tabla'
----------------------------------------------------------------------------------------------
EXEC DBO.Bitacora 'Se ha procedido a documentar la tabla de log de scripts (bitacora) y funciones del diccionario de datos'



--	DICCIONARIO DE DATOS
----------------------------------------------------------------------------------------------
EXEC DBO.GetDiccionario	@entities = 'dbo.LogScript'

EXEC DBO.GetDiccionario	@entities = 'dbo.setDiccionario
									,dbo.getDiccionario 
									,dbo.fn_Split'
----------------------------------------------------------------------------------------------
EXEC DBO.Bitacora 'Obteniendo el diccionario de datos recien documentado'


-- BITACOTA
SELECT * FROM dbo.LogScript
EXEC DBO.Bitacora 'Siempre poner los mensajes luego de realizar una accion ' 

----------------------------------------------------------------------------------------------
-- Ejemplo de como marcar un error en la bitacora
----------------------------------------------------------------------------------------------
BEGIN TRY
	-- ...
    -- error
    SELECT 8/0;
END TRY
BEGIN CATCH
    -- Manejo del error
    DECLARE @ErrorMessage NVARCHAR(4000), @ErrorSeverity INT, @ErrorState INT;
    SELECT  @ErrorMessage   = ERROR_MESSAGE(),
            @ErrorSeverity  = ERROR_SEVERITY(),
            @ErrorState     = ERROR_STATE();

    EXEC DBO.Bitacora '@error' , @ErrorMessage
END CATCH; 
----------------------------------------------------------------------------------------------

--[ Bitacora ]: Se puede poner esta linea al final para la bitacora 
----------------------------------------------------------------------------------------------
EXEC DBO.Bitacora 
