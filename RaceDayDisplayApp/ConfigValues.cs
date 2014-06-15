
using System.Configuration;

namespace RaceDayDisplayApp
{
    public static class ConfigValues
    {
        //number of meeting to display per country on the meetings list
        public const int MeetingListLimit = 50;
        
        //number of races to display on the race list
        public const int RaceListLimit = 150;

        //only the races/meetings of the following 7 days will be displayed
        public const int UpcomingRacesDaysLimit = 7; 

        //security 
        public const string FullAccessRole = "Web Full Access";
        //public const string GuestRole = "Web Guest";
        public const string GuestUserName = "guest";
        public const string GuestPassword = "password1";

        //max number of rows to be displayed on the "show more" statistics table
        public const int MaxStatisticsResults = 10;

        //database naming convention for races that are finished (XML parser uses this same convention)
        public const string RaceStatusDone = "PAYING";

        //default page
        public const string DefaultAction = "Index";

        //fixed size (in px) for runner and jockey columns
        public const int NameColumnWidth = 140;

        //every half a second, a thread checks if any cached race has to be refreshed
        public const double CacheRefreshIntervalMs = 500; //half a second

        //a race is deleted from cache 1 hour after the last time it was accessed
        public const int CacheExpireTimeSec = 1800; //half an hour

        //the static fields of the race are retrieved from DB every 50 min
        public const int MinSecsToUpdateRaceStat = 3000; //50 min

        //refresh interval to use for very old or very new races (never updated by the parser)
        public static readonly int DefaultRefreshInterval = 3600; //1 hour

        //some delay is added to the expected DB race update time  to avoid a collision with the XML parser on DB 
        public static float ClientAddedDelay = 1.0f; //float.Parse(ConfigurationManager.AppSettings["ClientAddedDelaySecs"]);
        public static float ServerAddedDelay = 0.5f; //float.Parse(ConfigurationManager.AppSettings["ServerAddedDelaySecs"]);
    }
}