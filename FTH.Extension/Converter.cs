using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Reflection;

namespace FTH.Extension
{
    public static class Converter
    {
        public static int ToInt(this object data)
        {
            if (data == null)
            {
                return 0;
            }
            else
            {
                int result;
                int.TryParse(data.ToString(), out result);
                return result;
            }
        }
        public static DateTime ToDateTime(this object data)
        {
            if (data == null)
            {
                return DateTime.MinValue;
            }
            else
            {
                DateTime result;
                DateTime.TryParse(data.ToString(), out result);
                return result;
            }
        }
        public static double ToDouble(this object data)
        {
            if (data == null)
            {
                return 0;
            }
            else
            {
                double result;
                double.TryParse(data.ToString(), out result);
                return result;
            }
        }
        public static bool ToBool(this object data)
        {
            if (data == null)
            {
                return false;
            }
            else
            {
                if (data is int && (int)data == 1)
                {
                    return true;
                }
                return bool.Parse(data.ToString());
            }
        }
        public static Stream ToStream(this string @this)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(@this);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
        public static List<T> ConvertTo<T>(this DataTable datatable) where T : new()
        {
            List<T> Temp = new List<T>();
            try
            {
                List<string> columnsNames = new List<string>();
                foreach (DataColumn DataColumn in datatable.Columns)
                    columnsNames.Add(DataColumn.ColumnName);
                Temp = datatable.AsEnumerable().ToList().ConvertAll<T>(row => GetObject<T>(row, columnsNames));
                return Temp;
            }
            catch
            {
                return Temp;
            }

        }
        public static T GetObject<T>(this DataRow row, List<string> columnsName) where T : new()
        {
            var obj = new T();
            try
            {
                var properties = typeof(T).GetProperties();
                foreach (PropertyInfo objProperty in properties)
                {
                    var columnname = columnsName.Find(name => name.ToLower() == objProperty.Name.ToLower());
                    if (!string.IsNullOrEmpty(columnname))
                    {
                        var value = row[columnname].ToString();
                        if (!string.IsNullOrEmpty(value))
                        {
                            if (Nullable.GetUnderlyingType(objProperty.PropertyType) != null)
                            {
                                value = row[columnname].ToString().Replace("$", "").Replace(",", "");
                                objProperty.SetValue(obj, Convert.ChangeType(value, Type.GetType(Nullable.GetUnderlyingType(objProperty.PropertyType).ToString())), null);
                            }
                            else
                            {
                                value = row[columnname].ToString().Replace("%", "");
                                objProperty.SetValue(obj, Convert.ChangeType(value, Type.GetType(objProperty.PropertyType.ToString())), null);
                            }
                        }
                    }
                }
                return obj;
            }
            catch
            {
                return obj;
            }
        }
        public static TResult ConvertObject<TRequest, TResult>(this TRequest objRequest)
            where TResult : new()
        {
            var result = new TResult();
            PropertyInfo[] resultPropertyInfos = typeof(TResult).GetProperties();
            foreach (PropertyInfo propertyInfo in objRequest.GetType().GetProperties())
            {
                if (resultPropertyInfos.Any(x => x.Name == propertyInfo.Name))
                {
                    PropertyInfo rInfo = resultPropertyInfos.First(x => x.Name == propertyInfo.Name);
                    if (propertyInfo.PropertyType == rInfo.PropertyType)
                    {
                        rInfo.SetValue(result, propertyInfo.GetValue(objRequest, null), null);
                    }
                }
            }
            return result;

        }
    }
}
