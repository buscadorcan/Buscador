namespace SharedApp.Models.Dtos
{
    public class BuscadorDto
    {
        public List<BuscadorResultadoDataDto>? Data { get; set; }
        public int TotalCount { get; set; }
        public List<vwPanelONADto>? PanelONA { get; set; }
    }
}