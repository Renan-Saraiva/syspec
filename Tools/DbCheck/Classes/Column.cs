using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace dbcheck
{
    public class Column
    {
        public Column()
        {
        }
        public Column(string name, string type, int length, bool nullable, int ordinal, bool identity, string defaultValue)
        {
            Name = name;
            Type = type;
            Length = length;
            Nullable = nullable;
            Ordinal = ordinal;
            Identity = identity;
            DefaultValue = defaultValue;
        }
        public Column(string name, string type, int length, bool nullable, int ordinal, bool identity, string defaultValue, byte numericPrecision, int numericScale)
        {
            Name = name;
            Type = type;
            Length = length;
            Nullable = nullable;
            Ordinal = ordinal;
            Identity = identity;
            DefaultValue = defaultValue;
            NumericPrecision = numericPrecision;
            NumericScale = numericScale;
        }
        public Column(string name, string type, int length, bool nullable, int ordinal, bool identity, string defaultValue, byte numericPrecision, int numericScale, bool computedPersisted, string computedStatement)
        {
            Name = name;
            Type = type;
            Length = length;
            Nullable = nullable;
            Ordinal = ordinal;
            Identity = identity;
            DefaultValue = defaultValue;
            NumericPrecision = numericPrecision;
            NumericScale = numericScale;
            ComputedStatement = computedStatement;
            ComputedPersisted = computedPersisted;
        }
        [XmlAttribute]
        public string Name;
        [XmlAttribute]
        public string Type;
        [XmlAttribute]
        public int Length;
        [XmlAttribute]
        public bool Nullable;
        [XmlAttribute]
        public int Ordinal;
        [XmlAttribute]
        public bool Identity;
        [XmlAttribute]
        public string DefaultValue;
        [XmlAttribute]
        public byte NumericPrecision;
        [XmlAttribute]
        public int NumericScale;
        [XmlAttribute]
        public bool ComputedPersisted;
        [XmlAttribute]
        public string ComputedStatement;
        [XmlIgnore]
        public bool isComputedColumn
        {
            get { return !String.IsNullOrEmpty(ComputedStatement); }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Name);
            sb.Append(" ");

            //computed Column
            if (isComputedColumn)
            {
                sb.AppendFormat("as {0}", ComputedStatement);
                if (ComputedPersisted)
                    sb.Append(" Persisted");
            }
            else
            {
                sb.Append(Type);
                switch (Type.ToLower())
                {
                    case "varchar":
                    case "char":
                    case "nvarchar":
                    case "nchar":
                    case "binary":
                    case "varbinary":
                        if (Length == -1)
                            sb.Append("(MAX)");
                        else
                            sb.AppendFormat("({0})", Length);
                        break;
                    case "decimal":
                    case "numeric":
                        sb.AppendFormat("({0},{1})", NumericPrecision, NumericScale);
                        break;
                }
            }

            if (Identity && !isComputedColumn)
                sb.Append(" identity");
            if (Nullable && !isComputedColumn)
                sb.Append(" null");
            else if (!isComputedColumn)
                sb.Append(" not null");
            if (DefaultValue.Length > 0 && !isComputedColumn)
            {
                sb.Append(" default ");
                sb.Append(DefaultValue);
            }
            return sb.ToString();
        }

        public string SqlCreate(string tableName)
        {
            return String.Format("alter table {0} add {1}\r\ngo", tableName, this);
        }

        public string SqlAlter(string tableName)
        {
            return String.Format("alter table {0} alter column {1}\r\ngo", tableName, this);
        }
        public string SqlAlter(string tableName, bool AddOrDropPersist)
        {
            if (AddOrDropPersist)
            {
                return String.Format("alter table {0} alter column {1} add Persisted\r\ngo", tableName, this.Name);
            }
            return String.Format("alter table {0} alter column {1} drop Persisted\r\ngo", tableName, this.Name);
        }

        public string SqlAlterWithOutDefault(string tableName) 
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Name);
            sb.Append(" ");

            //computed Column
            if (isComputedColumn)
            {
                sb.AppendFormat("as {0}", ComputedStatement);
                if (ComputedPersisted)
                    sb.Append(" Persisted");
            }
            else
            {
                sb.Append(Type);
                switch (Type.ToLower())
                {
                    case "varchar":
                    case "char":
                    case "nvarchar":
                    case "nchar":
                    case "binary":
                    case "varbinary":
                        if (Length == -1)
                            sb.Append("(MAX)");
                        else
                            sb.AppendFormat("({0})", Length);
                        break;
                    case "decimal":
                    case "numeric":
                        sb.AppendFormat("({0},{1})", NumericPrecision, NumericScale);
                        break;
                }
            }

            if (Identity && !isComputedColumn)
                sb.Append(" identity");
            if (Nullable && !isComputedColumn)
                sb.Append(" null");
            else if (!isComputedColumn)
                sb.Append(" not null");

            return string.Format("alter table {0} alter column {1}\r\ngo", tableName, sb.ToString());
        }
        public string SqlAlterDefault(string tableName) {
            return string.Format("ALTER TABLE {0} ADD DEFAULT {1} FOR {2}", tableName,this.DefaultValue, this.Name);
        }
        public string SqlDropDefault(string tableName, string ConstraintName)
        {
            StringBuilder sb = new StringBuilder();
            if (!string.IsNullOrEmpty(ConstraintName))
            {
                sb.Append("-- To set a new default value is necessary drop the current default constraint.\r\n");
                sb.AppendFormat("alter table {0} drop constraint {1}\r\ngo\r\n", tableName, ConstraintName);
            }
            return sb.ToString();
        }

        public string SqlDrop(string tableName)
        {
            return String.Format("alter table {0} drop column {1}\r\ngo", tableName, Name);
        }

        public bool Equals(string tableName, Column column, System.IO.StreamWriter log)
        {
            bool e = true;

            if (column.isComputedColumn) // != isComputedColumn || column.ComputedPersisted != ComputedPersisted
            {
                if (column.isComputedColumn == isComputedColumn)
                {
                    if (column.ComputedStatement != ComputedStatement)
                    {
                        log.WriteLine("--COLUMN '{0}.{1}' does not match definition.", tableName, column.Name);
                        log.WriteLine("-- >> WARNING << please check the need to exclude index");
                        log.WriteLine(column.SqlDrop(tableName));
                        log.WriteLine(column.SqlCreate(tableName));
                    }
                    else if (column.ComputedPersisted != ComputedPersisted)
                    {
                        log.WriteLine("--COLUMN '{0}.{1}' does not match definition.", tableName, column.Name);
                        log.WriteLine(column.SqlAlter(tableName, column.ComputedPersisted));
                    }
                }
                else
                {
                    log.WriteLine("--COLUMN '{0}.{1}' does not match definition.", tableName, column.Name);
                    log.WriteLine(column.SqlCreate(tableName));
                }
            }
            else if ((column.Type != Type || column.Length != Length || column.Nullable != Nullable || column.Identity != Identity || column.DefaultValue != DefaultValue) && !column.isComputedColumn) // || column.Ordinal != Ordinal
            {
                log.WriteLine("--COLUMN '{0}.{1}' does not match definition.", tableName, column.Name);
                if (column.Nullable && !Nullable)
                    if (string.IsNullOrEmpty(DefaultValue))
                        log.WriteLine("--COLUMN '{0}.{1}' Warning: You need to add a default value for fields that no longer accept null.", tableName, column.Name);

                //Tinha e agora não tem
                if (!string.IsNullOrEmpty(DefaultValue) && string.IsNullOrEmpty(column.DefaultValue))
                {
                    log.WriteLine(column.SqlDropDefault(tableName, GetConstraintDefault(tableName, column.Name)));
                    log.WriteLine(column.SqlAlter(tableName));
                }
                //Não tinha e agora tem
                else if (!string.IsNullOrEmpty(column.DefaultValue) && string.IsNullOrEmpty(DefaultValue))
                {
                    log.WriteLine(column.SqlAlterWithOutDefault(tableName));
                    log.WriteLine(column.SqlAlterDefault(tableName));
                }                
                // Não mudou nada
                else if (column.DefaultValue == DefaultValue)
                {
                    log.WriteLine(column.SqlAlterWithOutDefault(tableName));
                }
                // Novo valor
                else
                {
                    log.WriteLine(column.SqlDropDefault(tableName, GetConstraintDefault(tableName, column.Name)));
                    log.WriteLine(column.SqlAlterWithOutDefault(tableName));
                    log.WriteLine(column.SqlAlterDefault(tableName));
                }

                log.WriteLine();
                e = false;
            }
            return e;
        }

        public string GetConstraintDefault(string tableName, string columnName)
        {
            string ret = string.Empty;
            using (SqlConnection cn = new SqlConnection(General.ConnectionString))
            {
                cn.Open();
                // tabelas
                using (SqlCommand cm = new SqlCommand(Queries.ConstraintDefault, cn))
                {
                    cm.Parameters.Add("ColumnName", SqlDbType.VarChar).Value = columnName;
                    cm.Parameters.Add("TableName", SqlDbType.VarChar).Value = tableName;

                    using (SqlDataReader dr = cm.ExecuteReader(CommandBehavior.SingleResult))
                    {
                        while (dr.Read())
                        {
                            ret = dr.GetString(0);
                            break;
                        }
                        dr.Close();
                    }
                }
            }
            return ret;
        }
    }

    public class ColumnCollection : List<Column>
    {
        public Column this[string name]
        {
            get
            {
                foreach (Column column in this)
                    if (String.Compare(column.Name, name, true) == 0)
                        return column;
                return null;
            }
        }

        public override string ToString()
        {
            Column c;
            StringBuilder sb = new StringBuilder();
            if (this.Count > 0)
            {
                for (int i = 0; i < this.Count - 1; i++)
                {
                    c = this[i];
                    sb.Append(" ");
                    sb.Append(c);
                    sb.Append(",");
                    sb.AppendLine();
                }
                sb.Append(" ");
                c = this[this.Count - 1];
                sb.Append(c);
                sb.AppendLine();
            }
            return sb.ToString();
        }

        public bool Equals(Table table, System.IO.StreamWriter log)
        {
            bool e = true;

            // verifica se existem e compara as colunas
            foreach (Column column in table.Columns)
            {
                Column c = this[column.Name];
                if (c == null)
                {
                    log.WriteLine("--COLUMN '{0}.{1}' not found.", table.Name, column.Name);
                    log.WriteLine(column.SqlCreate(table.Name));
                    log.WriteLine();
                    e = false;
                }
                else
                {
                    if (!c.Equals(table.Name, column, log))
                        e = false;
                }
            }

            // procura colunas a mais
            foreach (Column column in this)
            {
                Column c = table.Columns[column.Name];
                if (c == null)
                {
                    log.WriteLine("--Unknown COLUMN '{0}.{1}' found.", table.Name, column.Name);
                    log.WriteLine("/*");
                    log.WriteLine(column.SqlDrop(table.Name));
                    log.WriteLine("*/");
                    log.WriteLine();
                    e = false;
                }
            }

            return e;
        }
    }
}
