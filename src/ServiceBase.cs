using System;
using System.Collections;
using System.Collections.Generic;
using Kiss.Utils;

namespace Kiss
{
    /// <summary>
    /// 统一的服务接口
    /// </summary>
    public abstract class ServiceBase
    {
        public abstract void Run();

        public static List<ServiceBase> GetsAll()
        {
            List<ServiceBase> list = new List<ServiceBase>();

            foreach (Type type in ServiceLocator.Instance.Find(typeof(ServiceBase), true))
            {
                ServiceBase s = Activator.CreateInstance(type) as ServiceBase;

                list.Add(s);
            }

            return list;
        }
    }

    /// <summary>
    /// 命令接口，用于处理没有返回值的操作
    /// </summary>
    /// <remarks>
    /// 命令名称定义在Type变量
    /// </remarks>
    public interface ICommand
    {
        /// <summary>
        /// 是否可以处理该命令
        /// </summary>
        bool CanProcss(string command);

        /// <summary>
        /// 处理命令
        /// </summary>
        string Execute(Hashtable ht);
    }

    /// <summary>
    /// 命令处理入口
    /// </summary>
    public static class CommandFactory
    {
        static readonly List<Type> types;

        static CommandFactory()
        {
            types = new List<Type>(ServiceLocator.Instance.Find(typeof(ICommand), true));
        }

        public static string Process(Hashtable ht)
        {
            if (ht == null) return null;

            string type = Convert.ToString(ht["Type"]);
            if (string.IsNullOrEmpty(type)) return null;

            foreach (var item in types)
            {
                ICommand cmd = Activator.CreateInstance(item) as ICommand;

                if (!cmd.CanProcss(type)) continue;

                Console.WriteLine("execute {0}:", item.Name);

                try
                {
                    string result = cmd.Execute(ht);

                    Console.WriteLine("end {0}:", item.Name);

                    return result;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ExceptionUtil.WriteException(ex));

                    return null;
                }
            }

            return null;
        }
    }
}
