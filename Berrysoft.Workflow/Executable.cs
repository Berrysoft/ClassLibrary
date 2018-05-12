using System;
using System.Threading.Tasks;

namespace Berrysoft.Workflow
{
    public interface IExecutable
    {
        IExecutor GetExecutor();
    }
    public interface IExecutor
    {
        IExecutable Execute();
        Task<IExecutable> ExecuteAsync();
    }
    public static class Executable
    {
        internal static readonly Func<bool> DefaultPredicate = (() => true);
        internal static readonly Func<IExecutable> DefaultFunc = (() => null);
        internal static readonly Func<Task<IExecutable>> DefaultFuncAsync = (() => new Task<IExecutable>(DefaultFunc));
        public static void Work(this IExecutable executable)
        {
            IExecutable current = executable;
            while (current != null)
            {
                IExecutor executor = current.GetExecutor();
                current = executor.Execute();
            }
        }
        public static async Task WorkAsync(this IExecutable executable)
        {
            IExecutable current = executable;
            while (current != null)
            {
                IExecutor executor = current.GetExecutor();
                current = await executor.ExecuteAsync();
            }
        }
    }
}
