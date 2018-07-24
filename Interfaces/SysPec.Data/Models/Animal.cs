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
    public class Animal
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        [Required]
        public int Lote { get; set; }
        public string LoteNome { get; set; }
        [Required]
        [Display(Name = "Nascido Em")]
        public DateTime NascidoEm { get; set; }
        public bool Habilitado { get; set; }
        [Required]
        [Display(Name="Raça")]
        public int Raca { get; set; }
        [Required]
        [Display(Name="Sexo")]
        public int Sexo { get; set; }
        [Required]
        [Range(1.0, 3000.0, ErrorMessage = "O Peso do animal deve estar entre 1 e 3000 kilos")]
        public float Peso { get; set; }

        public void Add()
        {
            int id;
            SqlXmlRun.Execute("SysPec_p_AddAnimal", this, "animal", out id);
            this.Id = id;
        }

        public void Save()
        {
            SqlXmlRun.Execute("SysPec_p_SaveAnimal", this, "lote");
        }

        public static Animal Get(int Id)
        {
            return SqlXmlGet<Animal>.Select("SysPec_p_GetAnimal", new SqlXmlParams("Id", Id));
        }

        public static void Disable(int Id)
        {
            SqlXmlRun.Execute("SysPec_p_DisableAnimal", new SqlXmlParams("Id", Id));
        }
    }

    [Serializable]
    [XmlRoot("Animais")]
    public class Animais : List<Animal>, IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public static Animais ListByLote(int IdLote)
        {
            return SqlXmlGet<Animais>.Select("SysPec_p_ListAnimaisByLote", new SqlXmlParams("IdLote", IdLote));
        }

        public static Animais ListByFazenda(int IdFazenda)
        {
            return SqlXmlGet<Animais>.Select("SysPec_p_ListAnimaisByFazenda", new SqlXmlParams("IdFazenda", IdFazenda));
        }

        public void AddAnimais(int IdFazenda, int IdLote)
        {
            SqlXmlRun.Execute("SysPec_p_AddAnimais", this, new SqlXmlParams("IdFazenda", IdFazenda, "IdLote", IdLote));
        }

        public static Animais ListByLoteName(string LoteName, int FazendaId)
        {
            return SqlXmlGet<Animais>.Select("SysPec_p_ListAnimaisByLoteName", new SqlXmlParams("LoteName", LoteName, "FazendaId",FazendaId));
        }

        public static Animais ListAnimaisByCodigo(string Codigo, int FazendaId)
        {
            return SqlXmlGet<Animais>.Select("SysPec_p_ListAnimaisByCodigo", new SqlXmlParams("Codigo", Codigo,"FazendaId",FazendaId));
        }

        public static int ListCountAnimaisByFazenda(int IdFazenda)
        {
            int Quantidade = 0;
            SqlXmlRun.Execute("SysPec_p_ListCountAnimaisByFazenda", out Quantidade, new SqlXmlParams("@IdFazenda", IdFazenda));
            return Quantidade;
        }
    }
}