using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using SysPec.Data.Utils;
using System.Xml.Serialization;

namespace SysPec.Data.Models
{
    public class Aplicacao : IEquatable<Aplicacao>
    {
        public int Id { get; set; }
        [Required]
        public int Lote { get; set; }
        [Required]
        [Display(Name="Vacina")]
        public int Vacina { get; set; }
        [Required]
        [Display(Name = "Método")]
        public int Metodo { get; set; }
        public DateTime CriadoEm { get; set; }
        [Display(Name="Anotações")]
        [StringLength(8000, ErrorMessage = "As Anotações não podem ultrapassar 8000 caracteres")]
        public string Anotacoes { get; set; }
        [Display(Name="Validade")]
        public DateTime Validade { get; set; }
        [Display(Name = "Dosagem")]
        public float Dosagem { get; set; }
        [Required]
        public int Animal { get; set; }
        public string CodigoAnimal {get; set;}
		public string LoteNome {get; set;}
		public int FazendaAnimal {get; set;}      
   
        public void Add()
        {
            int id;
            SqlXmlRun.Execute("SysPec_p_AddAplicacao", this, "result", out id);
            this.Id = id;
        }

        public void AddReplicandoParaLote() 
        {
            SqlXmlRun.Execute("SysPec_p_AddAplicacaoEmLote", this);
        }

        public static Aplicacao Get(int Id) 
        {
            return SqlXmlGet<Aplicacao>.Select("SysPec_p_GetAplicacao", new SqlXmlParams("Id", Id));
        }

        public bool Equals(Aplicacao other)
        {

            //Check whether the compared object is null.
            if (Object.ReferenceEquals(other, null)) return false;

            //Check whether the compared object references the same data.
            if (Object.ReferenceEquals(this, other)) return true;

            //Check whether the products' properties are equal.
            return this.Animal.Equals(other.Animal) && this.Vacina.Equals(other.Vacina);
        }

        // If Equals() returns true for a pair of objects 
        // then GetHashCode() must return the same value for these objects.

        public override int GetHashCode()
        {

            //Get hash code for the Name field if it is not null.
            int hashProductAnimal  = Animal.GetHashCode();

            //Get hash code for the Code field.
            int hashProductVacina = Vacina.GetHashCode();

            //Calculate the hash code for the product.
            return hashProductAnimal ^ hashProductVacina;
        }
    }

    [Serializable]
    [XmlRoot("Aplicacoes")]
    public class Aplicacoes : List<Aplicacao>, IDisposable
    {
        public void Dispose() 
        {
            GC.SuppressFinalize(this);
        }

        public static Aplicacoes ListByVacina(int VacinaId, int FazendaId) 
        {
            return SqlXmlGet<Aplicacoes>.Select("SysPec_p_ListAplicacaoByVacina", new SqlXmlParams("VacinaId", VacinaId, "FazendaId", FazendaId));
        }

        public static Aplicacoes ListByAnimal(int AnimalId)
        {
            return SqlXmlGet<Aplicacoes>.Select("SysPec_p_ListAplicacaoByAnimal", new SqlXmlParams("AnimalId", AnimalId));
        }
    }
}
