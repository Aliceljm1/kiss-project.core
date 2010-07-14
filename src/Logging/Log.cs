using System;

namespace Kiss.Logging
{
    [Serializable]
    [OriginalName("gLog")]
    public class Log : QueryObject<Log, int>
    {
        [PK]
        public override int Id { get { return base.Id; } set { base.Id = value; } }

        public string SiteKey { get; set; }
        public string Logger { get; set; }
        public string Message { get; set; }
        public string LogLevel { get; set; }
        public DateTime DateCreate { get; set; }
    }
}
