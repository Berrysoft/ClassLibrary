﻿using System;
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
