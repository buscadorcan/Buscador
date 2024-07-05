using Microsoft.AspNetCore.Components;
using SharedApp.Models.Dtos;

namespace ClientApp.Pages.Administracion.Esquemas
{
    public partial class RowModal
    {
        [Parameter] public List<HomologacionDto> columnas { get; set; }
        [Parameter] public List<HomologacionDto> listaVwHomologacion { get; set;}
    }
}