using Kiss.Utils;
using System;
using System.Threading;

namespace Kiss.Task
{
    public class TaskExecutor
    {
        public TaskExecutor(ITask job, object state)
        {
            this.Task = job;
            this.ExecutionState = state;
        }

        public ITask Task { get; private set; }
        public object ExecutionState { get; private set; }
        public bool IsRunning { get; private set; }

        public void Start()
        {
            ThreadPool.QueueUserWorkItem((state) =>
            {
                try
                {
                    IsRunning = true;
                    Task.Execute(state);
                }
                catch (Exception ex)
                {
                    LogManager.GetLogger<TaskExecutor>().Error(ExceptionUtil.WriteException(ex));
                }
                finally
                {
                    IsRunning = false;
                }

            }, ExecutionState);
        }
    }
}
