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
            Args = new Dictionary<string, string>();
            for (int i = 0; i < args.Length; i++)
            {
                if (!(args[i].StartsWith(LongHead) || args[i].StartsWith(ShortHead)))
                {
                    throw new ArgNotValidException(args[i]);
                }
#if NETCOREAPP2_0
                if (!Args.TryAdd(args[i], args[i + 1]))
                {
                    throw new ArgNotValidException(args[i]);
                }
#else
                try
                {
                    Args.Add(args[i], args[i + 1]);
                }
                catch (Exception ex)
                {
                    throw new ArgNotValidException(args[i], ex.Message, ex);
                }
#endif
                i++;
            }
        }
        public Dictionary<string, string> Args { get; }
        protected virtual string ShortHead => "-";
        protected virtual string LongHead => "--";
        public void Parse()
        {
            foreach (PropertyInfo prop in GetType().GetProperties())
            {
                if (Attribute.GetCustomAttribute(prop, typeof(OptionAttribute)) is OptionAttribute option)
                {
                    string arg;
                    bool assigned = false;
                    if (option.ShortArg != null && Args.ContainsKey(arg = ShortHead + option.ShortArg))
                    {
                        prop.SetValue(this, Convert.ChangeType(Args[arg], prop.PropertyType));
                        assigned = true;
                    }
                    if (option.LongArg != null && Args.ContainsKey(arg = LongHead + option.LongArg))
                    {
                        if (assigned)
                        {
                            throw new ArgRepeatedException(arg);
                        }
                        prop.SetValue(this, Convert.ChangeType(Args[arg], prop.PropertyType));
                        assigned = true;
                    }
                    if (!assigned && option.Required)
                    {
                        throw new ArgRequiredException(LongHead + option.LongArg);
                    }
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
