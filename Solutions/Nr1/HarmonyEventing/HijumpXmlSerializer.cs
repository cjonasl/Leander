using System;
using System.Collections.Generic;
using System.Xml;
using System.Runtime.Serialization;
using System.IO;
using System.Text;

namespace Leander.Nr1.HarmonyEventing
{
    public class HijumpXmlSerializer : NServiceBus.Serialization.IMessageSerializer
    {
        private readonly IList<Type> knownTypes = new List<Type>();

        public IList<Type> MessageTypes
        {
            get { return knownTypes; }
            set
            {
                knownTypes.Clear();
                foreach (var type in value)
                {
                    if (!type.IsInterface && typeof(IMessage).IsAssignableFrom(type)
                        && !knownTypes.Contains(type))
                    {
                        knownTypes.Add(type);
                    }
                }
            }
        }

        public void Serialize(object[] messages, Stream stream)
        {
            var xws = new XmlWriterSettings
            {
                ConformanceLevel = ConformanceLevel.Fragment
            };
            using (var xmlWriter = XmlWriter.Create(stream, xws))
            {
                var dcs = new DataContractSerializer(typeof(IMessage), knownTypes);
                foreach (var message in messages)
                {
                    dcs.WriteObject(xmlWriter, message);
                }
            }
        }

        public object[] Deserialize(Stream stream)
        {
            var xrs = new XmlReaderSettings
            {
                ConformanceLevel = ConformanceLevel.Fragment
            };
            using (var xmlReader = XmlReader.Create(stream, xrs))
            {
                var dcs = new DataContractSerializer(typeof(IMessage), knownTypes);
                var messages = new List<IMessage>();
                while (false == xmlReader.EOF)
                {
                    var message = (IMessage)dcs.ReadObject(xmlReader);
                    messages.Add(message);
                }
                return messages.ToArray();
            }
        }
    }
}
