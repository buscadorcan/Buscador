﻿namespace WebApp.Models.Dtos
{
  public class DataLakeOrganizacionDto
  {
    public int IdDataLakeOrganizacion { get; set; }
    public int? IdHomologacionSistema { get; set; }
    public int? IdDataLake { get; set; }
    public string? DataEsquemaJson { get; set; }
    public string? Estado { get; set; }
    public bool? Activo { get; set; }
  }
}
