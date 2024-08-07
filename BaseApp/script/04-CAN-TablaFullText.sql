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

DROP TABLE if exists dbo.CanFullText;
CREATE TABLE CanFullText(
     IdCanFullText		INT NOT NULL IDENTITY(1,1)
    ,IdCanDataSet		INT NOT NULL  -------- IdOrganizacionData 
    ,IdHomologacion		INT NOT NULL
    ,IdEnte				NVARCHAR(16) 
    ,IdVista			NVARCHAR(16) NOT NULL
    ,FullTextData		NVARCHAR(MAX)  COLLATE Latin1_General_CI_AI -- Modern_Spanish_CI_AS fulltext sin valorar tildes
    ,CONSTRAINT [PK_CFT_IdCanFullText] PRIMARY KEY CLUSTERED (IdCanFullText)  
);

IF	EXISTS (SELECT * FROM sys.fulltext_catalogs WHERE is_default = 1 AND name = 'CanFullText_cat')
BEGIN
	CREATE FULLTEXT CATALOG temp_catalog;
	ALTER FULLTEXT CATALOG temp_catalog AS DEFAULT;
	DROP FULLTEXT CATALOG [CanFullText_cat];
END
GO

CREATE FULLTEXT CATALOG CanFullText_cat WITH ACCENT_SENSITIVITY = OFF AS DEFAULT ;  
--ALTER FULLTEXT CATALOG CanFullText_cat REBUILD WITH ACCENT_SENSITIVITY = OFF;
GO

CREATE FULLTEXT INDEX ON CanFullText( 
	FullTextData LANGUAGE 3082			-->  3082	Spanish
)     
KEY INDEX [PK_CFT_IdCanFullText]
ON	CanFullText_cat
WITH STOPLIST = SYSTEM;

DROP FULLTEXT CATALOG temp_catalog;

--> actualizar el archivo:  con los sinonimos : C:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER\MSSQL\FTData\tsesn.xml
EXEC sys.sp_fulltext_load_thesaurus_file 3082;

EXEC sp_fulltext_service 'update_languages'  

 
EXEC DBO.Bitacora 'CREATE FULLTEXT CATALOG y INDEX ON CanFullText'
