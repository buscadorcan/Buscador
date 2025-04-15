using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedApp.Dtos
{
    [Table("vw_FiltrosAnidados")]
    public class vw_FiltrosAnidadosDto
    {
        [Key]
        public string KEY_FIL_ONA { get; set; }
        public string KEY_FIL_PAI { get; set; }
        public string KEY_FIL_EST { get; set; }
        public string KEY_FIL_ESO { get; set; }
        public string KEY_FIL_NOR { get; set; }
        public string KEY_FIL_REC { get; set; }

    }
}
