using System;
using System.Collections.Generic;
using System.Reflection;

namespace Berrysoft.Console
{
    /// <summary>
    /// Represents a command line option.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class OptionAttribute : Attribute
    {
        /// <summary>
        /// Initializes an instance of <see cref="OptionAttribute"/>.
        /// </summary>
        /// <param name="shortArg">Short name of the option.</param>
        /// <param name="longArg">Long name of the option.</param>
        public OptionAttribute(char shortArg, string longArg)
        {
            ShortArg = shortArg;
            LongArg = longArg;
        }
        /// <summary>
        /// Short name of the option.
        /// </summary>
        public char ShortArg { get; }
        /// <summary>
        /// Long name of the option.
        /// </summary>
        public string LongArg { get; }
        /// <summary>
        /// Get or set whether the option is explicitly required.
        /// </summary>
        public bool Required { get; set; }
        /// <summary>
        /// Get or set the explanation of the option.
        /// </summary>
        public string HelpText { get; set; }
        /// <summary>
        /// Get or set the type of converter.
        /// </summary>
        public Type ConverterType { get; set; }
    }
    /// <summary>
    /// Represents a <see langword="abstract"/> class to parse command line to properties.
    /// </summary>
    public abstract class CommandLine : Parser<OptionAttribute>
    {
        private Dictionary<string, char> validShortArgs = new Dictionary<string, char>();
        private Dictionary<string, string> validLongArgs = new Dictionary<string, string>();
        /// <summary>
        /// Short form of help option.
        /// </summary>
        public virtual char ShortHelpArg => 'h';
        /// <summary>
        /// Long form of help option.
        /// </summary>
        public virtual string LongHelpArg => "help";
        /// <summary>
        /// Initializes an instance of <see cref="CommandLine"/> class.
        /// </summary>
        public CommandLine()
            : base()
        { }
        /// <summary>
        /// Get a <see cref="string"/> key and <see cref="SettingsPropertyInfo{T}"/> value of a <see cref="PropertyInfo"/>.
        /// </summary>
        /// <param name="prop">The property info.</param>
        /// <returns>A key value pair.</returns>
        protected override (string Key, SettingsPropertyInfo<OptionAttribute> Value)? GetKeyValuePairFromPropertyInfo(PropertyInfo prop)
        {
            if (Attribute.GetCustomAttribute(prop, typeof(OptionAttribute)) is OptionAttribute option)
            {
                validShortArgs.Add(prop.Name, option.ShortArg);
                validLongArgs.Add(prop.Name, option.LongArg);
                return (prop.Name, new SettingsPropertyInfo<OptionAttribute>(option, prop, option.ConverterType));
            }
            return null;
        }
        /// <summary>
        /// Initializes a new <see cref="Dictionary{TKey, TValue}"/> of the args array.
        /// </summary>
        /// <param name="args">The args array.</param>
        /// <returns>An instance of <see cref="Dictionary{TKey, TValue}"/>.</returns>
        /// <exception cref="ArgInvalidException"></exception>
        private Dictionary<string, string> InitArgs(string[] args)
        {
            var result = new Dictionary<string, string>();
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == null || args[i].Length == 0 || args[i][0] != '-')
                {
                    throw ExceptionHelper.ArgInvalid(args[i]);
                }
                string argKey = args[i];
                string argValue = null;
                i++;
                if (i >= args.Length || args[i] == null || args[i].Length == 0 || args[i][0] == '-')
                {
                    if (argKey.StartsWith("--"))
                    {
                        int eqi = argKey.IndexOf('=');
                        if (eqi >= 0 && eqi + 1 < argKey.Length)
                        {
                            argValue = argKey.Substring(eqi + 1);
                            argKey = argKey.Substring(2, eqi - 2);
                        }
                        else
                        {
                            argKey = argKey.Substring(2);
                        }
                    }
                    else
                    {
                        if (argKey.Length > 2)
                        {
                            argValue = argKey.Substring(2);
                        }
                        argKey = argKey.Substring(1, 1);
                    }
                    i--;
                }
                else
                {
                    if (argKey.StartsWith("--"))
                    {
                        argKey = argKey.Substring(2);
                    }
                    else
                    {
                        argKey = argKey.Substring(1);
                    }
                    argValue = args[i];
                }
                if (argKey != LongHelpArg && !validLongArgs.ContainsValue(argKey) && argKey[0] != ShortHelpArg && !validShortArgs.ContainsValue(argKey[0]))
                {
                    throw ExceptionHelper.ArgInvalid(argKey);
                }
#if NETCOREAPP || NETSTANDARD2_1
                if (!result.TryAdd(argKey, argValue))
                {
                    throw ExceptionHelper.ArgInvalid(argKey);
                }
#else
                try
                {
                    result.Add(argKey, argValue);
                }
                catch (Exception ex)
                {
                    throw ExceptionHelper.ArgInvalid(argKey, ex.Message, ex);
                }
#endif
            }
            return result;
        }
        /// <summary>
        /// Parse args.
        /// </summary>
        /// <param name="args">The args string.</param>
        public void Parse(string args) => Parse(args?.Split(' '));
        /// <summary>
        /// Parse args.
        /// </summary>
        /// <param name="args">The args.</param>
        public void Parse(string[] args)
        {
            var argdic = InitArgs(args ?? throw ExceptionHelper.ArgumentNull(nameof(args)));
            bool help = argdic.ContainsKey(ShortHelpArg.ToString()) || argdic.ContainsKey(LongHelpArg);
            if (help && argdic.Count == 1)
            {
                PrintUsage();
                return;
            }
            foreach (string name in Names)
            {
                string arg = validShortArgs[name].ToString();
                bool assigned = false;
                object propValue = null;
                bool isbool = properties[name].Property.PropertyType == typeof(bool);
                if (argdic.TryGetValue(arg, out string value))
                {
                    if (help)
                    {
                        PrintUsage(name);
                    }
                    else
                    {
                        if (isbool)
                        {
                            propValue = true;
                        }
                        else
                        {
                            propValue = value;
                        }
                    }
                    assigned = true;
                }
                arg = validLongArgs[name];
                if (arg != null && argdic.TryGetValue(arg, out value))
                {
                    if (!assigned)
                    {
                        if (help)
                        {
                            PrintUsage(name);
                        }
                        else
                        {
                            if (isbool)
                            {
                                propValue = true;
                            }
                            else
                            {
                                propValue = value;
                            }
                        }
                        assigned = true;
                    }
                    else if (!help)
                    {
                        throw ExceptionHelper.ArgRepeated(arg);
                    }
                }
                if (!help && !assigned && properties[name].Attribute.Required)
                {
                    throw ExceptionHelper.ArgRequired(arg);
                }
                this[name] = propValue;
            }
        }
        /// <summary>
        /// Print help texts of all options.
        /// </summary>
        public void PrintUsage()
        {
            foreach (string name in Names)
            {
                PrintUsage(name);
            }
        }
        /// <summary>
        /// Print help text of a option with its property name.
        /// </summary>
        /// <param name="name">The property name.</param>
        private void PrintUsage(string name) => PrintUsage(properties[name].Attribute);
        /// <summary>
        /// Print help text of a option.
        /// </summary>
        /// <param name="opt">The option.</param>
        protected virtual void PrintUsage(OptionAttribute opt)
        { }
    }
}
