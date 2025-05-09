using BlazorBootstrap;
using Blazored.LocalStorage;
using Infractruture.Interfaces;
using Microsoft.AspNetCore.Components;
using SharedApp.Helpers;

namespace ClientAppAdministrador.Layout
{
    public partial class AdminLayout
    {
        Sidebar2 sidebar;

        [Inject]
        private ICatalogosService? _Icatalogo { get; set; }

        [Inject]
        ILocalStorageService iLocalStorageService { get; set; }

        private string TituloMenu;
        private string ClasePage;
        private string PathIconMenu;

        [Parameter] public EventCallback<List<NavItem>> OnNavItemsChanged { get; set; }

        // private IEnumerable<NavItem>? navItems = new List<NavItem>();
        private IEnumerable<CustomNavItem>? navItems = new List<CustomNavItem>();

        private bool IsSidebarVisible { get; set; } = true;
        private bool IsSubMenuVisible { get; set; } = false;

        private string SidebarClass => IsSidebarVisible ? "" : "hidden";
        private string SidebarToggleText => IsSidebarVisible ? "Ocultar Menú" : "Mostrar Menú";

        private string SubMenuClass => IsSubMenuVisible ? "show" : "";

        // Obtención de datos desde LocalStorage
        private int rol;
        private string nombreRol;
        private string nombre;
        private string apellidos;
        private string codigoRol;

        protected override async Task OnInitializedAsync()
        {

            codigoRol = await LocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Codigo_Rol_Local);

            if (codigoRol == "KEY_USER_CAN")
            {
                nombreRol = await LocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Nombre_Rol_Local);
            }
            else if (codigoRol == "KEY_USER_ONA")
            {
                nombreRol = await LocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Nombre_Rol_Local);
            }
            else if (codigoRol == "KEY_USER_READ")
            {
                nombreRol = await LocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Nombre_Rol_Local);
            }
            else
            {
                nombreRol = await LocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Nombre_Rol_Local);
            }

            nombre = await LocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Nombre_Local);
            apellidos = await LocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Apellido_Local);

            // if (navItems is null || navItems.Count == 0)

            navItems = await GetNavItemsAsync();

            await Task.Delay(2000);
        }

        private void ToggleSidebar()
        {
            IsSidebarVisible = !IsSidebarVisible;
        }

        private void ToggleSubMenu()
        {
            IsSubMenuVisible = !IsSubMenuVisible;
        }

        private async Task<Sidebar2DataProviderResult> Sidebar2DataProvider(Sidebar2DataProviderRequest request)
        {
            // if (navItems is null)
            //     navItems = await GetNavItemsAsync();

            // await Task.Delay(2000);
            // return await Task.FromResult(request.ApplyTo(navItems));
            if (navItems is null)
                navItems = await GetNavItemsAsync();

            await Task.Delay(2000);
            // Si el método ApplyTo espera una lista de NavItem, se extrae la propiedad NavItem de cada CustomNavItem:
            var navItemsBase = navItems.Select(c => c.NavItem).ToList();
            return await Task.FromResult(request.ApplyTo(navItemsBase));
        }
        public class CustomNavItem
        {
            public NavItem NavItem { get; set; }
            public string CodigoHomologacion { get; set; }
            public string Icono { get; set; }

            public CustomNavItem(NavItem navItem, string codigoHomologacion, string icono)
            {
                NavItem = navItem;
                CodigoHomologacion = codigoHomologacion;
                Icono = icono;
            }
        }
        private async Task<IEnumerable<CustomNavItem>> GetNavItemsAsync()
        {
            // Se obtiene la ruta base del icono desde LocalStorage o se usa un valor por defecto.
            PathIconMenu = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Menu_PathIconAppSetting) ?? "admin/img/";
            // Obtener los elementos del menú desde la API o servicio.
            var menuItems = await _Icatalogo.GetMenusAsync();

            // Obtener el título del menú y el rol del usuario desde LocalStorage.
            TituloMenu = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Menu_Titulo_Local);
            var rol = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Codigo_Rol_Local);

            // Filtrar elementos del menú donde el rol del usuario esté en CodigoHomologacionRol.
            var filteredMenuItems = menuItems
                .Where(menu =>
                    !string.IsNullOrEmpty(menu.CodigoHomologacionRol) &&
                    menu.CodigoHomologacionRol.Split(',')
                        .Select(r => r.Trim())
                        .Contains(rol))
                .OrderBy(menu => menu.MostrarWebOrden)
                .ToList();

            // Convertir los elementos filtrados en objetos CustomNavItem
            var customNavItems = filteredMenuItems.Select(menu =>
            {
                // Mapear a un valor del enum IconName de BlazorBootstrap.
                BlazorBootstrap.IconName iconName = menu.CodigoHomologacion switch
                {
                    "KEY_MENU_ONA" => BlazorBootstrap.IconName.PersonFill,
                    "KEY_MENU_CON" => BlazorBootstrap.IconName.Link,
                    "KEY_MENU_USU" => BlazorBootstrap.IconName.PeopleFill,
                    "KEY_MENU_GRU" => BlazorBootstrap.IconName.Diagram2Fill,
                    "KEY_MENU_CAM" => BlazorBootstrap.IconName.Columns,
                    "KEY_MENU_ESQ" => BlazorBootstrap.IconName.Diagram3Fill,
                    "KEY_MENU_VAL" => BlazorBootstrap.IconName.CheckCircleFill,
                    "KEY_MENU_SEP" => BlazorBootstrap.IconName.Dash,
                    _ => BlazorBootstrap.IconName.QuestionCircleFill
                };

                // Crear el objeto NavItem (del ensamblado BlazorBootstrap)
                var navItem = new NavItem
                {
                    Id = "", // O menu.IdHomologacionMenu.ToString() si lo necesitas
                    Text = menu.MostrarWeb,
                    Href = menu.href,
                    IconName = iconName
                };

                // Concatenar la ruta base con el nombre del archivo para obtener la ruta completa del icono.
                // fullIconPath hace referencia a Icono de vista vwMenu
                string fullIconPath = $"{PathIconMenu}{menu.Icono}";
                // Retornar el CustomNavItem con el NavItem original, el código de homologación y la ruta completa del icono.
                return new CustomNavItem(navItem, menu.CodigoHomologacion, fullIconPath);
            }).ToList();

            // Si necesitas enviar la lista de navegación a otro componente (por ejemplo, AdminLayout),
            // puedes enviar la lista de NavItem extraída de los CustomNavItem.
            await OnNavItemsChanged.InvokeAsync(customNavItems.Select(c => c.NavItem).ToList());

            return customNavItems;
        }

        private void SalirSesion()
        {
            try
            {}
            catch
            {
                throw;
            }
        }
    }
}
