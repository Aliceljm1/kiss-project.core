using Kiss.Plugin;

namespace Kiss.Notice
{
    [AutoInit(Title = "Notice")]
    public class NoticeInitializer : IPluginInitializer
    {
        #region IPluginInitializer Members

        public void Init(ServiceLocator sl, ref PluginSetting s)
        {
            if (!s.Enable)
                return;

            foreach (var item in Plugin.Plugins.GetPlugins<ChannelAttribute>())
            {
                sl.AddComponent("kiss.notice." + item.ChannelName, item.Decorates);
            }
        }

        #endregion
    }
}
