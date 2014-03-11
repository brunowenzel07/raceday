
using System.Configuration;

namespace RaceDayDisplayApp
{
    public static class Config
    {
        public static readonly int InactiveRefreshInterval = int.Parse(ConfigurationManager.AppSettings["Inactive_RefreshIntervalSec"]);
        public static readonly int ActiveRefreshInterval = int.Parse(ConfigurationManager.AppSettings["Active_RefreshIntervalSec"]);
        public static readonly int FinishingRefreshinterval = int.Parse(ConfigurationManager.AppSettings["Finishing_RefreshintervalSec"]);
        public static readonly int ActiveBeforeJumpTime = int.Parse(ConfigurationManager.AppSettings["Active_BeforeJumpTimeSec"]);
        public static readonly int FinishingBeforeJumpTime = int.Parse(ConfigurationManager.AppSettings["Finishing_BeforeJumpTimeSec"]);
    }
}