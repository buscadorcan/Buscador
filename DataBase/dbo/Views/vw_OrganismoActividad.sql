

CREATE view [dbo].[vw_OrganismoActividad]	as
select ONASiglas Organismos,count(*) Consultas
from EsquemaOrganiza
group by ONASiglas
