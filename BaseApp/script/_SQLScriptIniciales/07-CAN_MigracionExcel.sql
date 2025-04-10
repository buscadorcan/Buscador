DROP TABLE if exists [dbo].[MigracionExcel] ;
CREATE TABLE [dbo].[MigracionExcel]  (
    IdMigracionExcel		INT IDENTITY(1,1)
    ,MigracionNumero	INT				NOT NULL DEFAULT 0
    ,MigracionEstado	NVARCHAR(10)    NOT NULL DEFAULT ('')		
    ,ExcelFileName	  NVARCHAR(MAX) NOT NULL DEFAULT ('')
    ,MensageError		  NVARCHAR(MAX) NOT NULL DEFAULT ('')
    ,FechaCreacion		DATETIME	    NOT NULL DEFAULT GETDATE()
    ,IdUserCreacion		INT			      NOT NULL DEFAULT(0)
    ,CONSTRAINT  [PK_ME_IdMigracionExcel]		PRIMARY KEY CLUSTERED (IdMigracionExcel) 
    ,CONSTRAINT  [CK_ME_MigracionEstado]	CHECK   (MigracionEstado	IN ('PENDING', 'PROCESSING', 'SUCCESS', 'ERROR'))
);