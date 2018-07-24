using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace dbcheck
{
    public class Program
    {
        public Program()
        {
        }
        public Program(string name, string body, string type)
        {
            Name = name;
            Body = body;
            Type = type;
        }
        [XmlAttribute]
        public string Name;
        [XmlAttribute]
        public string Body;
        [XmlAttribute]
        public string Type;

        public override string ToString()
        {
            return Type + " " + Name;
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
            return String.Format("drop {0} {1}\r\ngo", Type, Name);
        }

        public bool Equals(Program program, System.IO.StreamWriter log)
        {
            bool e = true;

            if (program.Body != Body)
            {
                log.WriteLine("--{0} '{1}' does not match definition.", Type, Name);
                log.WriteLine(program.SqlDrop());
                log.WriteLine(program.SqlCreate());
                log.WriteLine();
                e = false;
            }

            return e;
        }
    }

    public class ProgramCollection : List<Program>
    {
        public Program this[string name]
        {
            get
            {
                foreach (Program program in this)
                    if (String.Compare(program.Name, name, true) == 0)
                        return program;
                return null;
            }
        }

        public string SqlCreate()
        {
            StringBuilder sb = new StringBuilder();
            foreach (Program c in this)
                sb.AppendLine(c.SqlCreate());
            return sb.ToString();
        }

        public int Equals(ProgramCollection programs, System.IO.StreamWriter log)
        {
            int e = 0;

            // compara os programas
            foreach (Program program in programs)
            {
                Program c = this[program.Name];
                if (c == null)
                {
                    log.WriteLine("--{0} '{1}' not found.", program.Type, program.Name);
                    log.WriteLine(program.SqlCreate());
                    log.WriteLine();
                    e++;
                }
                else
                {
                    if (!c.Equals(program, log))
                        e++;
                }
            }

            // procura programas a mais
            foreach (Program program in this)
            {
                Program c = programs[program.Name];
                if (c == null)
                {
                    log.WriteLine("--Unknown {0} '{1}' found.", program.Type, program.Name);
                    log.WriteLine("/*");
                    log.WriteLine(program.SqlDrop());
                    log.WriteLine("*/");
                    log.WriteLine();
                    e++;
                }
            }

            return e;
        }
    }
}
