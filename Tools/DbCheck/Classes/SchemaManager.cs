using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace dbcheck
{
    public class SchemaManager
    {

        public void SaveFile(string fileName, Database database)
        {
            FileStream fs = new FileStream(AppDomain.CurrentDomain.BaseDirectory + fileName, FileMode.Create);
            XmlWriter tw = XmlWriter.Create(fs, new XmlWriterSettings()
            {
                Indent = true,
                NewLineHandling = NewLineHandling.Entitize
            });
            XmlSerializer serializer = new XmlSerializer(typeof(Database));
            serializer.Serialize(tw, database);
            fs.Close();
        }

        public Database LoadFile(string path, string fileName)
        {
            if (path.Length == 0)
                path = AppDomain.CurrentDomain.BaseDirectory;

            if (!path.EndsWith("\\"))
                path += "\\";

            try
            {
                FileStream fs = new FileStream(path + fileName, FileMode.Open);
                XmlSerializer serializer = new XmlSerializer(typeof(Database));
                serializer.UnknownNode += new XmlNodeEventHandler(serializer_UnknownNode);
                serializer.UnknownAttribute += new XmlAttributeEventHandler(serializer_UnknownAttribute);
                Database l = (Database)serializer.Deserialize(fs);
                fs.Dispose();

                return l;
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Error reading schema file '{0}': {1}", fileName, ex.Message));
            }

        }

        private void serializer_UnknownNode(object sender, XmlNodeEventArgs e)
        {
            Console.WriteLine("Unknown node: " + e.Name + "='" + e.Text + "'");
        }

        private void serializer_UnknownAttribute(object sender, XmlAttributeEventArgs e)
        {
            System.Xml.XmlAttribute attr = e.Attr;
            Console.WriteLine("Unknown attribute: " + attr.Name + "='" + attr.Value + "'");
        }
    }
}
