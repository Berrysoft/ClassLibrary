using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Berrysoft.Console
{
    public interface ISimpleConverter
    {
        object Convert(object value);
        object ConvertBack(object value);
    }
    internal class SimpleConverter : ISimpleConverter
    {
        private readonly Type targetType;
        private SimpleConverter(Type targetType)
        {
            this.targetType = targetType;
        }
        private static Dictionary<Type, ISimpleConverter> converters = new Dictionary<Type, ISimpleConverter>();
        public static ISimpleConverter Create(Type targetType)
        {
            if (!converters.TryGetValue(targetType, out var converter))
            {
                converter = new SimpleConverter(targetType);
                converters[targetType] = converter;
            }
            return converter;
        }
        public object Convert(object value)
        {
            return System.Convert.ChangeType(value, targetType);
        }
        public object ConvertBack(object value)
        {
            if (value is Array array)
            {
                return array;
            }
            return value.ToString();
        }
    }
    internal class Setting<T>
        where T : Attribute
    {
        public T Attribute { get; }
        public PropertyInfo Property { get; }
        public ISimpleConverter Converter { get; }
        public Setting(PropertyInfo prop, Type converterType)
        {
            this.Property = prop;
            if (converterType != null)
            {
                this.Converter = (ISimpleConverter)converterType.Assembly.CreateInstance(converterType.FullName);
            }
            else
            {
                this.Converter = SimpleConverter.Create(prop.PropertyType);
            }
        }
    }
    public abstract class Parser<T>
        where T : Attribute
    {
        private readonly Dictionary<string, Setting<T>> properties;
        public Parser()
        {
            properties = new Dictionary<string, Setting<T>>();
            foreach (PropertyInfo prop in GetType().GetProperties())
            {
                var pair = GetKeyValuePairFromPropertyInfo(prop);
                if (pair.HasValue)
                {
                    properties.Add(pair.Value.Key, pair.Value.Value);
                }
            }
        }
        private protected abstract (string Key, Setting<T> Value)? GetKeyValuePairFromPropertyInfo(PropertyInfo prop);
        protected void SetValue(string name, object value)
        {
            if (value != null)
            {
                if (properties.TryGetValue(name, out Setting<T> setting))
                {
                    ISimpleConverter converter = setting.Converter;
                    setting.Property.SetValue(this, converter.Convert(value));
                }
            }
        }
        protected object GetValue(string name)
        {
            if (properties.TryGetValue(name, out Setting<T> setting))
            {
                ISimpleConverter converter = setting.Converter;
                return converter.ConvertBack(setting.Property.GetValue(this));
            }
            return null;
        }
        private protected Setting<T> GetSetting(string name) => properties[name];
        public ICollection<string> Names => properties.Keys;
    }
}
