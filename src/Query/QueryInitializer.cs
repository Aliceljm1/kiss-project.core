using System;
using Kiss.Plugin;
using Kiss.Utils;

namespace Kiss.Query
{
    [AutoInit(Title = "query")]
    public class QueryInitializer : IPluginInitializer
    {
        #region IPluginInitializer Members

        public void Init(ServiceLocator sl, PluginSetting setting)
        {
            if (!setting.Enable) return;

            string type = setting["type"];

            if (StringUtil.IsNullOrEmpty(type))
            {
                setting.Enable = false;
                return;
            }

            sl.AddComponent("kiss.query", typeof(IQueryPlugin), Type.GetType(type, true, true));
        }

        #endregion
    }
}
