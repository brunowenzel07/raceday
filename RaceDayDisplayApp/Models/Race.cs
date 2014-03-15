using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

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
    /// Class used to display the Races list in the RacesIndex view
    /// </summary>
    public class RaceDisplay : RaceBase
    {
        public string RaceName { get; set; }

        public DateTime RaceJumpDateTimeUTC { get; set; }

        public DateTime MeetingDate { get; set; }

        public string RaceCourseName { get; set; }
    }

    /// <summary>
    /// Class that is retrieved periodically while the race is active
    /// </summary>
    public class RaceDyn : RaceBase
    {
        [CustomDisplay(DisplayOn.NONE)]
        public IEnumerable<RunnerDyn> Runners { get; set; }

        [CustomDisplay(DisplayOn.NONE)]
        public bool isDone { get; set; }

        [Display(Name = "Win Pool", Order = 0)]
        [CustomDisplay(DisplayOn.BOTH)]
        public float RaceWinPool { get; set; }

        [Display(Name = "PP Pool", Order = 0)]
        [CustomDisplay(DisplayOn.BOTH)]
        public float RacePPPool { get; set; }

        [Display(Name = "EX Pool Total", Order = 0)]
        [CustomDisplay(DisplayOn.BOTH)]
        public float EXPoolTotal { get; set; }

        [Display(Name = "Ex Div Amount", Order = 0)]
        [CustomDisplay(DisplayOn.BOTH)]
        public float EXDivAmount { get; set; }

        [Display(Name = "QN Pool Total", Order = 0)]
        [CustomDisplay(DisplayOn.BOTH)]
        public float QNPoolTotal { get; set; }

        [Display(Name = "QN Div Amount", Order = 0)]
        [CustomDisplay(DisplayOn.BOTH)]
        public float QNDivAmount { get; set; }

        [Display(Name = "F4 Pool Total", Order = 0)]
        [CustomDisplay(DisplayOn.BOTH)]
        public float F4PoolTotal { get; set; }

        [Display(Name = "F4 Div Amount", Order = 0)]
        [CustomDisplay(DisplayOn.BOTH)]
        public float F4DivAmount { get; set; }

        [Display(Name = "TF Pool Total", Order = 0)]
        [CustomDisplay(DisplayOn.BOTH)]
        public float TFPoolTotal { get; set; }
    }

    /// <summary>
    /// Class used to show the Race details in the Race view
    /// </summary>
    public class Race : RaceDyn
    {
        [CustomDisplay(DisplayOn.NONE)]
        public int MeetingId { get; set; }

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

        internal void Update(RaceDyn race)
        {
            this.Runners.ToList().ForEach(r => 
                ((Runner)r).Update(race.Runners.First(rd => rd.RunnerId == r.RunnerId)));
            this.isDone = race.isDone;
            this.RaceWinPool = race.RaceWinPool;
            this.RacePPPool = race.RacePPPool;
            this.EXPoolTotal = race.EXPoolTotal;
            this.EXDivAmount = race.EXDivAmount;
            this.QNPoolTotal = race.QNPoolTotal;
            this.QNDivAmount = race.QNDivAmount;
            this.F4PoolTotal = race.F4PoolTotal;
            this.F4DivAmount = race.F4DivAmount;
            this.TFPoolTotal = race.TFPoolTotal;
        }
    }

    /// <summary>
    /// class used to keep the grid data in cache
    /// </summary>
    public class RaceCache : Race
    {
        public class RefreshInfo
        {
            public DateTime LastDBUpdate { get; set; }
            public DateTime LastServerRefresh { get; set; }
            public DateTime LastTotalRefresh { get; set; }
            public DateTime LastUserRequest { get; set; }
            public DateTime NextRefresh { get; set; }
        }

        public RefreshInfo RefreshVaues = new RefreshInfo();
    }


    public class NameValuePair
    {
        public string DisplayName;
        public string Value;
    }

}
