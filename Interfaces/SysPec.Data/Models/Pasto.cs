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
    public class Pasto
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "O Campo nome não pode ultrapassar 100 caracteres",MinimumLength=3)]
        public string Nome { get; set; }
        [Required]
        [Display(Name="Quantidade máxima de animais suportados")]
        [Range(1, 1000, ErrorMessage = "A Quantidade Máxima de Animais Suportados devem estar entre 1 e 1000 animais")]
        public int QtdAnimaisSuporte { get; set; }
        [StringLength(8000,ErrorMessage="As Anotações não podem ultrapassar 8000 caracteres")]
        public string Anotacoes { get; set; }
        public int Fazenda { get; set; }
        public bool Habilitado { get; set; }
        public int QtdAnimaisAtualmente { get; set; }

        public int PorcentagemUtilizacao
        {
            get
            {
                if (QtdAnimaisAtualmente > QtdAnimaisSuporte)
                    return 100;
                if (QtdAnimaisAtualmente == 0)
                    return 0;
                return Convert.ToInt32((double)((double)QtdAnimaisAtualmente / (double)QtdAnimaisSuporte) * 100);
            }
        }

        public string PorcentagemUtilizacaoFormatada
        {
            get
            {
                return string.Format("{0} %", this.PorcentagemUtilizacao);
            }
        }

        public void Add()
        {
            int id;
            SqlXmlRun.Execute("SysPec_p_AddPasto", this, "pasto", out id);
            this.Id = id;
        }

        public void Save()
        {
            SqlXmlRun.Execute("SysPec_p_SavePasto", this, "pasto");
        }

        public static Pasto Get(int Id)
        {
            return SqlXmlGet<Pasto>.Select("SysPec_p_GetPasto", new SqlXmlParams("Id", Id));
        }

        public int GetQtnAnimais()
        {
            int Quantidade = 0;
            SqlXmlRun.Execute("SysPec_p_GetQtnAnimalByPasto", out Quantidade, new SqlXmlParams("pasto", this.Id));
            this.QtdAnimaisAtualmente = Quantidade;
            return Quantidade;
        }
    }

    [Serializable]
    [XmlRoot("Pastos")]
    public class Pastos : List<Pasto>, IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public static Pastos List(int IdFazenda)
        {
            return SqlXmlGet<Pastos>.Select("SysPec_p_ListPasto", new SqlXmlParams("IdFazenda", IdFazenda));
        }
    }
}
