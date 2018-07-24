using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SysPec.Data.Utils;
using System.Xml.Serialization;
using System.ComponentModel.DataAnnotations;

namespace SysPec.Data.Models
{
    public class Racao
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Nome")]
        [StringLength(100, ErrorMessage = "O campo Nome não pode ultrapassar 100 caracteres", MinimumLength = 3)]
        public string Nome { get; set; }
        [Display(Name = "Anotações")]
        [StringLength(8000, ErrorMessage = "As Anotações não podem ultrapassar 8000 caracteres")]
        public string Anotacoes { get; set; }
        public int Criador { get; set; }

        public void Add()
        {
            int id;
            SqlXmlRun.Execute("SysPec_p_AddRacao", this, "racao", out id);
            this.Id = id;
        }

        public void Save()
        {
            SqlXmlRun.Execute("SysPec_p_SaveRacao", this, "racao");
        }

        public static Racao Get(int Id)
        {
            return SqlXmlGet<Racao>.Select("SysPec_p_GetRacao", new SqlXmlParams("Id", Id));
        }
    }

    [Serializable]
    [XmlRoot("Racoes")]
    public class Racoes : List<Racao>, IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public string GetNomeRacao(int id)
        {
            if(this.Count > 0 && id > 0)
                return this.FirstOrDefault(n => n.Id == id).Nome;
            return string.Empty;
        }

        public static Racoes List()
        {
            return SqlXmlGet<Racoes>.Select("");
        }

        public static Racoes ListByCriador(int IdCriador)
        {
            return SqlXmlGet<Racoes>.Select("SysPec_p_ListRacaoByCriador", new SqlXmlParams("IdCriador", IdCriador));
        }
    }
}
