using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SysPec.Data.Utils;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace SysPec.Data.Models
{
    public class Alimentacao
    {
        public int Id { get; set; }
        [Required]
        public int Animal { get; set; }
        [Display(Name = "Ração")]
        public int Racao { get; set; }
        [Required]
        public int Pasto { get; set; }
        [Required]
        [Range(1.0, 3000.0, ErrorMessage = "O Peso do animal deve estar entre 1 e 3000 kilos")]
        [Display(Name="Peso (Média de peso dos animais)")]
        public float Peso { get; set; }
        [Display(Name = "Anotações")]
        [StringLength(8000, ErrorMessage = "As Anotações não podem ultrapassar 8000 caracteres")]
        public string Anotacoes { get; set; }
        public DateTime CriadoEm { get; set; }
        [Required]
        public int Lote { get; set; }
        public bool Antigo { get; set; }

        public string CodigoAnimal { get; set; }
        public string LoteNome { get; set; }
        public int FazendaAnimal { get; set; }
        public string PastoNome { get; set; }

        public void Add()
        {
            int id;
            SqlXmlRun.Execute("SysPec_p_AddAlimentacao", this, "alimentacao", out id);
            this.Id = id;
        }

        public static Alimentacao Get(int Alimentacao)
        {
            return SqlXmlGet<Alimentacao>.Select("SysPec_p_GetAlimentacao", new SqlXmlParams("Id", Alimentacao));
        }

        public void AddReplicandoParaLote()
        {
            SqlXmlRun.Execute("SysPec_p_AddAlimentacaoEmLote", this);
        }
    }

    [Serializable]
    [XmlRoot("Alimentacoes")]
    public class Alimentacoes : List<Alimentacao>
    {
        public static Alimentacoes ListByRacao(int RacaoId, int FazendaId)
        {
            return SqlXmlGet<Alimentacoes>.Select("SysPec_p_ListAlimentacaoByFazendaAndRacao", new SqlXmlParams("RacaoId", RacaoId, "FazendaId", FazendaId));
        }

        public static Alimentacoes ListByPasto(int PastoId)
        {
            return SqlXmlGet<Alimentacoes>.Select("SysPec_p_ListAlimentacaoByPasto", new SqlXmlParams("PastoId", PastoId));
        }

        public static Alimentacoes ListByAnimal(int AnimalId)
        {
            return SqlXmlGet<Alimentacoes>.Select("SysPec_p_ListAlimentacaoByAnimal", new SqlXmlParams("AnimalId", AnimalId));
        }

        public static Alimentacoes ListByLote(int LoteId)
        {
            return SqlXmlGet<Alimentacoes>.Select("SysPec_p_ListAlimentacaoByLote", new SqlXmlParams("LoteId", LoteId));
        }
    }
}
