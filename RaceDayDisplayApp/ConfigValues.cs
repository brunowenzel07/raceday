
using System.Configuration;

namespace RaceDayDisplayApp
{
    public static class ConfigValues
    {
        public const string RaceStatusDone = "PAYING";
        public const string DefaultAction = "Index";

        public const int NameColumnWidth = 140;
        public const double CacheRefreshIntervalMs = 500; //half a second
        public const int CacheExpireTimeSec = 1800; //half an hour

        public const int MinSecsToUpdateRaceStat = 3000; //50 min
        public static readonly int DefaultRefreshInterval = 3600; //1 hour

        //public static readonly int ActiveRefreshInterval = int.Parse(ConfigurationManager.AppSettings["Active_RefreshIntervalSec"]);
        //public static readonly int FinishingRefreshinterval = int.Parse(ConfigurationManager.AppSettings["Finishing_RefreshintervalSec"]);
        //public static readonly int ActiveBeforeJumpTime = int.Parse(ConfigurationManager.AppSettings["Active_BeforeJumpTimeSec"]);
        //public static readonly int FinishingBeforeJumpTime = int.Parse(ConfigurationManager.AppSettings["Finishing_BeforeJumpTimeSec"]);

        public static float ClientAddedDelay = 1.0f; //float.Parse(ConfigurationManager.AppSettings["ClientAddedDelaySecs"]);
        public static float ServerAddedDelay = 0.5f; //float.Parse(ConfigurationManager.AppSettings["ServerAddedDelaySecs"]);
    }
}