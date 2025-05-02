namespace WebApp
{
    public static class Routes
    {
        /// <summary>
        /// Rutas del buscador
        /// </summary>
        public const string BUSCADOR = "api/buscador";
        public const string SEARCH_PHRASE = "search/phrase";
        public const string HOMOLOGACION_ESQUEMA_TODO = "homologacionEsquemaTodo";
        public const string HOMOLOGACION_ESQUEMA_ID = "homologacionEsquema";
        public const string FN_ESQUEMA_CABECERA_ID = "fnesquemacabecera";
        public const string HOMOLOGACION_ESQUEMA_DATO = "homologacionEsquemaDato";
        public const string ESQUEMA_DATO_BUSCADO = "EsquemaDatoBuscado";
        public const string PREDIC_WORDS = "predictWords";
        public const string VALIDATE_WORDS = "validateWords";
        public const string EVENTTRACKING = "addEventTracking";
        public const string GEO_CODE = "geocode";
        public const string EXCEL = "excel";
        public const string PDF = "pdf";
        /// <summary>
        /// RUTAS PARA EL CATALOGO
        /// </summary>
        public const string CATALOGOS = "api/catalogos";
        public const string GRID_SCHEMA = "grid/schema";
        public const string FILTERS_SCHEMA = "filters/schema";
        public const string FILTERS_DATA = "filters/data";
        public const string DIMENSION_SCHEMA = "dimensions/schema";
        public const string GRUPOS = "grupos";
        public const string ROLES = "roles";
        public const string ONAS = "onas";
        public const string MENU_CATALOGO = "menu";
        public const string PANEL = "panel";
        public const string ESQUEMA_ORGANIZACION = "EsquemaOrganiza";
        public const string VW_ONA = "vwona";
        public const string FILTERS_ANINADOS = "filters/anidados";
        public const string ANINADOS = "anidados";
        /// <summary>
        /// RUTAS PARA EL DYNAMIC 
        /// </summary>
        public const string DYNAMIC = "api/vistas";
        public const string GETPROPERTIES = $"columns/{{idOna}}/{{viewName}}";
        public const string GETVALUECOLUMNA = $"columns/{{idOna}}/{{valueColumn}}/{{viewName}}";
        public const string GETLISTAVALIDACIONESQUEMA = $"validacion/{{idOna}}/{{idEsquema}}";
        public const string TEST = $"test/{{idOna:int}}";
        public const string MIGRAR = $"migrar/{{idOna:int}}";
        /// <summary>
        /// RUTAS PARA EL EMAIL 
        /// </summary>
        public const string ENVIAR = "enviar";
        /// <summary>
        /// RUTAS PARA EL ESQUEMAS
        /// </summary>
        public const string ESQUEMA = "api/esquema";
        public const string VALIDACION = "validacion";
        public const string VALIDACION_ACTUALIZACION = "validacion/actualizar";
        public const string VISTA_COLUMNA = "vista/columna";
        public const string ESQUEMA_ID = $"esquemas/{{idOna}}";
        /// <summary>
        /// RUTAS PARA EL segumiento
        /// </summary>
        public const string EVENT_TRACKING = "api/eventtracking";
        public const string EVENT_USER_ALL = "EventUserAll";
        public const string EVENT = "Even";
        public const string EVENT_DELETE = $"DeleteEven/{{fini}}/{{ffin}}";
        public const string EVENT_DELETE_ID = $"DeleteEvenById/{{codigoEvento}}";
        public const string EVENT_SESSION = "EventSession";
        public const string COORDINATES = $"coordinates/{{ip}}";
        public const string EVENT_PAG_MAS_VISIT = "EventPagMasVisit";
        public const string EVENT_FILTRO_MAS_USADO = "EventFiltroMasUsado";
        /// <summary>
        /// RUTAS PARA EL GOOGLE
        /// </summary>
        public const string GOOGLE = "auth/google";
        public const string AUTHORIZE = "authorize";
        public const string CALLBACK = "callback";
        /// <summary>
        /// RUTAS PARA EL HOMOLOGACION
        /// </summary>
        public const string HOMOLOGACION = "api/homologacion";
        public const string FIND_BY_PARENT = "findByParent";
        public const string FIND_BY_CODIGO_HOMOLOGACION = $"findByCodigoHomologacion/{{codigoHomologacion}}";
        public const string FIND_BY_ALL_HOMOLOGACION = "FindByAll";
        /// <summary>
        /// RUTAS PARA EL LOG MIGRACION
        /// </summary>
        public const string LOG_MIGRACION = "api/logmigracion";
        /// <summary>
        /// RUTAS PARA EL LOG MIGRACION
        /// </summary>
        public const string MIGRACION_EXCEL = "api/migracionexcel";
        public const string MIGRACION_UPLOAD = "upload";
        /// <summary>
        /// RUTAS PARA EL CONEXION
        /// </summary>
        public const string CONEXION = "api/conexion";
        public const string LISTA_ONA = $"ListaOna/{{idOna:int}}";
        /// <summary>
        /// RUTAS PARA EL ONAC
        /// </summary>
        public const string ONAC = "api/onac";
        public const string POST_ONA_MIGRATE = "postOnaMigrate";
        /// <summary>
        /// RUTAS PARA EL MENU
        /// </summary>
        public const string MENU = "api/menu";
        /// <summary>
        /// RUTAS PARA EL ONA
        /// </summary>
        public const string ONA = "api/ona";
        public const string LISTA_ONA_BY_ID = $"Lista/{{idOna:int}}";
        public const string PAISES = "paises";
        /// <summary>
        /// RUTAS PARA LOS REPORTES
        /// </summary>
        public const string REPORTES_VISTA = "api/reportevista";
        public const string FIND_BY_VISTA = $"findByVista/{{codigoHomologacion}}";
        public const string ACREDITACION_ONA = "acreditacion-ona";
        public const string ACREDITACION_ESQUEMA = "acreditacion-esquema";
        public const string ESTADO_ESQUEMA = "estado-esquema";
        public const string OEC_PAIS = "oec-pais";
        public const string ESQUEMA_PAIS = "esquema-pais";
        public const string OEC_FECHA = "oec-fecha";
        public const string PROFESIONAL_CALIFICADO = "profesional-calificado";
        public const string PROFESIONAL_ONA = "profesional-ona";
        public const string PROFESIONAL_ESQUEMA = "profesional-esquema";
        public const string PROFESIONAL_FECHA = "profesional-fecha";
        public const string CALIFICA_UBICACION = "califica-ubicacion";
        public const string BUSQUEDA_FECHA = "busqueda-fecha";
        public const string BUSQUEDA_FILTRO = "busqueda-filtro";
        public const string BUSQUEDA_UBICACION = "busqueda-ubicacion";
        public const string ACTUALIZACION_ONA = "actualizacion-ona";
        public const string ORGANISMO_REGISTRADO = "organismo-registrado";
        public const string ORGANISMO_ESQUEMA = "organizacion-esquema";
        public const string ORGANISMO_ACTIVIDAD = "organismo-actividad";
        /// <summary>
        /// RUTAS PARA LOS thesaurus
        /// </summary>
        public const string THESAURUS = "api/thesaurus";
        public const string GET_THESAURUS = "obtener/thesaurus";
        public const string THESAURUS_AGREGAR = "agregar/expansion";
        public const string THESAURUS_ACTUALIZAR = "actualizar/expansion";
        public const string THESAURUS_AGREGAR_SUB = $"agregar/expansion/{{expansionExistente}}/sub/{{nuevoSub}}";
        public const string THESAURUS_EJECUTAR = "ejecutar/bat";
        public const string THESAURUS_RESET = "reset/sqlserver";
        /// <summary>
        /// RUTAS PARA LOS USUARIOS
        /// </summary>
        public const string USUARIOS = "api/usuarios";
        public const string LOGIN = "login";
        public const string VALIDAR = "validar";
        public const string RECUPERAR = "recuperar";
        public const string REGISTRO = "registro";
        public const string VALIDAR_EMAIL = "validar-email";
        public const string CAMBIAR_CLAVE = "cambiar_clave";
        /// <summary>
        /// RUTAS PARA LOS UTILIDAD
        /// </summary>
        public const string UPLOAD_ICON = "UploadIcon";
        
    }
}







    