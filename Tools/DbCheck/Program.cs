using System;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.IO;
using System.Security.Cryptography;
using System.Reflection;

namespace dbcheck
{
    static class Flags
    {
        public const int EXTRACT = 0;
        public const int COMPARE = 1;
        public const int FULLLOG = 2;
        public const int GENERATESQL = 3;
    }

    class MainProgram
    {
        //static string userName = string.Empty;
        static StreamWriter logFile = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "dbcheck.txt", false, Encoding.Default);

        static void writeLog(string msg)
        {
            Console.WriteLine(msg);
            logFile.WriteLine(msg);
        }

        static void Main(string[] args)
        {
            General.Schema = string.Empty;
            General.ServerName = string.Empty;
            General.DatabaseName = string.Empty;
            General.UserPassword = string.Empty;
            General.CompareXmlSchema = string.Empty;
            General.PrefixDb = "%";

            Version version = Assembly.GetExecutingAssembly().GetName().Version;


            writeLog(string.Format("-- DbCheck {0}.{1}.{2} - Database structure checker", version.Major, version.Minor, version.Build));
            writeLog("-- " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString());
            writeLog(string.Empty);

            if (args.Length == 0)
            {
                DisplayInfo();
                return;
            }

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "/?")
                {
                    DisplayInfo();
                    return;
                }
            }

            try
            {
                if (args[0].ToLower().EndsWith(".xml"))
                    General.Schema = args[0];

                for (int i = 1; i < args.Length; i++)
                {
                    string arg = args[i];
                    if (arg.StartsWith("/"))
                    {
                        switch (arg.ToLower())
                        {
                            case "/s":
                                General.ServerName = args[i + 1];
                                break;
                            case "/d":
                                General.DatabaseName = args[i + 1];
                                break;
                            case "/u":
                                General.UserName = args[i + 1];
                                break;
                            case "/p":
                                General.UserPassword = args[i + 1];
                                break;
                            case "/n":
                                General.GenerateScript = true;
                                break;
                            case "/x":
                                if (args.Length - 1 > i && !args[i + 1].StartsWith("/"))
                                    General.PrefixDb = args[i + 1];
                                General.ExtractDb = true;
                                break;
                            case "/c":
                                if (args.Length - 1 > i && !args[i + 1].StartsWith("/"))
                                    General.PrefixDb = args[i + 1];
                                General.CompareDb = true;
                                break;
                            //case "/v":
                            //    General.CompareXml = true;
                            //    General.CompareXmlSchema = args[i + 1];
                            //    break;
                            //case "/progs":
                            //    General.GenerateProgs = true;
                            //    break;
                            //case "/g":
                            //    General.GenerateGlobalization = true;
                            //    break;
                        }
                    }
                }
            }
            catch
            {
                writeLog("Invalid command arguments.");
                DisplayInfo();
                return;
            }

            if (General.Schema == "")
            {
                writeLog("Schema file not specified.");
                DisplayInfo();
                return;
            }

            if ((General.ExtractDb || General.CompareDb) && (General.GenerateScript || General.CompareXml))
            {
                writeLog("You cannot specify '/x' or '/c' with '/n' or '/v'.");
                DisplayInfo();
                return;
            }

            if ((General.ExtractDb || General.CompareDb) && (General.ServerName.Length == 0 || General.DatabaseName.Length == 0 || General.UserName.Length == 0))
            {
                writeLog("You must define '/s', '/d' and '/u' with the '/c' or '/x' switches.");
                DisplayInfo();
                return;
            }

            if ((General.ExtractDb || General.CompareDb) && General.UserPassword == "")
            {
                Console.Write("Password: ");
                ConsoleKeyInfo k;
                while ((k = Console.ReadKey(true)).KeyChar != '\r')
                    General.UserPassword += k.KeyChar;
                Console.WriteLine();
            }

            //connStr = String.Format("server={0};database={1};user id={2};password={3};", serverName, databaseName, userName, userPassword);

            // extract
            if (General.ExtractDb)
                ExtractSchema(General.Schema, General.ConnectionString, General.PrefixDb, General.ServerName, General.DatabaseName, General.UserName);

            // compare
            if (General.CompareDb)
                CompareDatabase(General.Schema, General.ConnectionString, General.PrefixDb, General.ServerName, General.DatabaseName, General.UserName);

            // new
            if (General.GenerateScript)
                GenerateScripts(General.Schema);

            // compare
            if (General.CompareXmlSchema.Length > 0)
                CompareScript(General.Schema, General.CompareXmlSchema);

            Console.WriteLine();

            // progs
            if (General.GenerateProgs)
                GenerateProgs(General.ConnectionString);

            //Globalization
            if (General.GenerateGlobalization)
                GenerateGlobalization(General.ConnectionString);

            // fim
            logFile.Close();

            Console.Write("Press any key to continue...");
            Console.ReadKey();
        }

        static void DisplayInfo()
        {
            writeLog("");
            writeLog("Syntax:");
            writeLog("  dbcheck <schema_file> [/s <server> /d <database> /u <user id> [/p <password>] [/x | /c]] | [/n <file> | /v <file>]");
            writeLog("Command switches:");
            writeLog("  /x    Extracts the database schema into the specified schema file.");
            writeLog("  /c    Compares the database with the specified schema file.");
            writeLog("  /n    Creates new object scripts from schema.");
            //writeLog("  /v    Compares schema file with obtained schema file.");
            //writeLog("  /g    Refreshes the records into Globalization table.");
        }

        static void GenerateScripts(string schema)
        {
            SchemaManager sm = new SchemaManager();
            Database db = sm.LoadFile("", schema);

            foreach (Table t in db.Tables)
                logFile.WriteLine(t.SqlCreate());

            logFile.WriteLine("");
            logFile.WriteLine("/*--------------------------------------------------*/");
            logFile.WriteLine("");

            foreach (Table t in db.Tables)
                foreach (Index i in t.Indexes)
                    logFile.WriteLine(i.SqlCreate(t.Name));

            logFile.WriteLine("");
            logFile.WriteLine("/*--------------------------------------------------*/");
            logFile.WriteLine("");

            foreach (Table t in db.Tables)
                foreach (Key k in t.Keys)
                    if (String.Compare(k.Type, "primary key", true) == 0)
                        logFile.WriteLine(k.SqlCreate(t.Name));

            logFile.WriteLine("");
            logFile.WriteLine("/*--------------------------------------------------*/");
            logFile.WriteLine("");

            foreach (Table t in db.Tables)
                foreach (Key k in t.Keys)
                    if (String.Compare(k.Type, "primary key", true) != 0)
                        logFile.WriteLine(k.SqlCreate(t.Name));

            logFile.WriteLine("");
            logFile.WriteLine("/*--------------------------------------------------*/");
            logFile.WriteLine("GO");
            logFile.WriteLine("");

            foreach (Table t in db.Tables)
                foreach (Trigger x in t.Triggers)
                    logFile.WriteLine(x.SqlCreate());

            logFile.WriteLine("");
            logFile.WriteLine("/*--------------------------------------------------*/");
            logFile.WriteLine("");

            foreach (Program p in db.Programs)
                logFile.WriteLine(p.SqlCreate());

            Console.WriteLine("Script generated into 'dbcheck.txt' file.");
        }

        static void Compare(Database db1, Database db2)
        {
            int c = 0;

            if ((c = db1.Tables.Equals(db2.Tables, logFile)) > 0)
                writeLog(String.Format("--Tables not matching definition: {0}.", c));
            else
                writeLog("--FULL TABLES SCHEMA MATCH!");

            writeLog("");

            if ((c = db1.Programs.Equals(db2.Programs, logFile)) > 0)
                writeLog(String.Format("--Programs not matching definition: {0}.", c));
            else
                writeLog("--FULL CODE MATCH!");

            Console.WriteLine();
            Console.WriteLine("--Results in 'dbcheck.txt' file.");
        }

        static void CompareScript(string schema, string xmlSchema)
        {
            SchemaManager sm = new SchemaManager();
            Database db1 = sm.LoadFile("", xmlSchema);
            Database db2 = sm.LoadFile("", schema);
            writeLog(String.Format("--Checking schema '{0}' against '{1}'...", xmlSchema, schema));
            writeLog("");
            Compare(db1, db2);
        }

        static void CompareDatabase(string schema, string connStr, string prefix, string server, string database, string user)
        {
            SchemaManager sm = new SchemaManager();
            Database db1 = GetDatabase(connStr, prefix, server, database, user);
            Database db2 = sm.LoadFile("", schema);
            writeLog(String.Format("--Checking database '{0}:{1}' against schema '{2}'...", server, database, schema));
            writeLog(string.Empty);
            Compare(db1, db2);
            WriteVersionLog(Assembly.GetExecutingAssembly());
        }

        static void GenerateProgs(string connStr)
        {
            var assembly = Assembly.GetExecutingAssembly();
            writeLog("--Creating Programs");
            writeLog(string.Empty);

            byte[] key = new byte[] { 22, 11, 134, 35, 85, 20, 121, 90, 84, 18, 71, 64, 73, 210, 77, 111, 192, 22, 7, 228, 150, 79, 99, 107 };
            byte[] iv = new byte[] { 185, 196, 192, 43, 246, 70, 247, 8, 94, 220, 43, 136, 207, 158, 137, 170, 199, 17, 51, 180, 241, 105, 139, 252 };
            Stream xFile = assembly.GetManifestResourceStream("dbcheck.progs.sql");
            CryptoStream cFile = new CryptoStream(xFile, new TripleDESCryptoServiceProvider().CreateDecryptor(key, iv), CryptoStreamMode.Read);
            byte[] b = new byte[xFile.Length];
            cFile.Read(b, 0, b.Length);
            StringReader sr = new StringReader(Encoding.UTF8.GetString(b));

            int t = 0;
            string line;
            StringBuilder comm = new StringBuilder();
            using (SqlConnection cn = new SqlConnection(connStr))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand("", cn))
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.Trim().ToLower().Equals("go"))
                        {
                            try
                            {
                                cm.CommandText = comm.ToString();
                                cm.ExecuteNonQuery();

                                if (t % 10 == 0)
                                    Console.Write(".");
                            }
                            catch (Exception ex)
                            {
                                writeLog(cm.CommandText);
                                writeLog(ex.Message);
                            }
                            comm = new StringBuilder();
                        }
                        else
                        {
                            if (line.StartsWith("declare @o varchar(200)"))
                                t++;
                            comm.AppendLine(line);
                        }
                    }
                    if (comm.Length > 0)
                    {
                        cm.CommandText = comm.ToString();
                        cm.ExecuteNonQuery();
                    }
                }
            }

            cFile.Close();
            cFile.Dispose();

            Console.WriteLine();
            writeLog(string.Empty);
            writeLog("Programs created: " + t.ToString());
            writeLog(string.Empty);
            writeLog("--Finished Creating Programs");
            WriteVersionLog(assembly);
        }

        static void ExtractSchema(string schema, string connStr, string prefix, string server, string database, string user)
        {
            SchemaManager sm = new SchemaManager();
            sm.SaveFile(schema, GetDatabase(connStr, prefix, server, database, user));
            writeLog(string.Empty);
            writeLog(String.Format("--Schema from '{0}:{1}' extracted to '{2}' file.", server, database, schema));
            WriteVersionLog(Assembly.GetExecutingAssembly());
        }

        static Database GetDatabase(string connStr, string prefix, string server, string database, string user)
        {
            Database db = new Database(server, database, user);

            using (SqlConnection cn = new SqlConnection(connStr))
            {
                cn.Open();
                // tabelas
                using (SqlCommand cm = new SqlCommand(Queries.Tables, cn))
                {
                    cm.Parameters.Add("prefix", SqlDbType.VarChar).Value = prefix;
                    using (SqlDataReader dr = cm.ExecuteReader(CommandBehavior.SingleResult))
                    {
                        while (dr.Read())
                            db.Tables.Add(new Table(dr.GetString(0)));
                        dr.Close();
                    }
                }
                // colunas
                using (SqlCommand cm = new SqlCommand(Queries.TableColumns, cn))
                {
                    cm.Parameters.Add("prefix", SqlDbType.VarChar).Value = prefix;
                    using (SqlDataReader dr = cm.ExecuteReader(CommandBehavior.SingleResult))
                    {
                        while (dr.Read())
                        {
                            db.Tables[dr.GetString(0)].Columns.Add(new Column(
                                dr.GetString(1),
                                dr.GetString(2),
                                (dr.GetValue(3) == DBNull.Value ? 0 : dr.GetInt32(3)),
                                (dr.GetString(4) == "YES"),
                                Convert.ToInt32(dr.GetValue(5)),
                                false,
                                (dr.GetValue(6) == DBNull.Value ? "" : dr.GetString(6)),
                                (dr.GetValue(7) == DBNull.Value ? (byte)0 : dr.GetByte(7)),
                                (dr.GetValue(8) == DBNull.Value ? 0 : dr.GetInt32(8)),
                                (dr.GetBoolean(9)),
                                (dr.GetValue(10) == DBNull.Value ? "" : dr.GetString(10))
                                )
                            );
                        }
                        dr.Close();
                    }
                }
                // colunas identity
                using (SqlCommand cm = new SqlCommand(Queries.IdentityColumns, cn))
                {
                    cm.Parameters.Add("prefix", SqlDbType.VarChar).Value = prefix;
                    using (SqlDataReader dr = cm.ExecuteReader(CommandBehavior.SingleResult))
                    {
                        while (dr.Read())
                        {
                            Table tb = db.Tables[dr.GetString(0)];
                            if (tb != null)
                                tb.Columns[dr.GetString(1)].Identity = true;
                        }
                        dr.Close();
                    }
                }
                // constraints
                using (SqlCommand cm = new SqlCommand(Queries.Constraints, cn))
                {
                    cm.Parameters.Add("prefix", SqlDbType.VarChar).Value = prefix;
                    using (SqlDataReader dr = cm.ExecuteReader(CommandBehavior.SingleResult))
                    {
                        while (dr.Read())
                        {
                            Table tb = db.Tables[dr.GetString(0)];
                            if (tb != null)
                                tb.Keys.Add(new Key(
                                    dr.GetString(1),
                                    dr.GetString(3),
                                    dr.GetString(2),
                                    String.IsNullOrEmpty(dr.GetString(4)) ? null : dr.GetString(4)
                                    ));
                        }
                        dr.Close();
                    }
                }
                // constraints columns
                using (SqlCommand cm = new SqlCommand(Queries.ConstraintColumns, cn))
                {
                    cm.Parameters.Add("prefix", SqlDbType.VarChar).Value = prefix;
                    using (SqlDataReader dr = cm.ExecuteReader(CommandBehavior.SingleResult))
                    {
                        while (dr.Read())
                        {
                            Table tb = db.Tables[dr.GetString(0)];
                            if (tb != null)
                            {
                                Key ke = tb.Keys[dr.GetString(1)];
                                if (ke != null)
                                {
                                    ke.Columns.Add(dr.GetString(2));
                                    if (!ke.UniqueColumns.Contains(dr.GetString(3)))
                                        ke.UniqueColumns.Add(dr.GetString(3));
                                }
                            }
                        }
                        dr.Close();
                    }
                }
                // indexes
                using (SqlCommand cm = new SqlCommand(Queries.Indexes, cn))
                {
                    cm.Parameters.Add("prefix", SqlDbType.VarChar).Value = prefix;
                    using (SqlDataReader dr = cm.ExecuteReader(CommandBehavior.SingleResult))
                    {
                        while (dr.Read())
                        {
                            Table tb = db.Tables[dr.GetString(0)];
                            if (tb != null)
                                tb.Indexes.Add(new Index(
                                    dr.GetString(1),
                                    dr.GetBoolean(2),
                                    dr.GetBoolean(3),
                                    dr.GetString(4),
                                    dr.GetString(5)
                                    ));
                        }
                        dr.Close();
                    }
                }
                // indexes columns
                using (SqlCommand cm = new SqlCommand(Queries.IndexKeys, cn))
                {
                    cm.Parameters.Add("prefix", SqlDbType.VarChar).Value = prefix;
                    using (SqlDataReader dr = cm.ExecuteReader(CommandBehavior.SingleResult))
                    {
                        while (dr.Read())
                        {
                            Table tb = db.Tables[dr.GetString(0)];
                            if (tb != null)
                                tb.Indexes[dr.GetString(1)].Columns.Add(
                                    dr.GetString(2)
                                    );
                        }
                        dr.Close();
                    }
                }
                // triggers
                using (SqlCommand cm = new SqlCommand(Queries.Triggers, cn))
                {
                    cm.Parameters.Add("prefix", SqlDbType.VarChar).Value = prefix;
                    using (SqlDataReader dr = cm.ExecuteReader(CommandBehavior.SingleResult))
                    {
                        while (dr.Read())
                        {
                            Table tb = db.Tables[dr.GetString(0)];
                            if (tb != null)
                            {
                                Trigger tr = tb.Triggers[dr.GetString(1)];
                                if (tr == null)
                                    tb.Triggers.Add(new Trigger(
                                        dr.GetString(1),
                                        dr.GetString(2).Replace("\t", "    ")
                                        ));
                                else
                                    tr.Body += dr.GetString(2).Replace("\t", "    ");
                            }
                        }
                        dr.Close();
                    }
                }
                // views
                using (SqlCommand cm = new SqlCommand(Queries.Views, cn))
                {
                    cm.Parameters.Add("prefix", SqlDbType.VarChar).Value = prefix;
                    using (SqlDataReader dr = cm.ExecuteReader(CommandBehavior.SingleResult))
                    {
                        while (dr.Read())
                        {
                            if (dr.GetValue(1) != DBNull.Value)
                            {
                                Program pp = db.Programs[dr.GetString(0)];
                                if (pp == null)
                                    db.Programs.Add(new Program(
                                        dr.GetString(0),
                                        dr.GetString(1).Replace("\t", "    "),
                                        "VIEW"
                                        ));
                                else
                                    pp.Body += dr.GetString(1).Replace("\t", "    ");
                            }
                        }
                        dr.Close();
                    }
                }
                // functions
                using (SqlCommand cm = new SqlCommand(Queries.Functions, cn))
                {
                    cm.Parameters.Add("prefix", SqlDbType.VarChar).Value = prefix;
                    using (SqlDataReader dr = cm.ExecuteReader(CommandBehavior.SingleResult))
                    {
                        while (dr.Read())
                        {
                            Program pr = db.Programs[dr.GetString(0)];
                            if (dr.GetValue(1).ToString() != "")
                                if (pr == null)
                                    db.Programs.Add(new Program(
                                        dr.GetString(0),
                                        dr.GetString(1).Replace("\t", "    "),
                                        "FUNCTION"
                                        ));
                                else
                                    pr.Body += dr.GetString(1).Replace("\t", "    ");
                        }
                        dr.Close();
                    }
                }
                // procedures
                using (SqlCommand cm = new SqlCommand(Queries.Procedures, cn))
                {
                    cm.Parameters.Add("prefix", SqlDbType.VarChar).Value = prefix;
                    using (SqlDataReader dr = cm.ExecuteReader(CommandBehavior.SingleResult))
                    {
                        while (dr.Read())
                        {
                            if (dr.GetValue(1) != DBNull.Value)
                            {
                                Program pp = db.Programs[dr.GetString(0)];
                                if (pp == null)
                                    db.Programs.Add(new Program(
                                        dr.GetString(0),
                                        dr.GetString(1).Replace("\t", "    "),
                                        "PROCEDURE"
                                        ));
                                else
                                    pp.Body += dr.GetString(1).Replace("\t", "    ");
                            }
                        }
                        dr.Close();
                    }
                }
            }
            return db;
        }

        static void GenerateGlobalization(string connStr)
        {
            writeLog("--Refreshing Globalization Table");
            writeLog(string.Empty);

            int t = 0;
            int p = 0;
            string line;
            var assembly = Assembly.GetExecutingAssembly();

            using (SqlConnection cn = new SqlConnection(connStr))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(string.Empty, cn))
                {
                    //Remove records of orb_Globalization
                    try
                    {
                        cm.CommandText = Queries.TruncateGlobalization;
                        cm.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        writeLog(cm.CommandText);
                        writeLog(ex.Message);
                    }

                    using (Stream stream = assembly.GetManifestResourceStream("dbcheck.globalization.sql"))
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            while ((line = reader.ReadLine()) != null)
                                if (line.ToLower().StartsWith("insert"))
                                    t++;
                        }
                    }
                    using (Stream stream = assembly.GetManifestResourceStream("dbcheck.globalization.sql"))
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            double progress = 0;
                            Console.Write("Progress: 0%");
                            while ((line = reader.ReadLine()) != null)
                            {
                                if (line.ToLower().StartsWith("insert"))
                                {
                                    try
                                    {
                                        cm.CommandText = line;
                                        cm.ExecuteNonQuery();
                                    }
                                    catch (Exception ex)
                                    {
                                        writeLog(cm.CommandText);
                                        writeLog(ex.Message);
                                    }
                                    p++;
                                    Console.SetCursorPosition(10, Console.CursorTop);
                                    if (progress < Math.Floor((Convert.ToDouble(p) / Convert.ToDouble(t)) * 100))
                                    {
                                        progress = Math.Floor((Convert.ToDouble(p) / Convert.ToDouble(t)) * 100);
                                        Console.Write(string.Format("{0}%", progress));
                                    }
                                }
                            }
                        }
                    }
                }
            }
            WriteVersionLog(assembly);

            Console.WriteLine();
            writeLog(string.Empty);
            writeLog(string.Format("Records created: {0}", t));
            writeLog(string.Empty);
            writeLog("--Finished Refreshing Globalization Table");
        }

        static void WriteVersionLog(Assembly assembly)
        {
            try
            {
                Version assemblyVersion = assembly.GetName().Version;
                writeLog(string.Format("--DbCheck Version {0}.{1}.{2}", assemblyVersion.Major, assemblyVersion.Minor, assemblyVersion.Build));
            }
            catch (Exception ex)
            {
                writeLog(string.Empty);
                writeLog(string.Format("The database couldn't be versioned: {0}", ex.Message));
            }
        }
    }
}
