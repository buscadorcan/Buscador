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
-- para el migrado
DROP TABLE if exists EsquemaFullText_BK;
GO
SELECT *
INTO EsquemaFullText_BK
FROM EsquemaFullText
GO

EXEC sp_fulltext_database 'enable';
GO
DROP TABLE if exists dbo.EsquemaFullText;
GO
CREATE TABLE EsquemaFullText(
     IdEsquemaFullText	INT NOT NULL IDENTITY(1,1)
    ,IdEsquemaData		INT NOT NULL 
    ,IdHomologacion		INT NOT NULL
    ,FullTextData		NVARCHAR(MAX)  COLLATE Latin1_General_CI_AI -- Modern_Spanish_CI_AS fulltext sin valorar tildes
    ,CONSTRAINT [PK_EFT_IdEsquemaFullText] PRIMARY KEY CLUSTERED (IdEsquemaFullText)  
);

IF	EXISTS (SELECT * FROM sys.fulltext_catalogs WHERE is_default = 1 AND name = 'EsquemaFullText_cat')
BEGIN
	CREATE	FULLTEXT CATALOG temp_catalog;
	ALTER	FULLTEXT CATALOG temp_catalog AS DEFAULT;
	DROP	FULLTEXT CATALOG [EsquemaFullText_cat];
END
GO

CREATE FULLTEXT CATALOG [EsquemaFullText_cat] WITH ACCENT_SENSITIVITY = OFF AS DEFAULT ;  
--ALTER FULLTEXT CATALOG EsquemaFullText_cat REBUILD WITH ACCENT_SENSITIVITY = OFF AS DEFAULT;
GO

CREATE FULLTEXT INDEX ON EsquemaFullText(
	FullTextData LANGUAGE 3082			-->  Spanish
)     
KEY INDEX [PK_EFT_IdEsquemaFullText]
ON	EsquemaFullText_cat
WITH STOPLIST = SYSTEM;
GO
DROP FULLTEXT CATALOG temp_catalog;
GO

--> Stoplist:
--CREATE FULLTEXT STOPLIST StoplistEs FROM SYSTEM STOPLIST;
--GO
--ALTER FULLTEXT STOPLIST StoplistEs ADD 'hola'	LANGUAGE 3082;
--ALTER FULLTEXT STOPLIST StoplistEs ADD 'en'		LANGUAGE 3082;
--ALTER FULLTEXT STOPLIST StoplistEs ADD 'el'		LANGUAGE 3082;
--ALTER FULLTEXT STOPLIST StoplistEs ADD 'la'		LANGUAGE 3082;
--ALTER FULLTEXT STOPLIST StoplistEs ADD 'lo'		LANGUAGE 3082;
--ALTER FULLTEXT STOPLIST StoplistEs ADD 'las'	LANGUAGE 3082;
--ALTER FULLTEXT STOPLIST StoplistEs ADD 'un'		LANGUAGE 3082;
--ALTER FULLTEXT STOPLIST StoplistEs ADD 'una'	LANGUAGE 3082;
--GO
--ALTER FULLTEXT INDEX ON EsquemaFullText SET STOPLIST StoplistEs;
--> Quitar el StoplistEs ALTER FULLTEXT INDEX ON EsquemaFullText SET STOPLIST OFF;
GO
SELECT stopword, language_id 
FROM sys.fulltext_system_stopwords 
WHERE language_id IN (3082) order by 2;
go
--SELECT * FROM sys.fulltext_stopwords 
--WHERE stoplist_id = (SELECT stoplist_id FROM sys.fulltext_stoplists WHERE name = 'StoplistEs');
--GO

--> actualizar el archivo:  con los sinonimos : C:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER\MSSQL\FTData\tsesn.xml
EXEC sys.sp_fulltext_load_thesaurus_file 3082;

EXEC sp_fulltext_service 'update_languages'  
GO

TRUNCATE TABLE EsquemaFullText
INSERT INTO EsquemaFullText (IdEsquemaData, IdHomologacion, FullTextData)
SELECT IdEsquemaData, IdHomologacion, FullTextData
FROM EsquemaFullText_BK
GO