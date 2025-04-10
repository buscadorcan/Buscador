DROP SEQUENCE IF EXISTS [dbo].[LogMigracionSequence];
CREATE SEQUENCE [dbo].[LogMigracionSequence]
  AS int
  START WITH 1
  INCREMENT BY 1 ;

-- ALTER TABLE "dbo"."LogMigracion" ALTER COLUMN Migracion int NOT NULL;
-- ALTER TABLE [dbo].[LogMigracion] ADD CONSTRAINT DF_LogMigracion_Migracion DEFAULT NEXT VALUE FOR [dbo].[LogMigracionSequence] FOR Migracion;
DROP TABLE if exists [dbo].[LogMigracion] ;
CREATE TABLE [dbo].[LogMigracion]  (
     IdLogMigracion		INT IDENTITY(1,1)						-- Identificador único para el registro del log
    ,IdConexion			INT         	NOT NULL            -- campo de la tabla conexcion.IdConexion
	,CodigoHomologacion	NVARCHAR(20)	NOT NULL                -- campo de la tabla conexcion.CodigoHomologacion
    ,Host               NVARCHAR(100)   NOT NULL                -- campo de la tabla conexcion.Host
    ,Puerto             INT             NOT NULL DEFAULT(0)     -- campo de la tabla conexcion.Puerto
    ,Usuario            NVARCHAR(100)   NOT NULL                -- campo de la tabla conexcion.Usuario
    ,Migracion			INT				NOT NULL DEFAULT NEXT VALUE FOR [dbo].[LogMigracionSequence] 		-- determina la secuencia de migración realizada
    ,Estado				NVARCHAR(10)    NOT NULL DEFAULT ('')	-- Estado de la migración (Ejemplo: 'OK', 'ERROR','START')
    ,OrigenFormato		NVARCHAR(10)    NOT NULL DEFAULT ('')	-- Nombre del formato-origen de los datos
    ,OrigenSistema		NVARCHAR(100)   NOT NULL DEFAULT ('')	-- Nombre del sistema que provee los datos o Base de datos
    ,OrigenVista		NVARCHAR(100)   NOT NULL DEFAULT ('')	-- Nombre de la vista de origen
    ,OrigenFilas		INT             NOT NULL DEFAULT 0		-- Número de registros a migrar
    ,EsquemaId			INT				NOT NULL DEFAULT 0		-- Id de  [dbo].[HomologacionEsquema]
    ,EsquemaVista		NVARCHAR(100)   NOT NULL DEFAULT ('')	-- Nombre de vista de HomologacionEsquema (VistaNombre) 
    ,EsquemaFilas		INT				NOT NULL DEFAULT 0		-- Número de registros migrados
    ,Tiempo				AS 
						CASE 
							WHEN Final IS NOT NULL THEN 
								CONVERT(VARCHAR, DATEDIFF(SECOND, Inicio, Final) / 3600) + ':' +
								RIGHT('0' + CONVERT(VARCHAR, (DATEDIFF(SECOND, Inicio, Final) % 3600) / 60), 2) + ':' +
								RIGHT('0' + CONVERT(VARCHAR, DATEDIFF(SECOND, Inicio, Final) % 60), 2)
							ELSE 
								NULL 
						END											-- Tiempo de migracion > 
    ,Inicio				DATETIME		NOT NULL DEFAULT GETDATE()	-- Fecha y hora de inicio de la migración
    ,Final				DATETIME		NOT NULL DEFAULT GETDATE()	-- Fecha y hora de finalización de la migración
    ,Fecha				DATETIME		NOT NULL DEFAULT GETDATE()	-- fecha de creación del registro
	,Observacion		NVARCHAR(MAX)	NOT NULL DEFAULT ('')		-- Detalle de cualquier error ocurrido de la migración
  
    ,CONSTRAINT  [PK_LM_IdLogMigracion]		PRIMARY KEY CLUSTERED (IdLogMigracion) 
    ,CONSTRAINT  [CK_LM_Estado]	            CHECK   (Estado			IN ('OK', 'ERROR','START','BATCH',''))
    ,CONSTRAINT  [CK_LM_OrigenFormato]		CHECK   (OrigenFormato	IN ('MYSQL', 'SQLSERVER', 'SQLLITE','EXCEL',''))
);


DROP TABLE if exists [dbo].[LogMigracionDetalle] ;
CREATE TABLE [dbo].[LogMigracionDetalle](
	 IdLogMigracionDetalle		INT IDENTITY(1,1)	NOT NULL 
	,IdLogMigracion		        INT             	NOT NULL 
	,NroMigracion				INT					NOT NULL 
	,EsquemaId					INT					NOT NULL 
	,EsquemaVista				[nvarchar](100)		NOT NULL 
	,EsquemaIdHomologacion		[nvarchar](10)	    NULL 
	,[IdHomologacion]			[int]				NULL 
	,[NombreHomologacion]		[nvarchar](90)		NULL 
	,OrigenVistaColumna			[sysname]			NULL
	,Fecha				        DATETIME		    NOT NULL DEFAULT GETDATE()	-- fecha de creación del registro
  
    ,CONSTRAINT  [PK_LMD_IdLogMigracionDetalle]		PRIMARY KEY CLUSTERED (IdLogMigracionDetalle) 
) ON [PRIMARY]
GO