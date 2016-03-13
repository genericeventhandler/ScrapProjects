// <copyright file="SerializableDictionary.cs" company="GenericEventHandler">
//     Copyright (c) GenericEventHandler all rights reserved. Licensed under the Mit license.
// </copyright>

namespace Westwind.Utilities.Dynamic
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    /// <summary>
    /// A serializable Dictionary that can be serialized to xml
    /// </summary>
    /// <typeparam name="TKey">the type of key, usually string</typeparam>
    /// <typeparam name="TValue">the type of object to save.</typeparam>
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable
    {
        private readonly DataContractSerializer serializer = new DataContractSerializer(typeof(Dictionary<TKey, TValue>));

        /// <inheritdoc/>
        public XmlSchema GetSchema()
        {
            return null;
        }

        /// <inheritdoc/>
        public void ReadXml(XmlReader reader)
        {
            var deserialized = (Dictionary<TKey, TValue>)serializer.ReadObject(reader);
            foreach (KeyValuePair<TKey, TValue> kvp in deserialized)
            {
                Add(kvp.Key, kvp.Value);
            }
        }

        /// <inheritdoc/>
        public void WriteXml(XmlWriter writer)
        {
            serializer.WriteObject(writer, this);
        }
    }
}