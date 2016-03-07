using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Westwind.Utilities.Dynamic;

namespace ScrapProject
{
    public class Fruit : Expando
    {
        public string Color { get; set; }
        public string Name { get; set; }
    }

    internal static class Program
    {
        private static void Encode(string filename)
        {
            using (Bitmap b = new Bitmap("beach.png"))
            {
                var width = b.Width;
                var height = b.Height;
                using (Graphics g = Graphics.FromImage(b))
                {
                    g.Clear(Color.White);

                    var si = new FileInfo(filename);
                    using (var fs = si.OpenRead())
                    {
                        int x = 0;
                        int y = 0;
                        int byt = 0;
                        while (byt >= 0)
                        {
                            byt = fs.ReadByte();
                            byte G;
                            byte r;
                            byte B;
                            int a;
                            if (byt >= 0)
                            {
                                var pixel = b.GetPixel(x, y);

                                G = pixel.G;
                                r = pixel.R;
                                B = pixel.B;
                                a = 255 - pixel.A;

                                b.SetPixel(x, y, Color.FromArgb(a, Color.FromArgb(r, G, B)));
                                x = x + 2;
                                if (x >= width)
                                {
                                    x = 0;
                                    y = y + 1;
                                    if (y >= height)
                                    {
                                        throw new OutOfMemoryException("not enought room");
                                    }
                                }
                            }
                        }
                    }
                }

                b.Save(filename + ".x.png", ImageFormat.Png);
            }
        }

        private static void Main(string[] args)
        {
            Encode(@"temp.txt");
            var apple = new Fruit { Name = "Granny smith", Color = "Green" };
            apple["Size"] = 3.0;
            apple["Pips"] = 5;

            Console.WriteLine("Writing out all properties, dynamic and hardcoded");

            const string outputFormat = "{0}\t\t\t{1}";

            foreach (var p in apple.GetProperties(true))
            {
                Console.WriteLine(outputFormat, p.Key, p.Value);
            }

            // we can use the property as well.

            dynamic appleX = apple;
            Console.WriteLine("By casting to dynamic we can access apple.Pips " + appleX.Pips);

            Console.Write("We can access the hard coded properties as well Name " + apple.Name + " and Color " +
                          apple.Color);
            Console.Write("We can access the hard coded properties as well via dynamic Name " + appleX.Name + " and Color " +
                          appleX.Color);
            Console.Write("We can also access like this apple[\"Pips\"] " + apple["Pips"]);
            Console.Write("And access the hard coded as well apple[\"Name\"] " + apple["Name"]);
            Console.ReadKey();
        }
    }
}

namespace Westwind.Utilities.Dynamic
{
    /// <summary>
    /// Class that provides extensible properties and methods. This dynamic object stores 'extra'
    /// properties in a dictionary or checks the actual properties of the newInstance.
    /// 
    /// This means you can subclass this expando and retrieve either native properties or properties
    /// from values in the dictionary.
    /// 
    /// This type allows you three ways to access its properties:
    /// 
    /// Directly: any explicitly declared properties are accessible
    /// Dynamic: dynamic cast allows access to dictionary and native properties/methods
    /// Dictionary: Any of the extended properties are accessible via IDictionary interface
    /// </summary>
    [Serializable]
    public class Expando : DynamicObject
    {
        private readonly SerializableDictionary<string, object> properties = new SerializableDictionary<string, object>();

        /// <summary>
        /// newInstance of object passed in
        /// </summary>
        [NonSerialized]
        private object instance;

        [NonSerialized]
        private PropertyInfo[] instancePropertyInfo;

        /// <summary>
        /// Cached type of the newInstance
        /// </summary>
        [NonSerialized]
        private Type instanceType;

        /// <summary>
        /// This constructor just works off the internal dictionary and any public properties of
        /// this object.
        /// 
        /// Note you can subclass Expando.
        /// </summary>
        protected Expando()
        {
            Initialize(this);
        }

        /// <summary>
        /// Allows passing in an existing newInstance variable to 'extend'.
        /// </summary>
        /// <remarks>
        /// You can pass in null here if you don't want to check native properties and only check
        /// the Dictionary!
        /// </remarks>
        /// <param name="instance"></param>
        // ReSharper disable once UnusedMember.Global
        protected Expando(object instance)
        {
            Initialize(instance);
        }

        private PropertyInfo[] InstancePropertyInfo
        {
            get
            {
                if (instancePropertyInfo == null && instance != null)
                    instancePropertyInfo = instance.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
                return instancePropertyInfo;
            }
        }

        /// <summary>
        /// Convenience method that provides a string Indexer to the properties collection AND the
        /// strongly typed properties of the object by name.
        /// 
        /// // dynamic exp["Address"] = "112 nowhere lane"; // strong var name =
        /// exp["StronglyTypedProperty"] as string;
        /// </summary>
        /// <remarks>
        /// The getter checks the properties dictionary first then looks in PropertyInfo for
        /// properties. The setter checks the newInstance properties before checking the properties dictionary.
        /// </remarks>
        /// <param name="key"></param>
        /// <returns></returns>
        public object this[string key]
        {
            get
            {
                try
                {
                    // try to get from properties collection first
                    return properties[key];
                }
                catch (KeyNotFoundException)
                {
                    // try reflection on instanceType
                    object result;
                    if (GetProperty(instance, key, out result))
                        return result;

                    // nope doesn't exist
                    throw;
                }
            }
            set
            {
                if (properties.ContainsKey(key))
                {
                    properties[key] = value;
                    return;
                }

                // check newInstance for existance of type first
                var miArray = instanceType.GetMember(key, BindingFlags.Public | BindingFlags.GetProperty);
                if (miArray.Length > 0)
                    SetProperty(instance, key, value);
                else
                    properties[key] = value;
            }
        }

        /// <summary>
        /// Checks whether a property exists in the Property collection or as a property on the newInstance
        /// </summary>
        /// <param name="item">the key value pair to check.</param>
        /// <param name="includeInstanceProperties">should we include the instance properties</param>
        /// <returns></returns>
        // ReSharper disable once UnusedMember.Global
        public bool Contains(KeyValuePair<string, object> item, bool includeInstanceProperties = false)
        {
            var res = properties.ContainsKey(item.Key);
            if (res)
                return true;

            if (includeInstanceProperties && instance != null)
            {
                foreach (var prop in InstancePropertyInfo)
                {
                    if (string.CompareOrdinal(prop.Name, item.Key) == 0)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Returns and the properties of
        /// </summary>
        /// <param name="includeInstanceProperties">should we include the instance properties</param>
        /// <returns></returns>
        public IEnumerable<KeyValuePair<string, object>> GetProperties(bool includeInstanceProperties = false)
        {
            if (includeInstanceProperties && instance != null)
            {
                foreach (var prop in InstancePropertyInfo)
                    yield return new KeyValuePair<string, object>(prop.Name, prop.GetValue(instance, null));
            }

            foreach (var key in properties.Keys)
                yield return new KeyValuePair<string, object>(key, properties[key]);
        }

        /// <summary>
        /// Try to retrieve a member by name first from newInstance properties followed by the
        /// collection entries.
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            // first check the properties collection for member
            if (properties.Keys.Contains(binder.Name))
            {
                result = properties[binder.Name];
                return true;
            }

            // Next check for Public properties via Reflection
            if (instance != null)
            {
                try
                {
                    return GetProperty(instance, binder.Name, out result);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }

            // failed to retrieve a property
            result = null;
            return false;
        }

        /// <summary>
        /// Dynamic invocation method. Currently allows only for Reflection based operation (no
        /// ability to add methods dynamically).
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="args"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            if (instance != null)
            {
                try
                {
                    // check newInstance passed in for methods to invoke
                    if (InvokeMethod(instance, binder.Name, args, out result))
                        return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }

            result = null;
            return false;
        }

        /// <summary>
        /// Property setter implementation tries to retrieve value from newInstance first then into
        /// this object
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            // first check to see if there's a native property to set
            if (instance != null)
            {
                try
                {
                    var result = SetProperty(instance, binder.Name, value);
                    if (result)
                        return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }

            // no match - set or add to dictionary
            properties[binder.Name] = value;
            return true;
        }

        /// <summary>
        /// Reflection Helper method to retrieve a property
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="name"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        protected bool GetProperty(object instance, string name, out object result)
        {
            if (instance == null)
            {
                instance = this;
            }

            var miArray = instanceType.GetMember(name, BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.Instance);
            if (miArray.Length > 0)
            {
                var mi = miArray[0];
                if (mi.MemberType == MemberTypes.Property)
                {
                    result = ((PropertyInfo)mi).GetValue(instance, null);
                    return true;
                }
            }

            result = null;
            return false;
        }

        /// <summary>
        /// Reflection helper method to set a property value
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected bool SetProperty(object instance, string name, object value)
        {
            if (instance == null)
            {
                instance = this;
            }

            var miArray = instanceType.GetMember(name, BindingFlags.Public | BindingFlags.SetProperty | BindingFlags.Instance);
            if (miArray.Length > 0)
            {
                var mi = miArray[0];
                if (mi.MemberType == MemberTypes.Property)
                {
                    ((PropertyInfo)mi).SetValue(this.instance, value, null);
                    return true;
                }
            }
            return false;
        }

        private void Initialize(object newInstance)
        {
            instance = newInstance;
            if (newInstance != null)
            {
                instanceType = newInstance.GetType();
            }
        }

        /// <summary>
        /// Reflection helper method to invoke a method
        /// </summary>
        /// <param name="newInstance"></param>
        /// <param name="name"></param>
        /// <param name="args"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private bool InvokeMethod(object newInstance, string name, object[] args, out object result)
        {
            if (newInstance == null)
            {
                newInstance = this;
            }

            // Look at the instanceType
            var miArray = instanceType.GetMember(name,
                                    BindingFlags.InvokeMethod |
                                    BindingFlags.Public | BindingFlags.Instance);

            if (miArray.Length > 0)
            {
                var mi = miArray[0] as MethodInfo;
                if (mi != null)
                {
                    result = mi.Invoke(newInstance, args);
                    return true;
                }
            }

            result = null;
            return false;
        }
    }

    [Serializable]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable, ISerializable
    {
        [NonSerialized]
        private readonly DataContractSerializer serializer = new DataContractSerializer(typeof(Dictionary<TKey, TValue>));

        public SerializableDictionary()
        {
        }

        protected SerializableDictionary(SerializationInfo info, StreamingContext context)
        {
            var keys = (List<TKey>)info.GetValue("-=keys=-", typeof(List<TKey>));
            foreach (var key in keys)
            {
                Add(key, (TValue)info.GetValue(key.ToString(), typeof(TValue)));
            }
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            var keys = new List<TKey>();
            foreach (var kvp in this)
            {
                keys.Add(kvp.Key);
            }

            info.AddValue("-=keys=-", keys);
            foreach (var kvp in this)
            {
                info.AddValue(kvp.Key.ToString(), kvp.Value);
            }
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