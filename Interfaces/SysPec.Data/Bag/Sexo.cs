using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysPec.Data.Models
{
    public class Sexo
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class Sexos : List<Sexo>
    {
        public static Sexos List() 
        {
            return new Sexos
            {
                new Sexo { Id = 1, Name = "Fêmea"},
                new Sexo { Id = 2, Name = "Macho"}
            };
        }

        public static string GetNomeSexo(int id)
        {
            return Sexos.List().FirstOrDefault(n => n.Id == id).Name;
        }
    }
}
