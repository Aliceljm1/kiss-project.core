using System;
using System.Collections;
using System.Collections.Generic;

namespace Kiss.Task
{
    public class Tasks
    {
        private static readonly Tasks instance = new Tasks();

        private Tasks()
        {
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public static Tasks Instance
        {
            get { return instance; }
        }

        private Dictionary<string, TaskExecutor> tasks = new Dictionary<string, TaskExecutor>();

        static readonly object locker = new object();

        public void Attach(string taskId, ITask task, object executionState)
        {
            if (string.IsNullOrEmpty(taskId)) throw new ArgumentException("taskId");

            if (tasks.ContainsKey(taskId)) throw new ArgumentException("taskId已存在");

            lock (locker)
            {
                var taskExecutor = new TaskExecutor(task, executionState);
                tasks[taskId] = taskExecutor;
                taskExecutor.Start();
            }
        }

        public Hashtable ReportProgress(string taskId, out bool isRunning)
        {
            isRunning = false;
            if (string.IsNullOrEmpty(taskId)) return null;

            if (tasks.ContainsKey(taskId))
            {
                lock (locker)
                {
                    if (tasks.ContainsKey(taskId))
                    {
                        TaskExecutor taskExecutor = tasks[taskId];

                        isRunning = taskExecutor.IsRunning;

                        Hashtable ht = taskExecutor.Task.ReportProcess();

                        //  remove from task list if task is over
                        if (!taskExecutor.IsRunning)
                            tasks.Remove(taskId);

                        return ht;
                    }
                }
            }

            return null;
        }
    }
}
