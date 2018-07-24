using System.Xml.Serialization;

namespace dbcheck
{
    [XmlRoot("Database", Namespace = "hppt://www.orbium.com.br", IsNullable = false)]
    public class Database
    {
        public Database()
        {
            Tables = new TableCollection();
        }
        public Database(string server, string name, string user)
        {
            Server = server;
            Name = name;
            User = user;
            Tables = new TableCollection();
            Programs = new ProgramCollection();
        }
        [XmlAttribute]
        public string Server;
        [XmlAttribute]
        public string Name;
        [XmlAttribute]
        public string User;
        public TableCollection Tables;
        public ProgramCollection Programs;
    }
}