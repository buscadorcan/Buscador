

CREATE view [dbo].[vwAcreditacionOna] as
 select OrgPais Pais, ONASiglas ONA, Count(orgpais) Organizaciones from EsquemaOrganiza
 group by OrgPais , ONASiglas 	
