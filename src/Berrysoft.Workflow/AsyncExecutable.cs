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
            : this(() => funcAsync().Wait(), funcAsync)
        { }
        public AsyncExecutable(Func<IExecutable> func, Func<Task<IExecutable>> funcAsync)
        {
            this.func = func ?? Executable.DefaultFunc;
            this.funcAsync = funcAsync ?? Executable.DefaultFuncAsync;
        }
        public AsyncExecutable(Func<Task> funcAsync)
            : this(() =>
             {
                 funcAsync().Wait();
                 return null;
             }, async () =>
             {
                 await funcAsync();
                 return null;
             })
        { }
        public AsyncExecutable(Action action, Func<Task> funcAsync)
            : this(() =>
             {
                 action();
                 return null;
             }, async () =>
             {
                 await funcAsync();
                 return null;
             })
        { }
        public IExecutor GetExecutor() => new Executor(this);
        internal struct Executor : IExecutor
        {
            private AsyncExecutable executable;
            public Executor(AsyncExecutable executable)
            {
                this.executable = executable;
            }
            public IExecutable Execute() => executable.func();
            public Task<IExecutable> ExecuteAsync() => executable.funcAsync();
        }
    }
}
