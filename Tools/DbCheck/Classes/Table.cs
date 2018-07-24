using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace dbcheck
{
    public class Table
    {
        public Table()
        {
            Columns = new ColumnCollection();
        }
        public Table(string name)
        {
            Name = name;
            Columns = new ColumnCollection();
            Keys = new KeyCollection();
            Indexes = new IndexCollection();
            Triggers = new TriggerCollection();
        }
        [XmlAttribute]
        public string Name;
        public ColumnCollection Columns;
        public KeyCollection Keys;
        public IndexCollection Indexes;
        public TriggerCollection Triggers;

        public override string ToString()
        {
            return Name;
        }

        public string SqlCreate()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("create table {0} (", Name);
            sb.AppendLine();
            sb.Append(Columns);
            sb.AppendLine(")");
            sb.AppendLine("go");
            return sb.ToString();
        }

        public string SqlDrop()
        {
            return String.Format("drop table {0}\r\ngo", Name);
        }

        public bool Equals(Table table, System.IO.StreamWriter log)
        {
            bool e = true;

            // verifica as colunas
            if (!Columns.Equals(table, log))
                e = false;

            // verifica as chaves
            if (!Keys.Equals(table, log))
                e = false;

            // verifica os indices
            if (!Indexes.Equals(table, log))
                e = false;

            // verifica os triggers
            if (!Triggers.Equals(table, log))
                e = false;

            return e;
        }
    }

    public class TableCollection : List<Table>
    {
        public Table this[string name]
        {
            get
            {
                foreach (Table table in this)
                    if (String.Compare(table.Name, name, true) == 0)
                        return table;
                return null;
            }
        }
        public int Equals(TableCollection tables, System.IO.StreamWriter log)
        {
            int e = 0;

            // verifica se as tabelas existem e tem a mesma estrutura
            foreach (Table table in tables)
            {
                Table t = this[table.Name];
                if (t == null)
                {
                    log.WriteLine("--TABLE '{0}' not found.", table.Name);
                    log.WriteLine();
                    log.WriteLine(table.SqlCreate());
                    log.WriteLine(table.Indexes.SqlCreate(table.Name));
                    log.WriteLine(table.Keys.SqlCreate(table.Name));
                    log.WriteLine(table.Triggers.SqlCreate());
                    log.WriteLine();
                    e++;
                }
                else
                {
                    if (!t.Equals(table, log))
                        e++;
                }
            }

            // procura tabelas extras
            foreach (Table table in this)
            {
                Table t = tables[table.Name];
                if (t == null)
                {
                    log.WriteLine("--Unknown TABLE '{0}' found.", table.Name);
                    log.WriteLine("/*");
                    log.WriteLine(table.SqlDrop());
                    log.WriteLine("*/");
                    e++;
                }
            }
            return e;
        }
    }
}
