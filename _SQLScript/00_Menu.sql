USE CAN;

DROP TABLE IF EXISTS MenuRol;
GO
CREATE TABLE [dbo].[MenuRol](
	 IdMenuRol      INT IDENTITY(1,1) NOT NULL
    ,IdHRol	        INT DEFAULT(NULL) 
    ,IdHMenu   	    INT DEFAULT(NULL) 
    ,Estado         NVARCHAR(1) NOT NULL DEFAULT('A')
	,FechaCreacion  DATETIME	NOT NULL DEFAULT(GETDATE())

    ,CONSTRAINT  [PK_MR_IdMenuRol]   PRIMARY KEY CLUSTERED (IdMenuRol)
    ,CONSTRAINT  [FK_MR_IdHRol]      FOREIGN KEY([IdHRol])  REFERENCES [dbo].[Homologacion] ([IdHomologacion])
    ,CONSTRAINT  [FK_MR_IdHMenu]     FOREIGN KEY([IdHMenu]) REFERENCES [dbo].[Homologacion] ([IdHomologacion])
    ,CONSTRAINT  [CK_MR_Estado]	     CHECK   (Estado IN ('A', 'X'))
)
GO

UPDATE  Homologacion
SET      CodigoHomologacion  ='KEY_MENU_SIN'                
        ,Estado              ='A'    
WHERE   IdHomologacion       = 29;
UPDATE  Homologacion
SET      MostrarWeb          ='Reporte CAN'        
        ,TooltipWeb          ='Reporte CAN'        
        ,InfoExtraJson       ='{ "icono": "user", "referencia":"/reportecan"}'            
        ,CodigoHomologacion  ='KEY_MENU_REP_CAN'                
        ,Estado              ='A'    
WHERE   IdHomologacion       = 30;
UPDATE  Homologacion
SET      MostrarWeb          ='Reporte ONA'        
        ,TooltipWeb          ='Reporte ONA'        
        ,InfoExtraJson       ='{ "icono": "user", "referencia":"/reporteona"}'            
        ,CodigoHomologacion  ='KEY_MENU_REP_ONA'                
        ,Estado              ='A'    
WHERE   IdHomologacion       = 31;
UPDATE  Homologacion
SET     Estado               ='X'    
WHERE   IdHomologacion      in (123,124);

WITH Menu_ AS
(
	SELECT	 hi.IdHomologacion		IdHomologacionMenu
			,hi.CodigoHomologacion	CodigoHomologacion
	FROM	Homologacion hi			(NOLOCK)
	JOIN	Homologacion hp			(NOLOCK) ON hi.IdHomologacionGrupo = hp.IdHomologacion
	WHERE	hp.CodigoHomologacion = 'KEY_MENU'
	AND		hp.Estado = 'A'
	AND		hi.Estado = 'A'
	AND		hi.CodigoHomologacion is not null
)
,  Rol_ AS
(
	SELECT	IdHomologacion		 IdHomologacionRol
			,CodigoHomologacion	 
	FROM	Homologacion  
	WHERE	CodigoHomologacion IN 
    ('KEY_USER_CAN'
    ,'KEY_USER_ONA'
    ,'KEY_USER_READ'
    -- ,'KEY_USER'
    )
)
INSERT INTO MenuRol ( IdHRol, IdHMenu )
SELECT      r.IdHomologacionRol, m.IdHomologacionMenu
FROM        Menu_ m
CROSS JOIN  Rol_  r
order by    r.IdHomologacionRol;


CREATE or ALTER  VIEW [dbo].[vwMenu] AS 
	SELECT	 --hm.IdHomologacion		IdHomologacionMenu
		 	 hm.MostrarWebOrden		MostrarWebOrden
			,hm.MostrarWeb			MostrarWeb
			,hm.TooltipWeb			TooltipWeb
			,ISNULL(JSON_VALUE(hm.InfoExtraJson, '$.icono')		,'user')	Icono
			,ISNULL(JSON_VALUE(hm.InfoExtraJson, '$.referencia'),'/')		href
			,hm.CodigoHomologacion	CodigoHomologacion
            ,hr.CodigoHomologacion	CodigoHomologacionRol
	FROM    MenuRol      mr         (NOLOCK) 
    JOIN    Homologacion hm			(NOLOCK)ON hm.IdHomologacion = mr.IdHMenu
	JOIN	Homologacion hr			(NOLOCK)ON hr.IdHomologacion = mr.IdHRol
	WHERE	mr.Estado = 'A'
	AND		hm.Estado = 'A'
	AND		hr.Estado = 'A'
	AND		hm.CodigoHomologacion is not null;
GO

	
