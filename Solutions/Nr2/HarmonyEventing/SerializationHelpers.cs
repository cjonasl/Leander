using System;
using System.IO;
using System.Text;

namespace Leander.Nr1.HarmonyEventing
{
    public class SerializationHelpers
    {

        public static string SerializeObject<T>(T eventMessage)
        {
            return SerializeObject(eventMessage, typeof(T));
        }


        public static string SerializeObject(object eventMessage, Type ObjType)
        {
            using (var memoryStream = new MemoryStream())
            {
                var messageMapper = new NServiceBus.MessageInterfaces.MessageMapper.Reflection.MessageMapper();
                messageMapper.Initialize(new[] { ObjType });

                var serializer = new HijumpXmlSerializer();
                serializer.MessageTypes.Add(ObjType);
                serializer.Serialize(new object[] { eventMessage }, memoryStream);

                return new UTF8Encoding().GetString(memoryStream.ToArray());
            }
        }

        public static T DeSerializeObject<T>(string xml)
        {
            if (string.IsNullOrEmpty(xml))
                return default(T);
            return (T)DeserializeXml(xml, typeof(T));
        }

        public static object DeserializeXml(string Xml, Type ObjType)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(Xml);

            var messageMapper = new NServiceBus.MessageInterfaces.MessageMapper.Reflection.MessageMapper();
            messageMapper.Initialize(new[] { ObjType });

            var deserializer = new HijumpXmlSerializer();
            deserializer.MessageTypes.Add(ObjType);
            object[] messages;
            using (var stream = new MemoryStream(bytes))
            {
                messages = deserializer.Deserialize(stream);
            }
            return messages;
        }
    }
}
