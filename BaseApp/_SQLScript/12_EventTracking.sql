--DROP TABLE IF EXISTS [dbo].[EventTracking]
--GO
drop table [dbo].[EventTracking]
go

CREATE TABLE [dbo].[EventTracking](
	 IdEventTracking	INT IDENTITY(1,1) NOT NULL
	,IdUsuario		    INT NOT NULL DEFAULT(0) 
	,CodigoHomologacionRol	NVARCHAR(50)  NOT NULL DEFAULT('') 
	,CodigoHomologacionMenu NVARCHAR(50)  NOT NULL DEFAULT('')
	,NombreControl		NVARCHAR(100) NOT NULL DEFAULT('')
	,NombreAccion		NVARCHAR(100) NOT NULL DEFAULT('')
	,UbicacionJson		NVARCHAR(max) NOT NULL DEFAULT('{}')
	,ParametroJson		NVARCHAR(max) NOT NULL DEFAULT('{}')
	,ErrorTracking		NVARCHAR(max) NOT NULL DEFAULT('')
	,FechaCreacion		DATETIME NOT NULL DEFAULT(GETDATE())  
	,CONSTRAINT  [PK_ET_IdEventTracking] PRIMARY KEY CLUSTERED (idEventTracking)
	,CONSTRAINT  [CK_ET_ParametroJson]	 CHECK   (ISJSON(ParametroJson) = 1 )
);
GO
