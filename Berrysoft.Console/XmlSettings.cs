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
    public sealed class SettingsAttribute : Attribute
    {
        public SettingsAttribute(string name)
        {
            Name = name;
        }
        public string Name { get; }
        public bool AllowMultiple { get; set; }
    }
    public abstract class XmlSettings
    {
        private Dictionary<SettingsAttribute, PropertyInfo> properties;
        private XName rootName;
#if NETCOREAPP2_0
        private readonly object syncLock = new object();
#endif
        public XmlSettings()
        {
            if (Attribute.GetCustomAttribute(GetType().GetTypeInfo(), typeof(SettingsAttribute)) is SettingsAttribute settings)
            {
                rootName = settings.Name;
            }
            else
            {
                rootName = "settings";
            }
            InitDictionary();
        }
        private void InitDictionary()
        {
            properties = new Dictionary<SettingsAttribute, PropertyInfo>();
            foreach (PropertyInfo prop in GetType().GetProperties())
            {
                if (Attribute.GetCustomAttribute(prop, typeof(SettingsAttribute)) is SettingsAttribute settings)
                {
                    properties.Add(settings, prop);
                }
            }
        }
        public void Open(string fileName)
        {
            XDocument document = XDocument.Load(fileName);
            XElement settings = document.Element(rootName);
            foreach (var prop in properties)
            {
                SettingsAttribute attr = prop.Key;
                PropertyInfo p = prop.Value;
                object propValue = null;
                if (attr.AllowMultiple)
                {
                    propValue = ChangeType(attr.Name, settings.Elements(attr.Name)?.Select(e => e.Value)?.ToArray(), p.PropertyType);
                }
                else
                {
                    propValue = ChangeType(attr.Name, settings.Element(attr.Name)?.Value, p.PropertyType);
                }
                if (propValue != null)
                {
                    p.SetValue(this, propValue);
                }
            }
        }
#if NETCOREAPP2_0
        public Task OpenAsync(string fileName) => OpenAsync(fileName, CancellationToken.None);
        public async Task OpenAsync(string fileName, CancellationToken cancellationToken)
        {
            XDocument document = null;
            using (StreamReader reader = new StreamReader(fileName))
            {
                document = await XDocument.LoadAsync(reader, LoadOptions.None, cancellationToken);
            }
            XElement settings = document.Element(rootName);
            properties.AsParallel().WithCancellation(cancellationToken).ForAll(prop =>
            {
                SettingsAttribute attr = prop.Key;
                PropertyInfo p = prop.Value;
                object propValue = null;
                if (attr.AllowMultiple)
                {
                    propValue = ChangeType(attr.Name, settings.Elements(attr.Name)?.Select(e => e.Value)?.ToArray(), p.PropertyType);
                }
                else
                {
                    propValue = ChangeType(attr.Name, settings.Element(attr.Name)?.Value, p.PropertyType);
                }
                if (propValue != null)  
                {
                    lock (syncLock)
                    {
                        p.SetValue(this, propValue);
                    }
                }
            });
        }
#endif
        public void Save(string fileName)
        {
            XDocument document = new XDocument(new XDeclaration("1.0", "utf-8", null));
            XElement settings = new XElement(rootName);
            document.Add(settings);
            foreach (var prop in properties)
            {
                SettingsAttribute attr = prop.Key;
                PropertyInfo p = prop.Value;
                object propValue = p.GetValue(this);
                if (attr.AllowMultiple)
                {
                    string[] values = (string[])ChangeBackType(attr.Name, propValue, p.PropertyType);
                    settings.Add(values.Select(v => new XElement(attr.Name, v)).ToArray());
                }
                else
                {
                    object value = ChangeBackType(attr.Name, propValue, p.PropertyType);
                    settings.Add(new XElement(attr.Name, value));
                }
            }
            document.Save(fileName);
        }
#if NETCOREAPP2_0
        public Task SaveAsync(string fileName) => SaveAsync(fileName, CancellationToken.None);
        public async Task SaveAsync(string fileName, CancellationToken cancellationToken)
        {
            XDocument document = new XDocument(new XDeclaration("1.0", "utf-8", null));
            XElement settings = new XElement(rootName);
            document.Add(settings);
            properties.AsParallel().WithCancellation(cancellationToken).ForAll(prop =>
            {
                SettingsAttribute attr = prop.Key;
                PropertyInfo p = prop.Value;
                object propValue = p.GetValue(this);
                if (attr.AllowMultiple)
                {
                    string[] values = (string[])ChangeBackType(attr.Name, propValue, p.PropertyType);
                    lock (syncLock)
                    {
                        settings.Add(values.Select(v => new XElement(attr.Name, v)).ToArray());
                    }
                }
                else
                {
                    object value = ChangeBackType(attr.Name, propValue, p.PropertyType);
                    lock (syncLock)
                    {
                        settings.Add(new XElement(attr.Name, value));
                    }
                }
            });
            using (StreamWriter writer = new StreamWriter(fileName))
            {
                await document.SaveAsync(writer, SaveOptions.None, cancellationToken);
            }
        }
#endif
        protected virtual object ChangeType(XName name, object value, Type conversionType)
        {
            return Convert.ChangeType(value, conversionType);
        }
        protected virtual object ChangeBackType(XName name, object value, Type conversionType)
        {
            return value.ToString();
        }
        public IEnumerable<SettingsAttribute> GetSettingsAttributes() => properties.Keys;
    }
}
