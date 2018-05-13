using System;
using System.Threading.Tasks;

namespace Berrysoft.Workflow
{
    public class DoLoop : IExecutable
    {
        
        private Func<bool> firstPredicate;
        private Func<bool> lastPredicate;
        private Func<bool> loopFunc;
        private Func<IExecutable> brokeFunc;
        public Func<bool> FirstPredicate => firstPredicate;
        public Func<bool> LastPredicate => lastPredicate;
        public Func<bool> LoopFunc => loopFunc;
        public Func<IExecutable> BrokeFunc => brokeFunc;
        public DoLoop(Func<bool> firstPredicate,Func<bool> lastPredicate,Func<bool> loopFunc,Func<IExecutable> brokeFunc)
        {
            this.firstPredicate = firstPredicate ?? Executable.DefaultPredicate;
            this.lastPredicate = lastPredicate ?? Executable.DefaultPredicate;
            this.loopFunc = loopFunc ?? Executable.DefaultPredicate;
            this.brokeFunc = brokeFunc ?? Executable.DefaultFunc;
        }
        public IExecutor GetExecutor() => new Executor(this);
        internal struct Executor : IExecutor
        {
            DoLoop executable;
            public Executor(DoLoop executable)
            {
                this.executable = executable;
            }
            public IExecutable Execute()
            {
                while (executable.firstPredicate() && executable.loopFunc() && executable.lastPredicate()) ;
                return executable.brokeFunc();
            }
            public Task<IExecutable> ExecuteAsync() => new Task<IExecutable>(Execute);
        }
    }
}
