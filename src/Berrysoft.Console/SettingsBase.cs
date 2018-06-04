using System;
using System.Reflection;

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
        public Type ConverterType { get; set; }
    }
    public abstract class SettingsBase : Parser<SettingsAttribute>
    {
        public SettingsBase()
            : base()
        { }
        protected override (string Key, SettingsPropertyInfo<SettingsAttribute> Value)? GetKeyValuePairFromPropertyInfo(PropertyInfo prop)
        {
            if (Attribute.GetCustomAttribute(prop, typeof(SettingsAttribute)) is SettingsAttribute attr)
            {
                return (attr.Name, new SettingsPropertyInfo<SettingsAttribute>(attr, prop, attr.ConverterType));
            }
            return null;
        }
        public abstract void Open(string fileName);
        public abstract void Save(string fileName);
    }
}
