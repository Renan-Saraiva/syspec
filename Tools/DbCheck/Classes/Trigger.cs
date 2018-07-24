using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace dbcheck
{
    public class Trigger
    {
        public Trigger()
        {
        }
        public Trigger(string name, string body)
        {
            Name = name;
            Body = body;
        }
        [XmlAttribute]
        public string Name;
        [XmlAttribute]
        public string Body;

        [XmlIgnore]
        public string CleanBody
        {
            get
            {
                StringBuilder sb = new StringBuilder(Body);
                sb.Replace("\r", "");
                sb.Replace("\n", "");
                sb.Replace(" ", "");
                return sb.ToString().ToLower();
            }
        }

        public override string ToString()
        {
            return Name;
        }

        public string SqlCreate()
        {
            if (Body.EndsWith("\r\n"))
                return Body + "go";
            else
                return Body + "\r\ngo";
        }

        public string SqlDrop()
        {
            return String.Format("drop trigger {0}\r\ngo", Name);
        }

        public bool Equals(string tableName, Trigger trigger, System.IO.StreamWriter log)
        {
            bool e = true;

            if (trigger.CleanBody != CleanBody)
            {
                log.WriteLine("--TRIGGER '{0}.{1}' does not match definition.", tableName, Name);
                log.WriteLine(trigger.SqlDrop());
                log.WriteLine(trigger.SqlCreate());
                log.WriteLine();
                e = false;
            }

            return e;
        }
    }

    public class TriggerCollection : List<Trigger>
    {
        public Trigger this[string name]
        {
            get
            {
                foreach (Trigger trigger in this)
                    if (String.Compare(trigger.Name, name, true) == 0)
                        return trigger;
                return null;
            }
        }

        public string SqlCreate()
        {
            StringBuilder sb = new StringBuilder();
            foreach (Trigger c in this)
                sb.AppendLine(c.SqlCreate());
            return sb.ToString();
        }

        public bool Equals(Table table, System.IO.StreamWriter log)
        {
            bool e = true;

            // verifica os triggers
            foreach (Trigger trigger in table.Triggers)
            {
                Trigger c = this[trigger.Name];
                if (c == null)
                {
                    log.WriteLine("--TRIGGER '{0}.{1}' not found.", table.Name, trigger);
                    log.WriteLine(trigger.SqlCreate());
                    log.WriteLine();
                    e = false;
                }
                else
                {
                    if (!c.Equals(table.Name, trigger, log))
                        e = false;
                }
            }

            // procura triggers a mais
            foreach (Trigger trigger in this)
            {
                Trigger c = table.Triggers[trigger.Name];
                if (c == null)
                {
                    log.WriteLine("--Unknown TRIGGER '{0}.{1}' found.", table.Name, trigger);
                    log.WriteLine("/*");
                    log.WriteLine(trigger.SqlDrop());
                    log.WriteLine("*/");
                    log.WriteLine();
                    e = false;
                }
            }

            return e;
        }
    }
}
