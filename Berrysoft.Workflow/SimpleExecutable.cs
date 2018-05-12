using System;
using System.Threading.Tasks;

namespace Berrysoft.Workflow
{
    public class SimpleExecutable : IExecutable
    {
        private Func<IExecutable> func;
        public Func<IExecutable> Func => func;
        public SimpleExecutable(Func<IExecutable> func)
        {
            this.func = func;
        }
        public IExecutor GetExecutor() => new Executor(this);
        internal struct Executor : IExecutor
        {
            private SimpleExecutable executable;
            public Executor(SimpleExecutable executable)
            {
                this.executable = executable;
            }
            public IExecutable Execute() => executable.func();
            public Task<IExecutable> ExecuteAsync() => new Task<IExecutable>(executable.func);
        }
    }
}
