
DROP TABLE IF EXISTS MENU;

CREATE TABLE [dbo].[MENU](
	 IdMenu         INT IDENTITY(1,1) NOT NULL
    ,IdHMenu   	    INT DEFAULT(NULL) 
    ,IdHRol	        INT DEFAULT(NULL) 
    ,Permiso	    NVARCHAR(1) NOT NULL DEFAULT('R')
    ,Estado         NVARCHAR(1) NOT NULL DEFAULT('A')
	,FechaCreacion  DATETIME	NOT NULL DEFAULT(GETDATE())

    ,CONSTRAINT  [PK_M_IdMenu]  PRIMARY KEY CLUSTERED (IdMenu)
    ,CONSTRAINT  [FK_M_IdHMenu] FOREIGN KEY([IdHMenu]) REFERENCES [dbo].[Homologacion] ([IdHomologacion])
    ,CONSTRAINT  [FK_M_IdHRol]  FOREIGN KEY([IdHRol])  REFERENCES [dbo].[Homologacion] ([IdHomologacion])
    ,CONSTRAINT  [CK_M_Estado]	CHECK   (Estado IN ('A', 'X'))
) ;
GO
