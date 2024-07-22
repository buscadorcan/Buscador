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
    ,IdOrganizacion          NVARCHAR(32) 
    ,IdVista                 NVARCHAR(32) NOT NULL
    ,FullTextOrganizacion    NVARCHAR(MAX) NULL			-- fulltext a considerear
    ,CONSTRAINT [PK_IdOrganizacionFullText] PRIMARY KEY CLUSTERED (IdOrganizacionFullText)  
);

IF EXISTS (SELECT * FROM sys.fulltext_catalogs WHERE name = 'OrganizacionFullText_cat')
	DROP FULLTEXT CATALOG [OrganizacionFullText_cat];
GO
CREATE FULLTEXT CATALOG OrganizacionFullText_cat WITH ACCENT_SENSITIVITY = ON AS DEFAULT ;
GO

CREATE FULLTEXT INDEX ON OrganizacionFullText( 
	FullTextOrganizacion LANGUAGE 3082			-->  3082	Spanish
)     
KEY INDEX [PK_IdOrganizacionFullText]
ON	OrganizacionFullText_cat
WITH STOPLIST = SYSTEM;


--> actualizar el archivo:  con los sinonimos : C:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER\MSSQL\FTData\tsesn.xml
EXEC sys.sp_fulltext_load_thesaurus_file 3082;

EXEC sp_fulltext_service 'update_languages'  

 
EXEC DBO.Bitacora 'CREATE FULLTEXT CATALOG y INDEX ON OrganizacionFullText'
