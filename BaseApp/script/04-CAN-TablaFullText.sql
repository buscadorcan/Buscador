/*----------------------------------------------------------------------------------------\
|    ©Copyright 2K24												                          BUSCADOR ANDINO		  |
|-----------------------------------------------------------------------------------------|
| Este código está protegido por las leyes y tratados internacionales de derechos de autor|
\-----------------------------------------------------------------------------------------/
  [App]          : Buscador Andino											
	- Date         : 2K24.JUN.25	
	- Author       : patricio.paccha														
	- Version	     : 1.0										
	- Description  : Tablas indexada para la busqueda de organizaciones certificadas
\----------------------------------------------------------------------------------------*/

USE CAN_DB;
GO

EXEC DBO.Bitacora '@script','04-CAN-TablaFullText.sql'

DROP TABLE if exists dbo.OrganizacionFullText;
CREATE TABLE OrganizacionFullText(
     IdOrganizacionFullText	 INT NOT NULL IDENTITY(1,1)
    ,IdDataLakeOrganizacion	 INT NOT NULL 
    ,IdHomologacion          INT NOT NULL
    ,FullTextOrganizacion    NVARCHAR(MAX) NULL			-- fulltext a considerear
    ,CONSTRAINT [PK_IdOrganizacionFullText] PRIMARY KEY CLUSTERED (IdOrganizacionFullText)  
);
EXEC DBO.Bitacora 'CREATE TABLE OrganizacionFullText'

CREATE FULLTEXT INDEX ON OrganizacionFullText
( FullTextOrganizacion LANGUAGE 3082 )   --3082	Spanish 
KEY INDEX [PK_IdOrganizacionFullText]
ON OrganizacionFullText_cat
WITH STOPLIST = SYSTEM;
EXEC DBO.Bitacora 'CREATE FULLTEXT INDEX ON OrganizacionFullText'

--SELECT fulltext_catalog_id, name FROM sys.fulltext_catalogs
IF EXISTS (SELECT * FROM sys.fulltext_catalogs WHERE name = 'OrganizacionFullText_cat')
begin
	DROP FULLTEXT INDEX ON OrganizacionFullText;
	DROP FULLTEXT CATALOG [OrganizacionFullText_cat];
end

CREATE FULLTEXT CATALOG OrganizacionFullText_cat WITH ACCENT_SENSITIVITY = OFF;
GO

-- select * from sys.fulltext_catalogs;
-- Select IdDataLakeOrganizacion from OrganizacionFullText WHERE CONTAINS((FullTextOrganizacion), 'empresa');