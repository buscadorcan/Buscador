DROP TABLE if exists [dbo].[EsquemaOrganiza]
CREATE TABLE [dbo].[EsquemaOrganiza](
	 PK					NVARCHAR (400)  
	,IdEsquemaData		INT NOT  NULL
	,IdEsquemaVista     INT NOT  NULL
	,VistaFK            NVARCHAR (400)  
	,VistaPK            NVARCHAR (400)  
	,DataEsquemaJson    NVARCHAR (max)
	,IdEsquema			INT NOT  NULL
	,ONAIdONA			INT NOT  NULL
	,ONASiglas			NVARCHAR (40)
	,ONAPais			NVARCHAR (40) 
	,ORGPais			NVARCHAR (40) 
	,ORGRazonSocial		NVARCHAR (40) 
	,ORGNombre			NVARCHAR (400) 
	,DataFecha          DATETIME NOT NULL DEFAULT(GETDATE())
	,CONSTRAINT         [PK_EO_IdEsquemaData]	PRIMARY KEY CLUSTERED (IdEsquemaData)
	,CONSTRAINT         [FK_EO_IdEsquemaVista]  FOREIGN KEY (IdEsquemaVista) REFERENCES EsquemaVista(IdEsquemaVista)
);