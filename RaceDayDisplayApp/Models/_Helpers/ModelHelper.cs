using RaceDayDisplayApp.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Web;

namespace RaceDayDisplayApp.Models
{
    public static class ModelHelper
    {
        //public static IEnumerable<ViewUserSetting> ToViewUserSettings(UserSettings userSettings, bool isHK)
        //{
        //    Dictionary<string, DisplayAttribute> lookup = new Dictionary<string, DisplayAttribute>();
        //    CustomDisplayAttribute attr = null;
        //    //loops through all the properties of the class UserSettings
        //    return typeof(UserSettings).GetProperties()
        //        .Where(p => (attr = getCustomDisplayAttribute(p)).Display == DisplayOn.BOTH
        //                    || (isHK && attr.Display == DisplayOn.HK)
        //                    || (!isHK && attr.Display == DisplayOn.AUS))
        //        .OrderBy(p => (lookup[p.Name] = getDisplayAttribute(p)).Order)
        //        .Select(p => new ViewUserSetting
        //        {
        //            PropertyName = getLinkedToAttribute(p).Attribute,
        //            DisplayName = lookup[p.Name].Name ?? p.Name,
        //            Checked = (bool)typeof(UserSettings).GetProperty(p.Name).GetValue(userSettings)
        //        });
        //}

        //public static IEnumerable<ViewUserSetting> ToViewUserSettings(UserSettings userSettings, CountryEnum country)
        //{
        //    Dictionary<string, bool> userConfig = new Dictionary<string, bool>();
        //    //loops through all the properties of the class UserSettings
        //    typeof(UserSettings).GetProperties().ToList().ForEach(p => 
        //        {
        //            var prop = typeof(UserSettings).GetProperty(p.Name);
        //            if (prop.PropertyType == typeof(bool))
        //            {
        //                var name = getLinkedToAttribute(p).Attribute;
        //                var chkd = (bool)prop.GetValue(userSettings);
        //                userConfig.Add(name, chkd);
        //            }
        //        });


        //    Dictionary<string, DisplayAttribute> lookup = new Dictionary<string, DisplayAttribute>();
        //    CustomDisplayAttribute attr = null;

        //    //loops through all the properties of the class UserSettings
        //    return typeof(Runner).GetProperties()
        //        .Where(p => (attr = getCustomDisplayAttribute(p)).RenderCheckbox  && Country.Match(country, attr.Display))
        //        .OrderBy(p => (lookup[p.Name] = getDisplayAttribute(p)).Order)
        //        .Select(p =>
        //        {
        //            bool val;
        //            if (!userConfig.TryGetValue(p.Name, out val))
        //                val = false;
                    
        //            return new ViewUserSetting 
        //            {
        //                PropertyName = p.Name,
        //                DisplayName = lookup[p.Name].Name ?? p.Name,
        //                Checked = val 
        //            };
        //        });
        //}

        /// <summary>
        /// Converts an object to a collection of name-value pair that can be consumed by the view
        /// Used to display Meeting and Race details
        /// </summary>
        public static IEnumerable<DisplayProperty> ToNameValuePairs(object obj, CountryEnum country)
        {
            Type t = obj.GetType();
            Dictionary<string, DisplayAttribute> lookup = new Dictionary<string, DisplayAttribute>();
            List<DisplayProperty> result = new List<DisplayProperty>();
            
            //loops through all the properties of the class UserSettings
            t.GetProperties()
                .Where(p => Country.Match(country, getCustomDisplayAttribute(p).Display))
                .OrderBy(p => (lookup[p.Name] = getDisplayAttribute(p)).Order)
                .ToList().ForEach(p =>
                {
                    PropertyInfo prop = t.GetProperty(p.Name);
                    var val = prop.GetValue(obj);
                    Type type = prop.PropertyType;
                    object defaultValue = type.IsValueType ? Activator.CreateInstance(type) : null;
                    if (val != null && !val.Equals(defaultValue))
                    {
                        var fp = new DisplayProperty();
                        fp.FieldName = p.Name;
                        fp.DisplayName = lookup[p.Name].Name ?? p.Name;
                        fp.Value = val.ToString();
                        result.Add(fp);
                    }
                });
            return result;
        }

        ///// <summary>
        ///// Creates a collection of grid column based on the properties of the class Runner and their attributes
        ///// </summary>
        //public static IEnumerable<MvcJqGrid.Column> GetGridColumns(Race race)
        //{
        //    Dictionary<string, DisplayAttribute> lookup = new Dictionary<string, DisplayAttribute>();
        //    Dictionary<string, CustomDisplayAttribute> lookup2 = new Dictionary<string, CustomDisplayAttribute>();
        //    //loops through all the properties of the class Runner
        //    var columns = typeof(Runner).GetProperties()
        //        .Where(p => Country.Match(race.CountryEnum, (lookup2[p.Name] = getCustomDisplayAttribute(p)).Display))
        //        .OrderBy(p => (lookup[p.Name] = getDisplayAttribute(p)).Order)
        //        .Select(p => {
        //            var col = new MvcJqGrid.Column(p.Name);
        //            col.SetLabel(lookup[p.Name].Name ?? p.Name);
                    
        //            //check if it's key
        //            object[] attributes = p.GetCustomAttributes(typeof(KeyAttribute), false);
        //            if (attributes != null && attributes.Length > 0)
        //            {
        //                col.SetKey(true).SetHidden(true);
        //            } 
                    
        //            //assign cell formatter
        //            CustomDisplayAttribute attr = lookup2[p.Name];
        //            if (attr.CustomFormatter != CustomFormatters.none)
        //            {
        //                col.SetCustomFormatter(attr.CustomFormatter.ToString());
        //            }
                    
        //            //assign col width
        //            if (attr.FixedColumnSize != -1)
        //                col.SetWidth(attr.FixedColumnSize).SetFixedWidth(true);
        //            return col;
        //        });

        //    return columns;
        //    //var statisticalColumns = new DBGateway().GetGridColumns(race);
        //    //return columns.Concat(statisticalColumns);
        //}


        /// <summary>
        /// Creates a collection of grid column based on the dynamic field retrieved from database
        /// </summary>
        public static IEnumerable<MvcJqGrid.Column> GetGridColumns(Race race)
        {
            List<MvcJqGrid.Column> columns = new List<MvcJqGrid.Column>();
            if (race.Runners.Count == 0)
                return columns;

            foreach (KeyValuePair<string, object> kvp in race.Runners[0])
            {
                var col = new MvcJqGrid.Column(kvp.Key);
                //TODO label != name
                col.SetLabel(kvp.Key);

                //TODO check if it's key
                //col.SetKey(true).SetHidden(true);

                //TODO assign cell formatter
                //col.SetCustomFormatter(attr.CustomFormatter.ToString());

                //TODO assign col width
                //col.SetWidth(attr.FixedColumnSize).SetFixedWidth(true);
                
                columns.Add(col);
            }

            return columns;
       }


        private static DisplayAttribute getDisplayAttribute(PropertyInfo info)
        {
            object[] attributes = info.GetCustomAttributes(typeof(DisplayAttribute), false);
            if (attributes != null && attributes.Length > 0)
            {
                return (DisplayAttribute)attributes[0];
            }
            return new DisplayAttribute { Name = info.Name, Order = 0 };
        }

        private static CustomDisplayAttribute getCustomDisplayAttribute(PropertyInfo info)
        {
            object[] attributes = info.GetCustomAttributes(typeof(CustomDisplayAttribute), false);
            if (attributes != null && attributes.Length > 0)
            {
                return (CustomDisplayAttribute)attributes[0];
            }
            return new CustomDisplayAttribute { Display = DisplayOn.ALL };
        }

        private static LinkedToAttribute getLinkedToAttribute(PropertyInfo info)
        {
            object[] attributes = info.GetCustomAttributes(typeof(LinkedToAttribute), false);
            if (attributes != null && attributes.Length > 0)
            {
                return (LinkedToAttribute)attributes[0];
            }
            return new LinkedToAttribute { Attribute = info.Name };
        }


    }
}