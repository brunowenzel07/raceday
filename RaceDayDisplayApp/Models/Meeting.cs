using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RaceDayDisplayApp.Models
{
    public class MeetingBase
    {
        [Key]
        [CustomDisplay(DisplayOn.NONE)]
        public int MeetingId { get; set; }

        [CustomDisplay(DisplayOn.NONE)]
        public DateTime MeetingDate { get; set; }

        [CustomDisplay(DisplayOn.NONE)]
        public DateTime MinRaceJumpDateTimeUTC { get; set; }

        [Display(Name = "Race Course", Order = 0)]
        [CustomDisplay(DisplayOn.BOTH)]
        public string RaceCourseName { get; set; }
    }

    public class Meeting : MeetingBase
    {
        [CustomDisplay(DisplayOn.NONE)]
        public string CountryCode { get; set; }

        [Display(Name = "Weather", Order = 0)]
        [CustomDisplay(DisplayOn.BOTH)]
        public string WeatherName { get; set; }

        [Display(Name = "Going", Order = 0)]
        [CustomDisplay(DisplayOn.BOTH)]
        public string DefaultGoingName { get; set; }

        //public int NumberOfRaces { get; set; }
        //public bool HK_isNightMeet { get; set; }

        [Display(Name = "Course Variant", Order = 0)]
        [CustomDisplay(DisplayOn.BOTH)]
        public string CourseVariantName { get; set; }
        
        //public string Code { get; set; }
        //public bool InUse { get; set; }
        //public DateTime SeasonStart { get; set; }

        [CustomDisplay(DisplayOn.NONE)]
        public IList<RaceBase> Races { get; set; }



        [CustomDisplay(DisplayOn.NONE)]
        public bool IsHK
        {
            get { return CountryCode == "HKG"; }
        }

        [CustomDisplay(DisplayOn.NONE)]
        public static Meeting DummyMeeting
        {
            get
            {
                Random rdm = new Random();
                Meeting m = FizzWare.NBuilder.Builder<Meeting>.CreateNew().Build();
                m.Races = FizzWare.NBuilder.Builder<RaceBase>.CreateListOfSize(rdm.Next(10) + 1).Build();

                return m;
            }
        }
    }

}