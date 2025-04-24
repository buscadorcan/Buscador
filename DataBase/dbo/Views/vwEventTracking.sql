CREATE   VIEW vwEventTracking AS
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
