using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysPec.Data.Models
{
    public class Raca
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class Racas : List<Raca>
    {
        public static Racas List()
        {
            return new Racas
            {
                new Raca { Id = 1, Name = "Salers"},
                new Raca { Id = 2, Name = "Santa Gertrudis"},
                new Raca { Id = 3, Name = "Senangus"},
                new Raca { Id = 4, Name = "Aberdeen Angus"},
                new Raca { Id = 5, Name = "Abondance"},
                new Raca { Id = 6, Name = "Aberdeen Angus"},
                new Raca { Id = 7, Name = "Abondance"},
                new Raca { Id = 8, Name = "Africander"},
                new Raca { Id = 9, Name = "Ayrshire"},
                new Raca { Id = 10, Name = "Barzona"},
                new Raca { Id = 11, Name = "Beef Friesian"},
                new Raca { Id = 12, Name = "Beefalo"},
                new Raca { Id = 13, Name = "Beefmaster"},
                new Raca { Id = 14, Name = "Belgian Blue"},
                new Raca { Id = 15, Name = "Belmont Red"},
                new Raca { Id = 16, Name = "Belted Galloway"},
                new Raca { Id = 17, Name = "Blonde D´Aquitaine"},
                new Raca { Id = 18, Name = "Bonsmara"},
                new Raca { Id = 19, Name = "Braford"},
                new Raca { Id = 20, Name = "Brahman"},
                new Raca { Id = 21, Name = "Brangus"},
                new Raca { Id = 22, Name = "Pardo Suiço"},
                new Raca { Id = 23, Name = "Campino Red Pied"},
                new Raca { Id = 24, Name = "Canadienne"},
                new Raca { Id = 25, Name = "Canchim"},
                new Raca { Id = 26, Name = "Caracu"},
                new Raca { Id = 27, Name = "Charbray"},
                new Raca { Id = 28, Name = "Charolês"},
                new Raca { Id = 29, Name = "Charolês Mocho"},
                new Raca { Id = 30, Name = "Chianina"},
                new Raca { Id = 31, Name = "Curraleiro"},
                new Raca { Id = 32, Name = "Danish Jersey"},
                new Raca { Id = 33, Name = "Devon"},
                new Raca { Id = 34, Name = "Devon Mocho"},
                new Raca { Id = 35, Name = "Dinamarquês Vermelho"},
                new Raca { Id = 36, Name = "East Fleming Red Pied"},
                new Raca { Id = 37, Name = "Flamenga"},
                new Raca { Id = 38, Name = "Fribourg"},
                new Raca { Id = 39, Name = "Galloway (BEEF)"},
                new Raca { Id = 40, Name = "Galloway Dairy"},
                new Raca { Id = 41, Name = "Gascone"},
                new Raca { Id = 42, Name = "Gelbvieh"},
                new Raca { Id = 43, Name = "Gir"},
                new Raca { Id = 44, Name = "Gir Leiteiro"},
                new Raca { Id = 45, Name = "Gir Leiteiro Mocho"},
                new Raca { Id = 46, Name = "Gir Mocho"},
                new Raca { Id = 47, Name = "Girolando"},
                new Raca { Id = 48, Name = "Gronninger"},
                new Raca { Id = 49, Name = "Guzerá"},
                new Raca { Id = 50, Name = "Guzerá Leiteiro"},
                new Raca { Id = 51, Name = "Guzolando"},
                new Raca { Id = 52, Name = "Hereford"},
                new Raca { Id = 53, Name = "Holandês Vermelho"},
                new Raca { Id = 54, Name = "Indubrasil"},
                new Raca { Id = 55, Name = "Jafarabadi"},
                new Raca { Id = 56, Name = "Jersey"},
                new Raca { Id = 57, Name = "Lavinia"},
                new Raca { Id = 58, Name = "Limousin"},
                new Raca { Id = 59, Name = "Limousin Mocho"},
                new Raca { Id = 60, Name = "Lincoln Red"},
                new Raca { Id = 61, Name = "Luing"},
                new Raca { Id = 62, Name = "Maine-Anjou"},
                new Raca { Id = 63, Name = "Marchigiana"},
                new Raca { Id = 64, Name = "Maremmana"},
                new Raca { Id = 65, Name = "Montbeliard"},
                new Raca { Id = 66, Name = "Nelore"},
                new Raca { Id = 67, Name = "Normando"},
                new Raca { Id = 68, Name = "Pantaneiro"},
                new Raca { Id = 69, Name = "Parthenais"},
                new Raca { Id = 70, Name = "Pie Rouge"},
                new Raca { Id = 71, Name = "Piemontês"},
                new Raca { Id = 72, Name = "Pinzgauer"},
                new Raca { Id = 73, Name = "Pitangueiras"},
                new Raca { Id = 74, Name = "Ranger"},
                new Raca { Id = 75, Name = "Red Angus"},
                new Raca { Id = 76, Name = "Red Brahman"},
                new Raca { Id = 77, Name = "Red Brangus"},
                new Raca { Id = 78, Name = "Romagnola"},
                new Raca { Id = 79, Name = "Rotbunte"},
                new Raca { Id = 80, Name = "Senepol"},																				
                new Raca { Id = 81, Name = "Shortorn"},
                new Raca { Id = 82, Name = "Siboney"},
                new Raca { Id = 83, Name = "Simbrasil"},
                new Raca { Id = 84, Name = "Sindi"},																				
                new Raca { Id = 85, Name = "South Poll"},
                new Raca { Id = 86, Name = "Tabapuã"},
                new Raca { Id = 87, Name = "Tarentaise"},
                new Raca { Id = 88, Name = "Tuli"},
                new Raca { Id = 89, Name = "Outra"}
            };
        }

        public static string GetNomeRaca(int id)
        {
            return Racas.List().FirstOrDefault(n => n.Id == id).Name;
        }
    }
}
