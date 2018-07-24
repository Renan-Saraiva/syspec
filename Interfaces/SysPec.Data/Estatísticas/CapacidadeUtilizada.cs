using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SysPec.Data.Utils;
using System.Xml.Serialization;

namespace SysPec.Data.Estatisticas
{
    public class CapacidadeUtilizada
    {
        public int FazendId { get; set; }
        public string FazendaNome { get; set; }
        public int QtdAnimaisSuporte { get; set; }
        public int QtdAnimais { get; set; }

        public int Porcentagem 
        {
            get 
            {
                if (QtdAnimais > QtdAnimaisSuporte)
                    return 100;
                if (QtdAnimais == 0)
                    return 0;
                return Convert.ToInt32((double)((double)QtdAnimais / (double)QtdAnimaisSuporte) * 100);
            }
        }

        public string PorcentagemFormatada 
        {
            get 
            {
                return string.Format("{0} %", this.Porcentagem);
            }
        }
    }

    [XmlRoot("Capacidades")]
    public class Capacidades : List<CapacidadeUtilizada>     
    {
        public static Capacidades ListByCriador(int CriadorId) 
        {
            return SqlXmlGet<Capacidades>.Select("SysPec_p_CapacidadeUtilizada", new SqlXmlParams("criador", CriadorId));
        }
    }
}