

CREATE VIEW [dbo].[vw_OecPais] AS
 select OrgPais Pais,  Count(orgpais) Organizacion from EsquemaOrganiza
 group by OrgPais 

