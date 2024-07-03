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
 
BULK INSERT dbo.Homologacion
FROM 'C:\pat_mic\Buscador\BaseApp\script\00-Homologacion.csv'
WITH (
    FIELDTERMINATOR = ';',
    ROWTERMINATOR = '\n',
    FIRSTROW = 2,
    CODEPAGE = '65001' --para las tildes
);
EXEC DBO.Bitacora 'BULK INSERT dbo.Homologacion'


BULK INSERT dbo.HomologacionEsquema
FROM 'C:\pat_mic\Buscador\BaseApp\script\00-HomologacionEsquema.csv'
WITH (
    FIELDTERMINATOR = ';',
    ROWTERMINATOR = '\n',
    FIRSTROW = 2,
    CODEPAGE = '65001' --para las tildes
);
EXEC DBO.Bitacora 'BULK INSERT dbo.HomologacionEsquema'

-- add CONSTRAINT
alter table dbo.Homologacion add constraint  [PK_H_IdHomologacion]			primary KEY CLUSTERED (IdHomologacion) 
alter table dbo.Vista		 add constraint  [FK_V_IdHomologacionSistema]	FOREIGN KEY (IdHomologacionSistema)	REFERENCES Homologacion(IdHomologacion)
alter table dbo.HomologacionEsquemaVista add constraint  [FK_HEV_IdH]		FOREIGN KEY (IdHomologacion)		REFERENCES Homologacion(IdHomologacion)
alter table dbo.HomologacionEsquema add constraint  [PK_HE_IdHomologacionEsquema]	PRIMARY KEY CLUSTERED (IdHomologacionEsquema) 
alter table dbo.HomologacionEsquemaVista add constraint   [FK_HEV_IdHE]		FOREIGN KEY (IdHomologacionEsquema)	REFERENCES HomologacionEsquema(IdHomologacionEsquema)
EXEC DBO.Bitacora 'Add CONSTRAINT dbo.Homologacion ,dbo.Vista, dbo.HomologacionEsquemaVista '


DECLARE @CLAVE NVARCHAR(32);
SET		@CLAVE = (SELECT LOWER(CONVERT(NVARCHAR(32), HASHBYTES('MD5', 'usuario23'), 2)));
INSERT INTO dbo.Usuario 
(Email, Nombre, Apellido, Telefono, Clave, Rol, IdUserCreacion, IdUserModifica)
VALUES ('admin@gmail.com', 'admin', 'admin', '593961371400', @CLAVE, 'ADMIN', 0, 0);
EXEC DBO.Bitacora 'INSERT INTO dbo.Usuario '



-- insert into Vista(IdHomologacionSistema, VistaNombre) values (12, 'vwEsq01');
-- insert into Vista(IdHomologacionSistema, VistaNombre) values (13, 'vwEsq01');
-- insert into Vista(IdHomologacionSistema, VistaNombre) values (14, 'vwEsq02');
-- insert into Vista(IdHomologacionSistema, VistaNombre) values (15, 'vwEsq02');

EXEC DBO.Bitacora 