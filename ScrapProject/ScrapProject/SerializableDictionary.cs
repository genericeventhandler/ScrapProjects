using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Westwind.Utilities.Dynamic
{
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable
    {
        private readonly DataContractSerializer serializer = new DataContractSerializer(typeof(Dictionary<TKey, TValue>));

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            var deserialized = (Dictionary<TKey, TValue>)serializer.ReadObject(reader);
            foreach (KeyValuePair<TKey, TValue> kvp in deserialized)
            {
                Add(kvp.Key, kvp.Value);
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            serializer.WriteObject(writer, this);
        }
    }
}