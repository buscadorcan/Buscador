






--- 5 ---
CREATE     VIEW [dbo].[vw_EsquemaPais]    AS
 SELECT 
    OrgPais AS Pais, 
    OrgEsquemaAcreditado AS Esquema, 
    COUNT(OrgPais) AS Organizacion
FROM 
    EsquemaOrganiza
GROUP BY 
    OrgPais, 
    OrgEsquemaAcreditado
HAVING 
    COUNT(OrgPais) > 0
