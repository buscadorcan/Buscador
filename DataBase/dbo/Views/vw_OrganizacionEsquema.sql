
CREATE view [dbo].[vw_OrganizacionEsquema]	as
 select OrgEsquemaAcreditado Esquema, count(OrgEsquemaAcreditado) Organizacion
 from EsquemaOrganiza
 group by OrgEsquemaAcreditado

