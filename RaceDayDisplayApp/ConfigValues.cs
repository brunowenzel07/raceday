
using System.Configuration;

namespace RaceDayDisplayApp
{
    public static class ConfigValues
    {
        public const int NAME_COLUMN_WIDTH = 140;
        public const double CacheRefreshIntervalMs = 500; //half a second

        public static readonly int InactiveRefreshInterval = int.Parse(ConfigurationManager.AppSettings["Inactive_RefreshIntervalSec"]);
        public static readonly int ActiveRefreshInterval = int.Parse(ConfigurationManager.AppSettings["Active_RefreshIntervalSec"]);
        public static readonly int FinishingRefreshinterval = int.Parse(ConfigurationManager.AppSettings["Finishing_RefreshintervalSec"]);
        public static readonly int ActiveBeforeJumpTime = int.Parse(ConfigurationManager.AppSettings["Active_BeforeJumpTimeSec"]);
        public static readonly int FinishingBeforeJumpTime = int.Parse(ConfigurationManager.AppSettings["Finishing_BeforeJumpTimeSec"]);

        public static readonly int MeetingsCacheSize = int.Parse(ConfigurationManager.AppSettings["MeetingsCacheSize"]);
        public static readonly int RacesCacheSize = int.Parse(ConfigurationManager.AppSettings["RacesCacheSize"]);
        public static readonly int RunnersCacheSize = int.Parse(ConfigurationManager.AppSettings["RunnersCacheSize"]);
        
    }
}