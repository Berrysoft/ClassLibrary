using System;

namespace Berrysoft.Console
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public sealed class OptionAttribute : Attribute
    {
        public OptionAttribute(char? shortArg, string longArg)
        {
            ShortArg = shortArg;
            LongArg = longArg;
        }
        public char? ShortArg { get; }
        public string LongArg { get; }
        public bool Required { get; set; }
        public string DefaultValue { get; set; }
        public string HelpText { get; set; }
    }
    public abstract class CommandLine
    {
    }
}
