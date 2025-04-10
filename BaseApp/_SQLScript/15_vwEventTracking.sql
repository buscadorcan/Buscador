USE CAN
GO

DROP VIEW IF EXISTS vw_EventUserAll
GO
CREATE OR ALTER VIEW vw_EventUserAll AS
        SELECT  'Usuario CAN' AS    vw_Text  ,'vw_EventUserCAN' AS vw_EventUser
UNION   SELECT  'Usuario ONA'                   ,'vw_EventUserONA'
UNION   SELECT  'Usuario de  Consulta'          ,'vw_EventUserREAD'
UNION   SELECT  'Usuario del Buscador'          ,'vw_EventUserSEARCH';
GO

DROP VIEW IF EXISTS vwEventTracking
GO
CREATE OR ALTER VIEW vwEventTracking AS
SELECT
     e.IdEventTracking                  AS CodigoEvento 
    ,ISNULL(u.IdUsuario,0)              AS CodigoUsuario 
    ,ISNULL(o.Siglas,'BuscadorCAN')     AS OnaSiglas 
    ,ISNULL(u.Nombre    ,'UsuarioWeb')  AS Nombre 
    ,ISNULL(u.Apellido  ,'')            AS Apellido 
    ,ISNULL(u.Email     ,'')            AS UsuarioEmail 
    ,CASE e.CodigoHomologacionRol   
        WHEN 'KEY_USER_CAN'         THEN 'Administrador CAN'  
        WHEN 'KEY_USER_ONA'         THEN 'Administrador ONA'
        WHEN 'KEY_USER_READ'        THEN 'Usuario de consulta'
        ELSE 'KEY_USER_SEARCH' 
    END                             AS UsuarioTipo 
    ,CASE e.CodigoHomologacionMenu  
        WHEN 'KEY_MENU_HOME'        THEN 'Inicio'  
        WHEN 'KEY_MENU_USER'        THEN 'Usuarios'
        WHEN 'KEY_MENU_ONA'         THEN 'ONA'
        WHEN 'KEY_MENU_CAN'         THEN 'CAN'
        WHEN 'KEY_MENU_REPORT'      THEN 'Reportes'
        WHEN 'KEY_MENU_LOG'         THEN 'Log'
        WHEN 'KEY_MENU_CONFIG'      THEN 'Configuración'
        ELSE e.CodigoHomologacionMenu
    END                             AS Pagina 
    ,e.NombreControl                AS PaginaControl 
    ,e.NombreAccion                 AS PaginaAccion 
    ,ISNULL(JSON_VALUE(e.UbicacionJson, '$.IpAddress'),'')  AS UsuarioIP 
    ,ISNULL(JSON_VALUE(e.UbicacionJson, '$.Country'),  '')  AS UsuarioPais
    ,ISNULL(JSON_VALUE(e.UbicacionJson, '$.City'),     '')  AS UsuarioCiudad
    ,e.ErrorTracking                AS ErrorTracking
    ,e.FechaCreacion                AS FechaCreacion
    ,e.CodigoHomologacionRol        AS CodigoHomologacionRol
    ,e.CodigoHomologacionMenu       AS CodigoHomologacionMenu
    ,e.UbicacionJson                AS UbicacionJson
    ,e.ParametroJson                AS ParametroJson
FROM       EventTracking e (NOLOCK)
LEFT JOIN  Usuario       u (NOLOCK) ON  u.IdUsuario        = e.IdUsuario
LEFT JOIN  Homologacion  h (NOLOCK) ON  h.IdHomologacion   = u.IdHomologacionRol
LEFT JOIN  ONA           o (NOLOCK) ON  o.IdONA            = u.IdONA;
GO

DROP VIEW IF EXISTS vw_EventUserCAN
GO
CREATE OR ALTER VIEW vw_EventUserCAN AS
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
    ,NULL AS ExactaBuscar
    ,NULL AS TextoBuscar 
    ,NULL AS FiltroPais 
    ,NULL AS FiltroOna 
    ,NULL AS FiltroEsquema 
    ,NULL AS FiltroNorma 
    ,NULL AS FiltroEstado
    ,ErrorTracking
    ,FechaCreacion
FROM    vwEventTracking
WHERE   CodigoHomologacionRol = 'KEY_USER_CAN'
GO


DROP VIEW IF EXISTS vw_EventUserONA
GO
CREATE OR ALTER VIEW vw_EventUserONA AS
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
    ,NULL AS ExactaBuscar
    ,NULL AS TextoBuscar 
    ,NULL AS FiltroPais 
    ,NULL AS FiltroOna 
    ,NULL AS FiltroEsquema 
    ,NULL AS FiltroNorma 
    ,NULL AS FiltroEstado
    ,ErrorTracking
    ,FechaCreacion
FROM    vwEventTracking
WHERE   CodigoHomologacionRol = 'KEY_USER_ONA'
GO


DROP VIEW IF EXISTS vw_EventUserREAD
GO
CREATE OR ALTER VIEW vw_EventUserREAD AS
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
    ,NULL AS ExactaBuscar
    ,NULL AS TextoBuscar 
    ,NULL AS FiltroPais 
    ,NULL AS FiltroOna 
    ,NULL AS FiltroEsquema 
    ,NULL AS FiltroNorma 
    ,NULL AS FiltroEstado
    ,ErrorTracking
    ,FechaCreacion
FROM    vwEventTracking
WHERE   CodigoHomologacionRol = 'KEY_USER_READ'
GO

DROP VIEW IF EXISTS vw_EventUserSEARCH
GO
CREATE OR ALTER VIEW vw_EventUserSEARCH AS
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
GO

