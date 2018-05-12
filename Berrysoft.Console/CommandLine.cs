﻿using System;
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
        private Dictionary<OptionAttribute, PropertyInfo> properties;
        private HashSet<string> validArgs;
        public Dictionary<string, string> Args { get; private set; }
        public virtual string ShortHead => "-";
        public virtual string LongHead => "--";
        public virtual string ShortHelpArg => "h";
        public virtual string LongHelpArg => "help";
        public CommandLine(string[] args)
        {
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }
            InitDictionary();
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
                if (!validArgs.Contains(args[i]))
                {
                    throw new ArgNotValidException(args[i]);
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
        private void InitDictionary()
        {
            properties = new Dictionary<OptionAttribute, PropertyInfo>();
            validArgs = new HashSet<string>();
            foreach (PropertyInfo prop in GetType().GetProperties())
            {
                if (Attribute.GetCustomAttribute(prop, typeof(OptionAttribute)) is OptionAttribute option)
                {
                    properties.Add(option, prop);
                    validArgs.Add(ShortHead + option.ShortArg);
                    validArgs.Add(LongHead + option.LongArg);
                }
            }
        }
        private bool ContainsHelp()
        {
            return Args.ContainsKey(ShortHead + ShortHelpArg) || Args.ContainsKey(LongHead + LongHelpArg);
        }
        public void Parse()
        {
            bool help = ContainsHelp();
            if (help && Args.Count == 1)
            {
                PrintUsage();
                return;
            }
            foreach (var prop in properties)
            {
                string arg = ShortHead + prop.Key.ShortArg;
                bool assigned = false;
                object propValue = null;
                PropertyInfo p = prop.Value;
                Type type = p.PropertyType;
                if (Args.TryGetValue(arg, out string value))
                {
                    if (help)
                    {
                        PrintUsage(prop.Key);
                    }
                    else
                    {
                        if (type == typeof(bool) && value == null)
                        {
                            propValue = true;
                        }
                        else
                        {
                            propValue = Convert.ChangeType(value, type);
                        }
                    }
                    assigned = true;
                }
                arg = LongHead + prop.Key.LongArg;
                if (Args.TryGetValue(arg, out string value2))
                {
                    if (help && !assigned)
                    {
                        PrintUsage(prop.Key);
                    }
                    else if (!assigned)
                    {
                        if (type == typeof(bool) && value2 == null)
                        {
                            propValue = true;
                        }
                        else
                        {
                            propValue = Convert.ChangeType(value2, type);
                        }
                    }
                    else
                    {
                        throw new ArgRepeatedException(prop.Key.LongArg);
                    }
                }
                if (!help && !assigned && prop.Key.Required)
                {
                    throw new ArgRequiredException(prop.Key.LongArg);
                }
                if (propValue != null)
                {
                    p.SetValue(this, propValue);
                }
            }
        }
        public void PrintUsage()
        {
            foreach (OptionAttribute option in GetOptionAttributes())
            {
                PrintUsage(option);
            }
        }
        protected virtual void PrintUsage(OptionAttribute opt)
        { }
        public IEnumerable<OptionAttribute> GetOptionAttributes()
        {
            foreach (var prop in properties)
            {
                yield return prop.Key;
            }
        }
    }
}