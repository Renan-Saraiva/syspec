using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dbcheck
{
    public class General
    {
        public static string ConnectionString 
        { 
            get 
            {
                return string.Format("server={0};database={1};user id={2};password={3};", General.ServerName, General.DatabaseName, General.UserName, General.UserPassword);
            }
        }
        public static string Schema { get; set; }
        public static string ServerName { get; set; }
        public static string DatabaseName { get; set; }
        public static string UserPassword { get; set; }
        public static string UserName { get; set; }
        public static string CompareXmlSchema { get; set; }
        public static string PrefixDb { get; set; } 
        public static bool ExtractDb { get; set; }
        public static bool CompareDb { get; set; }
        public static bool GenerateScript { get; set; }
        public static bool GenerateProgs { get; set; }
        public static bool CompareXml { get; set; }
        public static bool GenerateGlobalization { get; set; }
    }
}
