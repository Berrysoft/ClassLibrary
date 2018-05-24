using System;
using System.Collections.Generic;
using System.Reflection;
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
    public abstract class SettingsBase<TFrom, TTo>
    {
        private protected Dictionary<SettingsAttribute, PropertyInfo> properties;
        private protected XName rootName;
        public SettingsBase()
        {
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
        public abstract void Open(string fileName);
        public abstract void Save(string fileName);
        public IEnumerable<SettingsAttribute> GetSettingsAttributes() => properties.Keys;
        protected abstract TTo ChangeType(string name, TFrom value, Type conversionType);
        protected abstract TFrom ChangeBackType(string name, TTo value, Type conversionType);
    }
}
