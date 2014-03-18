using RaceDayDisplayApp.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Data.SqlClient;
using Dapper;

namespace RaceDayDisplayApp.DAL
{
    public class DBGateway
    {
        readonly string connectionString =
            ConfigurationManager.ConnectionStrings["RaceDayDB"].ConnectionString;

        /// <summary>
        /// Used in the Index page
        /// </summary>
        public IEnumerable<Country> GetCountries(bool filterByToday)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                var lookup = new Dictionary<int, Country>();
                var resultList = conn.Query<Country, MeetingBase, Country>(@"
                    SELECT c.Id as CountryId, c.Name, 
                           m.Id AS MeetingId, m.MeetingDate, rc.Name AS RaceCourseName,
						   MIN(r.RaceJumpDateTimeUTC) as MinRaceJumpDateTimeUTC,
                           MIN(r.LocalJumpTime) as MinRaceJumpTimeLocal
                    FROM Country AS c INNER JOIN Meeting AS m ON c.Id = m.CountryId
			                          LEFT JOIN RaceCourse AS rc ON m.RaceCourseId = rc.Id
									  LEFT JOIN Race AS r on r.MeetingId = m.Id
                    WHERE c.inUse = 1" + (filterByToday ? " AND m.MeetingDate = CONVERT(date, getdate())" : "") + @"
					GROUP BY c.Id, c.Name, m.Id, m.MeetingDate, rc.Name
					ORDER BY m.MeetingDate DESC, MinRaceJumpDateTimeUTC DESC",
                    (c, m) =>
                    {
                        Country country;
                        if (!lookup.TryGetValue(c.CountryId, out country))
                        {
                            lookup.Add(c.CountryId, country = c);
                        }
                        if (country.Meetings == null)
                            country.Meetings = new List<MeetingBase>();
                        if (m != null)
                        {
                            country.Meetings.Add(m);
                        }
                        return country;
                    },
                    splitOn: "MeetingId"
                    );

                return lookup.Values;
            }
        }

        /// <summary>
        /// RaceIndex and RaceDetails page
        /// </summary>
        public IEnumerable<RaceDisplay> GetRacesList(bool today)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                return conn.Query<RaceDisplay>(@"
                    SELECT Race.Id AS RaceId, Race.RaceNumber, Race.RaceName, Race.RaceJumpDateTimeUTC, 
                           Meeting.MeetingDate, RaceCourse.Name AS RaceCourseName, 
                           LocalJumpTime, AUS_StateId as StateId, isDone
                    FROM Race INNER JOIN Meeting on Meeting.Id = Race.MeetingId
		                      INNER JOIN RaceCourse on Meeting.RaceCourseId = RaceCourse.Id
                    " + (today ? "WHERE Meeting.MeetingDate = CONVERT(date, getdate())" : "") + @"
                    ORDER BY MeetingDate DESC, RaceJumpDateTimeUTC DESC");
                //TODO review the WHERE condition            
            }
        }

        /// <summary>
        /// Used in the meeting details page
        /// </summary>
        public Meeting GetMeeting(int meetingId)
        {
            //return Meeting.DummyMeeting;

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                    //Select Meeting.Id as MeetingId, RaceCourse.Name as RaceCourseName, Weather.Name as WeatherName, 
                    //       Going.Id as DefaultGoingName, CourseVariant.Name as CourseVariantName, Country.Code as CountryCode,
                    //       Race.Id as RaceId, Race.RaceNumber
                    //from Meeting left join RaceCourse    on Meeting.RaceCourseId=RaceCourse.Id
                    //               left join Weather       on Meeting.WeatherId=Weather.Id
                    //               left join Going         on Meeting.DefaultGoingId=Going.Id
                    //               left join CourseVariant on Meeting.CourseVariantId=CourseVariant.Id
                    //               left join Race          on Meeting.Id=Race.MeetingId
                    //               left join Country       on Meeting.CountryId=Country.Id
                    //where Meeting.Id = @MeetingId

                var lookup = new Dictionary<int, Meeting>();
                var resultList = conn.Query<Meeting,RaceBase,Meeting>(@"
                    Select meeting.*, Race.Id as RaceId, Race.RaceNumber
                    from getMeetingDetailsData(@MeetingId) meeting 
                         left join Race on meeting.MeetingId=Race.MeetingId",
                    (m, r) =>
                    {
                        Meeting meeting;
                        if (!lookup.TryGetValue(m.MeetingId, out meeting))
                        {
                            lookup.Add(m.MeetingId, meeting = m);
                        }
                        if (meeting.Races == null)
                            meeting.Races = new List<RaceBase>();
                        if (r != null)
                            meeting.Races.Add(r);
                        return meeting;
                    },
                    param: new { MeetingId = meetingId },
                    splitOn: "RaceId"
                    );

                return resultList.FirstOrDefault();
            }
        }

        /// <summary>
        /// used in the RaceDetails page
        /// </summary>
        internal Meeting GetMeetingByRaceId(int raceId)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                    //Select Meeting.Id as MeetingId, RaceCourse.Name as RaceCourseName, Weather.Name as WeatherName, 
                    //       Going.Id as DefaultGoingName, CourseVariant.Name as CourseVariantName, Country.Code as CountryCode
                    //from Race left join Meeting       on Meeting.Id=Race.MeetingId
                    //          left join RaceCourse    on Meeting.RaceCourseId=RaceCourse.Id
                    //          left join Weather       on Meeting.WeatherId=Weather.Id
                    //          left join CourseVariant on Meeting.CourseVariantId=CourseVariant.Id
                    //          left join Going		    on Race.RaceGoingId=Going.Id
                    //          left join Country		on Meeting.CountryId=Country.Id
                    //where Race.Id = @RaceId

                var resultList = conn.Query<Meeting>(@"
                    declare @m int
                    Select @m = MeetingId from Race where Race.Id = @RaceId
                    Select * from getMeetingDetailsData(@m)",
                    new { RaceId = raceId });

                return resultList.FirstOrDefault();
            }
        }

        /// <summary>
        /// used in the cache layer
        /// </summary>
        public RaceCache GetRaceWithRunnersStat(int raceId)
        {
            //return Race.DummyRace;

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                var race = conn.Query<RaceCache>(@"
                    Select Race.Id as RaceId, MeetingId, RaceNumber, RaceType.Name as RaceTypeName, 
                           Distance.Name as DistanceName, RaceJumpDateTimeUTC, 
	                       HK_RaceIndex, Going.Name as RaceGoingName, isTurf, 
                           isDone, isStarted, Country.Code as CountryCode, 
                           LocalJumpTime, MeetingDate, AUS_StateId as StateId
                    from Race left join RaceType on Race.RaceTypeId=RaceType.Id
			                    left join Distance on Race.DistanceId=Distance.Id
			                    left join Going    on Race.RaceGoingId=Going.Id
                                left join Meeting on Race.MeetingId=Meeting.Id
                                left join Country on Meeting.CountryId=Country.Id
                                left join RaceCourse on Meeting.RaceCourseId=RaceCourse.Id
                    where Race.Id = @RaceId",
                    new { RaceId = raceId }).FirstOrDefault();

                if (race != null)
                {
                    var runners = conn.Query<Runner>(@"
                        SELECT	RunnerId, RaceId, HorseNumber, Barrier, RunnerId, Name, HK_ChineseName,
		                        Jockey, Trainer, Gear, Z_newTrainerSinceLastStart,
		                        Z_BPAdvFactor, Age, Sex, Color,
		                        AUS_HcpWt, CarriedWt, HK_DeclaredHorseWtLbs, AUS_HcpRatingAtJump, HK_ActualWtLbs,
		                        HK_Rating, isScratched, Place, AUS_SPW, AUS_SPP,
		                        HK_WinOdds, Z_AUS_FinishTime, HK_FinishTime, JockeyPoints,
		                        STATISTICS1, STATISTICS2, STATISTICS3, STATISTICS4
                        from DataGrid_sta
                        where RaceId = @RaceId",
                        new { RaceId = raceId });

                    race.Runners = runners.ToList();
                }

                return race;
            }
        }

        /// <summary>
        /// Used in the cache layer
        /// </summary>
        public RaceDyn GetRaceWithRunnersDyn(int raceId)
        {
            //TODO
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                var race = conn.Query<RaceDyn>(@"
                    SELECT RaceId, isDone, RaceWinPool, RacePPPool, EXPoolTotal, EXDivAmount,
                    QNPoolTotal, QNDivAmount, F4PoolTotal, F4DivAmount, TFPoolTotal
                    FROM RaceDivPoolData
                    WHERE RaceId = @RaceId",
                    new { RaceId = raceId }).FirstOrDefault();

                if (race != null)
                {
                    var resultList = conn.Query<RunnerDyn>(@"
                        select RunnerId, RaceId, isScratched, RaceId, 
                        WinOdds, PlaceOdds, isWinFavorite, WinDropby20, WinDropby50,
                        isPlaceFavorite, PlaceDropby20, PlaceDropby50, 
                        ODDSLAST1, ODDSLAST2, ODDSLAST3
                        from DataGrid_dyn2
                        where RaceId=@RaceId",
                        new { RaceId = raceId });

                    race.Runners = resultList;
                }

                return race;
            }
        }

            
        /// <summary>
        /// used in the cache layer
        /// </summary>
        internal int GetSecondsSinceLastUpdate(Race race)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                var resultList = conn.Query<int>(@"
                        Select DATEDIFF(second, UpdateDateTime, getdate())
                        from LastUpdates
                        where MeetingId = @meetingId and RaceNumber=@raceNumber",
                    new { meetingId = race.MeetingId, raceNumber = race.RaceNumber });
                
                return resultList.FirstOrDefault();
            }
        }

        /// <summary>
        /// used in meeting details page, race details page and _GridSettings partial view
        /// </summary>
        public IEnumerable<ViewUserSetting> GetUserSettings(int userId, int meetingId, bool isHK)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                //retrieve the user settings
                var settings = conn.Query<UserSettings>(@"
                    SELECT * 
                    FROM Settings
                    WHERE userId = @UserId",
                   new { UserId = userId }).FirstOrDefault();

                if (settings == null)
                    settings = UserSettings.DEFAULT;

                var viewSettings = ModelHelper.ToViewUserSettings(settings, isHK);

                //retrieve the statistical columns
                var resultList2 = conn.Query(@"
                    SELECT rs.LongName, rs.Ordering
                    FROM RaceStatistics rs
                    WHERE rs.MeetingId = @MeetingId",
                    new { MeetingId = meetingId });

                string aux;
                var statSettings = resultList2.Select(rs => new ViewUserSetting
                {
                    PropertyName = aux = "STATISTICS" + rs.Ordering,
                    DisplayName = rs.LongName,
                    Checked = (bool)typeof(UserSettings).GetProperty(aux).GetValue(settings)
                });

                //concat user settings and statistical columns
                return viewSettings.Concat(statSettings);
            }
        }

        /// <summary>
        /// Used in RaceDetails page and _Race partial view
        /// </summary>
        public IEnumerable<MvcJqGrid.Column> GetGridColumns(Race race)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                //retrieve the statistical columns

                var resultList = conn.Query(@"
                    SELECT rs.LongName, rs.Ordering
                    FROM RaceStatistics rs
                    WHERE rs.RaceId = @RaceId",
                    new { RaceId = race.RaceId });

                return resultList.Select(e => new MvcJqGrid.Column("STATISTICS" + e.Ordering)
                       .SetLabel(e.LongName)).Cast<MvcJqGrid.Column>();
            }
        }

        public Dictionary<int, string> GetTimeZones()
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                var q = @"select AUS_State.Id as AUSStateId, CountryState_Timezone.Timezone as Code
                            from CountryState_Timezone inner join AUS_State 
                                    on AUS_State.Code = CountryState_Timezone.Code";

                var timezones = new Dictionary<int, string>();
                conn.Query(q).ToList().ForEach(x => timezones.Add(x.AUSStateId, x.Code));
                return timezones;
            }
        }


        public IEnumerable<dynamic> GetRunnerHistory(int raceId, int horseId)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                return conn.Query("Select * from getRunnerHistory(@raceId, @horseId)",
                                    new 
                                    { 
                                        raceId = raceId,
                                        horseId = horseId,
                                    });
            }
        }

        public dynamic GetHorseDetailsData(int horseId)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                return conn.Query("Select * from getHorseDetailsDataHKG(@horseId)",
                                    new { horseId = horseId }).FirstOrDefault();
            }
        }

        //        /// <summary>
        //        /// True if the race is held in Hong Kong
        //        /// </summary>
        //        public bool IsHK(int raceId)
        //        {
        //            using (var conn = new SqlConnection(connectionString))
        //            {
        //                conn.Open();

        //                var code = conn.Query<string>(@"
        //                    select Country.Code
        //                    from Race inner join Meeting on Race.MeetingId = Meeting.Id
        //		                      inner join Country on Meeting.CountryId = Country.Id
        //                    where Race.Id = @RaceId",
        //                   new { RaceId = raceId }).FirstOrDefault();

        //                return code == "HKG";
        //            }
        //        }


    }

}
