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

        [Display(Name = "Race No.", Order = 2)]
        [CustomDisplay(DisplayOn.ALL)]
        public int RaceNumber { get; set; }
    }

    /// <summary>
    /// Class used to display the Races list in the RacesIndex view
    /// </summary>
    public class RaceDisplay : RaceBase
    {
        public string RaceName { get; set; }

        public string RaceCourseName { get; set; }

        public string CountryCode { get; set; }

        public DateTime MeetingDate { get; set; }

        public TimeSpan LocalJumpTime { get; set; }

        public int StateId { get; set; }

        public string RaceStatus { get; set; }

        DateTime _raceJumpDateTimeUTC;
        
        public DateTime RaceJumpDateTimeUTC { 
            get {
                if (_raceJumpDateTimeUTC == default(DateTime))
                {
                    var aux = new DateTime(
                        MeetingDate.Year,
                        MeetingDate.Month,
                        MeetingDate.Day,
                        LocalJumpTime.Hours,
                        LocalJumpTime.Minutes,
                        LocalJumpTime.Seconds);

                    _raceJumpDateTimeUTC = StateId > 0 ? TimeZoneHelper.ToUTC(aux, StateId) : aux;
                }
                return _raceJumpDateTimeUTC;
            }
            set { _raceJumpDateTimeUTC = value; }
        }
    }

    /// <summary>
    /// Class that is retrieved periodically while the race is active
    /// </summary>
    public class RaceDyn : RaceBase
    {
        [CustomDisplay(DisplayOn.NONE)]
        public IEnumerable<RunnerDyn> Runners { get; set; }

        [CustomDisplay(DisplayOn.NONE)]
        public virtual bool isDone { get; set; }

        [Display(Name = "Win Pool", Order = 0)]
        [CustomDisplay(DisplayOn.ALL)]
        public float RaceWinPool { get; set; }

        [Display(Name = "PP Pool", Order = 0)]
        [CustomDisplay(DisplayOn.ALL)]
        public float RacePPPool { get; set; }

        [Display(Name = "EX Pool Total", Order = 0)]
        [CustomDisplay(DisplayOn.AUS)]
        public float EXPoolTotal { get; set; }

        [Display(Name = "Ex Div Amount", Order = 0)]
        [CustomDisplay(DisplayOn.AUS)]
        public float EXDivAmount { get; set; }

        [Display(Name = "QN Pool Total", Order = 0)]
        [CustomDisplay(DisplayOn.AUS)]
        public float QNPoolTotal { get; set; }

        [Display(Name = "QN Div Amount", Order = 0)]
        [CustomDisplay(DisplayOn.AUS)]
        public float QNDivAmount { get; set; }

        [Display(Name = "F4 Pool Total", Order = 0)]
        [CustomDisplay(DisplayOn.AUS)]
        public float F4PoolTotal { get; set; }

        [Display(Name = "F4 Div Amount", Order = 0)]
        [CustomDisplay(DisplayOn.AUS)]
        public float F4DivAmount { get; set; }

        [Display(Name = "TF Pool Total", Order = 0)]
        [CustomDisplay(DisplayOn.AUS)]
        public float TFPoolTotal { get; set; }

        public RaceDyn GetLightCopy()
        {
            var r = new RaceDyn();
            r.EXDivAmount = EXDivAmount;
            r.EXPoolTotal = EXPoolTotal;
            r.F4DivAmount = F4DivAmount;
            r.F4PoolTotal = F4PoolTotal;
            r.isDone = isDone;
            r.QNDivAmount = QNDivAmount;
            r.QNPoolTotal = QNPoolTotal;
            r.RaceId = RaceId;
            r.RaceNumber = RaceNumber;
            r.RacePPPool = RacePPPool;
            r.RaceWinPool = RaceWinPool;
            r.TFPoolTotal = TFPoolTotal;
            return r;
        }
    }

    /// <summary>
    /// Class used to show the Race details in the Race view
    /// </summary>
    public class Race : RaceDyn
    {
        [CustomDisplay(DisplayOn.NONE)]
        public int MeetingId { get; set; }

        [Display(Name = "Name", Order = 4)]
        [CustomDisplay(DisplayOn.ALL)]
        public string RaceName { get; set; }

        [Display(Name = "Type", Order = 5)]
        [CustomDisplay(DisplayOn.ALL)]
        public string RaceTypeName { get; set; }

        [CustomDisplay(DisplayOn.NONE)]
        public int CountryId { get; set; }

        [CustomDisplay(DisplayOn.NONE)]
        public int supermeetingtypeid { get; set; }

        [CustomDisplay(DisplayOn.NONE)]
        public int superracetypeid { get; set; }

        [CustomDisplay(DisplayOn.NONE)]
        public int RaceCourseId { get; set; }

        [CustomDisplay(DisplayOn.NONE)]
        public string DistanceName { get; set; }

        [Display(Name = "Distance", Order = 6)]
        [CustomDisplay(DisplayOn.ALL)]
        public string _DistanceName { get { return DistanceName + "m"; } }

        [Display(Name = "Race Index", Order = 1)]
        [CustomDisplay(DisplayOn.HK)]
        public int HK_RaceIndex { get; set; }

        //public System.TimeSpan RaceWinningTime { get; set; }

        [Display(Name = "Race Going", Order = 7)]
        [CustomDisplay(DisplayOn.ALL)]
        public string RaceGoingName { get; set; }

        //[CustomDisplay(DisplayOn.NONE)]
        //public bool isTurf { get; set; }

        //[Display(Name = "Is turf", Order = 0)]
        //[CustomDisplay(DisplayOn.BOTH)]
        //public string IsTurf { get { return isTurf ? null : "No"; } }//only displays "No" if not isTurf

        [Display(Name = "Surface", Order = 8)]
        [CustomDisplay(DisplayOn.ALL)]
        public string RaceTrackType { get; set; }

        [Display(Name = "No./Runners", Order = 9)]
        [CustomDisplay(DisplayOn.ALL)]
        public short NumberOfRunners { get; set; }

        //[CustomDisplay(DisplayOn.NONE)]
        //public bool isStarted { get; set; } 

        //[Display(Name = "Jump Time", Order = 0)]
        //[CustomDisplay(DisplayOn.BOTH)]
        //public string Formatted_JumpDateTimeUTC { get { return RaceJumpDateTimeUTC.ToString() + " UTC"; } }

        [Display(Name = "Status", Order = 10)]
        [CustomDisplay(DisplayOn.ALL)]
        public string RaceStatus { get; set; }

        bool _isDone;

        [CustomDisplay(DisplayOn.NONE)]
        public override bool isDone
        {
            get
            {
                if (!_isDone)
                {
                    _isDone = RaceStatus != null ? RaceStatus == ConfigValues.RaceStatusDone : false;
                }
                return _isDone;
            }
            set { _isDone = value; }
        }

        [CustomDisplay(DisplayOn.NONE)]
        public DateTime MeetingDate { get; set; }

        [CustomDisplay(DisplayOn.NONE)]
        public TimeSpan LocalJumpTime { get; set; }

        [Display(Name = "Jump Time", Order = 3)]
        [CustomDisplay(DisplayOn.ALL)]
        public string _LocalJumpTime { get { return string.Format("{0}:{1}", LocalJumpTime.Hours, LocalJumpTime.Minutes); } }

        [CustomDisplay(DisplayOn.NONE)]
        public int StateId { get; set; }

        [CustomDisplay(DisplayOn.NONE)]
        public string CountryCode { get; set; }

        [CustomDisplay(DisplayOn.NONE)]
        public bool IsHK
        {
            get { return CountryCode == "HKG"; }
        }


        DateTime _raceJumpDateTimeUTC;

        [CustomDisplay(DisplayOn.NONE)]
        public DateTime RaceJumpDateTimeUTC
        {
            get
            {
                if (_raceJumpDateTimeUTC == default(DateTime))
                {
                    if (StateId == default(int))
                        return DateTime.MaxValue;

                    var aux = new DateTime(
                        MeetingDate.Year,
                        MeetingDate.Month,
                        MeetingDate.Day,
                        LocalJumpTime.Hours,
                        LocalJumpTime.Minutes,
                        LocalJumpTime.Seconds);
                    _raceJumpDateTimeUTC = TimeZoneHelper.ToUTC(aux, StateId);
                }
                return _raceJumpDateTimeUTC;
            }
            set { _raceJumpDateTimeUTC = value; }
        }


        //[CustomDisplay(DisplayOn.NONE)]
        //public static Race DummyRace
        //{
        //    get
        //    {
        //        return FizzWare.NBuilder.Builder<Race>.CreateNew().Build();
        //    }
        //}

        internal void Update(RaceDyn race)
        {
            this.Runners.ToList().ForEach(r =>
                {
                    var runDyn = race.Runners.FirstOrDefault(rd => rd.RunnerId == r.RunnerId);
                    if (runDyn != null)
                        ((Runner)r).Update(runDyn);
                });
            this.isDone = this.isDone || race.isDone;
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
            //public DateTime LastTotalRefresh { get; set; }
            public DateTime LastUserRequest { get; set; }
            public DateTime NextRefresh { get; set; }
        }

        public RefreshInfo RefreshValues = new RefreshInfo();
    }


    public class DisplayProperty
    {
        public string FieldName;
        public string DisplayName;
        public string Value;
    }

}
