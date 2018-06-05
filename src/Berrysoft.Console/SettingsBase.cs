using System;
using System.Reflection;

namespace Berrysoft.Console
{
    /// <summary>
    /// Represents a piece of settings.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class SettingsAttribute : Attribute
    {
        /// <summary>
        /// Initialize a new instance of <see cref="SettingsAttribute"/> class.
        /// </summary>
        /// <param name="name">Name of this piece.</param>
        public SettingsAttribute(string name)
        {
            Name = name;
        }
        /// <summary>
        /// Name of this piece of settings.
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Get or set the type of converter.
        /// </summary>
        public Type ConverterType { get; set; }
    }
    /// <summary>
    /// Represents a base class of settings file parser.
    /// </summary>
    public abstract class SettingsBase : Parser<SettingsAttribute>
    {
        /// <summary>
        /// Initialize a new instance of <see cref="SettingsBase"/> class.
        /// </summary>
        public SettingsBase()
            : base()
        { }
        /// <summary>
        /// Get a <see cref="string"/> key and <see cref="SettingsPropertyInfo{T}"/> value of a <see cref="PropertyInfo"/>.
        /// </summary>
        /// <param name="prop">The property info.</param>
        /// <returns>A key value pair.</returns>
        protected override (string Key, SettingsPropertyInfo<SettingsAttribute> Value)? GetKeyValuePairFromPropertyInfo(PropertyInfo prop)
        {
            if (Attribute.GetCustomAttribute(prop, typeof(SettingsAttribute)) is SettingsAttribute attr)
            {
                return (attr.Name, new SettingsPropertyInfo<SettingsAttribute>(attr, prop, attr.ConverterType));
            }
            return null;
        }
        /// <summary>
        /// Open a settings file and parse it.
        /// </summary>
        /// <param name="fileName">Path of the file.</param>
        public abstract void Open(string fileName);
        /// <summary>
        /// Save the settings to a file.
        /// </summary>
        /// <param name="fileName">Path of the file.</param>
        public abstract void Save(string fileName);
    }
}
