using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using System.Linq;

namespace RaceDayDisplayApp.Models
{
    public static class RunnerHistoryHelper
    {
        public static string[] ControlFields = new[] { "Season" };//,  "SwampedLast?", "FU", "LU" };

        static string fixedGroupName;
        static Dictionary<string, List<string>> fields;
        static Dictionary<string, string> formatters;

        static RunnerHistoryHelper()
        {
            loadConfigFile();
        }

        private static void loadConfigFile()
        {
            fields = new Dictionary<string, List<string>>();
            formatters = new Dictionary<string, string>();

            XDocument doc = XDocument.Load(AppDomain.CurrentDomain.BaseDirectory + "RunnerHistoryFields.config");
            doc.Root.Elements("group").ToList().ForEach(g =>
                {
                    var groupFields = new List<string>();
                    g.Elements("field").ToList().ForEach(f =>
                        {
                            if (f.Attribute("formatter") != null)
                                formatters.Add(f.Value, f.Attribute("formatter").Value);

                            groupFields.Add(f.Value);
                        });

                    var name = (string)g.Attribute("name").Value;
                    fields.Add(name, groupFields);

                    if (bool.Parse(g.Attribute("alwaysVisible").Value))
                        fixedGroupName = name;
                });
        }

        public static string FormatField(string name, object value)
        {
            if (value == null)
                return null;

            string formatter;
            if (formatters.TryGetValue(name, out formatter))
            {
                string val;
                switch (formatter)
	            {
                    case "onlyDate":
                        val = ((DateTime)value).ToShortDateString();
                        break;
                    case "onlySecs":
                        var aux = (TimeSpan)value;
                        val = string.Format("{0}.{1}", aux.Seconds, aux.Milliseconds / 10);
                        break;
                    //case "oneDigit":
                    //    val = ((float)value).ToString("0.0");
                    //    break;
                    default:
                        val = value.ToString();
                    break;
	            }
                return val;
            }
            return value.ToString();
        }

        internal static Dictionary<string,string> GetFieldsIndexes(dynamic runnerHistoryItem)
        {
            var groups = new Dictionary<string, List<int>>();

            fields.ToList().ForEach(f => groups.Add(f.Key, new List<int>(f.Value.Count())));

            int i=1;
            foreach (KeyValuePair<string, object> kvp in runnerHistoryItem)
            {
                if (!ControlFields.Contains(kvp.Key))
                {
                    foreach (var f in fields)
                    {
                        if (f.Value.Contains(kvp.Key))
                            groups[f.Key].Add(i);
                    }
                    i++;
                }
            }

            var result = new Dictionary<string, string>();
            groups.ToList().ForEach(g => 
                {
                    var aux = g.Value;
                    if (g.Key != fixedGroupName)
                        aux.AddRange(groups[fixedGroupName]);
                    
                    result.Add(g.Key, "['" + string.Join("','", aux) + "']");
                });

            return result;
        }

        public static int GetNumAttrs(dynamic obj)
        {
            return ((IEnumerable<KeyValuePair<string, object>>)obj).Where(kvp => !ControlFields.Contains(kvp.Key)).Count();
        }
    }
}