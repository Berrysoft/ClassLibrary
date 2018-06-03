using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Berrysoft.Console
{
    public abstract class XmlSettings : SettingsBase
    {
        private XName rootName;
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
            foreach (string name in Settings)
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
                SetValue(name, propValue);
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
            Settings.AsParallel().WithCancellation(cancellationToken).ForAll(name =>
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
                    SetValue(name, propValue);
                }
            });
        }
#endif
        public override void Save(string fileName)
        {
            XDocument document = new XDocument(new XDeclaration("1.0", "utf-8", null));
            XElement settings = new XElement(rootName);
            document.Add(settings);
            foreach (string name in Settings)
            {
                bool mul = IsMultiple(name);
                object propValue = GetValue(name);
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
            Settings.AsParallel().WithCancellation(cancellationToken).ForAll(name =>
            {
                bool mul = IsMultiple(name);
                object propValue = GetValue(name);
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
