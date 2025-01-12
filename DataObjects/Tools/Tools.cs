using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DataObjects.Tools
{
    public static class Tools
    {
        public static T DeserializeXmlFromString<T>(this string xmlContent)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (StringReader reader = new StringReader(xmlContent))
            {
                return (T)serializer.Deserialize(reader);
            }
        }

        public static T DeserializeXml<T>(string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
            {
                return (T)serializer.Deserialize(fileStream);
            }
        }

        public static void SerializeToXml<T>(T obj, string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                serializer.Serialize(writer, obj);
            }
        }
    }
}
