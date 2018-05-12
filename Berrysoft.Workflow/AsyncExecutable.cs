using System;
using System.Threading.Tasks;

namespace Berrysoft.Workflow
{
    public class AsyncExecutable : IExecutable
    {
        private Func<IExecutable> func;
        private Func<Task<IExecutable>> funcAsync;
        public Func<IExecutable> Func => func;
        public Func<Task<IExecutable>> FuncAsync => funcAsync;
        public AsyncExecutable(Func<Task<IExecutable>> funcAsync)
            : this(null, funcAsync)
        { }
        public AsyncExecutable(Func<IExecutable> func, Func<Task<IExecutable>> funcAsync)
        {
            this.func = func ?? throw new ArgumentNullException(nameof(func));
            this.funcAsync = funcAsync;
        }
        public IExecutor GetExecutor()
        {
            throw new NotImplementedException();
        }
        internal struct Executor : IExecutor
        {
            private AsyncExecutable executable;
            public Executor(AsyncExecutable executable)
            {
                this.executable = executable;
            }
            public IExecutable Execute() => (executable.func ?? throw new NotImplementedException()).Invoke();
            public Task<IExecutable> ExecuteAsync() => executable.funcAsync();
        }
    }
}
