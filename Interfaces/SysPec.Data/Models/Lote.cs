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
    public class Lote
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "O campo Nome não pode ultrapassar 100 caracteres", MinimumLength = 3)]
        public string Nome { get; set; }
        public int Fazenda { get; set; }
        [StringLength(8000, ErrorMessage = "As Anotações não podem ultrapassar 8000 caracteres")]
        public string Anotacoes { get; set; }
        public DateTime CriadoEm { get; set; }
        public bool Habilitado { get; set; }

        public void Add()
        {
            int id;
            DateTime now;
            SqlXmlRun.Execute("SysPec_p_AddLote", this, "lote", out id, out now);
            this.Id = id;
            this.CriadoEm = now;
        }

        public void Save()
        {
            SqlXmlRun.Execute("SysPec_p_SaveLote", this);            
        }

        public static Lote Get(int Id)
        {
            return SqlXmlGet<Lote>.Select("SysPec_p_GetLote", new SqlXmlParams("Id", Id));
        }

        public static int GetQtnAnimais(int Id)
        {
            int Quantidade = 0;
            SqlXmlRun.Execute("SysPec_p_GetQtnAnimalByLote", out Quantidade, new SqlXmlParams("@lote", Id));
            return Quantidade;
        }
    }

    [Serializable]
    [XmlRoot("Lotes")]
    public class Lotes : List<Lote>, IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public static Lotes List(int IdFazenda)
        {
            return SqlXmlGet<Lotes>.Select("SysPec_p_ListLote", new SqlXmlParams("IdFazenda", IdFazenda));
        }
    }
}
