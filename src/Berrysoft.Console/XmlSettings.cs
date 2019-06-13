using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Berrysoft.Console
{
    /// <summary>
    /// Represents a piece of settings accepts multiple values.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class MultipleAttribute : Attribute
    { }
    /// <summary>
    /// Represents a xml settings file parser.
    /// </summary>
    public abstract class XmlSettings : SettingsBase
    {
        private readonly XName rootName;
        private HashSet<string> multipleNames = new HashSet<string>();
#if NETSTANDARD
        private readonly object syncLock = new object();
#endif
        /// <summary>
        /// Initializes a new instance of <see cref="XmlSettings"/> class.
        /// </summary>
        public XmlSettings()
            : base()
        {
            if (Attribute.GetCustomAttribute(GetType().GetTypeInfo(), typeof(SettingsAttribute)) is SettingsAttribute settings)
            {
                rootName = settings.Name;
            }
            else
            {
                rootName = "settings";
            }
        }
        /// <summary>
        /// Get a <see cref="string"/> key and <see cref="SettingsPropertyInfo{T}"/> value of a <see cref="PropertyInfo"/>.
        /// </summary>
        /// <param name="prop">The property info.</param>
        /// <returns>A key value pair.</returns>
        protected override (string Key, SettingsPropertyInfo<SettingsAttribute> Value)? GetKeyValuePairFromPropertyInfo(PropertyInfo prop)
        {
            var result = base.GetKeyValuePairFromPropertyInfo(prop);
            if (result != null)
            {
                if (Attribute.GetCustomAttribute(prop, typeof(MultipleAttribute)) is MultipleAttribute)
                {
                    multipleNames.Add(result.Value.Key);
                }
            }
            return result;
        }
        private bool IsMultiple(string name) => multipleNames.Contains(name);
        /// <summary>
        /// Open an xml file and parse it.
        /// </summary>
        /// <param name="fileName">Path of the file.</param>
        public override void Open(string fileName)
        {
            XDocument document = XDocument.Load(fileName);
            XElement settings = document.Element(rootName);
            foreach (string name in Names)
            {
                bool mul = IsMultiple(name);
                object propValue = null;
                if (mul)
                {
                    var elements = settings.Elements(name);
                    if (elements != null)
                    {
                        propValue = elements.Select(e => e.Value).ToArray();
                    }
                }
                else
                {
                    var element = settings.Element(name);
                    if (element != null)
                    {
                        propValue = element.Value;
                    }
                }
                this[name] = propValue;
            }
        }
#if NETSTANDARD2_1
        /// <summary>
        /// Open an xml file and parse it asynchronously.
        /// </summary>
        /// <param name="fileName">Path of the file.</param>
        /// <returns>A <see cref="Task"/> of the task.</returns>
        public Task OpenAsync(string fileName) => OpenAsync(fileName, CancellationToken.None);
        /// <summary>
        /// Open an xml file and parse it asynchronously.
        /// </summary>
        /// <param name="fileName">Path of the file.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A <see cref="Task"/> of the task.</returns>
        public async Task OpenAsync(string fileName, CancellationToken cancellationToken)
        {
            XDocument document = null;
            using (StreamReader reader = new StreamReader(fileName))
            {
                document = await XDocument.LoadAsync(reader, LoadOptions.None, cancellationToken);
            }
            XElement settings = document.Element(rootName);
            Names.AsParallel().WithCancellation(cancellationToken).ForAll(name =>
            {
                bool mul = IsMultiple(name);
                object propValue = null;
                if (mul)
                {
                    var elements = settings.Elements(name);
                    if (elements != null)
                    {
                        propValue = elements.Select(e => e.Value).ToArray();
                    }
                }
                else
                {
                    var element = settings.Element(name);
                    if (element != null)
                    {
                        propValue = element.Value;
                    }
                }
                lock (syncLock)
                {
                    this[name] = propValue;
                }
            });
        }
#endif
        private static readonly XDeclaration DefaultXmlDeclaration = new XDeclaration("1.0", "utf-8", null);
        /// <summary>
        /// Save the settings to an xml file.
        /// </summary>
        /// <param name="fileName">Path of the file.</param>
        public override void Save(string fileName)
        {
            XDocument document = new XDocument(DefaultXmlDeclaration);
            XElement settings = new XElement(rootName);
            document.Add(settings);
            foreach (string name in Names)
            {
                bool mul = IsMultiple(name);
                object propValue = this[name];
                if (mul)
                {
                    string[] values = (string[])propValue;
                    settings.Add(values.Select(v => new XElement(name, v)).ToArray());
                }
                else
                {
                    settings.Add(new XElement(name, propValue));
                }
            }
            document.Save(fileName);
        }
#if NETSTANDARD2_1
        /// <summary>
        /// Save the settings to an xml file.
        /// </summary>
        /// <param name="fileName">Path of the file.</param>
        /// <returns>A <see cref="Task"/> of the task.</returns>
        public Task SaveAsync(string fileName) => SaveAsync(fileName, CancellationToken.None);
        /// <summary>
        /// Save the settings to an xml file.
        /// </summary>
        /// <param name="fileName">Path of the file.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A <see cref="Task"/> of the task.</returns>
        public async Task SaveAsync(string fileName, CancellationToken cancellationToken)
        {
            XDocument document = new XDocument(DefaultXmlDeclaration);
            XElement settings = new XElement(rootName);
            document.Add(settings);
            Names.AsParallel().WithCancellation(cancellationToken).ForAll(name =>
            {
                bool mul = IsMultiple(name);
                object propValue = this[name];
                if (mul)
                {
                    string[] values = (string[])propValue;
                    lock (syncLock)
                    {
                        settings.Add(values.Select(v => new XElement(name, v)).ToArray());
                    }
                }
                else
                {
                    lock (syncLock)
                    {
                        settings.Add(new XElement(name, propValue));
                    }
                }
            });
            using (StreamWriter writer = new StreamWriter(fileName))
            {
                await document.SaveAsync(writer, SaveOptions.None, cancellationToken);
            }
        }
#endif
    }
}
