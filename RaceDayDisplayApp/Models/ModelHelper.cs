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
        public static IEnumerable<ViewUserSetting> ToViewUserSettings(UserSettings userSettings, bool isHK)
        {
            Dictionary<string, DisplayAttribute> lookup = new Dictionary<string, DisplayAttribute>();
            CustomDisplayAttribute attr = null;
            //loops through all the properties of the class UserSettings
            return typeof(UserSettings).GetProperties()
                .Where(p => (attr = getCustomDisplayAttribute(p)).Display == DisplayOn.BOTH
                            || (isHK && attr.Display == DisplayOn.HK)
                            || (!isHK && attr.Display == DisplayOn.AUS))
                .OrderBy(p => (lookup[p.Name] = getDisplayAttribute(p)).Order)
                .Select(p => new ViewUserSetting
                {
                    PropertyName = getLinkedToAttribute(p).Attribute,
                    DisplayName = lookup[p.Name].Name ?? p.Name,
                    Checked = (bool)typeof(UserSettings).GetProperty(p.Name).GetValue(userSettings)
                });
        }

        /// <summary>
        /// Converts an object to a collection of name-value pair that can be consumed by the view
        /// </summary>
        public static IEnumerable<NameValuePair> ToNameValuePairs(object obj, bool isHK)
        {
            Type t = obj.GetType();
            Dictionary<string, DisplayAttribute> lookup = new Dictionary<string, DisplayAttribute>();
            CustomDisplayAttribute attr = null;
            //loops through all the properties of the class UserSettings
            return t.GetProperties()
                .Where(p => (attr = getCustomDisplayAttribute(p)).Display == DisplayOn.BOTH
                            || (isHK && attr.Display == DisplayOn.HK)
                            || (!isHK && attr.Display == DisplayOn.AUS))
                .OrderBy(p => (lookup[p.Name] = getDisplayAttribute(p)).Order)
                .Select(p => new NameValuePair
                {
                    DisplayName = lookup[p.Name].Name ?? p.Name,
                    Value = (t.GetProperty(p.Name).GetValue(obj) ?? "").ToString()
                });
        }

        /// <summary>
        /// Creates a collection of grid column based on the properties of the class Runner and their attributes
        /// </summary>
        public static IEnumerable<MvcJqGrid.Column> GetGridColumns(Race race)
        {
            Dictionary<string, DisplayAttribute> lookup = new Dictionary<string, DisplayAttribute>();
            Dictionary<string, CustomDisplayAttribute> lookup2 = new Dictionary<string, CustomDisplayAttribute>();
            CustomDisplayAttribute attr = null;
            //loops through all the properties of the class Runner
            var columns = typeof(Runner).GetProperties()
                .Where(p => (lookup2[p.Name] = attr = getCustomDisplayAttribute(p)).Display == DisplayOn.BOTH
                            || (race.IsHK && attr.Display == DisplayOn.HK)
                            || (!race.IsHK && attr.Display == DisplayOn.AUS))
                .OrderBy(p => (lookup[p.Name] = getDisplayAttribute(p)).Order)
                .Select(p => {
                    var col = new MvcJqGrid.Column(p.Name);
                    col.SetLabel(lookup[p.Name].Name ?? p.Name);

                    attr = lookup2[p.Name];
                    if (attr.CustomFormatter != CustomFormatters.none)
                        col.SetCustomFormatter(attr.CustomFormatter.ToString());
                    if (attr.FixedColumnSize != -1)
                        col.SetWidth(attr.FixedColumnSize).SetFixedWidth(true);
                    return col;
                });

            var statisticalColumns = new DBGateway().GetGridColumns(race);
            return columns.Concat(statisticalColumns);
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
            return new CustomDisplayAttribute { Display = DisplayOn.BOTH };
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