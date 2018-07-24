using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace dbcheck
{
    public class Index
    {
        public Index()
        {
        }
        public Index(string name, bool clustered, bool unique, string fileGroup)
        {
            Name = name;
            Clustered = clustered;
            Unique = unique;
            FileGroup = fileGroup;
            Columns = new List<string>();
        }
        public Index(string name, bool clustered, bool unique, string fileGroup, string partitionSchema)
        {
            Name = name;
            Clustered = clustered;
            Unique = unique;
            FileGroup = fileGroup;
            PartitionSchema = partitionSchema;
            Columns = new List<string>();
        }
        [XmlAttribute]
        public string Name;
        [XmlAttribute]
        public bool Clustered;
        [XmlAttribute]
        public bool Unique;
        [XmlAttribute]
        public string FileGroup;
        [XmlAttribute]
        public string PartitionSchema;
        [XmlAttribute]
        public List<string> Columns;

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (Clustered)
                sb.Append("clustered ");
            if (Unique)
                sb.Append("unique ");
            sb.Append("index ");
            sb.Append(Name);
            sb.Append(" on {0} (");
            if (Columns.Count > 0)
            {
                foreach (string s in Columns)
                {
                    sb.Append(s.Replace('|', ' '));
                    sb.Append(",");
                }
                sb.Remove(sb.Length - 1, 1);
            }
            sb.AppendFormat(") on [{0}]", FileGroup);
            return sb.ToString();
        }

        public string SqlCreate(string tableName)
        {
            return String.Format("create {0}\r\ngo", String.Format(this.ToString(), tableName));
        }

        public string SqlDrop(string tableName)
        {
            return String.Format("drop index {0}.{1}\r\ngo", tableName, Name);
        }

        public bool Equals(string tableName, Index index, System.IO.StreamWriter log)
        {
            bool e = true;

            if (index.Clustered != Clustered || index.Unique != Unique || index.Columns.Count != Columns.Count || String.Compare(index.FileGroup, FileGroup, true) != 0)
            {
                //-- TODO: Checa se a base já esta particionada [CO]
                if (String.IsNullOrEmpty(PartitionSchema))
                {
                    log.WriteLine("--INDEX '{0}.{1}' does not match definition.", tableName, Name);
                    log.WriteLine(index.SqlDrop(tableName));
                    log.WriteLine(index.SqlCreate(tableName));
                    log.WriteLine();
                    e = false;
                }
            }
            else
            {
                for (int i = 0; i < Columns.Count; i++)
                {
                    string c = Columns[i];
                    if (index.Columns.IndexOf(c) != i)
                    {
                        log.WriteLine("--INDEX column '{0}' in '{1}.{2}' does not match definition.", c, tableName, Name);
                        log.WriteLine(index.SqlDrop(tableName));
                        log.WriteLine(index.SqlCreate(tableName));
                        log.WriteLine();
                        e = false;
                    }
                }
            }

            return e;
        }
    }

    public class IndexCollection : List<Index>
    {
        public Index this[string name]
        {
            get
            {
                foreach (Index index in this)
                    if (String.Compare(index.Name, name, true) == 0)
                        return index;
                return null;
            }
        }

        public string SqlCreate(string tableName)
        {
            StringBuilder sb = new StringBuilder();
            foreach (Index c in this)
            {
                sb.AppendLine(c.SqlCreate(tableName));
            }
            return sb.ToString();
        }

        public bool Equals(Table table, System.IO.StreamWriter log)
        {
            bool e = true;

            // compara os indices atuais
            foreach (Index index in table.Indexes)
            {
                Index c = this[index.Name];
                if (c == null)
                {
                    log.WriteLine("--INDEX '{0}.{1}' not found.", table.Name, index);
                    log.WriteLine(index.SqlCreate(table.Name));
                    log.WriteLine();
                    e = false;
                }
                else
                {
                    if (!c.Equals(table.Name, index, log))
                        e = false;
                }
            }

            // procura por indices a mais
            foreach (Index index in this)
            {
                Index c = table.Indexes[index.Name];
                if (c == null)
                {
                    log.WriteLine("--Unknown INDEX '{0}.{1}' found.", table.Name, index);
                    log.WriteLine("/*");
                    log.WriteLine(index.SqlDrop(table.Name));
                    log.WriteLine("*/");
                    log.WriteLine();
                    e = false;
                }
            }

            return e;
        }
    }
}
