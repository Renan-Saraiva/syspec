using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SysPec.App.Helpers
{
    public class Helper
    {
        public static string GetSubString(string value, int maxLenght) 
        {
            if (!string.IsNullOrEmpty(value))
                if (value.Length > maxLenght)
                {
                    return string.Format("{0}...", value.Substring(0, maxLenght));
                }
                else
                    return value;
            else
                return string.Empty;
        }
    }
}