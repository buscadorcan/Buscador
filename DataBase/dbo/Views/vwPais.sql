

CREATE VIEW [dbo].[vwPais] AS 
SELECT 
    h1.IdHomologacion AS IdHomologacionPais,
    h1.MostrarWeb AS Pais
FROM 
    Homologacion h1 WITH (NOLOCK)
JOIN 
    Homologacion h2 WITH (NOLOCK) ON h1.IdHomologacionGrupo = h2.IdHomologacion
WHERE 
    h2.CodigoHomologacion = 'KEY_PAIS'
    AND h1.Estado = 'A'
    AND h2.Estado = 'A'
ORDER BY 
    h1.MostrarWeb
OFFSET 0 ROWS;
