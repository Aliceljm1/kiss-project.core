using System.Collections;

namespace Kiss.Task
{
    public interface ITask
    {
        void Execute(object state);
        Hashtable ReportProcess();
    }
}
