using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysPec.Data.Models
{
    public class Genealogia
    {
        public int Animal { get; set; }
        public string Pai { get; set; }
        public string Mae { get; set; }
    }

    public class Genealogias : List<Genealogia>, IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
