using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Berrysoft.Workflow
{
    public class Condition : IExecutable
    {
        private Func<bool> predicate;
        private Func<IExecutable> trueFunc;
        private Func<IExecutable> falseFunc;
        public Func<bool> Predicate => predicate;
        public Func<IExecutable> TrueFunc => trueFunc;
        public Func<IExecutable> FalseFunc => falseFunc;
        public Condition(Func<bool> predicate, Func<IExecutable> trueFunc)
            : this(predicate, trueFunc, null)
        { }
        public Condition(Func<bool> predicate, Func<IExecutable> trueFunc, Func<IExecutable> falseFunc)
        {
            this.predicate = predicate;
            this.trueFunc = trueFunc;
            this.falseFunc = falseFunc;
        }
        public IExecutor GetExecutor() => new Executor(this);
        internal struct Executor : IExecutor
        {
            private Condition executable;
            public Executor(Condition executable)
            {
                this.executable = executable;
            }
            public IExecutable Execute()
            {
                if (executable.predicate())
                {
                    return executable.trueFunc();
                }
                else if (executable.falseFunc != null)
                {
                    return executable.falseFunc();
                }
                else
                {
                    return null;
                }
            }
            public Task<IExecutable> ExecuteAsync() => new Task<IExecutable>(Execute);
        }
    }
}
