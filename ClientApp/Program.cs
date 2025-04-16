using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Authorization;
using ClientApp;
using Infractruture.Services;
using Infractruture.Interfaces;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
// Registramos la configuración en DI para poder inyectarla en servicios:
builder.Services.AddSingleton(builder.Configuration);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddBlazorBootstrap();

builder.Services.AddScoped(sp => new HttpClient {
  BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
});

builder.Services.AddScoped<IBusquedaService, BusquedaService>();
builder.Services.AddScoped<IUsuariosService, UsuariosService>();
builder.Services.AddScoped<IServiceAutenticacion, ServiceAutenticacion>();
builder.Services.AddScoped<ICatalogosService, CatalogosService>();
builder.Services.AddScoped<IHomologacionService, HomologacionService>();
builder.Services.AddScoped<IHomologacionEsquemaService, HomologacionEsquemaService>();
builder.Services.AddSingleton<Infractruture.Services.ToastService>();
builder.Services.AddScoped<IDynamicService, DynamicService>();
builder.Services.AddScoped<IConexionService, ConexionService>();
builder.Services.AddScoped<IMigracionExcelService, MigracionExcelService>();
builder.Services.AddScoped<ILogMigracionService, LogMigracionService>();
builder.Services.AddScoped<IONAService, ONAsService>();
builder.Services.AddScoped<IMenuService, MenuService>();
builder.Services.AddScoped<IEsquemaService, EsquemaService>();
builder.Services.AddScoped<IUtilitiesService, UtilitiesService>();
builder.Services.AddScoped<IThesaurusService, ThesaurusService>();
builder.Services.AddScoped<ILoginRetryValidatorService, LoginRetryValidatorService>();
builder.Services.AddScoped<IEventService, EventService>();

builder.Services.AddScoped<IReporteService, ReporteService>();

builder.Services.AddBlazoredLocalStorage();

builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(
    s => s.GetRequiredService<AuthStateProvider>());

await builder.Build().RunAsync();
