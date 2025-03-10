La Estructura del Proyecto (ClientApp):

Componentes principales
•	App.razor → Componente raíz de la aplicación Blazor.
•	Program.cs → Punto de entrada para configurar servicios y la aplicación Blazor.

Layouts y navegación
•	Layout/AdminLayout.razor → Definición del diseño administrativo.
•	Layout/AdminSidebar.razor → Barra lateral para la administración.
•	Layout/MainLayout.razor → Layout principal para otras secciones.
•	Layout/NavMenu.razor → Menú de navegación de la aplicación.

Modelos de datos
•	Models/BuscarRequest.cs → Modelo para enviar solicitudes de búsqueda.
•	Models/DataHomologacionEsquema.cs → Probablemente relacionado con la homologación de esquemas.
•	Models/RespuestaRegistro.cs → Maneja respuestas de registros.

Helpers (utilidades)
•	Helpers/IJsHelper.cs → Interoperabilidad con JavaScript.
•	Helpers/Inicializar.cs → Posiblemente para inicialización de la aplicación.
•	Helpers/JwtParser.cs → Manejo de tokens JWT (seguridad y autenticación).

Páginas
•	Pages/Administracion/ → Módulo de administración.
•	Pages/Autenticacion/ → Módulo de autenticación de usuarios.
•	Pages/BuscadorCan/ → Posiblemente maneja la funcionalidad principal de búsqueda.
•	Pages/RedireccionarAlAcceso.razor → Página para redirigir accesos.

Servicios
•	_Imports.razor → Importaciones compartidas entre componentes.
•	App.razor → Punto de inicio de la UI Blazor.
•	Program.cs → Configuración de Blazor WebAssembly o Blazor Server.
