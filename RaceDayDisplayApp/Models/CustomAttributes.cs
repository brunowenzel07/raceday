using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RaceDayDisplayApp.Models
{
    
    /// <summary>
    /// used in both Settings and Runner classes
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class CustomDisplayAttribute : Attribute
    {
        public DisplayOn Display { get; set; }

        public CustomFormatters CustomFormatter { get; set; }

        public int FixedColumnSize { get; set; }

        public bool RenderCheckbox { get; set; }

        public CustomDisplayAttribute(DisplayOn disp = DisplayOn.BOTH, CustomFormatters form = CustomFormatters.none, int colSize = -1, bool checkbox = true)
        {
            Display = disp;
            CustomFormatter = form;
            FixedColumnSize = colSize;
            RenderCheckbox = checkbox;
        }
    }

    public enum DisplayOn
    {
        NONE,
        AUS,
        HK,
        BOTH
    }

    /// <summary>
    /// Each value is associated to a JavaScriptFormatter
    /// </summary>
    public enum CustomFormatters
    {
        none,
        currencyFormatter,
        percentageFormatter,
        linkFormatter,
        winOddsFormatter,
        placeOddsFormatter
    }


    /// <summary>
    /// Used in Settings class
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class LinkedToAttribute : Attribute
    {
        public string Attribute { get; set; }

        public LinkedToAttribute(string attr = "")
        {
            Attribute = attr;
        }
    }

    ///// <summary>
    ///// This attribute is used to arrange class properties by order of declaration
    ///// </summary>
    //[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    //public sealed class OrderAttribute : Attribute
    //{
    //    private readonly int order_;
    //    public OrderAttribute([CallerLineNumber]int order = 0)
    //    {
    //        order_ = order;
    //    }

    //    public int Order { get { return order_; } }
    //}

}
