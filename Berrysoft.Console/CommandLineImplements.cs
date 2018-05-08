using System;
using System.Collections.Generic;
using System.Text;

namespace Berrysoft.Console
{
    public abstract class UnixCommandLine : CommandLine
    {
        public UnixCommandLine(string[] args)
            : base(args)
        { }
        protected override string ShortHead => "-";
        protected override string LongHead => "--";
    }
    public abstract class WindowsCommandLine : CommandLine
    {
        public WindowsCommandLine(string[] args)
            : base(args)
        { }
        protected override string ShortHead => "/";
        protected override string LongHead => "/";
    }
}
