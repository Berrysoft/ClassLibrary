using System;
using System.Collections.Generic;
using System.Reflection;

namespace Berrysoft.Console
{
    /// <summary>
    /// Exposes methods of a simple converter.
    /// </summary>
    public interface ISimpleConverter
    {
        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">Value to be converted.</param>
        /// <returns>A converted value.</returns>
        object Convert(object value);
        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">Value to be converted.</param>
        /// <returns>A converted value.</returns>
        object ConvertBack(object value);
    }
    internal class SimpleConverter : ISimpleConverter
    {
        private readonly Type targetType;
        private SimpleConverter(Type targetType)
        {
            this.targetType = targetType;
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
    }
    /// <summary>
    /// Represents some information of a property and its settings attribute.
    /// </summary>
    /// <typeparam name="T">The type of the attribute.</typeparam>
    public class SettingsPropertyInfo<T>
        where T : Attribute
    {
        /// <summary>
        /// The settings attribute of the property.
        /// </summary>
        public T Attribute { get; }
        /// <summary>
        /// The <see cref="PropertyInfo"/> of the property.
        /// </summary>
        public PropertyInfo Property { get; }
        /// <summary>
        /// The converter of the property.
        /// </summary>
        public ISimpleConverter Converter { get; }
        /// <summary>
        /// Initializes an instance of <see cref="SettingsPropertyInfo{T}"/> class.
        /// </summary>
        /// <param name="attr">The settings attribute of the property.</param>
        /// <param name="prop">The <see cref="PropertyInfo"/> of the property.</param>
        /// <param name="converterType">The converter of the property.</param>
        public SettingsPropertyInfo(T attr, PropertyInfo prop, Type converterType)
        {
            this.Attribute = attr;
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
    /// <summary>
    /// Represents a parser of a settings file or command lines.
    /// </summary>
    /// <typeparam name="T">The type of the attribute.</typeparam>
    public abstract class Parser<T>
        where T : Attribute
    {
        private protected readonly Dictionary<string, SettingsPropertyInfo<T>> properties;
        /// <summary>
        /// Initializes an instance of <see cref="Parser{T}"/> class.
        /// </summary>
        public Parser()
        {
            properties = new Dictionary<string, SettingsPropertyInfo<T>>();
            foreach (PropertyInfo prop in GetType().GetProperties())
            {
                var pair = GetKeyValuePairFromPropertyInfo(prop);
                if (pair.HasValue)
                {
                    properties.Add(pair.Value.Key, pair.Value.Value);
                }
            }
        }
        /// <summary>
        /// Get a <see cref="string"/> key and <see cref="SettingsPropertyInfo{T}"/> value of a <see cref="PropertyInfo"/>.
        /// </summary>
        /// <param name="prop">The property info.</param>
        /// <returns>A key value pair.</returns>
        protected abstract (string Key, SettingsPropertyInfo<T> Value)? GetKeyValuePairFromPropertyInfo(PropertyInfo prop);
        /// <summary>
        /// Get or set the value of a property by name.
        /// </summary>
        /// <param name="name">A specified name of the property.</param>
        /// <returns>The value of the specified property. <see langword="null"/> when the name is invalid.</returns>
        protected object this[string name]
        {
            get
            {
                if (properties.TryGetValue(name, out SettingsPropertyInfo<T> setting))
                {
                    ISimpleConverter converter = setting.Converter;
                    return converter.ConvertBack(setting.Property.GetValue(this));
                }
                return null;
            }
            set
            {
                if (value != null)
                {
                    if (properties.TryGetValue(name, out SettingsPropertyInfo<T> setting))
                    {
                        ISimpleConverter converter = setting.Converter;
                        setting.Property.SetValue(this, converter.Convert(value));
                    }
                }
            }
        }
        /// <summary>
        /// Get names of all properties with specified attribute.
        /// </summary>
        public ICollection<string> Names => properties.Keys;
    }
}
