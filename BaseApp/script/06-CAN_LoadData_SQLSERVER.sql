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

SELECT   ROW_NUMBER() OVER(ORDER BY IdHomologacion ASC) IdHomologacionNew
		,IdHomologacion	
		,IdHomologacionGrupo	
		,MostrarWebOrden	
		,MostrarWeb	
		,TooltipWeb	
		,CodigoHomologacion	
		,NombreHomologado
--into	Homologacion1
FROM Homologacion
where Estado = 'A'

SET IDENTITY_INSERT dbo.Homologacion ON;
INSERT INTO [dbo].[Homologacion] 
(		 IdHomologacion	
		,IdHomologacionGrupo	
		,MostrarWebOrden	
		,MostrarWeb	
		,TooltipWeb	
		,CodigoHomologacion	
		,NombreHomologado
)
select	 IdHomologacionNew
		,IdHomologacionGrupo	
		,MostrarWebOrden	
		,MostrarWeb	
		,TooltipWeb	
		,CodigoHomologacion	
		,NombreHomologado
From Homologacion1
SET IDENTITY_INSERT dbo.Homologacion OFF;
GO


 select *
 into Esquema2
 from Esquema


DECLARE @TextoBuscar	 NVARCHAR(5);
DECLARE @TextoReemplazar NVARCHAR(5);
-- Cursor para iterar sobre cada registro de Homologacion
DECLARE homologacion_cursor CURSOR FOR
SELECT IdHomologacion, IdHomologacionNew	FROM Homologacion1 where IdHomologacion > 10;
-- Abrir el cursor
OPEN homologacion_cursor;
FETCH NEXT FROM homologacion_cursor INTO @TextoBuscar, @TextoReemplazar;
WHILE @@FETCH_STATUS = 0
BEGIN
    UPDATE Esquema2
    SET EsquemaJson = REPLACE(EsquemaJson, ':'+@TextoBuscar+'}', ':'+@TextoReemplazar+'}');
	print @TextoBuscar + '  --  '+ @TextoReemplazar
    -- Leer el siguiente registro del cursor
    FETCH NEXT FROM homologacion_cursor INTO @TextoBuscar, @TextoReemplazar;
END;
CLOSE homologacion_cursor;
DEALLOCATE homologacion_cursor;

SET IDENTITY_INSERT dbo.Esquema ON;
INSERT INTO [dbo].[Esquema] 
(		 IdEsquema	
		,MostrarWebOrden	
		,MostrarWeb	
		,TooltipWeb	
		,EsquemaJson
)
select	 ROW_NUMBER() OVER(ORDER BY IdEsquema ASC)
		,MostrarWebOrden	
		,MostrarWeb	
		,TooltipWeb	
		,EsquemaJson
From Esquema2
SET IDENTITY_INSERT dbo.Esquema OFF;
GO


 insert EsquemaVista ( IdONA,IdEsquema,VistaOrigen	)
 select o.IdONA,IdEsquema, e.esquemaVista
 from Esquema   e
 cross join ona o 
 where e.esquemaVista <>''
 go


	truncate table EsquemaVistaColumna
	insert   EsquemaVistaColumna (
	 IdEsquemaVista          
    ,ColumnaEsquemaIdH       
    ,ColumnaEsquema	         
    ,ColumnaVista	         
	)						 
	SELECT   DISTINCT
			-- e.IdEsquema 				EsquemaId					
			--,e.EsquemaVista				EsquemaVista				
			 ev.IdEsquemaVista							IdEsquemaVista
			,JSON_VALUE(j.value, '$.IdHomologacion') 	ColumnaEsquemaIdH		
			--,isnull(h.IdHomologacion,0)					ColumnaEsquemaIdH
			,isnull(h.NombreHomologado,'') 				ColumnaEsquema
			,isnull(v.COLUMN_NAME,'')					ColumnaVista		
    FROM	[dbo].[Esquema]				e
	INNER JOIN  [dbo].[EsquemaVista]	ev		on e.IdEsquema = ev.IdEsquema   
    CROSS APPLY OPENJSON(e.EsquemaJson)			j
    LEFT JOIN	[dbo].[Homologacion]			h	ON h.IdHomologacion = JSON_VALUE(j.value, '$.IdHomologacion') 
    LEFT JOIN	SAE.INFORMATION_SCHEMA.COLUMNS	v	ON v.COLUMN_NAME = h.NombreHomologado
         AND	v.TABLE_NAME = e.EsquemaVista
         --AND  v.TABLE_SCHEMA = 'dbo' 
    WHERE	ISJSON( e.EsquemaJson)= 1
	AND  e.EsquemaVista	<> '' 
	AND  ev.IdONA = 1

UPDATE EsquemaVistaColumna 
SET ColumnaVistaPK = 1 
WHERE ColumnaVista IN (	
 'IdOnaDatos'					-- VistaOrigen = 'VS_Organizacion'				--1	esq_oec
,'IdLaboratorioCalibracion'		-- VistaOrigen = 'VS_LaboratorioCalibracion'	--2	Esquema de Laboratorios de Calibración
,'IdProducto'					-- VistaOrigen = 'VS_Producto'					--3	Esquema de Certificación de Producto
,'IdSistemaGestion'				-- VistaOrigen = 'VS_SistemaGestion'			--4	Esquema de Certificación de Sistemas de Gestión.
,'IdOrganismoInspeccion'		-- VistaOrigen = 'VS_OrganismoInspeccion'		--5	Esquema de Organismos de Inspección
,'IdEnsayo'						-- VistaOrigen = 'VS_Ensayo'					--6	Esquema de Laboratorios de Ensayo
,'IdClinico'					-- VistaOrigen = 'VS_Clinico'					--7	Esquema de Laboratorios Clínicos
,'IdCertificacionPersona'       -- VistaOrigen = 'VS_CertificacionPersona'		--8	Esquema de Certificación de Personas
,'IdVerificacionValidacion'     -- VistaOrigen = 'VS_VerificacionValidacion'	--9	Esquema de Organismos de Verificación/Validación
,'IdEnsayoAptitud'		        -- VistaOrigen = 'VS_EnsayoAptitud'				--11Esquema de Ensayos de Aptitud
)
--,( 1, 10 ,	 							--10	Organismo Nacional de Acreditación
											--12	Esquema de Muestreo
											--13	Esquema de Productores de materiales de referencia



 
EXEC sp_configure 'show advanced options', 1;
RECONFIGURE;
EXEC sp_configure 'Ad Hoc Distributed Queries', 1;
RECONFIGURE;


/*-----------------------------------------------------------------------------------------/
En local acá se debe colocar la ruta del archivo completa desde la raíz del disco
en docker se la puede copiar a un directorio temporal del container para ejecutarla
Desde la raiz del proyecto se peude copiar al docker usando los comandos:
  - docker cp .\BaseApp\script\00-Homologacion.csv mssql-container:/tmp/
  - docker cp .\BaseApp\script\00-HomologacionEsquema.csv mssql-container:/tmp/
\----------------------------------------------------------------------------------------*/

SET IDENTITY_INSERT dbo.Homologacion ON;
BULK INSERT dbo.Homologacion
FROM '/SAE/Homologacion0.csv'
WITH (
    FIELDTERMINATOR = ';',
    ROWTERMINATOR = '\n',
    FIRSTROW = 2,
	KEEPIDENTITY
);
SET IDENTITY_INSERT dbo.Homologacion OFF;


SET IDENTITY_INSERT dbo.HomologacionEsquema ON;
BULK INSERT dbo.HomologacionEsquema
FROM '/SAE/HomologacionEsquema0.csv'
WITH (
    FIELDTERMINATOR = ';',
    ROWTERMINATOR = '\n',
    FIRSTROW = 2,
	KEEPIDENTITY
);
SET IDENTITY_INSERT dbo.HomologacionEsquema OFF;
--EXEC DBO.Bitacora 'BULK INSERT dbo.HomologacionEsquema'


-- DECLARE @CLAVE NVARCHAR(32);
-- SET		@CLAVE = (SELECT LOWER(CONVERT(NVARCHAR(32), HASHBYTES('MD5', 'usuario23'), 2)));
-- INSERT INTO dbo.Usuario 
-- (Email, Nombre, Apellido, Telefono, Clave, Rol, IdUserCreacion, IdUserModifica)
-- VALUES ('admin@gmail.com', 'admin', 'admin', '593961371400', @CLAVE, 'ADMIN', 0, 0);
-- EXEC DBO.Bitacora 'INSERT INTO dbo.Usuario '


-- EXEC DBO.Bitacora 