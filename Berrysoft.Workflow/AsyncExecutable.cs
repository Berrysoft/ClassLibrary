using System;
using System.Threading.Tasks;

namespace Berrysoft.Workflow
{
    public class AsyncExecutable : IExecutable
    {
        Func<IExecutable> func;
        Func<Task<IExecutable>> funcAsync;
        public AsyncExecutable(Func<Task<IExecutable>> funcAsync)
            : this(null, funcAsync)
        { }
        public AsyncExecutable(Func<IExecutable> func, Func<Task<IExecutable>> funcAsync)
        {
            this.func = func;
            this.funcAsync = funcAsync;
        }
        public IExecutor GetExecutor()
        {
            throw new NotImplementedException();
        }
        internal struct Executor : IExecutor
        {
            AsyncExecutable executable;
            public Executor(AsyncExecutable executable)
            {
                this.executable = executable;
            }
            public IExecutable Execute() => (executable.func ?? throw new NotImplementedException()).Invoke();
            public Task<IExecutable> ExecuteAsync() => executable.funcAsync();
        }
    }
}
