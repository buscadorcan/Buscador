
/*----------------------------------------------------------------------------------------\
|    ©Copyright 2K25					                          BUSCADOR ANDINO		  |
|-----------------------------------------------------------------------------------------|
| Este código está protegido por las leyes y tratados internacionales de derechos de autor|
\-----------------------------------------------------------------------------------------/
  [App]				: Buscador Andino											
	- Date          : 2K25.FEB.25	
	- Author        : patricio.paccha														
	- Version	    : 1.0										
	- Description   : Vista para las organicaiones certificadas
\----------------------------------------------------------------------------------------*/
--CREATE   VIEW [dbo].[vwEsquemaOrganiza] AS
----| 2K25.FEB.05 | patricio.paccha | BUSCADOR ANDINO | Versión: 1.0 
----| Description   : Vista para las organicaiones certificadas
--SELECT [PK]
--      ,[IdEsquemaData]
--      ,[IdEsquema]
--      ,[ONAIdONA]
--      ,[ONAPais]
--      ,[ONAUrlIcono]
--      ,[ONASiglas]
--      ,[OrgNombreComercial]
--      ,[OrgRazonSocial]
--      ,[OrgPais]
--      ,[OrgCiudad]
--      ,[OrgDireccion]
--      ,[OrgTelefono]
--      ,[OrgWeb]
--      ,[OrgCorreo]
--      ,[OrgEstadoAcreditado]
--      ,[OrgEsquemaAcreditado]
--      ,[OrgNormaAcreditada]
--      ,[OrgReconocimiento]
--	  , CASE  WHEN [OrgFechaEfectivaAcreditacion] IS NOT NULL AND [OrgFechaEfectivaAcreditacion] <> '' 
--            THEN FORMAT(TRY_CONVERT(DATETIME, [OrgFechaEfectivaAcreditacion], 101), 'yyyy/MM/dd')
--		else '--'
--		end as OrgFechaEfectivaAcreditacion
--      ,[OrgPeriodoVigencia]
-- , CASE  WHEN [OrgFechaActualizacion] IS NOT NULL AND [OrgFechaActualizacion] <> '' 
--            THEN FORMAT(TRY_CONVERT(DATETIME, [OrgFechaActualizacion], 101), 'yyyy/MM/dd')
--		else '--'
--		end as OrgFechaActualizacion
--      ,[OrgCodigoAcreditacion]
--      ,[OrgUrlCertificado]
--      ,[DataFecha]
--  FROM [CAN].[dbo].[EsquemaOrganiza](NOLOCK)
