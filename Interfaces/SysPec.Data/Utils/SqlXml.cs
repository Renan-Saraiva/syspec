using System;
using System.Xml.Serialization;
using System.IO;
using System.Data.SqlClient;
using System.Data;
using System.Text;
using System.Collections;
using System.Xml;
using System.Reflection;

namespace SysPec.Data.Utils
{
    public enum EncondingType
    {
        [XmlEnum("0")]
        ASCII = 0,
        [XmlEnum("1")]
        BigEndianUnicode = 1,
        [XmlEnum("2")]
        Default = 2,
        [XmlEnum("3")]
        Unicode = 3,
        [XmlEnum("4")]
        UTF32 = 4,
        [XmlEnum("5")]
        UTF7 = 5,
        [XmlEnum("6")]
        UTF8 = 6
    }

    public struct NilInt32 : System.Xml.Serialization.IXmlSerializable
    {
        object _value;
        public NilInt32(int value)
        {
            _value = value;
        }

        public int Value
        {
            get { return (int)_value; }
        }

        public static implicit operator NilInt32(int v)
        {
            return new NilInt32(v);
        }

        public static implicit operator int(NilInt32 v)
        {
            return v.Value;
        }

        public bool IsNull
        {
            get { return (_value == null); }
        }

        public static NilInt32 Null;

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            string s = reader.ReadString();
            int v;
            if (int.TryParse(s, out v))
                _value = v;
            reader.ReadEndElement();
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            if (!IsNull)
                writer.WriteString(Value.ToString());
        }

        public override string ToString()
        {
            return (!IsNull ? ((int)_value).ToString() : "Null");
        }
    }

    public struct NilString : System.Xml.Serialization.IXmlSerializable
    {
        object _value;
        public NilString(string value)
        {
            _value = value;
        }

        public string Value
        {
            get { return (string)_value; }
        }

        public static implicit operator NilString(string v)
        {
            return new NilString(v);
        }

        public static implicit operator string(NilString v)
        {
            return v.Value;
        }

        public bool IsNull
        {
            get { return (_value == null); }
        }

        public static NilString Null;

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            _value = reader.ReadString();
            reader.ReadEndElement();
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            if (!IsNull)
                writer.WriteString(Value);
        }

        public override string ToString()
        {
            return (!IsNull ? ((string)_value).ToString() : "Null");
        }
    }

    public struct NilDateTime : System.Xml.Serialization.IXmlSerializable
    {
        object _value;
        public NilDateTime(DateTime value)
        {
            _value = value;
        }

        public DateTime Value
        {
            get { return (DateTime)_value; }
        }

        public static implicit operator NilDateTime(DateTime v)
        {
            return new NilDateTime(v);
        }

        public static implicit operator DateTime(NilDateTime v)
        {
            return v.Value;
        }

        public bool IsNull
        {
            get { return (_value == null); }
        }

        public static NilDateTime Null;

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            string s = reader.ReadString();
            DateTime v;
            if (DateTime.TryParse(s, out v))
                _value = v;
            reader.ReadEndElement();
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            if (!IsNull)
                writer.WriteString(Value.ToString("s"));
        }

        public override string ToString()
        {
            return (!IsNull ? ((DateTime)_value).ToString() : "Null");
        }
    }

    public struct NilBoolean : System.Xml.Serialization.IXmlSerializable
    {
        object _value;
        public NilBoolean(bool value)
        {
            _value = value;
        }

        public bool Value
        {
            get { return (bool)_value; }
        }

        public static implicit operator NilBoolean(bool v)
        {
            return new NilBoolean(v);
        }

        public static implicit operator bool(NilBoolean v)
        {
            return v.Value;
        }

        public bool IsNull
        {
            get { return (_value == null); }
        }

        public static NilBoolean Null;

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            string s = reader.ReadString();
            switch (s.ToLower())
            {
                case "true":
                case "1":
                    _value = true;
                    break;
                case "false":
                case "0":
                    _value = false;
                    break;
            }
            reader.ReadEndElement();
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            if (!IsNull)
                writer.WriteString(Value.ToString());
        }

        public override string ToString()
        {
            return (!IsNull ? ((bool)_value).ToString() : "Null");
        }
    }

    public struct NilDouble : System.Xml.Serialization.IXmlSerializable
    {
        object _value;
        public NilDouble(double value)
        {
            _value = value;
        }

        public double Value
        {
            get { return (double)_value; }
        }

        public static implicit operator NilDouble(double v)
        {
            return new NilDouble(v);
        }

        public static implicit operator double(NilDouble v)
        {
            return v.Value;
        }

        public bool IsNull
        {
            get { return (_value == null); }
        }

        public static NilInt32 Null;

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            string s = reader.ReadString();
            double v;
            if (double.TryParse(s, out v))
                _value = v;
            reader.ReadEndElement();
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            if (!IsNull)
                writer.WriteString(Value.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat));
        }

        public override string ToString()
        {
            return (!IsNull ? Value.ToString() : "Null");
        }
    }

    public static class XmlSerialization
    {

        public static string ToPropertiesString(object target)
        {
            StringBuilder sb = new StringBuilder();
            Type t = target.GetType();
            foreach (PropertyInfo p in t.GetProperties())
                try { sb.AppendFormat("{0}={1} ", p.Name, p.GetValue(target, null)); }
                catch { sb.AppendFormat("{0}=Error ", p.Name); }
            if (sb.Length > 0)
                sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }

        internal static string Serialize(object target)
        {
            try
            {
                string r = null;
                using (MemoryStream xml = new MemoryStream())
                {
                    XmlSerializer ser = new XmlSerializer(target.GetType());
                    XmlSerializerNamespaces ns = new XmlSerializerNamespaces(new XmlQualifiedName[] { new XmlQualifiedName() });
                    XmlTextWriter txt = new XmlTextWriter(xml, Encoding.Unicode);
                    ser.Serialize(txt, target, ns, "");
                    r = Encoding.Unicode.GetString(xml.ToArray());
                }
                return r;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return null;
            }
        }

        internal static string SqlSerialize(object target)
        {
            StringBuilder sb = new StringBuilder();
            using (System.IO.StringWriter sw = new System.IO.StringWriter(sb))
            {
                XmlSerializer xmlS = new XmlSerializer(target.GetType());
                xmlS.Serialize(sw, target);
                sw.Close();
            }
            return System.Text.RegularExpressions.Regex.Replace(sb.ToString(), "\r\n(\\s+)<(\\w+) xsi:nil=\"true\" />", "");
        }

        internal static byte[] GetBinaryBytes(string cs, string procedure, string idParam, int idValue)
        {
            byte[] r = null;
            using (SqlConnection cn = new SqlConnection(cs))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(procedure, cn))
                {
                    cm.CommandType = CommandType.StoredProcedure;
                    cm.Parameters.Add(idParam, SqlDbType.Int).Value = idValue;
                    using (SqlDataReader dr = cm.ExecuteReader(CommandBehavior.SingleRow))
                    {
                        if (dr.Read())
                        {
                            System.Data.SqlTypes.SqlBinary bin = dr.GetSqlBinary(0);
                            if (!bin.IsNull)
                                r = bin.Value;
                        }
                        dr.Close();
                    }
                }
            }
            return r;
        }

        internal static void GetBinary(string cs, string procedure, string idParam, int idValue, string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                using (SqlConnection cn = new SqlConnection(cs))
                {
                    cn.Open();
                    using (SqlCommand cm = new SqlCommand(procedure, cn))
                    {
                        cm.CommandType = CommandType.StoredProcedure;
                        cm.Parameters.Add(idParam, SqlDbType.Int).Value = idValue;
                        using (SqlDataReader dr = cm.ExecuteReader(CommandBehavior.SingleRow))
                        {
                            if (dr.Read())
                            {
                                System.Data.SqlTypes.SqlBinary bin = dr.GetSqlBinary(0);
                                fs.Write(bin.Value, 0, bin.Length);
                            }
                            dr.Close();
                        }
                    }
                }
                fs.Close();
            }
        }

        internal static void SaveBinaryFromStream(string cs, string procedure, string idParam, int idValue, Stream stream)
        {
            const int bufferSize = 8000;
            using (SqlConnection cn = new SqlConnection(cs))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(procedure, cn))
                {
                    cm.CommandType = CommandType.StoredProcedure;
                    cm.Parameters.Add(idParam, SqlDbType.Int).Value = idValue;
                    cm.Parameters.Add("value", SqlDbType.VarBinary);
                    int len = 0;
                    byte[] buffer = new byte[bufferSize];
                    while ((len = stream.Read(buffer, 0, bufferSize)) > 0)
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            ms.Write(buffer, 0, len);
                            cm.Parameters[1].Value = ms.ToArray();
                            cm.ExecuteNonQuery();
                            ms.Close();
                        }
                    }
                }
            }
        }

        internal static void SaveBinaryFromStream(string cs, string procedure, string idParam, int idValue, Stream stream, bool clearFirst)
        {
            if (clearFirst)
            {
                using (SqlConnection cn = new SqlConnection(cs))
                {
                    cn.Open();
                    using (SqlCommand cm = new SqlCommand(procedure, cn))
                    {
                        cm.CommandType = CommandType.StoredProcedure;
                        cm.Parameters.Add(idParam, SqlDbType.Int).Value = idValue;
                        cm.Parameters.Add("value", SqlDbType.VarBinary).Value = DBNull.Value;
                        cm.ExecuteNonQuery();
                    }
                }
            }
            SaveBinaryFromStream(cs, procedure, idParam, idValue, stream);
        }

        internal static void SaveBinaryFromText(string cs, string procedure, string idParam, int idValue, string text)
        {
            const int bufferSize = 8000;
            using (MemoryStream ts = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            {
                using (SqlConnection cn = new SqlConnection(cs))
                {
                    cn.Open();
                    using (SqlCommand cm = new SqlCommand(procedure, cn))
                    {
                        cm.CommandType = CommandType.StoredProcedure;
                        cm.Parameters.Add(idParam, SqlDbType.Int).Value = idValue;
                        cm.Parameters.Add("value", SqlDbType.VarBinary);
                        int len = 0;
                        byte[] buffer = new byte[bufferSize];
                        while ((len = ts.Read(buffer, 0, bufferSize)) > 0)
                        {
                            using (MemoryStream ms = new MemoryStream())
                            {
                                ms.Write(buffer, 0, len);
                                cm.Parameters[1].Value = ms.ToArray();
                                cm.ExecuteNonQuery();
                                ms.Close();
                            }
                        }
                    }
                }
                ts.Close();
            }
        }

        internal static void SaveBinaryFromText(string cs, string procedure, string idParam, int idValue, string text, EncondingType enconde)
        {
            const int bufferSize = 8000;
            MemoryStream ts;

            switch (enconde)
            {
                case EncondingType.ASCII:
                    ts = new MemoryStream(Encoding.ASCII.GetBytes(text));
                    break;
                case EncondingType.BigEndianUnicode:
                    ts = new MemoryStream(Encoding.BigEndianUnicode.GetBytes(text));
                    break;
                case EncondingType.Unicode:
                    ts = new MemoryStream(Encoding.Unicode.GetBytes(text));
                    break;
                case EncondingType.UTF32:
                    ts = new MemoryStream(Encoding.UTF32.GetBytes(text));
                    break;
                case EncondingType.UTF7:
                    ts = new MemoryStream(Encoding.UTF7.GetBytes(text));
                    break;
                case EncondingType.UTF8:
                    ts = new MemoryStream(Encoding.UTF8.GetBytes(text));
                    break;
                case EncondingType.Default:
                default:
                    ts = new MemoryStream(Encoding.Default.GetBytes(text));
                    break;
            }

            using (SqlConnection cn = new SqlConnection(cs))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(procedure, cn))
                {
                    cm.CommandType = CommandType.StoredProcedure;
                    cm.Parameters.Add(idParam, SqlDbType.Int).Value = idValue;
                    cm.Parameters.Add("value", SqlDbType.VarBinary);
                    int len = 0;
                    byte[] buffer = new byte[bufferSize];
                    while ((len = ts.Read(buffer, 0, bufferSize)) > 0)
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            ms.Write(buffer, 0, len);
                            cm.Parameters[1].Value = ms.ToArray();
                            cm.ExecuteNonQuery();
                            ms.Close();
                        }
                    }
                }
            }
            ts.Close();
        }
    }

    public class SqlXmlParamsOut
    {
        Hashtable _hash = new Hashtable();
        Hashtable _hash2 = new Hashtable();
        public SqlXmlParamsOut() { }
        public SqlXmlParamsOut(params object[] values)
        {
            if (values.Length % 2 != 0)
                throw new ArgumentException("SqlXmlParams: incorrect number of parameters.");
            for (int i = 0; i < values.Length; i += 2)
                if (values[i] is string)
                {
                    _hash.Add((string)values[i], values[i + 1]);
                    _hash2.Add((string)values[i], null);
                }
                else
                    throw new ArgumentException("SqlXmlParams: parameter key must be of type string.");
        }
        public object this[string key]
        {
            get { return _hash2[key]; }
        }
        public override string ToString()
        {
            if (_hash.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                foreach (string key in _hash.Keys)
                    sb.AppendFormat("{0}={1} ", key, _hash[key]);
                return sb.ToString(0, sb.Length - 1);
            }
            else
                return "";
        }
        public ICollection Keys
        {
            get { return _hash.Keys; }
        }
        public SqlParameter Get(string key)
        {
            SqlParameter p = new SqlParameter(key, (SqlDbType)_hash[key]);
            p.Direction = ParameterDirection.Output;
            return p;
        }
        public void Set(string key, object value)
        {
            _hash2[key] = value;
        }
    }

    public static class SqlXmlGet<T> where T : new()
    {
        public static T Deserialize(string xml)
        {
            T r = default(T);
            XmlSerializer ser = new XmlSerializer(typeof(T));
            StringReader sr = new StringReader(xml);
            r = (T)ser.Deserialize(sr);
            return r;
        }

        public static T Select(string procedure)
        {
            return Select(General.ConnectionString, procedure, new SqlXmlParams());
        }

        public static T Select(string connectionString, string procedure)
        {
            return Select(connectionString, procedure, new SqlXmlParams());
        }

        public static T Select(string procedure, SqlXmlParams values)
        {
            return Select(General.ConnectionString, procedure, values);
        }

        public static T Select(CommandType commandType, string METHOD, string command, SqlXmlParams values)
        {
            T r = default(T);
            using (SqlConnection cn = new SqlConnection(General.ConnectionString))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(command, cn))
                {
                    cm.CommandType = commandType;
                    foreach (string p in values.Keys)
                        cm.Parameters.Add(values.Get(p));
                    try
                    {
                        XmlSerializer ser = new XmlSerializer(typeof(T));
                        r = (T)ser.Deserialize(cm.ExecuteXmlReader());
                    }
                    catch (Exception ex)
                    {
                        if (ex.InnerException is System.Data.SqlTypes.SqlNullValueException)
                            r = default(T);
                        else
                            throw new Exception(String.Format("{0}\r\n{1}({2})", ex.Message, METHOD, values), ex);
                    }
                }
            }
            if (typeof(T).GetInterface("System.Collections.IList") != null && r == null)
                r = new T();
            return r;
        }

        public static T Select(CommandType commandType, string METHOD, string command, int pageIndex, int pageSize, out int totalRowCount, SqlXmlParams values)
        {
            T r = default(T);
            totalRowCount = 0;
            using (SqlConnection cn = new SqlConnection(General.ConnectionString))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(command, cn))
                {
                    cm.CommandType = commandType;
                    cm.Parameters.Add("pageIndex", SqlDbType.Int).Value = pageIndex;
                    cm.Parameters.Add("pageSize", SqlDbType.Int).Value = pageSize;
                    cm.Parameters.Add("totalRowCount", SqlDbType.Int).Direction = ParameterDirection.Output;
                    foreach (string p in values.Keys)
                        cm.Parameters.Add(values.Get(p));
                    try
                    {
                        XmlSerializer ser = new XmlSerializer(typeof(T));
                        r = (T)ser.Deserialize(cm.ExecuteXmlReader());
                        totalRowCount = Convert.ToInt32(cm.Parameters["totalRowCount"].Value);
                    }
                    catch (Exception ex)
                    {
                        if (ex.InnerException is System.Data.SqlTypes.SqlNullValueException)
                            r = default(T);
                        else
                            throw new Exception(String.Format("{0}\r\n{1}({2},{3},{4})", ex.Message, METHOD, pageIndex, pageSize, values), ex);
                    }
                }
            }
            if (typeof(T).GetInterface("System.Collections.IList") != null && r == null)
                r = new T();
            return r;
        }

        public static T Select(string connectionString, string procedure, SqlXmlParams values)
        {
            T r = default(T);
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(procedure, cn))
                {
                    if (procedure.ToLower() == "orb_p_getqueueoutwarningemails")
                        cm.CommandTimeout = 180;

                    cm.CommandType = CommandType.StoredProcedure;
                    foreach (string p in values.Keys)
                        cm.Parameters.Add(values.Get(p));
                    try
                    {
                        XmlSerializer ser = new XmlSerializer(typeof(T));
                        r = (T)ser.Deserialize(cm.ExecuteXmlReader());
                    }
                    catch (Exception ex)
                    {
                        if (ex.InnerException is System.Data.SqlTypes.SqlNullValueException)
                            r = default(T);
                        else if (ex.Message.Contains("deadlocked") && !ex.Message.Contains("TryAgain"))
                        {
                            try
                            {
                                System.Threading.Thread.Sleep(390);
                                r = Select(connectionString, procedure, values);
                            }
                            catch (Exception exTryAgain)
                            {
                                throw new Exception(String.Format("TryAgain > {0}\r\n{1}({2})", exTryAgain.Message, procedure, values), exTryAgain);
                            }
                        }
                        else
                            throw new Exception(String.Format("{0}\r\n{1}({2})", ex.Message, procedure, values), ex);
                    }
                }
            }
            if (typeof(T).GetInterface("System.Collections.IList") != null && r == null)
                r = new T();
            return r;
        }

        public static T Select(string procedure, int pageIndex, int pageSize, out int totalRowCount, SqlXmlParams values)
        {
            T r = default(T);
            totalRowCount = 0;
            using (SqlConnection cn = new SqlConnection(General.ConnectionString))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(procedure, cn))
                {
                    cm.CommandType = CommandType.StoredProcedure;
                    cm.Parameters.Add("pageIndex", SqlDbType.Int).Value = pageIndex;
                    cm.Parameters.Add("pageSize", SqlDbType.Int).Value = pageSize;
                    cm.Parameters.Add("totalRowCount", SqlDbType.Int).Direction = ParameterDirection.Output;
                    foreach (string p in values.Keys)
                        cm.Parameters.Add(values.Get(p));
                    try
                    {
                        XmlSerializer ser = new XmlSerializer(typeof(T));
                        r = (T)ser.Deserialize(cm.ExecuteXmlReader());
                        totalRowCount = Convert.ToInt32(cm.Parameters["totalRowCount"].Value);
                    }
                    catch (Exception ex)
                    {
                        if (ex.InnerException is System.Data.SqlTypes.SqlNullValueException)
                            r = default(T);
                        else
                            throw new Exception(String.Format("{0}\r\n{1}({2},{3},{4})", ex.Message, procedure, pageIndex, pageSize, values), ex);
                    }
                }
            }
            if (typeof(T).GetInterface("System.Collections.IList") != null && r == null)
                r = new T();
            return r;
        }

        public static T Select(string procedure, int pageIndex, int pageSize, out int totalRowCount, SqlXmlParams values, int cmdTimeout)
        {
            T r = default(T);
            totalRowCount = 0;
            using (SqlConnection cn = new SqlConnection(General.ConnectionString))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(procedure, cn))
                {
                    cm.CommandType = CommandType.StoredProcedure;
                    cm.CommandTimeout = cmdTimeout;
                    cm.Parameters.Add("pageIndex", SqlDbType.Int).Value = pageIndex;
                    cm.Parameters.Add("pageSize", SqlDbType.Int).Value = pageSize;
                    cm.Parameters.Add("totalRowCount", SqlDbType.Int).Direction = ParameterDirection.Output;
                    foreach (string p in values.Keys)
                        cm.Parameters.Add(values.Get(p));
                    try
                    {
                        XmlSerializer ser = new XmlSerializer(typeof(T));
                        r = (T)ser.Deserialize(cm.ExecuteXmlReader());
                        totalRowCount = Convert.ToInt32(cm.Parameters["totalRowCount"].Value);
                    }
                    catch (Exception ex)
                    {
                        if (ex.InnerException is System.Data.SqlTypes.SqlNullValueException)
                            r = default(T);
                        else
                            throw new Exception(String.Format("{0}\r\n{1}({2},{3},{4})", ex.Message, procedure, pageIndex, pageSize, values), ex);
                    }
                }
            }
            if (typeof(T).GetInterface("System.Collections.IList") != null && r == null)
                r = new T();
            return r;
        }

        public static Array SelectArray(string procedure, SqlXmlParams values)
        {
            return SelectArray(General.ConnectionString, procedure, values);
        }

        public static Array SelectArray(string connectionString, string procedure, SqlXmlParams values)
        {
            ArrayList r = new ArrayList();
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(procedure, cn))
                {
                    cm.CommandType = CommandType.StoredProcedure;
                    foreach (string p in values.Keys)
                        cm.Parameters.Add(values.Get(p));
                    try
                    {
                        using (SqlDataReader dr = cm.ExecuteReader())
                        {
                            while (dr.Read())
                                r.Add(dr.GetValue(0));
                            dr.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ex.InnerException is System.Data.SqlTypes.SqlNullValueException)
                            r = new ArrayList();
                        else
                            throw new Exception(String.Format("{0}\r\n{1}({2})", ex.Message, procedure, values), ex);
                    }
                }
            }
            return r.ToArray();
        }
    }

    public class SqlXmlParams
    {
        Hashtable _hash = new Hashtable();
        public SqlXmlParams() { }
        public SqlXmlParams(params object[] values)
        {
            if (values.Length % 2 != 0)
                throw new ArgumentException("SqlXmlParams: incorrect number of parameters.");
            for (int i = 0; i < values.Length; i += 2)
                if (values[i] is string)
                    _hash.Add((string)values[i], values[i + 1]);
                else
                    throw new ArgumentException("SqlXmlParams: parameter key must be of type string.");
        }
        public object this[string key]
        {
            get { return _hash[key]; }
        }
        public override string ToString()
        {
            if (_hash.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                foreach (string key in _hash.Keys)
                    sb.AppendFormat("{0}={1} ", key, _hash[key]);
                return sb.ToString(0, sb.Length - 1);
            }
            else
                return "";
        }
        public ICollection Keys
        {
            get { return _hash.Keys; }
        }
        public SqlParameter Get(string key)
        {
            return new SqlParameter(key, _hash[key]);
        }
    }

    public static class SqlXmlRun
    {

        public static void Execute(string connectionString, string procedure, object target, SqlXmlParams pIn, SqlXmlParamsOut pOut)
        {
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(procedure, cn))
                {
                    cm.CommandType = CommandType.StoredProcedure;
                    cm.Parameters.Add("xml", SqlDbType.Xml, int.MaxValue).Value = Serialize(target);
                    foreach (string p in pIn.Keys)
                        cm.Parameters.Add(pIn.Get(p));
                    foreach (string p in pOut.Keys)
                        cm.Parameters.Add(pOut.Get(p));
                    try
                    {
                        cm.ExecuteNonQuery();
                        foreach (string p in pOut.Keys)
                            pOut.Set(p, cm.Parameters[p].Value);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(String.Format("{0}\r\n{1}({2})", ex.Message, procedure, pIn), ex);
                    }
                }
            }
        }

        public static void Execute(string procedure, object target, string idName, out int id, out DateTime now, out string ticket)
        {
            Execute(General.ConnectionString, procedure, target, idName, out id, out now, out ticket);
        }

        public static void Execute(string connectionString, string procedure, object target, string idName, out int id, out DateTime now, out string ticket)
        {
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(procedure, cn))
                {
                    cm.CommandType = CommandType.StoredProcedure;
                    cm.Parameters.Add("xml", SqlDbType.Xml, int.MaxValue).Value = Serialize(target);
                    cm.Parameters.Add(idName, SqlDbType.Int).Direction = ParameterDirection.Output;
                    cm.Parameters.Add("now", SqlDbType.DateTime).Direction = ParameterDirection.Output;
                    cm.Parameters.Add("ticket", SqlDbType.VarChar, 15).Direction = ParameterDirection.Output;
                    try
                    {
                        cm.ExecuteNonQuery();
                        id = (int)cm.Parameters[idName].Value;
                        now = (DateTime)cm.Parameters["now"].Value;
                        ticket = (string)cm.Parameters["ticket"].Value;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(String.Format("{0}\r\n{1}({2})", ex.Message, procedure, target), ex);
                    }
                }
            }
        }

        public static void Execute(string procedure, object target, string idName, out int id, out DateTime now)
        {
            Execute(General.ConnectionString, procedure, target, idName, out id, out now);
        }

        public static void Execute(string connectionString, string procedure, object target, string idName, out int id, out DateTime now)
        {
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(procedure, cn))
                {
                    cm.CommandType = CommandType.StoredProcedure;
                    cm.Parameters.Add("xml", SqlDbType.Xml, int.MaxValue).Value = Serialize(target);
                    cm.Parameters.Add(idName, SqlDbType.Int).Direction = ParameterDirection.Output;
                    cm.Parameters.Add("now", SqlDbType.DateTime).Direction = ParameterDirection.Output;
                    try
                    {
                        cm.ExecuteNonQuery();
                        id = (int)cm.Parameters[idName].Value;
                        now = (DateTime)cm.Parameters["now"].Value;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(String.Format("{0}\r\n{1}({2})", ex.Message, procedure, target), ex);
                    }
                }
            }
        }

        public static void Execute(string procedure, object target, string idName, out int id)
        {
            Execute(General.ConnectionString, procedure, target, idName, out id);
        }

        public static void Execute(string procedure, object target, string idName)
        {
            Execute(General.ConnectionString, procedure, target, idName);
        }

        public static void Execute(string connectionString, string procedure, object target, string idName, out int id)
        {
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(procedure, cn))
                {
                    cm.CommandType = CommandType.StoredProcedure;
                    cm.Parameters.Add("xml", SqlDbType.Xml, int.MaxValue).Value = Serialize(target);
                    cm.Parameters.Add(idName, SqlDbType.Int).Direction = ParameterDirection.Output;
                    try
                    {
                        cm.ExecuteNonQuery();
                        id = (int)cm.Parameters[idName].Value;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(String.Format("{0}\r\n{1}({2})", ex.Message, procedure, target), ex);
                    }
                }
            }
        }

        public static void Execute(string connectionString, string procedure, object target, string idName)
        {
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(procedure, cn))
                {
                    cm.CommandType = CommandType.StoredProcedure;
                    cm.Parameters.Add("xml", SqlDbType.Xml, int.MaxValue).Value = Serialize(target);
                    try
                    {
                        cm.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(String.Format("{0}\r\n{1}({2})", ex.Message, procedure, target), ex);
                    }
                }
            }
        }

        public static void Execute(string procedure, object target, out DateTime now)
        {
            Execute(General.ConnectionString, procedure, target, out now);
        }

        public static void Execute(string connectionString, string procedure, object target, out DateTime now)
        {
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(procedure, cn))
                {
                    cm.CommandType = CommandType.StoredProcedure;
                    cm.Parameters.Add("xml", SqlDbType.Xml, int.MaxValue).Value = Serialize(target);
                    cm.Parameters.Add("now", SqlDbType.DateTime).Direction = ParameterDirection.Output;
                    try
                    {
                        cm.ExecuteNonQuery();
                        now = (DateTime)cm.Parameters["now"].Value;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(String.Format("{0}\r\n{1}({2})", ex.Message, procedure, target), ex);
                    }
                }
            }
        }

        public static void Execute(string procedure, object target)
        {
            Execute(General.ConnectionString, procedure, target);
        }

        public static void Execute(CommandType commandType, string METHOD, string command, object target)
        {
            using (SqlConnection cn = new SqlConnection(General.ConnectionString))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(command, cn))
                {
                    cm.CommandType = commandType;
                    cm.Parameters.Add("xml", SqlDbType.Xml, int.MaxValue).Value = Serialize(target);
                    try
                    {
                        cm.ExecuteNonQuery();
                    }
                    catch (SqlException ex)
                    {
                        throw new Exception(String.Format("{0}\r\n{1}({2})", ex.Message, METHOD, target), ex);
                    }
                }
            }
        }

        public static void Execute(CommandType commandType, string METHOD, string command, object target, SqlXmlParams values)
        {
            using (SqlConnection cn = new SqlConnection(General.ConnectionString))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(command, cn))
                {
                    cm.CommandType = commandType;
                    if (target != null)
                        cm.Parameters.Add("xml", SqlDbType.Xml, int.MaxValue).Value = Serialize(target);
                    foreach (string p in values.Keys)
                        cm.Parameters.Add(values.Get(p));
                    try
                    {
                        cm.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(String.Format("{0}\r\n{1}({2})", ex.Message, METHOD, target), ex);
                    }
                }
            }
        }

        public static void Execute(CommandType commandType, string METHOD, string command, object target, string idName, out int id)
        {
            using (SqlConnection cn = new SqlConnection(General.ConnectionString))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(command, cn))
                {
                    cm.CommandType = commandType;
                    if (target != null)
                        cm.Parameters.Add("xml", SqlDbType.Xml, int.MaxValue).Value = Serialize(target);
                    cm.Parameters.Add(idName, SqlDbType.Int).Direction = ParameterDirection.Output;
                    try
                    {
                        cm.ExecuteNonQuery();
                        id = (int)cm.Parameters[idName].Value;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(String.Format("{0}\r\n{1}({2})", ex.Message, METHOD, target), ex);
                    }
                }
            }
        }

        public static void Execute(CommandType commandType, string METHOD, string command, SqlXmlParams pIn, SqlXmlParamsOut pOut)
        {
            using (SqlConnection cn = new SqlConnection(General.ConnectionString))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(command, cn))
                {
                    cm.CommandType = commandType;
                    foreach (string p in pIn.Keys)
                        cm.Parameters.Add(pIn.Get(p));
                    foreach (string p in pOut.Keys)
                        cm.Parameters.Add(pOut.Get(p));
                    try
                    {
                        cm.ExecuteNonQuery();
                        foreach (string p in pOut.Keys)
                            pOut.Set(p, cm.Parameters[p].Value);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(String.Format("{0}\r\n{1}({2})", ex.Message, METHOD, pIn), ex);
                    }
                }
            }
        }

        public static void Execute(string connectionString, string procedure, object target)
        {
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(procedure, cn))
                {
                    cm.CommandType = CommandType.StoredProcedure;
                    cm.Parameters.Add("xml", SqlDbType.Xml, int.MaxValue).Value = Serialize(target);
                    try
                    {
                        cm.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(String.Format("{0}\r\n{1}({2})", ex.Message, procedure, target), ex);
                    }
                }
            }
        }

        public static void Execute(string connectionString, string procedure, object target, SqlXmlParams values)
        {
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(procedure, cn))
                {
                    cm.CommandType = CommandType.StoredProcedure;
                    cm.Parameters.Add("xml", SqlDbType.Xml, int.MaxValue).Value = Serialize(target);
                    foreach (string p in values.Keys)
                        cm.Parameters.Add(values.Get(p));
                    try
                    {
                        cm.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(String.Format("{0}\r\n{1}({2})", ex.Message, procedure, target), ex);
                    }
                }
            }
        }

        public static void Execute(string procedure, object target, SqlXmlParams values)
        {
            Execute(General.ConnectionString, procedure, target, values);
        }

        public static void Execute(string connectionString, string procedure, SqlXmlParams pIn, SqlXmlParamsOut pOut)
        {
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(procedure, cn))
                {
                    cm.CommandType = CommandType.StoredProcedure;
                    foreach (string p in pIn.Keys)
                        cm.Parameters.Add(pIn.Get(p));
                    foreach (string p in pOut.Keys)
                        cm.Parameters.Add(pOut.Get(p));
                    try
                    {
                        cm.ExecuteNonQuery();
                        foreach (string p in pOut.Keys)
                            pOut.Set(p, cm.Parameters[p].Value);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(String.Format("{0}\r\n{1}({2})", ex.Message, procedure, pIn), ex);
                    }
                }
            }
        }

        public static void Execute(string connectionString, string procedure, SqlXmlParams values)
        {
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(procedure, cn))
                {
                    cm.CommandType = CommandType.StoredProcedure;
                    foreach (string p in values.Keys)
                        cm.Parameters.Add(values.Get(p));
                    try
                    {
                        cm.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(String.Format("{0}\r\n{1}({2})", ex.Message, procedure, values), ex);
                    }
                }
            }
        }

        public static void Execute(string procedure, SqlXmlParams values)
        {
            Execute(General.ConnectionString, procedure, values);
        }

        public static void Execute(string procedure, out bool result, SqlXmlParams values)
        {
            using (SqlConnection cn = new SqlConnection(General.ConnectionString))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(procedure, cn))
                {
                    cm.CommandType = CommandType.StoredProcedure;
                    cm.Parameters.Add("result", SqlDbType.Bit).Direction = ParameterDirection.Output;
                    foreach (string p in values.Keys)
                        cm.Parameters.Add(values.Get(p));
                    try
                    {
                        cm.ExecuteNonQuery();
                        result = Convert.ToBoolean(cm.Parameters["result"].Value);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(String.Format("{0}\r\n{1}({2})", ex.Message, procedure, values), ex);
                    }
                }
            }
        }

        public static void Execute(string connectionString, string procedure, out bool result, SqlXmlParams values)
        {
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(procedure, cn))
                {
                    cm.CommandType = CommandType.StoredProcedure;
                    cm.Parameters.Add("result", SqlDbType.Bit).Direction = ParameterDirection.Output;
                    foreach (string p in values.Keys)
                        cm.Parameters.Add(values.Get(p));
                    try
                    {
                        cm.ExecuteNonQuery();
                        result = Convert.ToBoolean(cm.Parameters["result"].Value);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(String.Format("{0}\r\n{1}({2})", ex.Message, procedure, values), ex);
                    }
                }
            }
        }

        public static void Execute(string procedure, out string result, SqlXmlParams values)
        {
            Execute(procedure, out result, 15, values);
        }

        public static void Execute(string procedure, out string result, int varcharSize, SqlXmlParams values)
        {
            using (SqlConnection cn = new SqlConnection(General.ConnectionString))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(procedure, cn))
                {
                    cm.CommandType = CommandType.StoredProcedure;
                    cm.Parameters.Add("result", SqlDbType.VarChar, varcharSize).Direction = ParameterDirection.Output;
                    foreach (string p in values.Keys)
                        cm.Parameters.Add(values.Get(p));
                    try
                    {
                        cm.ExecuteNonQuery();
                        result = Convert.ToString(cm.Parameters["result"].Value);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(String.Format("{0}\r\n{1}({2})", ex.Message, procedure, values), ex);
                    }
                }
            }
        }

        public static void Execute(string procedure, out double result, SqlXmlParams values)
        {
            Execute(General.ConnectionString, procedure, out result, values);
        }

        public static void Execute(string procedure, out DateTime result, SqlXmlParams values)
        {
            using (SqlConnection cn = new SqlConnection(General.ConnectionString))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(procedure, cn))
                {
                    cm.CommandType = CommandType.StoredProcedure;
                    cm.Parameters.Add("result", SqlDbType.DateTime).Direction = ParameterDirection.Output;
                    foreach (string p in values.Keys)
                        cm.Parameters.Add(values.Get(p));
                    try
                    {
                        cm.ExecuteNonQuery();
                        result = Convert.ToDateTime(cm.Parameters["result"].Value);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(String.Format("{0}\r\n{1}({2})", ex.Message, procedure, values), ex);
                    }
                }
            }
        }

        public static void Execute(string connectionString, string procedure, out DateTime result, SqlXmlParams values)
        {
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(procedure, cn))
                {
                    cm.CommandType = CommandType.StoredProcedure;
                    cm.Parameters.Add("result", SqlDbType.DateTime).Direction = ParameterDirection.Output;
                    foreach (string p in values.Keys)
                        cm.Parameters.Add(values.Get(p));
                    try
                    {
                        cm.ExecuteNonQuery();
                        result = Convert.ToDateTime(cm.Parameters["result"].Value);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(String.Format("{0}\r\n{1}({2})", ex.Message, procedure, values), ex);
                    }
                }
            }
        }

        public static void Execute(string connectionString, string procedure, out double result, SqlXmlParams values)
        {
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(procedure, cn))
                {
                    cm.CommandType = CommandType.StoredProcedure;
                    cm.Parameters.Add("result", SqlDbType.Float).Direction = ParameterDirection.Output;
                    foreach (string p in values.Keys)
                        cm.Parameters.Add(values.Get(p));
                    try
                    {
                        cm.ExecuteNonQuery();
                        result = Convert.ToDouble(cm.Parameters["result"].Value);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(String.Format("{0}\r\n{1}({2})", ex.Message, procedure, values), ex);
                    }
                }
            }
        }

        public static void Execute(string procedure, out int result, SqlXmlParams values)
        {
            Execute(General.ConnectionString, procedure, out result, values);
        }

        public static void Execute(CommandType commandType, string METHOD, string command, out int result, SqlXmlParams values)
        {
            using (SqlConnection cn = new SqlConnection(General.ConnectionString))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(command, cn))
                {
                    cm.CommandType = commandType;
                    cm.Parameters.Add("result", SqlDbType.Int).Direction = ParameterDirection.Output;
                    foreach (string p in values.Keys)
                        cm.Parameters.Add(values.Get(p));
                    try
                    {
                        cm.ExecuteNonQuery();
                        result = Convert.ToInt32(cm.Parameters["result"].Value);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(String.Format("{0}\r\n{1}({2})", ex.Message, METHOD, values), ex);
                    }
                }
            }
        }

        public static void Execute(string connectionString, string procedure, out int result, SqlXmlParams values)
        {
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(procedure, cn))
                {
                    cm.CommandType = CommandType.StoredProcedure;
                    cm.Parameters.Add("result", SqlDbType.Int).Direction = ParameterDirection.Output;
                    foreach (string p in values.Keys)
                        cm.Parameters.Add(values.Get(p));
                    try
                    {
                        cm.ExecuteNonQuery();
                        result = Convert.ToInt32(cm.Parameters["result"].Value);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(String.Format("{0}\r\n{1}({2})", ex.Message, procedure, values), ex);
                    }
                }
            }
        }

        public static void Execute(string procedure, out int result, out int result2, SqlXmlParams values)
        {
            using (SqlConnection cn = new SqlConnection(General.ConnectionString))
            {
                cn.Open();
                using (SqlCommand cm = new SqlCommand(procedure, cn))
                {
                    cm.CommandType = CommandType.StoredProcedure;
                    cm.Parameters.Add("result", SqlDbType.Int).Direction = ParameterDirection.Output;
                    cm.Parameters.Add("result2", SqlDbType.Int).Direction = ParameterDirection.Output;
                    foreach (string p in values.Keys)
                        cm.Parameters.Add(values.Get(p));
                    try
                    {
                        cm.ExecuteNonQuery();
                        result = Convert.ToInt32(cm.Parameters["result"].Value);
                        result2 = Convert.ToInt32(cm.Parameters["result2"].Value);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(String.Format("{0}\r\n{1}({2})", ex.Message, procedure, values), ex);
                    }
                }
            }
        }

        public static string Serialize(object target)
        {
            StringBuilder sb = new StringBuilder();
            using (System.IO.StringWriter sw = new System.IO.StringWriter(sb))
            {
                XmlSerializer xmlS = new XmlSerializer(target.GetType());
                xmlS.Serialize(sw, target);
                sw.Close();
            }
            return System.Text.RegularExpressions.Regex.Replace(sb.ToString(), "\r\n(\\s*)<(\\w+)(\\s+)xsi:nil=\"true\"[^>]*>", "");
        }

        public static string SerializeField(object target)
        {
            string r = null;
            using (MemoryStream xml = new MemoryStream())
            {
                XmlSerializer ser = new XmlSerializer(target.GetType());
                XmlSerializerNamespaces ns = new XmlSerializerNamespaces(new XmlQualifiedName[] { new XmlQualifiedName() });
                XmlTextWriter txt = new XmlTextWriter(xml, Encoding.UTF8);
                ser.Serialize(txt, target, ns, "");
                r = XmlEncode(Encoding.UTF8.GetString(xml.ToArray()));
            }
            r = System.Text.RegularExpressions.Regex.Replace(r, "<(\\w+)(\\s+)xsi:nil=\"true\"[^>]*>", "");
            return r;
        }

        public static string XmlEncode(string xml)
        {
            char[] o = xml.ToCharArray();
            StringBuilder sb = new StringBuilder();
            for (int i = 1; i < o.Length; i++)
                if ((int)o[i] > 127)
                    sb.AppendFormat("&#{0};", (int)o[i]);
                else
                    sb.Append(o[i]);
            return sb.ToString();
        }

    }
}