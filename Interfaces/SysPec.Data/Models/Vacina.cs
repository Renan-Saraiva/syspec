using SysPec.Data.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SysPec.Data.Models
{
    public class Vacina
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
            SqlXmlRun.Execute("SysPec_p_AddVacina", this, "vacina", out id);
            this.Id = id;
        }

        public void Save()
        {
            SqlXmlRun.Execute("SysPec_p_SaveVacina", this, "vacina");
        }

        public static Vacina Get(int id)
        {
            return SqlXmlGet<Vacina>.Select("SysPec_p_GetVacina", new SqlXmlParams("vacina", id));
        }
    }

    [Serializable]
    [XmlRoot("Vacinas")]
    public class Vacinas : List<Vacina>, IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public string GetNomeVacina(int id)
        {
            if (this.Count > 0 && id > 0)
                return this.FirstOrDefault(n => n.Id == id).Nome;
            else
                return string.Empty;
        }

        public static Vacinas List()
        {
            return SqlXmlGet<Vacinas>.Select("SysPec_p_ListVacina");
        }

        public static Vacinas ListByCriador(int IdCriador)
        {
            return SqlXmlGet<Vacinas>.Select("SysPec_p_ListVacinaByCriador", new SqlXmlParams("criador", IdCriador));
        }
    }
}
