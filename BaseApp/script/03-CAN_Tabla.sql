
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
GO
DROP TABLE if exists dbo.Conexion;
GO

EXEC DBO.Bitacora 'DROP TABLE if exists 
dbo.WebSiteLog;
dbo.OrganizacionPais;
dbo.Persona;
dbo.UsuarioEndPointWebPermiso;
dbo.Usuario;
dbo.EndPointWeb;
dbo.DataLakePersona;
dbo.DataLakeOrganizacion;
dbo.DataLake;
dbo.HomologacionEsquema;
dbo.Homologacion;
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
);

CREATE TABLE dbo.HomologacionEsquema(
	 IdHomologacionEsquema     	INT IDENTITY(1,1) NOT NULL
    ,MostrarWebOrden			INT DEFAULT(1) NOT NULL
    ,MostrarWeb					NVARCHAR(200) NOT NULL DEFAULT('GRILLA')
	,TooltipWeb					NVARCHAR(200) NOT NULL DEFAULT('')
    ,EsquemaJson				NVARCHAR(max) NOT NULL DEFAULT('{}')
    ,VistaNombre				NVARCHAR(200) NOT NULL DEFAULT('')
	,Estado						NVARCHAR(1) NOT NULL DEFAULT('A')
	,FechaCreacion				DATETIME	NOT NULL DEFAULT(GETDATE())
    ,FechaModifica				DATETIME	NOT NULL DEFAULT(GETDATE())  
    ,IdUserCreacion				INT			NOT NULL DEFAULT(0)
    ,IdUserModifica				INT			NOT NULL DEFAULT(0)  

    ,CONSTRAINT  [PK_HE_IdHomologacionEsquema]	PRIMARY KEY CLUSTERED (IdHomologacionEsquema) 
    ,CONSTRAINT  [CK_HE_EsquemaJson]			CHECK   (ISJSON(EsquemaJson) = 1 )
    ,CONSTRAINT  [CK_HE_Estado]				    CHECK   (Estado IN ('A', 'X'))
     --,CONSTRAINT  UK_HE_VistaNombre		        UNIQUE (VistaNombre)
);

CREATE TABLE dbo.DataLake(
     IdDataLake				INT IDENTITY(1,1) NOT NULL
	,DataTipo				NVARCHAR(15) NOT NULL DEFAULT('NO_DEFINIDO') 
	,DataSistemaOrigen		NVARCHAR(15) NOT NULL	
    ,DataSistemaOrigenId	NVARCHAR(10) NOT NULL 
    ,DataSistemaFecha		DATETIME	NOT NULL
	,Estado					NVARCHAR(1) NOT NULL DEFAULT('A')
    ,DataFechaCarga			DATETIME	NOT NULL DEFAULT(GETDATE())
	
	,CONSTRAINT  [PK_DL_IdDataLake]	PRIMARY KEY CLUSTERED (IdDataLake) 
	,CONSTRAINT  [UK_DL_DataTipo]	CHECK (DataTipo IN ('ORGANIZACION', 'PERSONA','NO_DEFINIDO'))
    ,CONSTRAINT  [CK_DL_Estado]		CHECK   (Estado IN ('A', 'X'))
);

CREATE TABLE dbo.DataLakeOrganizacion(
     IdDataLakeOrganizacion INT IDENTITY(1,1) NOT NULL
    ,IdHomologacionEsquema	INT NOT NULL  --FOREIGN KEY REFERENCES HomologacionEsquema (IdHomologacionEsquema)
    ,IdDataLake				INT NOT NULL  FOREIGN KEY REFERENCES DataLake (IdDataLake)
    ,DataEsquemaJson        NVARCHAR(max) NOT NULL DEFAULT('{}')
    ,DataFechaCarga			DATETIME	  NOT NULL DEFAULT(GETDATE())
    ,FechaCreacion			DATETIME	NOT NULL DEFAULT(GETDATE())
	,Estado					NVARCHAR(1) NOT NULL DEFAULT('A')
	
	,CONSTRAINT  [PK_DLO_IdDataLakeOrganizacion]	PRIMARY KEY CLUSTERED (IdDataLakeOrganizacion) 
    ,CONSTRAINT  [CK_DLO_DataEsquemaJson]		CHECK   (ISJSON(DataEsquemaJson) = 1 )
    ,CONSTRAINT  [CK_DLO_Estado]					CHECK   (Estado IN ('A', 'X'))
);

CREATE TABLE dbo.DataLakePersona(
     IdDataLakePersona		INT IDENTITY(1,1) NOT NULL
    ,IdHomologacionEsquema	INT NOT NULL  --FOREIGN KEY REFERENCES HomologacionEsquema (IdHomologacionEsquema)
    ,IdDataLake				INT NOT NULL  FOREIGN KEY REFERENCES DataLake (IdDataLake)
    ,DataEsquemaJson        NVARCHAR(max) NOT NULL DEFAULT('{}')
    ,DataFechaCarga			DATETIME	  NOT NULL DEFAULT(GETDATE())
    ,FechaCreacion			DATETIME	NOT NULL DEFAULT(GETDATE())
	,Estado					NVARCHAR(1) NOT NULL DEFAULT('A')
	
	,CONSTRAINT  [PK_DKP_DataLakePersona]	PRIMARY KEY CLUSTERED (IdDataLakePersona) 
    ,CONSTRAINT  [CK_DKP_DataEsquemaJson]	CHECK   (ISJSON(DataEsquemaJson) = 1 )
    ,CONSTRAINT  [CK_DKP_Estado]			CHECK   (Estado IN ('A', 'X'))
);

CREATE TABLE dbo.WebSiteLog (
     IdWebSiteLog		INT IDENTITY(1,1) NOT NULL
    ,TextoBusquedo      NVARCHAR(900) NOT NULL
    ,FiltroUsado        NVARCHAR(max) NOT NULL DEFAULT('{}')
	,FechaCreacion		DATETIME	NOT NULL DEFAULT(GETDATE())
	
	,CONSTRAINT  [PK_WSL_WebSiteLog]		PRIMARY KEY CLUSTERED (IdWebSiteLog) 
);

CREATE TABLE dbo.Conexion (
     IdConexion			INT IDENTITY(1,1) NOT NULL
    ,IdUsuario			INT NOT NULL
    ,IdSistema      INT NOT NULL
    ,BaseDatos      NVARCHAR(100) NOT NULL
    ,Host           NVARCHAR(100) NOT NULL
    ,Puerto         INT NOT NULL
    ,Usuario        NVARCHAR(100) NOT NULL
    ,Contrasenia    NVARCHAR(100) NOT NULL
    ,MotorBaseDatos NVARCHAR(100) NOT NULL
    ,Filtros        NVARCHAR(MAX) NOT NULL DEFAULT('{}')
    ,FechaConexion		DATETIME NOT NULL DEFAULT(GETDATE())
    ,TiempoEspera    INT NOT NULL DEFAULT(0) -- Tiempo de espera en segundos
    ,Estado			NVARCHAR(1) NOT NULL DEFAULT('A')
    ,FechaCreacion				DATETIME	NOT NULL DEFAULT(GETDATE())
    ,FechaModifica				DATETIME	NOT NULL DEFAULT(GETDATE())  
    ,IdUserCreacion				INT			NOT NULL DEFAULT(0)
    ,IdUserModifica				INT			NOT NULL DEFAULT(0)  
  
  ,CONSTRAINT  [PK_C_IdConexion]		    PRIMARY KEY CLUSTERED (IdConexion) 
    ,CONSTRAINT  [FK_C_IdUsuario]		    FOREIGN KEY (IdUsuario) REFERENCES Usuario(IdUsuario)
    ,CONSTRAINT  [CK_C_Estado]			    CHECK   (Estado IN ('A', 'X'))
    ,CONSTRAINT  [CK_C_MotorBaseDatos]  CHECK   (MotorBaseDatos IN ('MYSQL', 'SQLSERVER', 'SQLLITE'))
    ,CONSTRAINT  [CK_C_Filtros]			CHECK   (ISJSON(Filtros) = 1 )
);

EXEC DBO.Bitacora 'CREATE TABLE
dbo.Usuario, 
dbo.EndPointWeb, 
dbo.UsuarioEndPointWebPermiso, 
dbo.Homologacion, 
dbo.HomologacionEsquema, 
dbo.DataLake, 
dbo.DataLakeOrganizacion, 
dbo.DataLakePersona, 
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

EXEC dbo.setDiccionario	'dbo.DataLake'  , NULL                   ,'Datos de los sistemas que proporcionan la data'
EXEC dbo.setDiccionario	'dbo.DataLake'  ,'IdDataLake			','PK'
EXEC dbo.setDiccionario	'dbo.DataLake'  ,'DataTipo				','determian si es: ORGANIZACION, PERSONA, NO_DEFINIDO'
EXEC dbo.setDiccionario	'dbo.DataLake'  ,'DataSistemaOrigen		','nombre del Sistema Origen'
EXEC dbo.setDiccionario	'dbo.DataLake'  ,'DataSistemaOrigenId	','Id Sistema Origen'
EXEC dbo.setDiccionario	'dbo.DataLake'  ,'DataSistemaFecha		','Fecha del regsitro en el Sistema Origen'
EXEC dbo.setDiccionario	'dbo.DataLake'  ,'Estado				','Estado del registro (A= activo, X= Eliminado lógico)'
EXEC dbo.setDiccionario	'dbo.DataLake'  ,'DataFechaCarga		','Fecha de carga del registro'
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

EXEC dbo.setDiccionario	'dbo.DataLakePersona'   , NULL                  ,'Gestiona datos migrados de la personas certificadas'
EXEC dbo.setDiccionario	'dbo.DataLakePersona'   ,'IdDataLakePersona		','PK'
EXEC dbo.setDiccionario	'dbo.DataLakePersona'   ,'IdHomologacionEsquema	','FK'
EXEC dbo.setDiccionario	'dbo.DataLakePersona'   ,'IdDataLake			','FK'
EXEC dbo.setDiccionario	'dbo.DataLakePersona'   ,'DataEsquemaJson       ','Data migrada en formato JSON'
EXEC dbo.setDiccionario	'dbo.DataLakePersona'   ,'DataFechaCarga		','Fecha de carga del registro'
EXEC dbo.setDiccionario	'dbo.DataLakePersona'   ,'FechaCreacion			','Fecha de creación del registro en la tabla'
EXEC dbo.setDiccionario	'dbo.DataLakePersona'   ,'Estado				','Estado del registro (A= activo, X= Eliminado lógico)'
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
                    dbo.DataLake, 
                    dbo.DataLakeOrganizacion, 
                    dbo.DataLakePersona, 
                    dbo.WebSiteLog'
GO

EXEC DBO.getDiccionario '   dbo.Usuario, 
                            dbo.EndPointWeb, 
                            dbo.UsuarioEndPointWebPermiso, 
                            dbo.Homologacion, 
                            dbo.HomologacionEsquema, 
                            dbo.DataLake, 
                            dbo.DataLakeOrganizacion, 
                            dbo.DataLakePersona, 
                            dbo.WebSiteLog'
GO
