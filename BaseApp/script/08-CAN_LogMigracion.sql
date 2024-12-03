DROP TABLE if exists [dbo].[LogMigracion] ;
CREATE TABLE [dbo].[LogMigracion]  (
    IdLogMigracion		INT IDENTITY(1,1)			-- Identificador único para el registro del log
    ,MigracionNumero	INT				NOT NULL DEFAULT 0 		    -- determina la secuencia de migración realizada
    ,MigracionEstado	NVARCHAR(10)    NOT NULL DEFAULT ('')		-- Estado de la migración (Ejemplo: 'OK', 'ERROR','START')
	,MigracionTipo 		NVARCHAR(10)    NOT NULL DEFAULT ('')      -- Si la migracion se realiza por: APP o SQLSERVER
    ,OrigenFormato		NVARCHAR(10)    NOT NULL DEFAULT ('')      -- Nombre del formato-origen de los datos
    ,OrigenSistema		NVARCHAR(100)   NOT NULL DEFAULT ('')     -- Nombre del sistema que provee los datos
    ,OrigenVista		NVARCHAR(100)   NOT NULL DEFAULT ('')     -- Nombre de la vista de origen
    ,OrigenTotalFila	INT             NOT NULL DEFAULT 0                    -- Número de registros a migrar
    ,IdHomologacionEsquema  INT         NOT NULL DEFAULT 0  
    ,VistaNombreEsquema		NVARCHAR(255)   NOT NULL DEFAULT ('')		-- Migrar segun nombre de vista de HomologacionEsquema (VistaNombre) 
    ,TotalFila			INT				NOT NULL DEFAULT 0                   -- Número de registros migrados
    ,TiempoInicio		DATETIME		NOT NULL DEFAULT GETDATE()          -- Fecha y hora de inicio de la migración
    ,TiempoFinal		DATETIME		NOT NULL DEFAULT GETDATE()          -- Fecha y hora de finalización de la migración
	,TiempoTotal        AS 
						CASE 
							WHEN TiempoFinal IS NOT NULL THEN 
								CONVERT(VARCHAR, DATEDIFF(SECOND, TiempoInicio, TiempoFinal) / 3600) + ':' +
								RIGHT('0' + CONVERT(VARCHAR, (DATEDIFF(SECOND, TiempoInicio, TiempoFinal) % 3600) / 60), 2) + ':' +
								RIGHT('0' + CONVERT(VARCHAR, DATEDIFF(SECOND, TiempoInicio, TiempoFinal) % 60), 2)
							ELSE 
								NULL 
						END
    ,MensageError		NVARCHAR(MAX) NOT NULL DEFAULT ('')              -- Detalle de cualquier error ocurrido de la migración
    ,FechaCreacion		DATETIME	NOT NULL DEFAULT GETDATE()
  
    ,CONSTRAINT  [PK_LM_IdLogMigracion]		PRIMARY KEY CLUSTERED (IdLogMigracion) 
    ,CONSTRAINT  [CK_LM_MigracionEstado]	CHECK   (MigracionEstado	IN ('OK', 'ERROR','START','BATCH',''))
    ,CONSTRAINT  [CK_LM_MigracionTipo]		CHECK   (MigracionTipo		IN ('APP', 'SQLSERVER',''))
    ,CONSTRAINT  [CK_LM_OrigenFormato]		CHECK   (OrigenFormato		IN ('MYSQL', 'SQLSERVER', 'SQLLITE','EXCEL',''))
);