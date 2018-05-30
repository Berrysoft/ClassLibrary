using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Berrysoft.Console
{
    public abstract class XmlSettings : SettingsBase<object, object>
    {
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
        public override void Open(string fileName)
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
        public override void Save(string fileName)
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
#if NETCOREAPP2_1
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
    }
}
