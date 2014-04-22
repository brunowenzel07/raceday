using RaceDayDisplayApp.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace RaceDayDisplayApp.Models
{
    static class TimeZoneHelper
    {
        private static Dictionary<int, string> winRegEntries;

        static TimeZoneHelper()
        {
            var config = loadConfigFile();
            var codes = new DBGateway().GetTimeZones();

            winRegEntries = new Dictionary<int, string>();
            codes.ToList().ForEach(c => winRegEntries.Add(c.Key, config[c.Value]));
        }

        private static Dictionary<string, string> loadConfigFile()
        {
            Dictionary<string, string> config = new Dictionary<string, string>();

            XDocument doc = XDocument.Load(AppDomain.CurrentDomain.BaseDirectory + "timezones.config");
            doc.Root.Elements("timeZone").ToList().ForEach(t =>
                config.Add((string)t.Attribute("tz"), (string)t.Attribute("winRegEntry")));

            return config;
        }

        public static DateTime ToUTC(DateTime dt, int AUS_StateId)
        {
            string winEntry;
            if (!winRegEntries.TryGetValue(AUS_StateId, out winEntry))
                return dt; //TODO log warning
            
            TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById(winEntry);
            return TimeZoneInfo.ConvertTimeToUtc(dt, tz);
        }
    }
}