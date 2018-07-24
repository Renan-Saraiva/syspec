using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace dbcheck
{
    public class Key
    {
        public Key()
        {
        }
        public Key(string name, string uniqueTable, string type)
        {
            Name = name;
            Type = type;
            UniqueTable = uniqueTable;
            Columns = new List<string>();
            UniqueColumns = new List<string>();
        }
        public Key(string name, string uniqueTable, string type, string partitionSchema)
        {
            Name = name;
            Type = type;
            UniqueTable = uniqueTable;
            PartitionSchema = partitionSchema;
            Columns = new List<string>();
            UniqueColumns = new List<string>();
        }
        [XmlAttribute]
        public string Name;
        [XmlAttribute]
        public string UniqueTable;
        [XmlAttribute]
        public string Type;
        [XmlAttribute]
        public List<string> Columns;
        [XmlAttribute]
        public List<string> UniqueColumns;
        [XmlAttribute]
        public string FileGroup;
        [XmlAttribute]
        public string PartitionSchema;

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("constraint ");
            sb.Append(Name);
            sb.Append(" ");
            sb.Append(Type);
            sb.Append(" (");
            if (Columns.Count > 0)
            {
                foreach (string s in Columns)
                {
                    sb.Append(s);
                    sb.Append(",");
                }
                sb.Remove(sb.Length - 1, 1);
            }
            sb.Append(")");
            if (UniqueTable.Length > 0)
            {
                sb.Append(" references ");
                sb.Append(UniqueTable);
                sb.Append(" (");
                if (UniqueColumns.Count > 0)
                {
                    foreach (string s in UniqueColumns)
                    {
                        sb.Append(s);
                        sb.Append(",");
                    }
                    sb.Remove(sb.Length - 1, 1);
                }
                sb.Append(")");
            }
            if (String.Compare(Type, "primary key", true) == 0)
            {
                if (!String.IsNullOrEmpty(FileGroup) && FileGroup.Length > 0)
                    sb.AppendFormat(" on [{0}]", FileGroup);
                else
                    sb.Append(" on [primary]");
            }
            else
            {
                if (String.Compare(Type, "foreign key", true) != 0)
                {
                    if (!String.IsNullOrEmpty(FileGroup) && FileGroup.Length > 0)
                        sb.AppendFormat(" on [{0}]", FileGroup);
                    else
                        sb.Append(" on [indexes]");
                }
            }
            return sb.ToString();
        }

        public string SqlCreate(string tableName)
        {
            return String.Format("alter table {0} add {1}\r\ngo", tableName, this);
        }

        public string SqlDrop(string tableName)
        {
            return String.Format("alter table {0} drop constraint {1}\r\ngo", tableName, Name);
        }

        public bool Equals(string tableName, Key key, System.IO.StreamWriter log)
        {
            bool e = true;

            if (key.Type != Type || key.Columns.Count != Columns.Count || key.UniqueTable != UniqueTable || key.UniqueColumns.Count != UniqueColumns.Count)
            {
                //-- TODO: Checa se a base já esta particionada [CO]
                if (String.IsNullOrEmpty(this.PartitionSchema))
                {
                    log.WriteLine("--KEY '{0}.{1}' does not match definition.", tableName, Name);
                    log.WriteLine(key.SqlDrop(tableName));
                    log.WriteLine(key.SqlCreate(tableName));
                    log.WriteLine();
                    e = false;
                }
            }
            else
            {
                for (int i = 0; i < Columns.Count; i++)
                {
                    string c = Columns[i];
                    string c2 = "";
                    if (key.Type == "FOREIGN KEY")
                        c2 = UniqueColumns[i];
                    if (key.Columns.IndexOf(c) != i || (key.Type == "FOREIGN KEY" && key.UniqueColumns.IndexOf(c2) != i))
                    {
                        log.WriteLine("--KEY column '{0}' in '{1}.{2}' does not match definition.", c, tableName, Name);
                        log.WriteLine(key.SqlDrop(tableName));
                        log.WriteLine(key.SqlCreate(tableName));
                        log.WriteLine();
                        e = false;
                    }
                }
            }

            return e;
        }
    }

    public class KeyCollection : List<Key>
    {
        public Key this[string name]
        {
            get
            {
                foreach (Key key in this)
                    if (String.Compare(key.Name, name, true) == 0)
                        return key;
                return null;
            }
        }

        public string SqlCreate(string tableName)
        {
            StringBuilder sb = new StringBuilder();
            foreach (Key c in this)
            {
                sb.AppendLine(c.SqlCreate(tableName));
            }
            return sb.ToString();
        }

        public bool Equals(Table table, System.IO.StreamWriter log)
        {
            bool e = true;

            // compara as chaves
            foreach (Key key in table.Keys)
            {
                Key c = this[key.Name];
                if (c == null)
                {
                    log.WriteLine("--KEY '{0}.{1}' not found.", table.Name, key.Name);
                    log.WriteLine(key.SqlCreate(table.Name));
                    log.WriteLine();
                    e = false;
                }
                else
                {
                    if (!c.Equals(table.Name, key, log))
                        e = false;
                }
            }

            // procura chaves a mais
            foreach (Key key in this)
            {
                Key c = table.Keys[key.Name];
                if (c == null)
                {
                    log.WriteLine("--Unknown KEY '{0}.{1}' found.", table.Name, key.Name);
                    log.WriteLine("/*");
                    log.WriteLine(key.SqlDrop(table.Name));
                    log.WriteLine("*/");
                    log.WriteLine();
                    e = false;
                }
            }

            return e;
        }
    }
}
