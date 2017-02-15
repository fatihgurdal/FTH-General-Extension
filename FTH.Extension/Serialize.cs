using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Web.Script.Serialization;
using Newtonsoft.Json;

namespace FTH.Extension
{
    public static class Serialize
    {
        public const string EncrytPass = "FTH";

        #region - XML -

        #region - Serialize -
        public static void XmlSerialize<T>(this T obj, string path) where T : class
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));

            XmlWriter writer = XmlWriter.Create(path);

            serializer.Serialize(writer, obj);

            writer.Close();
            ((IDisposable)writer).Dispose();
        }

        public static string XmlStringSerialize<T>(this T obj) where T : class
        {
            XmlSerializer xmlSerializer = new XmlSerializer(obj.GetType());

            using (StringWriter textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, obj);
                return textWriter.ToString();
            }
        }
        #endregion

        #region - Desrializer -
        public static T XmlDesrializer<T>(this string path) where T : class
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));

            XmlReader reader = XmlReader.Create(path);

            object obj = serializer.Deserialize(reader);

            reader.Close();
            ((IDisposable)reader).Dispose();

            return (T)obj;
        }

        public static T XmlStringDesrializer<T>(this string @this) where T : class
        {
            var reader = XmlReader.Create(@this.Trim().ToStream(), new XmlReaderSettings()
            {
                ConformanceLevel = ConformanceLevel.Document
            });
            return new XmlSerializer(typeof(T)).Deserialize(reader) as T;
        }
        #endregion

        #endregion

        #region - JSON -

        #region - Serialize -
        public static string JSONSerialize<T>(this T serializeObject) where T : class
        {
            return JSONSerialize(serializeObject, false);
        }
        public static string JSONSerialize<T>(this T serializeObject, bool useEncryt) where T : class
        {
            JsonSerializerSettings jsSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            string retvalStr = JsonConvert.SerializeObject(serializeObject, jsSettings);

            string result = useEncryt ? Encrypter.Encrypt(retvalStr, EncrytPass) : retvalStr;

            return result;
        }
        #endregion

        #region - Desrializer -
        public static T JSONDeserialize<T>(this string deserializeString)
        {
            return JSONDeserialize<T>(deserializeString, false);
        }
        public static T JSONDeserialize<T>(this string deserializeString, bool useEncryt)
        {
            if (useEncryt)
            {
                deserializeString = Encrypter.Decrypt(deserializeString, EncrytPass);

            }

            var jsSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            return JsonConvert.DeserializeObject<T>(deserializeString, jsSettings);
        }
        #endregion

        #endregion
    }
}
