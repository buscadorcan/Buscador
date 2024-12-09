
/*----------------------------------------------------------------------------------------\
|    ©Copyright 2K24												BUSCADOR ANDINO		  |
|-----------------------------------------------------------------------------------------|
| Este código está protegido por las leyes y tratados internacionales de derechos de autor|
\-----------------------------------------------------------------------------------------/
  [App]            : Buscador Andino											
	- Date         : 2K24.JUN.25	
	- Author       : patricio.paccha														
	- Version	   : 1.0										
	- Description  : Tablas del buscador andino	para organizaciones certificadas
\----------------------------------------------------------------------------------------*/

USE CAN_DB;
GO

EXEC DBO.Bitacora '@script','03-CAN_Tabla.sql'

DROP TABLE if exists dbo.WebSiteLog;
DROP TABLE if exists dbo.OrganizacionPais;
DROP TABLE if exists dbo.Persona;
DROP TABLE if exists dbo.UsuarioEndPointWebPermiso;
DROP TABLE if exists dbo.Usuario;
DROP TABLE if exists dbo.EndPointWeb;
DROP TABLE if exists dbo.DataLakePersona;
DROP TABLE if exists dbo.DataLakeOrganizacion;
DROP TABLE if exists dbo.DataLake;
DROP TABLE if exists dbo.HomologacionEsquema;
DROP TABLE if exists dbo.Homologacion;
DROP TABLE if exists dbo.Conexion;
DROP TABLE if exists [dbo].[OrganizacionData];
DROP TABLE if exists [dbo].[OrganizacionFullText] ;
GO

EXEC DBO.Bitacora 'DROP TABLE if exists 
dbo.WebSiteLog
dbo.OrganizacionPais
dbo.Persona
dbo.UsuarioEndPointWebPermiso
dbo.Usuario
dbo.EndPointWeb
dbo.DataLakeOrganizacion
dbo.HomologacionEsquema
dbo.Homologacion
 dbo.Conexion '
GO

CREATE TABLE dbo.Usuario (
     IdUsuario				INT IDENTITY(1,1) 
    ,Email					NVARCHAR(100) NOT NULL
    ,Nombre					NVARCHAR(500) NOT NULL
    ,Apellido				NVARCHAR(500) NOT NULL DEFAULT('')
	,Telefono				NVARCHAR(20)  NOT NULL DEFAULT('')
    ,Clave					NVARCHAR(MAX) NOT NULL
    ,Rol					NVARCHAR(20)  NOT NULL DEFAULT('USER')
	,Estado					NVARCHAR(1) NOT NULL DEFAULT('A')
    ,FechaCreacion			DATETIME	NOT NULL DEFAULT(GETDATE())
    ,FechaModifica			DATETIME	NOT NULL DEFAULT(GETDATE())  
    ,IdUserCreacion			INT			NOT NULL DEFAULT(0)
    ,IdUserModifica			INT			NOT NULL DEFAULT(0)

    ,CONSTRAINT PK_U_IdUsuario	PRIMARY KEY CLUSTERED (IdUsuario)  
    ,CONSTRAINT UK_U_Email		UNIQUE (Email)
    ,CONSTRAINT CK_U_Rol		CHECK  (Rol IN ('ADMIN', 'USER'))
    ,CONSTRAINT CK_U_Estado		CHECK  (Estado IN ('A', 'X'))
);

CREATE TABLE dbo.EndPointWeb (
     IdEndPointWeb			INT IDENTITY(1,1)  
    ,UrlWeb					NVARCHAR(MAX)
    ,Nombre					NVARCHAR(100) NOT NULL
	,Estado					NVARCHAR(1) NOT NULL DEFAULT('A')
    ,FechaCreacion			DATETIME	NOT NULL DEFAULT(GETDATE())
    ,FechaModifica			DATETIME	NOT NULL DEFAULT(GETDATE())  
    ,IdUserCreacion			INT			NOT NULL DEFAULT(0)
    ,IdUserModifica			INT			NOT NULL DEFAULT(0)
    
	,CONSTRAINT PK_EPW_IdEndpoint	PRIMARY KEY CLUSTERED (IdEndPointWeb)  
    ,CONSTRAINT UK_EPW_Nombre		UNIQUE (Nombre)
    ,CONSTRAINT CK_EPW_Estado		CHECK  (Estado IN ('A', 'X'))
);

CREATE TABLE dbo.UsuarioEndPointWebPermiso (
     IdUsuarioEndPointWebPermiso    INT IDENTITY(1,1) 
    ,IdUsuario                      INT NOT NULL DEFAULT(0)
    ,IdEndPointWeb                  INT NOT NULL DEFAULT(0)
    ,Accion                         NVARCHAR(10) NOT NULL
	,Estado						    NVARCHAR(1) NOT NULL DEFAULT('A')
    ,FechaCreacion				    DATETIME	NOT NULL DEFAULT(GETDATE())
    ,FechaModifica			        DATETIME	NOT NULL DEFAULT(GETDATE())  
    ,IdUserCreacion				    INT			NOT NULL DEFAULT(0)
    ,IdUserModifica			        INT			NOT NULL DEFAULT(0)

	,CONSTRAINT PK_UEPWP_IdUsuarioEndpointPermiso	PRIMARY KEY CLUSTERED (IdUsuarioEndPointWebPermiso)  
    ,CONSTRAINT FK_UEPWP_IdUsuario		FOREIGN KEY (IdUsuario)		REFERENCES Usuario(IdUsuario)
    ,CONSTRAINT FK_UEPWP_IdEndPointWeb	FOREIGN KEY (IdEndPointWeb)	REFERENCES EndPointWeb(IdEndPointWeb)
    ,CONSTRAINT UK_UEPWP_Accion			CHECK (Accion IN ('GET', 'POST', 'PUT', 'DELETE'))
    ,CONSTRAINT CK_UEPWP_Estado			CHECK (Estado IN ('A', 'X'))
);

CREATE TABLE dbo.Homologacion (
     IdHomologacion     	INT IDENTITY(1,1) NOT NULL
    ,IdHomologacionGrupo	INT DEFAULT(NULL) 
	,Mostrar				CHAR(1) NOT NULL DEFAULT 'S' 
    ,MostrarWebOrden		INT DEFAULT(0) 
    ,MostrarWeb				NVARCHAR(90)  NOT NULL
	,TooltipWeb				NVARCHAR(200) NOT NULL DEFAULT('')
	,MascaraDato			NVARCHAR(10)  NOT NULL DEFAULT('TEXTO')
	,SiNoHayDato			NVARCHAR(10)  NOT NULL DEFAULT('')
    ,InfoExtraJson			NVARCHAR(max) NOT NULL DEFAULT('{}')
    ,CodigoHomologacion		NVARCHAR(20) NOT NULL DEFAULT('')
    ,NombreHomologado		NVARCHAR(90) NOT NULL DEFAULT('')
	,Estado					NVARCHAR(1)  NOT NULL DEFAULT('A')
    ,FechaCreacion			DATETIME	 NOT NULL DEFAULT(GETDATE())
    ,FechaModifica			DATETIME	 NOT NULL DEFAULT(GETDATE())  
    ,IdUserCreacion			INT			 NOT NULL DEFAULT(0)
    ,IdUserModifica			INT			 NOT NULL DEFAULT(0)  
	
	,CONSTRAINT  [PK_H_IdHomologacion] primary KEY CLUSTERED (IdHomologacion) 
    ,CONSTRAINT  [CK_H_InfoExtraJson]       CHECK   (ISJSON(InfoExtraJson) = 1 )
    ,CONSTRAINT  [CK_H_MascaraDato]			CHECK   (MascaraDato IN ('TEXTO', 'FECHA', 'NUMERICO'))
    ,CONSTRAINT  [CK_H_Estado]				CHECK   (Estado IN ('A', 'X'))
	,CONSTRAINT  [CK_H_Mostrar]				CHECK	(Mostrar IN ('S', 'N'))
);

CREATE TABLE dbo.HomologacionEsquema(
   IdHomologacionEsquema    INT IDENTITY(1,1) NOT NULL
  ,MostrarWebOrden			INT DEFAULT(1) NOT NULL
  ,MostrarWeb				NVARCHAR(200) NOT NULL DEFAULT('GRILLA')
  ,TooltipWeb				NVARCHAR(200) NOT NULL DEFAULT('')
  ,EsquemaJson				NVARCHAR(max) NOT NULL DEFAULT('{}')
  ,DataTipo				    NVARCHAR(15)  NOT NULL DEFAULT('NO_DEFINIDO')	
  ,VistaNombre				NVARCHAR(100) NOT NULL DEFAULT('')
  ,IdVistaNombre			NVARCHAR(100) NOT NULL DEFAULT('')
  ,Estado					NVARCHAR(1)	  NOT NULL DEFAULT('A')
  ,FechaCreacion			DATETIME	  NOT NULL DEFAULT(GETDATE())
  ,FechaModifica			DATETIME	  NOT NULL DEFAULT(GETDATE())  
  ,IdUserCreacion			INT			  NOT NULL DEFAULT(0)
  ,IdUserModifica			INT			  NOT NULL DEFAULT(0)  

  ,CONSTRAINT  [PK_HE_IdHomologacionEsquema]	PRIMARY KEY CLUSTERED (IdHomologacionEsquema) 
  ,CONSTRAINT  [CK_HE_EsquemaJson]				CHECK   (ISJSON(EsquemaJson) = 1 )
  ,CONSTRAINT  [CK_HE_Estado]				    CHECK   (Estado IN ('A', 'X'))
  ,CONSTRAINT  [UK_HE_DataTipo]					CHECK	(DataTipo IN ('ORGANIZACION', 'PERSONA','NO_DEFINIDO'))
  --,CONSTRAINT  UK_HE_VistaNombre		        UNIQUE (VistaNombre)
);

CREATE TABLE dbo.Conexion (
     IdConexion			INT IDENTITY(1,1)	NOT NULL
	,CodigoHomologacion	NVARCHAR(20)		NOT NULL
  ,Siglas             NVARCHAR(20)    NOT NULL
    ,BaseDatos          NVARCHAR(100) NOT NULL
    ,Host               NVARCHAR(100) NOT NULL
    ,Puerto             INT NOT NULL DEFAULT(0)
    ,Usuario            NVARCHAR(100) NOT NULL
    ,Contrasenia        NVARCHAR(100) NOT NULL
    ,MotorBaseDatos     NVARCHAR(100) NOT NULL
    ,Filtros            NVARCHAR(MAX) NOT NULL DEFAULT('{}')
    ,FechaConexion		DATETIME NOT NULL DEFAULT(GETDATE())
    ,TiempoEspera		INT NOT NULL DEFAULT(0)				-- Tiempo de espera en segundos
    ,Migrar				NVARCHAR(1) NOT NULL DEFAULT('S')
    
	,Estado				NVARCHAR(1) NOT NULL DEFAULT('A')
    ,FechaCreacion		DATETIME	NOT NULL DEFAULT(GETDATE())
    ,FechaModifica		DATETIME	NOT NULL DEFAULT(GETDATE())  
    ,IdUserCreacion		INT			NOT NULL DEFAULT(0)
    ,IdUserModifica		INT			NOT NULL DEFAULT(0)  
  
    ,CONSTRAINT  [PK_C_IdConexion]			PRIMARY KEY CLUSTERED (IdConexion) 
    ,CONSTRAINT  [CK_C_Estado]				CHECK   (Estado IN ('A', 'X'))
    ,CONSTRAINT  [CK_C_Migrar]				CHECK   (Migrar IN ('S', 'N'))
    ,CONSTRAINT  [CK_C_MotorBaseDatos]		CHECK   (MotorBaseDatos IN ('MYSQL', 'SQLSERVER', 'SQLLITE'))
    ,CONSTRAINT  [CK_C_Filtros]				CHECK   (ISJSON(Filtros) = 1 )
	,CONSTRAINT  [CK_C_CodigoHomologacion]	CHECK   (CodigoHomologacion IN 
													('KEY_ECU_SAE'
													,'KEY_COL_ONAC'
													,'KEY_PER_INACAL'
													,'KEY_BOL_DTA'	 ))
);

CREATE TABLE dbo.WebSiteLog (
     IdWebSiteLog		INT IDENTITY(1,1) NOT NULL
    ,TextoBusquedo      NVARCHAR(900) NOT NULL
    ,FiltroUsado        NVARCHAR(max) NOT NULL DEFAULT('{}')
	,FechaCreacion		DATETIME	NOT NULL DEFAULT(GETDATE())
	
	,CONSTRAINT  [PK_WSL_WebSiteLog]		PRIMARY KEY CLUSTERED (IdWebSiteLog) 
);

DROP TABLE if exists [dbo].[ONA] ;
CREATE TABLE [dbo].[ONA] (
     IdONA			    INT IDENTITY(1,1)
    ,IdHomologacion	    INT NOT NULL
    ,Siglas			    NVARCHAR(30)  NOT NULL
    ,RazonSocial		NVARCHAR(300) NOT NULL
    ,Pais			    NVARCHAR(100) NOT NULL
    ,Ciudad			    NVARCHAR(100) NOT NULL
    ,Direccion		    NVARCHAR(300) NOT NULL
    ,PaginaWeb		    NVARCHAR(300) NOT NULL
    ,Correo			    NVARCHAR(300) NOT NULL
    ,Telefono		    NVARCHAR(20)  NOT NULL
    ,UrlIcono		    NVARCHAR(300) NOT NULL
    ,UrlLogo			NVARCHAR(300) NOT NULL

    ,Estado				NVARCHAR(1) NOT NULL DEFAULT('A')
    ,FechaCreacion		DATETIME	NOT NULL DEFAULT(GETDATE())
    ,FechaModifica		DATETIME	NOT NULL DEFAULT(GETDATE())  
    ,IdUserCreacion		INT			NOT NULL DEFAULT(0)
    ,IdUserModifica		INT			NOT NULL DEFAULT(0)  
  
    ,CONSTRAINT  [PK_O_IdONA]		    PRIMARY KEY CLUSTERED (IdONA) 
    ,CONSTRAINT   FK_O_IdHomologacion	FOREIGN KEY (IdHomologacion) REFERENCES Homologacion(IdHomologacion)
    ,CONSTRAINT  [CK_O_Estado]		    CHECK   (Estado IN ('A', 'X'))
  
);


DROP TABLE if exists [dbo].[EsquemaVista] ;
CREATE TABLE [dbo].[EsquemaVista] (
     IdEsquemaVista			    INT IDENTITY(1,1)
    ,IdConexion                 INT NOT NULL
    ,IdHomologacionEsquema	    INT NOT NULL
    ,VistaNombreEsquema		    NVARCHAR(100) NOT NULL
    ,VistaNombreOrigen  		NVARCHAR(100) NOT NULL

    ,Estado				NVARCHAR(1) NOT NULL DEFAULT('A')
    ,FechaCreacion		DATETIME	NOT NULL DEFAULT(GETDATE())
    ,FechaModifica		DATETIME	NOT NULL DEFAULT(GETDATE())  
    ,IdUserCreacion		INT			NOT NULL DEFAULT(0)
    ,IdUserModifica		INT			NOT NULL DEFAULT(0)  
  
    ,CONSTRAINT  [PK_EV_IdEsquemaVista]		    PRIMARY KEY CLUSTERED (IdEsquemaVista) 
    ,CONSTRAINT   FK_EV_IdHomologacionEsquema	FOREIGN KEY (IdHomologacionEsquema) REFERENCES HomologacionEsquema(IdHomologacionEsquema)
    ,CONSTRAINT   FK_EV_IdConexion              FOREIGN KEY (IdConexion)				REFERENCES Conexion (IdConexion)
    ,CONSTRAINT  [CK_EV_Estado]					CHECK   (Estado IN ('A', 'X'))
  
);

DROP TABLE if exists [dbo].[LogMigracion] ;
CREATE TABLE [dbo].[LogMigracion]  (
     IdLogMigracion		INT IDENTITY(1,1)						-- Identificador único para el registro del log
    ,IdConexion			INT         	NOT NULL            -- campo de la tabla conexcion.IdConexion
	,CodigoHomologacion	NVARCHAR(20)	NOT NULL                -- campo de la tabla conexcion.CodigoHomologacion
    ,Host               NVARCHAR(100)   NOT NULL                -- campo de la tabla conexcion.Host
    ,Puerto             INT             NOT NULL DEFAULT(0)     -- campo de la tabla conexcion.Puerto
    ,Usuario            NVARCHAR(100)   NOT NULL                -- campo de la tabla conexcion.Usuario
    ,Migracion			INT				NOT NULL DEFAULT 0 		-- determina la secuencia de migración realizada
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
	,Estado				        NVARCHAR(1)         NOT NULL DEFAULT('A')		 
  
    ,CONSTRAINT  [PK_LMD_IdLogMigracionDetalle]		PRIMARY KEY CLUSTERED (IdLogMigracionDetalle) 
) ON [PRIMARY]
GO

DROP TABLE IF EXISTS LogMigracionVista
CREATE TABLE LogMigracionVista (	
    NroFila     INT, 
	OrigenVista NVARCHAR(100),  
	JsonData    NVARCHAR(MAX)
);

EXEC DBO.Bitacora 'CREATE TABLE
dbo.Usuario, 
dbo.EndPointWeb, 
dbo.UsuarioEndPointWebPermiso, 
dbo.Homologacion, 
dbo.HomologacionEsquema, 
dbo.DataLakeOrganizacion, 
dbo.WebSiteLog
dbo.Conexion '
GO

EXEC dbo.setDiccionario	'dbo.Usuario'   , NULL                   ,'usuarios que gestionan el buscador andino' 
EXEC dbo.setDiccionario	'dbo.Usuario'   ,'IdUsuario				','PK'
EXEC dbo.setDiccionario	'dbo.Usuario'   ,'Email					','correo electonico'
EXEC dbo.setDiccionario	'dbo.Usuario'   ,'Nombre				','nombres del usuario'
EXEC dbo.setDiccionario	'dbo.Usuario'   ,'Apellido				','apellido del usuario'
EXEC dbo.setDiccionario	'dbo.Usuario'   ,'Telefono				','telefono del usuario'
EXEC dbo.setDiccionario	'dbo.Usuario'   ,'Clave					','clave del usuario'
EXEC dbo.setDiccionario	'dbo.Usuario'   ,'Rol					','rol del usuario (USER, ADMIN)'
EXEC dbo.setDiccionario	'dbo.Usuario'   ,'Estado				','Estado del registro (A= activo, X= Eliminado lógico)'
EXEC dbo.setDiccionario	'dbo.Usuario'   ,'FechaCreacion			','Fecha de creación del registro en la tabla'
EXEC dbo.setDiccionario	'dbo.Usuario'   ,'FechaModifica			','Fecha de actualización del registro en la tabla'
EXEC dbo.setDiccionario	'dbo.Usuario'   ,'IdUserCreacion		','Identificador del usuario que crea el registro'
EXEC dbo.setDiccionario	'dbo.Usuario'   ,'IdUserModifica		','Identificador del usuario que crea el registro'
GO

EXEC dbo.setDiccionario	'dbo.EndPointWeb'   , NULL                   ,'almacena las rutas de los servicios' 
EXEC dbo.setDiccionario	'dbo.EndPointWeb'   ,'IdEndPointWeb			','PK'
EXEC dbo.setDiccionario	'dbo.EndPointWeb'   ,'UrlWeb				','url de los servicios'
EXEC dbo.setDiccionario	'dbo.EndPointWeb'   ,'Nombre				','nombre de los serivicios'
EXEC dbo.setDiccionario	'dbo.EndPointWeb'   ,'Estado				','Estado del registro (A= activo, X= Eliminado lógico)'
EXEC dbo.setDiccionario	'dbo.EndPointWeb'   ,'FechaCreacion			','Fecha de creación del registro en la tabla'
EXEC dbo.setDiccionario	'dbo.EndPointWeb'   ,'FechaModifica			','Fecha de actualización del registro en la tabla'
EXEC dbo.setDiccionario	'dbo.EndPointWeb'   ,'IdUserCreacion		','Identificador del usuario que crea el registro'
EXEC dbo.setDiccionario	'dbo.EndPointWeb'   ,'IdUserModifica		','Identificador del usuario que crea el registro'
GO

EXEC dbo.setDiccionario	'dbo.UsuarioEndPointWebPermiso' , NULL                            ,'alamacena la relación entre Usuario EndPointWeb y Permiso' 
EXEC dbo.setDiccionario	'dbo.UsuarioEndPointWebPermiso' ,'IdUsuarioEndPointWebPermiso    ','PK'
EXEC dbo.setDiccionario	'dbo.UsuarioEndPointWebPermiso' ,'IdUsuario                      ','FK'
EXEC dbo.setDiccionario	'dbo.UsuarioEndPointWebPermiso' ,'IdEndPointWeb                  ','FK'
EXEC dbo.setDiccionario	'dbo.UsuarioEndPointWebPermiso' ,'Accion                         ','Estados: GET, POST, PUT, DELETE'
EXEC dbo.setDiccionario	'dbo.UsuarioEndPointWebPermiso' ,'Estado						 ','Estado del registro (A= activo, X= Eliminado lógico)'
EXEC dbo.setDiccionario	'dbo.UsuarioEndPointWebPermiso' ,'FechaCreacion				     ','Fecha de creación del registro en la tabla'
EXEC dbo.setDiccionario	'dbo.UsuarioEndPointWebPermiso' ,'FechaModifica			         ','Fecha de actualización del registro en la tabla'
EXEC dbo.setDiccionario	'dbo.UsuarioEndPointWebPermiso' ,'IdUserCreacion				 ','Identificador del usuario que crea el registro'
EXEC dbo.setDiccionario	'dbo.UsuarioEndPointWebPermiso' ,'IdUserModifica			     ','Identificador del usuario que modifica el registro'
GO

EXEC dbo.setDiccionario	'dbo.Homologacion'  , NULL                   ,'Campos de los esquemas que son homologados para la busqueda' 
EXEC dbo.setDiccionario	'dbo.Homologacion'  ,'IdHomologacion     	','PK'
EXEC dbo.setDiccionario	'dbo.Homologacion'  ,'IdHomologacionGrupo	','FK (Homologacion)'
EXEC dbo.setDiccionario	'dbo.Homologacion'  ,'MostrarWebOrden		','Orden de visialización en la web'
EXEC dbo.setDiccionario	'dbo.Homologacion'  ,'MostrarWeb			','Texto a mostrar en la web'
EXEC dbo.setDiccionario	'dbo.Homologacion'  ,'TooltipWeb			','Texto ayuda a mostrar en la web'
EXEC dbo.setDiccionario	'dbo.Homologacion'  ,'MascaraDato			','Mascara acorde al tipo de dato'
EXEC dbo.setDiccionario	'dbo.Homologacion'  ,'SiNoHayDato			','Texto a mostrar si no hay el dato'
EXEC dbo.setDiccionario	'dbo.Homologacion'  ,'InfoExtraJson			','información extra en tipo json'
EXEC dbo.setDiccionario	'dbo.Homologacion'  ,'CodigoHomologacion	','Código interno usado para la agrupacion de los campos'
EXEC dbo.setDiccionario	'dbo.Homologacion'  ,'NombreHomologado		','Nombre del campo homologado'
EXEC dbo.setDiccionario	'dbo.Homologacion'  ,'Estado				','Estado del registro (A= activo, X= Eliminado lógico)'
EXEC dbo.setDiccionario	'dbo.Homologacion'  ,'FechaCreacion			','Fecha de creación del registro en la tabla'
EXEC dbo.setDiccionario	'dbo.Homologacion'  ,'FechaModifica			','Fecha de actualización del registro en la tabla'
EXEC dbo.setDiccionario	'dbo.Homologacion'  ,'IdUserCreacion		','Identificador del usuario que crea el registro'
EXEC dbo.setDiccionario	'dbo.Homologacion'  ,'IdUserModifica		','Identificador del usuario que modifica el registro'
GO

EXEC dbo.setDiccionario	'dbo.HomologacionEsquema'   , NULL                       ,'Gestiona los esquemas y los campos que la componen'
EXEC dbo.setDiccionario	'dbo.HomologacionEsquema'   ,'IdHomologacionEsquema     ','PK'
EXEC dbo.setDiccionario	'dbo.HomologacionEsquema'   ,'MostrarWebOrden			','Orden de visialización en la web'
EXEC dbo.setDiccionario	'dbo.HomologacionEsquema'   ,'MostrarWeb				','Texto a mostrar en la web'
EXEC dbo.setDiccionario	'dbo.HomologacionEsquema'   ,'TooltipWeb				','Texto ayuda a mostrar en la web'
EXEC dbo.setDiccionario	'dbo.HomologacionEsquema'   ,'EsquemaJson				','Campos que forman la estructura del esquema'
EXEC dbo.setDiccionario	'dbo.HomologacionEsquema'   ,'Estado					','Estado del registro (A= activo, X= Eliminado lógico)'
EXEC dbo.setDiccionario	'dbo.HomologacionEsquema'   ,'FechaCreacion				','Fecha de creación del registro en la tabla'
EXEC dbo.setDiccionario	'dbo.HomologacionEsquema'   ,'FechaModifica				','Fecha de actualización del registro en la tabla'
EXEC dbo.setDiccionario	'dbo.HomologacionEsquema'   ,'IdUserCreacion			','Identificador del usuario que crea el registro'
EXEC dbo.setDiccionario	'dbo.HomologacionEsquema'   ,'IdUserModifica			','Identificador del usuario que modifica el registro'
GO

EXEC dbo.setDiccionario	'dbo.DataLakeOrganizacion'  , NULL                   ,'Gestiona datos migrados de la organizaciones certificadas'
EXEC dbo.setDiccionario	'dbo.DataLakeOrganizacion'  ,'IdDataLakeOrganizacion','PK'
EXEC dbo.setDiccionario	'dbo.DataLakeOrganizacion'  ,'IdHomologacionEsquema	','FK'
EXEC dbo.setDiccionario	'dbo.DataLakeOrganizacion'  ,'IdDataLake			','FK'
EXEC dbo.setDiccionario	'dbo.DataLakeOrganizacion'  ,'DataEsquemaJson       ','Data migrada en formato JSON'
EXEC dbo.setDiccionario	'dbo.DataLakeOrganizacion'  ,'DataFechaCarga		','Fecha de carga del registro'
EXEC dbo.setDiccionario	'dbo.DataLakeOrganizacion'  ,'FechaCreacion			','Fecha de creación del registro en la tabla'
EXEC dbo.setDiccionario	'dbo.DataLakeOrganizacion'  ,'Estado				','Estado del registro (A= activo, X= Eliminado lógico)'
GO

EXEC dbo.setDiccionario	'dbo.WebSiteLog'    , NULL               ,'Almacena el log de registro de las busquedas' 
EXEC dbo.setDiccionario	'dbo.WebSiteLog'    ,'IdWebSiteLog		','PK'
EXEC dbo.setDiccionario	'dbo.WebSiteLog'    ,'TextoBusquedo		','Texto Busquedo por el usario'
EXEC dbo.setDiccionario	'dbo.WebSiteLog'    ,'FiltroUsado		','Filtro usado para la Busqueda por el usario'
EXEC dbo.setDiccionario	'dbo.WebSiteLog'    ,'FechaCreacion		','Fecha de creación del registro en la tabla'
GO	

EXEC DBO.Bitacora 'Se documento :
                    dbo.Usuario, 
                    dbo.EndPointWeb, 
                    dbo.UsuarioEndPointWebPermiso, 
                    dbo.Homologacion, 
                    dbo.HomologacionEsquema, 
                    dbo.DataLakeOrganizacion, 
                    dbo.WebSiteLog'
GO

EXEC DBO.getDiccionario '   dbo.Usuario, 
                            dbo.EndPointWeb, 
                            dbo.UsuarioEndPointWebPermiso, 
                            dbo.Homologacion, 
                            dbo.HomologacionEsquema, 
                            dbo.DataLakeOrganizacion, 
                            dbo.WebSiteLog'
GO
