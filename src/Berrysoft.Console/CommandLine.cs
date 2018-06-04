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
        /// Initialize an instance of <see cref="OptionAttribute"/>.
        /// </summary>
        /// <param name="shortArg">Short name of the option.</param>
        /// <param name="longArg">Long name of the option.</param>
        public OptionAttribute(char shortArg, string longArg)
            : this(new string(shortArg, 1), longArg)
        { }
        /// <summary>
        /// Initialize an instance of <see cref="OptionAttribute"/>.
        /// </summary>
        /// <param name="shortArg">Short name of the option.</param>
        /// <param name="longArg">Long name of the option.</param>
        public OptionAttribute(string shortArg, string longArg)
        {
            ShortArg = shortArg;
            LongArg = longArg;
        }
        /// <summary>
        /// Short name of the option.
        /// </summary>
        public string ShortArg { get; }
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
        public Type ConverterType { get; set; }
    }
    /// <summary>
    /// Represents a <see langword="abstract"/> class to parse command line to properties.
    /// </summary>
    public abstract class CommandLine : Parser<OptionAttribute>
    {
        private Dictionary<string, string> validShortArgs = new Dictionary<string, string>();
        private Dictionary<string, string> validLongArgs = new Dictionary<string, string>();
        private Dictionary<string, string> args;
        private HashSet<string> required = new HashSet<string>();
        /// <summary>
        /// Head of short options.
        /// </summary>
        public virtual string ShortHead => "-";
        /// <summary>
        /// Head of long options.
        /// </summary>
        public virtual string LongHead => "--";
        /// <summary>
        /// Short form of help option.
        /// </summary>
        public virtual string ShortHelpArg => "h";
        /// <summary>
        /// Long form of help option.
        /// </summary>
        public virtual string LongHelpArg => "help";
        public CommandLine()
            : base()
        { }
        private protected override (string Key, Setting<OptionAttribute> Value)? GetKeyValuePairFromPropertyInfo(PropertyInfo prop)
        {
            if (Attribute.GetCustomAttribute(prop, typeof(OptionAttribute)) is OptionAttribute option)
            {
                string sht = ShortHead + option.ShortArg;
                string lng = LongHead + option.LongArg;
                validShortArgs.Add(prop.Name, sht);
                validLongArgs.Add(prop.Name, lng);
                if (option.Required)
                {
                    required.Add(prop.Name);
                }
                return (prop.Name, new Setting<OptionAttribute>(prop, option.ConverterType));
            }
            return null;
        }
        /// <summary>
        /// Initialize <see cref="args"/>.
        /// </summary>
        /// <param name="args">Raw args.</param>
        private void InitArgs(string[] args)
        {
            this.args = new Dictionary<string, string>();
            for (int i = 0; i < args.Length; i++)
            {
                if (!StartsWithHead(args[i]))
                {
                    throw ExceptionHelper.ArgInvalid(args[i]);
                }
                string argValue;
                if (i + 1 < args.Length)
                {
                    argValue = args[i + 1];
                    if (StartsWithHead(argValue))
                    {
                        argValue = null;
                    }
                }
                else
                {
                    argValue = null;
                }
                if (!validShortArgs.ContainsValue(args[i]) && !validLongArgs.ContainsValue(args[i]))
                {
                    throw ExceptionHelper.ArgInvalid(args[i]);
                }
#if NETCOREAPP2_1
                if (!this.args.TryAdd(args[i], argValue))
                {
                    throw ExceptionHelper.ArgInvalid(args[i]);
                }
#else
                try
                {
                    this.args.Add(args[i], argValue);
                }
                catch (Exception ex)
                {
                    throw ExceptionHelper.ArgInvalid(args[i], ex.Message, ex);
                }
#endif
                if (argValue != null)
                {
                    i++;
                }
            }
        }
        /// <summary>
        /// Determines whether an arg starts with a head.
        /// </summary>
        /// <param name="arg">The arg.</param>
        /// <returns><see langword="true"/> if the arg starts with a head; otherwise, <see langword="false"/>.</returns>
        private bool StartsWithHead(string arg)
        {
            return arg.StartsWith(LongHead) || arg.StartsWith(ShortHead);
        }
        private bool ContainsHelp()
        {
            return args.ContainsKey(ShortHead + ShortHelpArg) || args.ContainsKey(LongHead + LongHelpArg);
        }
        public void Parse(string[] args)
        {
            InitArgs(args ?? throw ExceptionHelper.ArgumentNull(nameof(args)));
            bool help = ContainsHelp();
            if (help && this.args.Count == 1)
            {
                PrintUsage();
                return;
            }
            foreach (string name in Names)
            {
                string arg = validShortArgs[name];
                bool assigned = false;
                object propValue = null;
                bool isbool = GetSetting(name).Property.PropertyType == typeof(bool);
                if (this.args.TryGetValue(arg, out string value))
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
                if (this.args.TryGetValue(arg, out value))
                {
                    if (help && !assigned)
                    {
                        PrintUsage(name);
                    }
                    else if (!assigned)
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
                    else
                    {
                        throw ExceptionHelper.ArgRepeated(arg);
                    }
                }
                if (!help && !assigned && required.Contains(name))
                {
                    throw ExceptionHelper.ArgRequired(arg);
                }
                SetValue(name, propValue);
            }
        }
        protected virtual object ChangeType(string argName, object value, Type conversionType)
        {
            return Convert.ChangeType(value, conversionType);
        }
        public void PrintUsage()
        {
            foreach (string name in Names)
            {
                PrintUsage(name);
            }
        }
        private void PrintUsage(string name) => PrintUsage(GetSetting(name).Attribute);
        protected virtual void PrintUsage(OptionAttribute opt)
        { }
    }
}
