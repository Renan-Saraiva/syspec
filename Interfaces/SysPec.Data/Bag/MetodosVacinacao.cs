using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysPec.Data.Models
{
    public class MetodosVacinacao
    {
        public string Name { get; set; }
        public int Id { get; set; }
    }

    public class Metodos : List<MetodosVacinacao> 
    {
        public static Metodos List() 
        {
            return new Metodos {
                new MetodosVacinacao { Id = 1 , Name = "Injeção" },
                new MetodosVacinacao { Id = 2 , Name = "Pulverização" },
                new MetodosVacinacao { Id = 3 , Name = "Oral" },
                new MetodosVacinacao { Id = 4 , Name = "Pomada" },
                new MetodosVacinacao { Id = 5 , Name = "Outros" }
            };
        }

        public static string GetNomeMetodo(int Id) 
        {
            return Metodos.List().FirstOrDefault(n => n.Id == Id).Name;
        }
    }
}
