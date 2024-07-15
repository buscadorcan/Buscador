using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Authorization;
using ClientApp;
using ClientApp.Services;
using ClientApp.Services.IService;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
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
builder.Services.AddSingleton<ClientApp.Services.ToastService>();
builder.Services.AddScoped<IDynamicService, DynamicService>();
builder.Services.AddScoped<IConexionService, ConexionService>();

builder.Services.AddBlazoredLocalStorage();

builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(
    s => s.GetRequiredService<AuthStateProvider>());

await builder.Build().RunAsync();
