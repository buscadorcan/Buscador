/*----------------------------------------------------------------------------------------\
|    ©Copyright 2K24												                          BUSCADOR ANDINO		  |
|-----------------------------------------------------------------------------------------|
| Este código está protegido por las leyes y tratados internacionales de derechos de autor|
\-----------------------------------------------------------------------------------------/
  [App]				: Buscador Andino											
	- Date          : 2K24.JUN.25	
	- Author        : patricio.paccha														
	- Version	    : 1.0										
	- Description   : Tabla indexada para la busqueda de organizaciones certificadas
\----------------------------------------------------------------------------------------*/

USE CAN;
GO

DROP TABLE if exists dbo.EsquemaFullText;
CREATE TABLE EsquemaFullText(
     IdEsquemaFullText		INT NOT NULL IDENTITY(1,1)
    ,IdEsquemaData		INT NOT NULL  --------IdCanDataSet, IdOrganizacionData 
    ,IdHomologacion		INT NOT NULL
    ,FullTextData		NVARCHAR(MAX)  COLLATE Latin1_General_CI_AI -- Modern_Spanish_CI_AS fulltext sin valorar tildes
    ,CONSTRAINT [PK_CFT_IdEsquemaFullText] PRIMARY KEY CLUSTERED (IdEsquemaFullText)  
);

IF	EXISTS (SELECT * FROM sys.fulltext_catalogs WHERE is_default = 1 AND name = 'EsquemaFullText_cat')
BEGIN
	CREATE FULLTEXT CATALOG temp_catalog;
	ALTER FULLTEXT CATALOG temp_catalog AS DEFAULT;
	DROP FULLTEXT CATALOG [EsquemaFullText_cat];
END
GO

CREATE FULLTEXT CATALOG EsquemaFullText_cat WITH ACCENT_SENSITIVITY = OFF AS DEFAULT ;  
--ALTER FULLTEXT CATALOG EsquemaFullText_cat REBUILD WITH ACCENT_SENSITIVITY = OFF AS DEFAULT;
GO

CREATE FULLTEXT INDEX ON EsquemaFullText( 
	FullTextData LANGUAGE 3082			-->  Spanish
)     
KEY INDEX [PK_CFT_IdEsquemaFullText]
ON	EsquemaFullText_cat
WITH STOPLIST = SYSTEM;

DROP FULLTEXT CATALOG temp_catalog;

--> actualizar el archivo:  con los sinonimos : C:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER\MSSQL\FTData\tsesn.xml
EXEC sys.sp_fulltext_load_thesaurus_file 3082;

EXEC sp_fulltext_service 'update_languages'  

 
