using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Data.SqlClient;
using Dapper;

namespace RaceDayDisplayApp.Models
{
    public class MeetingDB
    {
        readonly string connectionString =
            ConfigurationManager.ConnectionStrings["RaceDayDB"].ConnectionString;

        public IEnumerable<Country> GetCountries()
        {
            return GetCountries(false);
        }

        public IEnumerable<Country> GetCountries(bool filterByToday)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                var lookup = new Dictionary<int, Country>();
                var resultList = conn.Query<Country, MeetingBase, Country>(@"
                    SELECT c.Id as CountryId, c.Name, 
                           m.Id AS MeetingId, m.MeetingDate, rc.Name AS RaceCourseName,
						   MIN(r.RaceJumpDateTimeUTC) as MinRaceJumpDateTimeUTC
                    FROM Country AS c INNER JOIN Meeting AS m ON c.Id = m.CountryId
			                          LEFT JOIN RaceCourse AS rc ON m.RaceCourseId = rc.Id
									  LEFT JOIN Race AS r on r.MeetingId = m.Id
                    WHERE c.inUse = 1" + (filterByToday ? " AND m.MeetingDate = CONVERT(date, getdate())" : "") + @"
					GROUP BY c.Id, c.Name, m.Id, m.MeetingDate, rc.Name
					ORDER BY m.MeetingDate, MinRaceJumpDateTimeUTC",
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

        public Meeting GetMeeting(int meetingId)
        {
            //return Meeting.DummyMeeting;

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                var lookup = new Dictionary<int, Meeting>();
                var resultList = conn.Query<Meeting,RaceBase,Meeting>(@"
                    Select Meeting.Id as MeetingId, RaceCourse.Name as RaceCourseName, Weather.Name as WeatherName, 
	                       Going.Id as DefaultGoingName, CourseVariant.Name as CourseVariantName, Country.Code as CountryCode,
                           Race.Id as RaceId, Race.RaceNumber
                    from Meeting left join RaceCourse    on Meeting.RaceCourseId=RaceCourse.Id
			                       left join Weather       on Meeting.WeatherId=Weather.Id
			                       left join Going         on Meeting.DefaultGoingId=Going.Id
			                       left join CourseVariant on Meeting.CourseVariantId=CourseVariant.Id
                                   left join Race          on Meeting.Id=Race.MeetingId
                                   left join Country       on Meeting.CountryId=Country.Id
                    where Meeting.Id = @MeetingId",
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

        public Race GetRace(int raceId)
        {
            //return Race.DummyRace;

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                var resultList = conn.Query<Race>(@"
                    Select Race.Id as RaceId, RaceNumber, RaceType.Name as RaceTypeName, 
                           Distance.Name as DistanceName, RaceJumpDateTimeUTC, 
	                       HK_RaceIndex, Going.Name as RaceGoingName, isTurf, 
                           isDone, isStarted, Country.Code as CountryCode
                    from Race left join RaceType on Race.RaceTypeId=RaceType.Id
			                    left join Distance on Race.DistanceId=Distance.Id
			                    left join Going    on Race.RaceGoingId=Going.Id
                                left join Meeting on Race.MeetingId=Race.Id
                                left join Country on Meeting.CountryId=Country.Id
                    where Race.Id = @RaceId",
                   new {  RaceId = raceId });

                return resultList.FirstOrDefault();
            }
        }

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

        public IEnumerable<RunnerBase> GetRunnersDyn(int raceId)
        {
            //return RunnerBase.DummyRunnerList;
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                var resultList = conn.Query<RunnerBase>(@"
                    select RunnerId, RaceId, isScratched, isDone, RaceId, ODDSLAST1, ODDSLAST2, ODDSLAST3
                    from DataGrid_dyn2
                    where RaceId=@RaceId",
                   new { RaceId = raceId });

                return resultList;
            }
        }

        public IEnumerable<Runner> GetRunnersStat(int raceId)
        {
            //return Runner.DummyRunnerList; //for testing purposes

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                var resultList = conn.Query<Runner>(@"
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

                return resultList;
            }

        }

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
                    new {RaceId = race.RaceId});

                var statColumns = resultList.Select(e => new MvcJqGrid.Column("STATISTICS" + e.Ordering)
                                  .SetLabel(e.LongName)).Cast<MvcJqGrid.Column>();
                
                return ModelHelper.GetGridColumns(race.IsHK).Concat(statColumns);
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


        public IEnumerable<RaceDisplay> GetRacesList(bool today)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                return conn.Query<RaceDisplay>(@"
                    SELECT Race.Id AS RaceId, Race.RaceNumber, Race.RaceName, Race.RaceJumpDateTimeUTC, Meeting.MeetingDate, RaceCourse.Name AS RaceCourseName
                    FROM Race INNER JOIN Meeting on Meeting.Id = Race.MeetingId
		                      INNER JOIN RaceCourse on Meeting.RaceCourseId = RaceCourse.Id
                    WHERE RaceJumpDateTimeUTC IS NOT NULL" + (today ? " AND Meeting.MeetingDate = CONVERT(date, getdate())" : "") + @"
                    ORDER BY MeetingDate, RaceJumpDateTimeUTC");
                    //TODO review the WHERE condition            
            }
        }

        internal Meeting GetRaceWithMeeting(int raceId)
        {

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                var resultList = conn.Query<Meeting, Race, Meeting>(@"
                    Select Meeting.Id as MeetingId, RaceCourse.Name as RaceCourseName, Weather.Name as WeatherName, 
	                       Going.Id as DefaultGoingName, CourseVariant.Name as CourseVariantName, Country.Code as CountryCode,
                           Race.Id as RaceId, RaceNumber, RaceType.Name as RaceTypeName, 
                           Distance.Name as DistanceName, RaceJumpDateTimeUTC, 
	                       HK_RaceIndex, Going.Name as RaceGoingName, isTurf, 
                           isDone, isStarted, Country.Code as CountryCode
                    from Race left join Meeting       on Meeting.Id=Race.MeetingId
							  left join RaceCourse    on Meeting.RaceCourseId=RaceCourse.Id
			                  left join Weather       on Meeting.WeatherId=Weather.Id
			                  left join CourseVariant on Meeting.CourseVariantId=CourseVariant.Id
							  left join RaceType	  on Race.RaceTypeId=RaceType.Id
			                  left join Distance	  on Race.DistanceId=Distance.Id
			                  left join Going		  on Race.RaceGoingId=Going.Id
                              left join Country		  on Meeting.CountryId=Country.Id
                    where Race.Id = @RaceId",
                    (m, r) =>
                    {
                        m.Races = new List<RaceBase>();
                        if (r != null)
                            m.Races.Add(r);
                        return m;
                    },
                    param: new { RaceId = raceId },
                    splitOn: "RaceId"
                    );

                return resultList.FirstOrDefault();
            }
        }
    }

}
