using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using SysPec.Data.Utils;

namespace SysPec.Data.Models
{
    public class Criador
    {
        public int Id { get; set; }
        [Required]
        [StringLength(150, ErrorMessage = "O nome deve ter no minímos {2} caracteres", MinimumLength = 3)]
        public string Nome { get; set; }
        public string Usuario { get; set; }
        public string Telefone { get; set; }

        public void Add()
        {
            int id;
            SqlXmlRun.Execute("SysPec_p_AddCriador", this, "user", out id);
            this.Id = id;
        }

        public void Save()
        {
            SqlXmlRun.Execute("SysPec_p_SaveCriador", this, "user");
        }

        public static Criador Get(string userId) 
        {
            return SqlXmlGet<Criador>.Select("SysPec_p_GetCriador", new SqlXmlParams("user", userId));
        }
    }

    public class Criadores : List<Criador>, IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
