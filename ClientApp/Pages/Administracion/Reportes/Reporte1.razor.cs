using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SharedApp.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientApp.Pages.Administracion.Reportes
{
    public partial class Reporte1
    {
        // Datos simulados para el mapa
        public List<VwAcreditacionOADto> Data { get; set; } = new List<VwAcreditacionOADto>
        {
            new VwAcreditacionOADto { Pais = "Ecuador", ONA = "sae", Organizaciones = 10, Latitude = -0.1807, Longitude = -78.4678 },
            new VwAcreditacionOADto { Pais = "Peru", ONA = "IBMETRO", Organizaciones = 15, Latitude = -12.0464, Longitude = -77.0428 }
        };

        // Método ejecutado después de renderizar el componente
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                if (Data == null || !Data.Any())
                {
                    Console.WriteLine("No hay datos disponibles para inicializar el mapa.");
                    return;
                }

                // Validar los datos antes de enviarlos a JavaScript
                foreach (var item in Data)
                {
                    if (item.Latitude == 0 || item.Longitude == 0)
                    {
                        Console.WriteLine($"Datos inválidos: {item.Pais} - Latitud: {item.Latitude}, Longitud: {item.Longitude}");
                    }
                }

                // Enviar los datos al método JavaScript para inicializar el mapa
                try
                {
                    await JS.InvokeVoidAsync("initMap", Data);
                }
                catch (JSException jsEx)
                {
                    Console.WriteLine($"Error al inicializar el mapa en JavaScript: {jsEx.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error inesperado: {ex.Message}");
                }
            }
        }
    }

    // Modelo de datos utilizado para representar puntos en el mapa
    public class VwAcreditacionOADto
    {
        public string Pais { get; set; }
        public string ONA { get; set; }
        public int Organizaciones { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
