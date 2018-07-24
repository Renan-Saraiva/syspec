using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysPec.Data.Models
{
    public class LoteModelBag
    {
        public Lote Lote { get; set; }
        [Range(1, 100, ErrorMessage = "A Quantidade de Animais deve estar entre 1 e 100 animais")]
        [Display(Name = "Quantidade de animais")]
        public int QtdDeAnimais { get; set; }
        public Animal Animal { get; set; }
    }
}
