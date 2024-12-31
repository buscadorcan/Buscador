using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class VwMenu
    {
        [Key]
        public int IdHomologacionItemMenu { get; set; }
        public string ItemMenu { get; set; }
        public string CodigoHomologacion { get; set; }
    }
}
