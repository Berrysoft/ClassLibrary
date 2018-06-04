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
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class MultipleAttribute : Attribute
    { }
    public abstract class XmlSettings : SettingsBase
    {
        private readonly XName rootName;
        private HashSet<string> multipleNames = new HashSet<string>();
#if NETCOREAPP2_1
        private readonly object syncLock = new object();
#endif
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
                    propValue = settings.Elements(name)?.Select(e => e.Value)?.ToArray();
                }
                else
                {
                    propValue = settings.Element(name)?.Value;
                }
                this[name] = propValue;
            }
        }
#if NETCOREAPP2_1
        public Task OpenAsync(string fileName) => OpenAsync(fileName, CancellationToken.None);
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
                    propValue = settings.Elements(name)?.Select(e => e.Value)?.ToArray();
                }
                else
                {
                    propValue = settings.Element(name)?.Value;
                }
                lock (syncLock)
                {
                    this[name] = propValue;
                }
            });
        }
#endif
        public override void Save(string fileName)
        {
            XDocument document = new XDocument(new XDeclaration("1.0", "utf-8", null));
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
#if NETCOREAPP2_1
        public Task SaveAsync(string fileName) => SaveAsync(fileName, CancellationToken.None);
        public async Task SaveAsync(string fileName, CancellationToken cancellationToken)
        {
            XDocument document = new XDocument(new XDeclaration("1.0", "utf-8", null));
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
