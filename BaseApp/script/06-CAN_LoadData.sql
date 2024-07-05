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
 
EXEC DBO.Bitacora '@script','05-CAN_LoadData.sql'
EXEC sp_configure 'show advanced options', 1;
RECONFIGURE;
EXEC sp_configure 'Ad Hoc Distributed Queries', 1;
RECONFIGURE;
EXEC DBO.Bitacora 'sp_configure BULK INSERT'

SET IDENTITY_INSERT dbo.Homologacion ON;
BULK INSERT dbo.Homologacion
FROM 'C:\pat_mic\Buscador\BaseApp\script\00-Homologacion.csv'
WITH (
    FIELDTERMINATOR = ';',
    ROWTERMINATOR = '\n',
    FIRSTROW = 2,
    CODEPAGE = '65001', --para las tildes
	KEEPIDENTITY
);
SET IDENTITY_INSERT dbo.Homologacion OFF;
EXEC DBO.Bitacora 'BULK INSERT dbo.Homologacion'

SET IDENTITY_INSERT dbo.HomologacionEsquema ON;
BULK INSERT dbo.HomologacionEsquema
FROM 'C:\pat_mic\Buscador\BaseApp\script\00-HomologacionEsquema.csv'
WITH (
    FIELDTERMINATOR = ';',
    ROWTERMINATOR = '\n',
    FIRSTROW = 2,
    CODEPAGE = '65001', --para las tildes
	KEEPIDENTITY
);
SET IDENTITY_INSERT dbo.HomologacionEsquema OFF;
EXEC DBO.Bitacora 'BULK INSERT dbo.HomologacionEsquema'


DECLARE @CLAVE NVARCHAR(32);
SET		@CLAVE = (SELECT LOWER(CONVERT(NVARCHAR(32), HASHBYTES('MD5', 'usuario23'), 2)));
INSERT INTO dbo.Usuario 
(Email, Nombre, Apellido, Telefono, Clave, Rol, IdUserCreacion, IdUserModifica)
VALUES ('admin@gmail.com', 'admin', 'admin', '593961371400', @CLAVE, 'ADMIN', 0, 0);
EXEC DBO.Bitacora 'INSERT INTO dbo.Usuario '


EXEC DBO.Bitacora 