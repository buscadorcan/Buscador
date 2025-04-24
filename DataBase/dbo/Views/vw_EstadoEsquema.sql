CREATE VIEW [dbo].[vw_EstadoEsquema]  AS
Select OrgEsquemaAcreditado Esquema, 
ISNULL(OrgEstadoAcreditado, 'Nulo') Estado, 
count (orgesquemaacreditado) Organizacion  from EsquemaOrganiza
group by OrgEsquemaAcreditado , OrgEstadoAcreditado
