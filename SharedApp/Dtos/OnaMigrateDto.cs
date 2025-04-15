namespace SharedApp.Dtos
{
        public class OnaMigrateDto
        {
            public int IdEsquemaData { get; set; }
            public int IdEsquemaVista { get; set; }
            public string? VistaFK { get; set; }
            public string? VistaPK { get; set; }
            public string DataEsquemaJson  { get; set; }
            public DateTime DataFecha { get; set; }

        }
        public class OnaMigrateRequestDto
        {
            public string vista { get; set; }
            public int IdOna { get; set; }
            public int IdEsquema { get; set; }
         }
}
