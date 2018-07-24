using SysPec.Data.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Data;

namespace SysPec.Data.Models
{
    public class Fazenda
    {
        public int Id { get; set; }
        
        [Required]
        [Display(Name="Nome")]
        [StringLength(100, ErrorMessage = "O nome deve ter no minímos {2} caracteres", MinimumLength = 3)]
        public string Nome { get; set; }
        
        public int Criador { get; set; }

        [Required]
        [Display(Name = "Abreviatura")]
        [StringLength(3, ErrorMessage = "A abreviatura deve ter 3 caracteres", MinimumLength = 3)]
        public string  Abreviatura { get; set; }

        public void Add()
        {
            int id;
            SqlXmlRun.Execute("SysPec_p_AddFazenda", this, "fazenda", out id);
            this.Id = id;
        }

        public void Save()
        {
            SqlXmlRun.Execute("SysPec_p_SaveFazenda", this);
        }

        public static Fazenda Get(int Id)
        {
            return SqlXmlGet<Fazenda>.Select("SysPec_p_GetFazenda", new SqlXmlParams("Id", Id));
        }

        public void GetSumario(out int Animais,out int Lotes, out int Racoes, out int Pastos)
        {
            SqlXmlParamsOut Outs = new SqlXmlParamsOut("animais", SqlDbType.Int,"lotes", SqlDbType.Int,"racoes", SqlDbType.Int,"pastos", SqlDbType.Int);
            SqlXmlRun.Execute(General.ConnectionString, "SysPec_p_FazendaSumario", new SqlXmlParams("fazenda", this.Id, "criador", this.Criador), Outs);
            Animais = Convert.ToInt32(Outs["animais"]);
            Lotes = Convert.ToInt32(Outs["lotes"]);
            Racoes = Convert.ToInt32(Outs["racoes"]);
            Pastos = Convert.ToInt32(Outs["pastos"]);
        }
    }

    [Serializable]
    [XmlRoot("Fazendas")]
    public class Fazendas : List<Fazenda>, IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public static Fazendas List(int criador)
        {
            return SqlXmlGet<Fazendas>.Select("SysPec_p_ListFazenda", new SqlXmlParams("criador", criador));
        }
    }
}
