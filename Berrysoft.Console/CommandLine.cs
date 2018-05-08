using System;
using System.Collections.Generic;
using System.Reflection;

namespace Berrysoft.Console
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class OptionAttribute : Attribute
    {
        public OptionAttribute(char shortArg, string longArg)
            : this(new string(shortArg, 1), longArg)
        { }
        public OptionAttribute(string shortArg, string longArg)
        {
            ShortArg = shortArg;
            LongArg = longArg;
        }
        public string ShortArg { get; }
        public string LongArg { get; }
        public bool Required { get; set; }
        public string HelpText { get; set; }
    }
    public abstract class CommandLine
    {
        public CommandLine(string[] args)
        {
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }
            InitArgs(args);
        }
        private void InitArgs(string[] args)
        {
            Args = new Dictionary<string, string>();
            for (int i = 0; i < args.Length; i++)
            {
                if (!StartsWithHead(args[i]))
                {
                    throw new ArgNotValidException(args[i]);
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
#if NETCOREAPP2_0
                if (!Args.TryAdd(args[i], argValue))
                {
                    throw new ArgNotValidException(args[i]);
                }
#else
                try
                {
                    Args.Add(args[i], argValue);
                }
                catch (Exception ex)
                {
                    throw new ArgNotValidException(args[i], ex.Message, ex);
                }
#endif
                if (argValue != null)
                {
                    i++;
                }
            }
        }
        private bool StartsWithHead(string arg)
        {
            return arg.StartsWith(LongHead) || arg.StartsWith(ShortHead);
        }
        public Dictionary<string, string> Args { get; private set; }
        protected virtual string ShortHead => "-";
        protected virtual string LongHead => "--";
        public void Parse()
        {
            foreach (PropertyInfo prop in GetType().GetProperties())
            {
                if (Attribute.GetCustomAttribute(prop, typeof(OptionAttribute)) is OptionAttribute option)
                {
                    string arg;
                    bool required = option.Required;
                    object propValue = null;
                    if (option.ShortArg != null && Args.ContainsKey(arg = ShortHead + option.ShortArg))
                    {
                        if (prop.PropertyType == typeof(bool) && Args[arg] == null)
                        {
                            propValue = true;
                        }
                        else
                        {
                            propValue = Convert.ChangeType(Args[arg], prop.PropertyType);
                        }
                    }
                    if (option.LongArg != null && Args.ContainsKey(arg = LongHead + option.LongArg))
                    {
                        if (propValue != null)
                        {
                            throw new ArgRepeatedException(arg);
                        }
                        if (prop.PropertyType == typeof(bool) && Args[arg] == null)
                        {
                            propValue = true;
                        }
                        else
                        {
                            propValue = Convert.ChangeType(Args[arg], prop.PropertyType);
                        }
                    }
                    if (propValue == null && required)
                    {
                        throw new ArgRequiredException(LongHead + option.LongArg);
                    }
                    prop.SetValue(this, propValue);
                }
            }
        }
        public IEnumerable<OptionAttribute> GetOptionAttributes()
        {
            foreach (PropertyInfo prop in GetType().GetProperties())
            {
                if (Attribute.GetCustomAttribute(prop, typeof(OptionAttribute)) is OptionAttribute option)
                {
                    yield return option;
                }
            }
        }
    }
}
