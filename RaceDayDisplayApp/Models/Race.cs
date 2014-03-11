using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RaceDayDisplayApp.Models
{
    /// <summary>
    /// Class used to fill the Races DropDownList in the Details view
    /// </summary>
    public class RaceBase
    {
        [Key]
        [CustomDisplay(DisplayOn.NONE)]
        public int RaceId { get; set; }

        [Display(Name = "Race Number", Order = 0)]
        [CustomDisplay(DisplayOn.BOTH)]
        public int RaceNumber { get; set; }
    }

    /// <summary>
    /// Clase used to display the Races list in the RacesIndex view
    /// </summary>
    public class RaceDisplay : RaceBase
    {
        public string RaceName { get; set; }

        public DateTime RaceJumpDateTimeUTC { get; set; }

        public DateTime MeetingDate { get; set; }

        public string RaceCourseName { get; set; }
    }

    /// <summary>
    /// Class used to show the Race details in the Race view
    /// </summary>
    public class Race : RaceBase
    {
        //public int MeetingId { get; set; }

        [Display(Name = "Race Type", Order = 0)]
        [CustomDisplay(DisplayOn.BOTH)]
        public string RaceTypeName { get; set; }

        [Display(Name = "Distance", Order = 0)]
        [CustomDisplay(DisplayOn.BOTH)]
        public string DistanceName { get; set; }

        [CustomDisplay(DisplayOn.NONE)]
        public DateTime RaceJumpDateTimeUTC { get; set; }

        [Display(Name = "Jump Time", Order = 0)]
        [CustomDisplay(DisplayOn.BOTH)]
        public string Formatted_JumpDateTimeUTC { get { return RaceJumpDateTimeUTC.ToString() + " UTC"; } }

        [Display(Name = "Race Index", Order = 0)]
        [CustomDisplay(DisplayOn.HK)]
        public int HK_RaceIndex { get; set; }

        //public System.TimeSpan RaceWinningTime { get; set; }

        [Display(Name = "Race Going", Order = 0)]
        [CustomDisplay(DisplayOn.BOTH)]
        public string RaceGoingName { get; set; }

        [CustomDisplay(DisplayOn.NONE)]
        public bool isTurf { get; set; }

        [Display(Name = "Is turf", Order = 0)]
        [CustomDisplay(DisplayOn.BOTH)]
        public string IsTurf { get { return isTurf ? null : "No"; } }//only displays "No" if not isTurf

        //public short NumberOfRunners { get; set; }
        
        //CONTROL FIELDS

        [CustomDisplay(DisplayOn.NONE)]
        public bool isDone { get; set; } 

        [CustomDisplay(DisplayOn.NONE)]
        public bool isStarted { get; set; }

        [CustomDisplay(DisplayOn.NONE)]
        public string CountryCode { get; set; }

        [CustomDisplay(DisplayOn.NONE)]
        public bool IsHK
        {
            get { return CountryCode == "HKG"; }
        }

        [CustomDisplay(DisplayOn.NONE)]
        public static Race DummyRace
        {
            get
            {
                return FizzWare.NBuilder.Builder<Race>.CreateNew().Build();
            }
        }
    }


    public class NameValuePair
    {
        public string DisplayName;
        public string Value;
    }

}
