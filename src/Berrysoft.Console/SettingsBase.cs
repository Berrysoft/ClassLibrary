using System;
using System.Collections.Generic;
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
        public bool AllowMultiple { get; set; }
        public Type ConverterType { get; set; }
    }
    public interface ISimpleConverter
    {
        object Convert(object value);
        object ConvertBack(object value);
    }
    internal class SimpleConverter : ISimpleConverter
    {
        private Type targetType;
        public SimpleConverter(Type targetType)
        {
            this.targetType = targetType;
        }
        public object Convert(object value)
        {
            return System.Convert.ChangeType(value, targetType);
        }
        public object ConvertBack(object value)
        {
            if(value is Array array)
            {
                return array;
            }
            return value.ToString();
        }
    }
    internal class Setting
    {
        public SettingsAttribute Attribute { get; }
        public PropertyInfo Property { get; }
        public ISimpleConverter Converter { get; }
        public Setting(SettingsAttribute attr, PropertyInfo prop, Type converterType)
        {
            this.Attribute = attr;
            this.Property = prop;
            if (converterType != null)
            {
                this.Converter = (ISimpleConverter)converterType.Assembly.CreateInstance(converterType.FullName);
            }
            else
            {
                this.Converter = new SimpleConverter(prop.PropertyType);
            }
        }
    }
    public abstract class SettingsBase
    {
        private Dictionary<string, Setting> properties;
        public SettingsBase()
        {
            InitDictionary();
        }
        private void InitDictionary()
        {
            properties = new Dictionary<string, Setting>();
            foreach (PropertyInfo prop in GetType().GetProperties())
            {
                if (Attribute.GetCustomAttribute(prop, typeof(SettingsAttribute)) is SettingsAttribute settings)
                {
                    properties.Add(settings.Name, new Setting(settings, prop, settings.ConverterType));
                }
            }
        }
        public abstract void Open(string fileName);
        public abstract void Save(string fileName);
        protected bool IsMultiple(string name)
        {
            if (properties.TryGetValue(name, out Setting value))
            {
                return value.Attribute.AllowMultiple;
            }
            return false;
        }
        protected void SetValue(string name, object value)
        {
            if (value != null)
            {
                if (properties.TryGetValue(name, out Setting setting))
                {
                    ISimpleConverter converter = setting.Converter;
                    setting.Property.SetValue(this, converter.Convert(value));
                }
            }
        }
        protected object GetValue(string name)
        {
            if (properties.TryGetValue(name, out Setting setting))
            {
                ISimpleConverter converter = setting.Converter;
                return converter.ConvertBack(setting.Property.GetValue(this));
            }
            return null;
        }
        public ICollection<string> Settings => properties.Keys;
    }
}
