using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using System.Linq;
using System.Text;
using RaceDayDisplayApp.Common;

namespace RaceDayDisplayApp.Models
{
    public static class LiveDataGridHelper
    {
        public static string[] ControlFields = new string[0]; //set control fields, if any

        static Dictionary<string, string> alwaysVisibleGroups = new Dictionary<string, string>();
        static Dictionary<KeyValuePair<string, string>, List<string>> fields;
        static Dictionary<string, string> formatters;

        static LiveDataGridHelper()
        {
            loadConfigFile();
        }

        private static void loadConfigFile()
        {
            fields = new Dictionary<KeyValuePair<string, string>, List<string>>();
            formatters = new Dictionary<string, string>();

            XDocument doc = XDocument.Load(AppDomain.CurrentDomain.BaseDirectory + "LiveDataGrid.config");
            doc.Root.Elements("group").ToList().ForEach(g =>
                {
                    string name = "", value = "", countryCode = "";
                    try
                    {
                        name = (string)g.Attribute("name").Value;
                        if (g.Attribute("countrycode") != null)
                            countryCode = (string)g.Attribute("countrycode").Value;

                        var groupFields = new List<string>();
                        g.Elements("field").ToList().ForEach(f =>
                            {
                                value = f.Value;
                                if (f.Attribute("formatter") != null && !formatters.ContainsKey(value))
                                    formatters.Add(value, f.Attribute("formatter").Value);

                                groupFields.Add(value);
                            });

                        fields.Add(new KeyValuePair<string, string>(countryCode, name), groupFields);

                        if (bool.Parse(g.Attribute("alwaysVisible").Value))
                            alwaysVisibleGroups[countryCode] = name;
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Error parsing RunnerHistoryFields.xml - group=" + name + " country=" + countryCode + " value=" + value, e);
                    }
                });
        }

        /// <summary>
        /// Used at view level to format fields before displaying them
        /// </summary>
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

        /// <summary>
        /// Used on the Runner History page to render group buttons based on the config file. 
        /// The view will use the result to render buttons this way:
        /// <input id="btn_@(i)" class="btn_hist" type="button" value="@(item.Key)" onclick="showFields(@(i++), @(item.Value))" />
        /// </summary>
        public static Dictionary<string,string> GetFieldsIndexes(string countryCode, dynamic runnerHistoryItem)
        {
            var groups = new Dictionary<string, List<int>>();

            var countryFields = fields.Where(f => f.Key.Key == "" || f.Key.Key == countryCode).ToList();
            //get field groups for this country from config file and create a container for each group
            countryFields.ForEach(f => groups.Add(f.Key.Value, new List<int>(f.Value.Count()))); 

            List<string> warningFields = new List<string>(); //to log a mismatch between config file and database fields
            int i=1;
            foreach (KeyValuePair<string, object> kvp in runnerHistoryItem) //loop through dynamic data
            {
                if (!ControlFields.Contains(kvp.Key)) //filter control fields
                {
                    bool found = false;

                    foreach (var f in countryFields) //loop through field groups for this country
                    {
                        if (f.Value.Contains(kvp.Key)) //only add those fields contained in the config file
                        {
                            groups[f.Key.Value].Add(i);
                            found = true;
                        }
                    }
                    if (!found)
                        warningFields.Add(kvp.Key);
                    i++;
                }
            }

            if (warningFields.Count > 0)
                Log.Instance.Warn("The following Runner's attributes are not listed in the config file: " + string.Join(", ", warningFields));

            var result = new Dictionary<string, string>();
            groups.ToList().ForEach(g => 
                {
                    var aux = g.Value;
                    string fixedGroup;
                    //add country-specific fixed fields
                    if (alwaysVisibleGroups.TryGetValue(countryCode, out fixedGroup) && g.Key != fixedGroup)
                        aux.AddRange(groups[fixedGroup]);
                    //add country-independent fixed fields
                    if (alwaysVisibleGroups.TryGetValue("", out fixedGroup) && g.Key != fixedGroup)
                        aux.AddRange(groups[fixedGroup]);
                    
                    result.Add(g.Key, "['" + string.Join("','", aux) + "']");
                });

            return result;
        }

        /// <summary>
        /// Just returns the number of DISPLAY attributes for Runners. It is used by the view to calculate the colspan on the html table
        /// </summary>
        public static int GetNumAttrs(dynamic obj)
        {
            return ((IEnumerable<KeyValuePair<string, object>>)obj).Where(kvp => !ControlFields.Contains(kvp.Key)).Count();
        }
    }
}