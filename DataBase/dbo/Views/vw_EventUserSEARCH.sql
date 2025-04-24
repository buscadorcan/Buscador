CREATE   VIEW vw_EventUserSEARCH AS
SELECT
     CodigoEvento 
    ,CodigoUsuario 
    ,OnaSiglas 
    ,Nombre 
    ,Apellido 
    ,UsuarioEmail 
    ,UsuarioTipo 
    ,Pagina 
    ,PaginaControl 
    ,PaginaAccion 
    ,UsuarioIP 
    ,UsuarioPais
    ,UsuarioCiudad
    ,ISNULL(JSON_VALUE(ParametroJson, '$.ExactaBuscar'),'')                AS ExactaBuscar 
    ,ISNULL(JSON_VALUE(ParametroJson, '$.TextoBuscar'),'')                 AS TextoBuscar 
    ,ISNULL((SELECT STRING_AGG(value, ', ') FROM OPENJSON(ParametroJson, '$.FiltroPais'))           ,'') AS FiltroPais 
    ,ISNULL((SELECT STRING_AGG(value, ', ') FROM OPENJSON(ParametroJson, '$.FiltroOna'))            ,'') AS FiltroOna 
    ,ISNULL((SELECT STRING_AGG(value, ', ') FROM OPENJSON(ParametroJson, '$.FiltroEsquema'))        ,'') AS FiltroEsquema
    ,ISNULL((SELECT STRING_AGG(value, ', ') FROM OPENJSON(ParametroJson, '$.FiltroNorma'))          ,'') AS FiltroNorma 
    ,ISNULL((SELECT STRING_AGG(value, ', ') FROM OPENJSON(ParametroJson, '$.FiltroEstado'))         ,'') AS FiltroEstado 
    ,ISNULL((SELECT STRING_AGG(value, ', ') FROM OPENJSON(ParametroJson, '$.FiltroRecomocimiento')) ,'') AS FiltroRecomocimiento
    ,ErrorTracking
    ,FechaCreacion
FROM    vwEventTracking
WHERE   UsuarioTipo = 'KEY_USER_SEARCH'
