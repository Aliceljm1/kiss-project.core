using System;

namespace Kiss.Logging
{
    [Serializable]
    [OriginalEntityName("gLog")]
    public class Log : QueryObject<Log, int>
    {
        public string SiteKey { get; set; }
        public string Logger { get; set; }
        public string Message { get; set; }
        public string LogLevel { get; set; }
        public DateTime DateCreate { get; set; }
    }
}
