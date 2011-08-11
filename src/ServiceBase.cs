using System;
using System.Collections.Generic;

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
}
