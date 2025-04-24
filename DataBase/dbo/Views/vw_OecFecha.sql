
CREATE VIEW [dbo].[vw_OecFecha]    AS

select Fecha, Organizacion from 


(SELECT 
    CASE 
	 WHEN a.OrgFechaEfectivaAcreditacion IS NOT NULL AND a.OrgFechaEfectivaAcreditacion <> '' 
             THEN FORMAT(TRY_CONVERT(DATETIME, a.OrgFechaEfectivaAcreditacion, 101), 'yyyy/MM')
        WHEN a.OrgFechaActualizacion IS NOT NULL AND a.OrgFechaActualizacion <> '' 
             THEN FORMAT(TRY_CONVERT(DATETIME, a.OrgFechaActualizacion, 101), 'yyyy/MM')
       
        ELSE NULL 
    END AS Fecha, count(*) Organizacion
FROM EsquemaOrganiza a
WHERE 
    (a.OrgFechaActualizacion IS NOT NULL AND a.OrgFechaActualizacion <> '') 
    OR 
    (a.OrgFechaEfectivaAcreditacion IS NOT NULL AND a.OrgFechaEfectivaAcreditacion <> '')
	group by 
	   CASE 
	    WHEN a.OrgFechaEfectivaAcreditacion IS NOT NULL AND a.OrgFechaEfectivaAcreditacion <> '' 
             THEN FORMAT(TRY_CONVERT(DATETIME, a.OrgFechaEfectivaAcreditacion, 101), 'yyyy/MM')
        WHEN a.OrgFechaActualizacion IS NOT NULL AND a.OrgFechaActualizacion <> '' 
             THEN FORMAT(TRY_CONVERT(DATETIME, a.OrgFechaActualizacion, 101), 'yyyy/MM')
       
        ELSE NULL 
    END) organizaciones
	where fecha IS NOT NULL;

