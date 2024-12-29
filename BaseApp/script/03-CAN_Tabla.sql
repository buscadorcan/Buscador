
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

DROP TABLE if exists [dbo].[Homologacion] ;
CREATE TABLE [dbo].[Homologacion] (
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
	
	,CONSTRAINT  [PK_H_IdHomologacion]  PRIMARY KEY CLUSTERED (IdHomologacion) 
    ,CONSTRAINT  [CK_H_InfoExtraJson]   CHECK   (ISJSON(InfoExtraJson) = 1 )
    ,CONSTRAINT  [CK_H_MascaraDato]	    CHECK   (MascaraDato IN ('TEXTO', 'FECHA', 'NUMERICO','ICO','FORMULA'))
	,CONSTRAINT  [CK_H_Mostrar]		    CHECK	(Mostrar IN ('S', 'N'))
    ,CONSTRAINT  [CK_H_Estado]		    CHECK   (Estado IN ('A', 'X'))
);


DROP TABLE if exists UsuarioEndPointWebPermiso
DROP TABLE if exists EndPointWeb
DROP TABLE if exists Usuario

DROP TABLE if exists [dbo].[UsuarioEndPoint]
DROP TABLE if exists [dbo].[Usuario]
DROP TABLE if exists [dbo].[EsquemaData] 
DROP TABLE if exists [dbo].[EsquemaVistaColumna] 
DROP TABLE if exists [dbo].[EsquemaVista] 
DROP TABLE if exists [dbo].[ONAConexion] 
DROP TABLE if exists [dbo].[ONA] 
DROP TABLE if exists [dbo].[Esquema] ;
CREATE TABLE [dbo].[Esquema](
   IdEsquema                INT IDENTITY(1,1) NOT NULL
  ,MostrarWebOrden			INT DEFAULT(1) NOT NULL
  ,MostrarWeb				NVARCHAR(200) NOT NULL DEFAULT('GRILLA')
  ,TooltipWeb				NVARCHAR(200) NOT NULL DEFAULT('')
  ,EsquemaVista		        NVARCHAR(100) NOT NULL DEFAULT ('')
  ,EsquemaJson				NVARCHAR(max) NOT NULL DEFAULT('{}')
  ,Estado					NVARCHAR(1)	  NOT NULL DEFAULT('A')
  ,FechaCreacion			DATETIME	  NOT NULL DEFAULT(GETDATE())
  ,FechaModifica			DATETIME	  NOT NULL DEFAULT(GETDATE())  
  ,IdUserCreacion			INT			  NOT NULL DEFAULT(0)
  ,IdUserModifica			INT			  NOT NULL DEFAULT(0)  

  ,CONSTRAINT  [PK_E_IdEsquema]	    PRIMARY KEY CLUSTERED (IdEsquema) 
  ,CONSTRAINT  [CK_E_EsquemaJson]	CHECK   (ISJSON(EsquemaJson) = 1 )
  ,CONSTRAINT  [CK_E_Estado]		CHECK   (Estado IN ('A', 'X'))
);
CREATE TABLE [dbo].[ONA] (
     IdONA			    INT IDENTITY(1,1)
    ,RazonSocial		NVARCHAR(300)NOT NULL
    ,Siglas			    NVARCHAR(50) NOT NULL
    ,Pais			    NVARCHAR(50) NOT NULL
    ,Ciudad			    NVARCHAR(50) NOT NULL
    ,Correo			    NVARCHAR(300) NULL
    ,Direccion		    NVARCHAR(300) NULL
    ,PaginaWeb		    NVARCHAR(300) NULL
    ,Telefono		    NVARCHAR(20)  NULL
    ,UrlIcono		    NVARCHAR(300) NULL
    ,UrlLogo			NVARCHAR(300) NULL
    ,InfoExtraJson		NVARCHAR(max) NOT NULL DEFAULT('{}')
    ,Estado				NVARCHAR(1) NOT NULL DEFAULT('A')
    ,FechaCreacion		DATETIME	NOT NULL DEFAULT(GETDATE())
    ,FechaModifica		DATETIME	NOT NULL DEFAULT(GETDATE())  
    ,IdUserCreacion		INT			NOT NULL DEFAULT(0)
    ,IdUserModifica		INT			NOT NULL DEFAULT(0)  
  
    ,CONSTRAINT  [PK_O_IdONA]           PRIMARY KEY CLUSTERED (IdONA) 
    ,CONSTRAINT  [CK_O_Estado]          CHECK   (Estado IN ('A', 'X'))
    ,CONSTRAINT  [CK_O_InfoExtraJson]   CHECK   (ISJSON(InfoExtraJson) = 1 )
);
CREATE TABLE [dbo].[ONAConexion] (
     IdONA			    INT NOT NULL
    ,Host               NVARCHAR(100) NOT NULL  NULL DEFAULT('')
    ,Puerto             INT NOT NULL DEFAULT(0)  
    ,Usuario            NVARCHAR(100) NOT NULL  NULL DEFAULT('')
    ,Contrasenia        NVARCHAR(100) NOT NULL  NULL DEFAULT('')
    ,BaseDatos          NVARCHAR(100) NOT NULL  NULL DEFAULT('')
    ,OrigenDatos         NVARCHAR(100)NOT NULL  NULL DEFAULT('')
    ,Migrar				NVARCHAR(1) NOT NULL DEFAULT('S')
    
	,Estado				NVARCHAR(1) NOT NULL DEFAULT('A')
    ,FechaCreacion		DATETIME	NOT NULL DEFAULT(GETDATE())
    ,FechaModifica		DATETIME	NOT NULL DEFAULT(GETDATE())  
    ,IdUserCreacion		INT			NOT NULL DEFAULT(0)
    ,IdUserModifica		INT			NOT NULL DEFAULT(0)  
  
    ,CONSTRAINT  [PK_OC_IdONA]			PRIMARY KEY CLUSTERED (IdONA) 
    ,CONSTRAINT  [FK_OC_IdONA]          FOREIGN KEY([IdONA]) REFERENCES [dbo].[ONA] ([IdONA])
    ,CONSTRAINT  [CK_OC_Migrar]			CHECK   (Migrar IN ('S', 'N'))
    ,CONSTRAINT  [CK_OC_Estado]			CHECK   (Estado IN ('A', 'X'))
    ,CONSTRAINT  [CK_OC_OrigenDatos]	CHECK   (OrigenDatos IN ('MYSQL', 'SQLSERVER', 'SQLLITE','EXCEL'))
);
CREATE TABLE [dbo].[EsquemaVista] (
     IdEsquemaVista     INT IDENTITY(1,1)
    ,IdONA              INT NOT NULL
    ,IdEsquema	        INT NOT NULL
    ,VistaOrigen		NVARCHAR(100)  NOT NULL
    --,VistaColumnaPK     NVARCHAR(100)  NOT NULL

    ,Estado				NVARCHAR(1) NOT NULL DEFAULT('A')
    ,FechaCreacion		DATETIME	NOT NULL DEFAULT(GETDATE())
    ,FechaModifica		DATETIME	NOT NULL DEFAULT(GETDATE())  
    ,IdUserCreacion		INT			NOT NULL DEFAULT(0)
    ,IdUserModifica		INT			NOT NULL DEFAULT(0)  
  
    ,CONSTRAINT  [PK_EV_IdEsquemaVista]PRIMARY KEY CLUSTERED (IdEsquemaVista) 
    ,CONSTRAINT  [FK_EV_IdEsquema]     FOREIGN KEY (IdEsquema) REFERENCES Esquema(IdEsquema)
    ,CONSTRAINT  [FK_EV_IdONA]         FOREIGN KEY (IdONA)     REFERENCES ONA(IdONA)
    ,CONSTRAINT  [CK_EV_Estado]        CHECK   (Estado IN ('A', 'X'))
);
CREATE TABLE [dbo].[EsquemaVistaColumna] (
     IdEsquemaVistaColumna  INT IDENTITY(1,1)
    ,IdEsquemaVista         INT NOT NULL
    ,ColumnaEsquemaIdH      INT NOT NULL
    ,ColumnaEsquema	        NVARCHAR(100) NOT NULL
    ,ColumnaVista	        NVARCHAR(100) NOT NULL
    ,ColumnaVistaPK	        BIT DEFAULT(0) NOT NULL

    ,Estado				NVARCHAR(1) NOT NULL DEFAULT('A')
    ,FechaCreacion		DATETIME	NOT NULL DEFAULT(GETDATE())
    ,FechaModifica		DATETIME	NOT NULL DEFAULT(GETDATE())  
    ,IdUserCreacion		INT			NOT NULL DEFAULT(0)
    ,IdUserModifica		INT			NOT NULL DEFAULT(0)  
  
    ,CONSTRAINT  [PK_EVC_IdEsquemaVistaColumna] PRIMARY KEY CLUSTERED (IdEsquemaVistaColumna) 
    ,CONSTRAINT  [FK_EVC_IdEsquemaVista]        FOREIGN KEY (IdEsquemaVista)  REFERENCES EsquemaVista(IdEsquemaVista)
    ,CONSTRAINT  [CK_EVC_Estado]		        CHECK   (Estado IN ('A', 'X'))
);
CREATE TABLE [dbo].[EsquemaData](
	 IdEsquemaData      INT IDENTITY(1,1) NOT NULL
	,IdEsquemaVista     INT NOT NULL
	,VistaFK            NVARCHAR (10) NULL             --idEnte
	,VistaPK            NVARCHAR (10)  NOT NULL
	,DataEsquemaJson    NVARCHAR (max) NOT NULL
	,DataFecha          DATETIME NOT NULL DEFAULT(GETDATE())
    ,CONSTRAINT         [PK_ED_IdEsquemaData] PRIMARY KEY CLUSTERED (IdEsquemaData ASC)
    ,CONSTRAINT         [FK_ED_IdEsquemaVista] FOREIGN KEY (IdEsquemaVista) REFERENCES EsquemaVista(IdEsquemaVista)
);
CREATE TABLE [dbo].[Usuario] (
     IdUsuario				INT IDENTITY(1,1) 
    ,IdHomologacionRol     	INT NOT NULL
    ,IdONA                 	INT
    ,Nombre					NVARCHAR(100) NOT NULL
    ,Apellido				NVARCHAR(100) NOT NULL DEFAULT('')
	,Telefono				NVARCHAR(20)  NOT NULL DEFAULT('')
    ,Email					NVARCHAR(100) NOT NULL
    ,Clave					NVARCHAR(333) NOT NULL
	,Estado					NVARCHAR(1) NOT NULL DEFAULT('A')
    ,FechaCreacion			DATETIME	NOT NULL DEFAULT(GETDATE())
    ,FechaModifica			DATETIME	NOT NULL DEFAULT(GETDATE())  
    ,IdUserCreacion			INT			NOT NULL DEFAULT(0)
    ,IdUserModifica			INT			NOT NULL DEFAULT(0)

    ,CONSTRAINT PK_U_IdUsuario	        PRIMARY KEY CLUSTERED (IdUsuario)  
    ,CONSTRAINT FK_U_IdHomologacionRol  FOREIGN KEY(IdHomologacionRol) REFERENCES Homologacion (IdHomologacion)
    ,CONSTRAINT FK_U_IdONA              FOREIGN KEY(IdONA)             REFERENCES ONA (IdONA)
    ,CONSTRAINT CK_U_Estado		        CHECK  (Estado IN ('A', 'X'))
    ,CONSTRAINT UK_U_Email		        UNIQUE (Email)
);
CREATE TABLE [dbo].[UsuarioEndPoint] (
     IdUsuarioEndPoint      INT IDENTITY(1,1) 
    ,IdHomologacionEndPoint INT NOT NULL DEFAULT(0)
    ,IdUsuario              INT NOT NULL DEFAULT(0)
    ,Accion                 NVARCHAR(10) NOT NULL
	,Estado			        NVARCHAR(1) NOT NULL DEFAULT('A')
    ,FechaCreacion	        DATETIME	NOT NULL DEFAULT(GETDATE())
    ,FechaModifica		    DATETIME	NOT NULL DEFAULT(GETDATE())  
    ,IdUserCreacion	        INT			NOT NULL DEFAULT(0)
    ,IdUserModifica		    INT			NOT NULL DEFAULT(0)

	,CONSTRAINT PK_UEP_IdUsuarioEndpoint        PRIMARY KEY CLUSTERED (IdUsuarioEndPoint)  
    ,CONSTRAINT FK_UEP_IdUsuario		        FOREIGN KEY (IdUsuario)		        REFERENCES Usuario(IdUsuario)
    ,CONSTRAINT FK_UEP_IdHomologacionEndPoint   FOREIGN KEY (IdHomologacionEndPoint)REFERENCES Homologacion(IdHomologacion)
    ,CONSTRAINT UK_UEP_Accion			        CHECK (Accion IN ('GET', 'POST', 'PUT', 'DELETE'))
    ,CONSTRAINT CK_UEP_Estado			        CHECK (Estado IN ('A', 'X'))
);
GO

DROP TABLE if exists [dbo].[Catalogo] ;
CREATE TABLE [dbo].[Catalogo](
	 IdCatalogo     INT IDENTITY(1,1) NOT NULL
    ,IdH     	    INT DEFAULT(NULL) 
    ,IdHPadre	    INT DEFAULT(NULL) 
    ,Nombre		    NVARCHAR(200)NOT NULL DEFAULT('')
    ,Estado         NVARCHAR(1)  NOT NULL DEFAULT('A')
	,FechaCreacion  DATETIME	 NOT NULL DEFAULT(GETDATE())

    ,CONSTRAINT  [PK_C_IdCatalogo]  PRIMARY KEY CLUSTERED (IdCatalogo)
    ,CONSTRAINT  [CK_C_Estado]		CHECK   (Estado IN ('A', 'X'))
)  

DROP TABLE if exists [dbo].[EsquemaFullText] ;
CREATE TABLE [dbo].[EsquemaFullText](
	 IdEsquemaFullText  INT IDENTITY(1,1) NOT NULL
	,IdEsquemaData      INT NOT NULL
	,IdHomologacion     INT NOT NULL
	,FullTextData       NVARCHAR(max) NULL
    ,CONSTRAINT         [PK_EFT_IdEsquemaFullText] PRIMARY KEY CLUSTERED (IdEsquemaFullText ASC)
)  

DROP TABLE if exists [dbo].[LogMigracionDetalle]
DROP TABLE if exists [dbo].[LogMigracion] ;
CREATE TABLE [dbo].[LogMigracion]  (
     IdLogMigracion		INT IDENTITY(1,1)						    -- Identificador único para el registro del log
    ,IdONA			    INT NOT NULL                                -- campo de la tabla OnaConexcion                        
    ,Host               NVARCHAR(100) NOT NULL  DEFAULT('')    -- campo de la tabla OnaConexcion                
    ,Puerto             INT NOT NULL DEFAULT(0)                     -- campo de la tabla OnaConexcion
    ,Usuario            NVARCHAR(100) NOT NULL  DEFAULT('')    -- campo de la tabla OnaConexcion                
    ,BaseDatos          NVARCHAR(100) NOT NULL  DEFAULT('')    -- campo de la tabla OnaConexcion                
    ,OrigenDatos        NVARCHAR(100)NOT NULL   DEFAULT('')     -- campo de la tabla OnaConexcion            
    ,Migrar				NVARCHAR(1) NOT NULL DEFAULT('S')           -- campo de la tabla OnaConexcion                        
	
    ,Migracion			INT				NOT NULL DEFAULT 0 		-- determina la secuencia de migración realizada
    ,Estado				NVARCHAR(10)    NOT NULL DEFAULT ('')	-- Estado de la migración (Ejemplo: 'OK', 'ERROR','START')
    ,EsquemaId		    INT				NOT NULL DEFAULT 0		-- Id de  EsquemaVista
    ,EsquemaVista		NVARCHAR(100)   NOT NULL DEFAULT ('')	-- Nombre de vista de HomologacionEsquema (VistaNombre) 
    ,EsquemaFilas		INT				NOT NULL DEFAULT 0		-- Número de registros migrados
    ,VistaOrigen		NVARCHAR(100)   NOT NULL DEFAULT ('')	-- Nombre de la vista de origen
    ,VistaFilas		    INT             NOT NULL DEFAULT 0		-- Número de registros a migrar
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
);
CREATE TABLE [dbo].[LogMigracionDetalle](
	 IdLogMigracionDetalle	INT IDENTITY(1,1)	NOT NULL 
	,IdLogMigracion		    INT    
	,NroMigracion			INT	 
    ,IdEsquemaVista         INT  
    ,ColumnaEsquemaIdH      INT  
    ,ColumnaEsquema	        NVARCHAR(100)  
    ,ColumnaVista	        NVARCHAR(100)  
    ,ColumnaVistaPK	        BIT DEFAULT(0)  
	,Fecha 	    DATETIME	NOT NULL DEFAULT GETDATE()	-- fecha de creación del registro
  
    ,CONSTRAINT  [PK_LMD_IdLogMigracionDetalle]		PRIMARY KEY CLUSTERED (IdLogMigracionDetalle) 
) ON [PRIMARY]
GO

DROP TABLE IF EXISTS LogMigracionVista
CREATE TABLE LogMigracionVista (	
    NroFila     INT, 
	OrigenVista NVARCHAR(100),  
	JsonData    NVARCHAR(MAX)
);

CREATE TABLE dbo.WebSiteLog (
     IdWebSiteLog		INT IDENTITY(1,1) NOT NULL
    ,TextoBusquedo      NVARCHAR(900) NOT NULL
    ,FiltroUsado        NVARCHAR(max) NOT NULL DEFAULT('{}')
	,FechaCreacion		DATETIME	NOT NULL DEFAULT(GETDATE())
	
	,CONSTRAINT  [PK_WSL_WebSiteLog]		PRIMARY KEY CLUSTERED (IdWebSiteLog) 
);

DROP VIEW if exists [dbo].[vwPais];
GO
CREATE VIEW [dbo].[vwPais] AS 
	SELECT	h1.IdHomologacion	IdHomologacionPais
			, h1.MostrarWeb		Pais
	FROM	Homologacion h1
	JOIN	Homologacion h2 ON h1.IdHomologacionGrupo = h2.IdHomologacion
	WHERE	h2.CodigoHomologacion = 'KEY_PAIS'
GO


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
