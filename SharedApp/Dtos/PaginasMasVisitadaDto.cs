namespace SharedApp.Dtos
{
   public  class PaginasMasVisitadaDto
    {
        public string CodigoHomologacionRol {  get; set; }
        public string CodigoHomologacionMenu { get; set; }  
        public string IpAddress { get; set; }
        public int uso {  get; set; }
        public double? Latitud { get; set; } = null;
        public double? Longitud { get; set; } = null;
    }
}
