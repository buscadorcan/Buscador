namespace SharedApp.Dtos
{
    /// <summary>
    /// Clase BuscadorDto
    /// </summary>
    public class BuscadorDto
    {
        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        public List<BuscadorResultadoDataDto>? Data { get; set; }
        /// <summary>
        /// Gets or sets the total count.
        /// </summary>
        public int TotalCount { get; set; }
        /// <summary>
        /// Gets or sets the informations ONA.
        /// </summary>
        public List<vwPanelONADto>? PanelONA { get; set; }
    }
}