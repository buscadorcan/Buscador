/*----------------------------------------------------------------------------------------\
|    ©Copyright 2K24												BUSCADOR ANDINO		  |
|-----------------------------------------------------------------------------------------|
| Este código está protegido por las leyes y tratados internacionales de derechos de autor|
\-----------------------------------------------------------------------------------------/
  [App]            : Buscador Andino											
	- Date         : 2K24.JUN.25	
	- Author       : patricio.paccha														
	- Version	   : 1.0										
	- Description  : Carga homologacion, homologacionEsquema
\----------------------------------------------------------------------------------------*/
USE [CAN_DB]
GO


SET IDENTITY_INSERT dbo.Homologacion ON;
select * from Homologacion

USE [CAN_DB]
GO
SET IDENTITY_INSERT dbo.Homologacion ON;
INSERT INTO [dbo].[Homologacion] 
(		    [IdHomologacion]
		   ,[IdHomologacionGrupo]
           ,[MostrarWebOrden]
           ,[MostrarWeb]
           ,[TooltipWeb]
           ,[MascaraDato]
           ,[SiNoHayDato]
           ,[InfoExtraJson]
           ,[CodigoHomologacion]
           ,[NombreHomologado]
           ,[Estado]
           ,[FechaCreacion]
           ,[FechaModifica]
           ,[IdUserCreacion]
           ,[IdUserModifica]
)
select	 
            [IdHomologacion]
		   ,[IdHomologacionGrupo]
           ,[MostrarWebOrden]
           ,[MostrarWeb]
           ,isnull([TooltipWeb],'')
           ,[MascaraDato]
           ,isnull([SiNoHayDato],'')
           ,[InfoExtraJson]
           ,isnull([CodigoHomologacion],'')
           ,isnull([NombreHomologado],'')
           ,[Estado]
           ,[FechaCreacion]
           ,[FechaModifica]
           ,[IdUserCreacion]
           ,[IdUserModifica]
From Homologacion0
SET IDENTITY_INSERT dbo.Homologacion OFF;


USE [CAN_DB]
GO
SET IDENTITY_INSERT dbo.HomologacionEsquema ON;
INSERT INTO [dbo].[HomologacionEsquema]
           ([IdHomologacionEsquema]
		   ,[MostrarWebOrden]
           ,[MostrarWeb]
           ,[TooltipWeb]
           ,[EsquemaJson]
           ,[VistaNombre]
           ,[Estado]
           ,[FechaCreacion]
           ,[FechaModifica]
           ,[IdUserCreacion]
           ,[IdUserModifica]
           ,[IdVistaNombre]
           ,[DataTipo])
 
select [IdHomologacionEsquema]
		   ,[MostrarWebOrden]
           ,[MostrarWeb]
           ,[TooltipWeb]
           ,[EsquemaJson]
           ,[VistaNombre]
           ,[Estado]
           ,[FechaCreacion]
           ,[FechaModifica]
           ,[IdUserCreacion]
           ,[IdUserModifica]
           ,[VistaNombre] IdVistaNombre
           ,'ORGANIZACION' DataTipo
from dbo.HomologacionEsquema0 
SET IDENTITY_INSERT dbo.HomologacionEsquema ON;
GO


select * from HomologacionEsquema

truncate table  HomologacionEsquema

 
EXEC DBO.Bitacora '@script','06-CAN_LoadData_SQLSERVER.sql'

EXEC sp_configure 'show advanced options', 1;
RECONFIGURE;
EXEC sp_configure 'Ad Hoc Distributed Queries', 1;
RECONFIGURE;


/*-----------------------------------------------------------------------------------------/
En local acá se debe colocar la ruta del archivo completa desde la raíz del disco
en docker se la puede copiar a un directorio temporal del container para ejecutarla
Desde la raiz del proyecto se peude copiar al docker usando los comandos:
  - docker cp .\BaseApp\script\00-Homologacion.csv mssql-container:/tmp/
  - docker cp .\BaseApp\script\00-HomologacionEsquema.csv mssql-container:/tmp/
\----------------------------------------------------------------------------------------*/

SET IDENTITY_INSERT dbo.Homologacion ON;
BULK INSERT dbo.Homologacion
FROM '/SAE/Homologacion0.csv'
WITH (
    FIELDTERMINATOR = ';',
    ROWTERMINATOR = '\n',
    FIRSTROW = 2,
	KEEPIDENTITY
);
SET IDENTITY_INSERT dbo.Homologacion OFF;


SET IDENTITY_INSERT dbo.HomologacionEsquema ON;
BULK INSERT dbo.HomologacionEsquema
FROM '/SAE/HomologacionEsquema0.csv'
WITH (
    FIELDTERMINATOR = ';',
    ROWTERMINATOR = '\n',
    FIRSTROW = 2,
	KEEPIDENTITY
);
SET IDENTITY_INSERT dbo.HomologacionEsquema OFF;
--EXEC DBO.Bitacora 'BULK INSERT dbo.HomologacionEsquema'


-- DECLARE @CLAVE NVARCHAR(32);
-- SET		@CLAVE = (SELECT LOWER(CONVERT(NVARCHAR(32), HASHBYTES('MD5', 'usuario23'), 2)));
-- INSERT INTO dbo.Usuario 
-- (Email, Nombre, Apellido, Telefono, Clave, Rol, IdUserCreacion, IdUserModifica)
-- VALUES ('admin@gmail.com', 'admin', 'admin', '593961371400', @CLAVE, 'ADMIN', 0, 0);
-- EXEC DBO.Bitacora 'INSERT INTO dbo.Usuario '


-- EXEC DBO.Bitacora 